using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Tiles.Archives;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class GhostCrawler : ModNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + "GhostBody";

        private const float UpperArmLength = 216f;
        private const float ForearmLength = 240f;
        private const float ReachDistance = 420f;
        private const float DragEndDistance = 160f;
        private const float ReachSpread = 40f;
        private const float PushForward = 220f;
        private const float PushSpread = 240f;
        private const float MinStride = 90f;
        private const float StraightPullDuration = 100f;
        private const float TurnPullDuration = 45f;
        private const float MaxStepTurn = 0.5f;
        private const float MaxReachAngle = 1.05f;
        private const float TurnRange = MathHelper.PiOver2;
        private const float StopTurnDistance = 60f;

        private Vector2 forward = Vector2.UnitX;
        private readonly Vector2[] handWorld = new Vector2[2];
        private readonly Vector2[] plantWorld = new Vector2[2];
        private int activeArm;
        private float pullProgress;
        private float pullDuration = StraightPullDuration;
        private float pullStride;
        private Vector2 stepDir = Vector2.UnitX;
        private float stepStartAngle;
        private float stepTurn;
        private Vector2 pullStartCenter;
        private bool gaitInitialized;

        private Limb[] limbs;

        private const int SegmentCount = 10;
        private readonly Vector2[] segmentPos = new Vector2[SegmentCount];

        private int headWhoAmI = -1;

        /// <summary>
        /// The body's current facing direction.
        /// </summary>
        public Vector2 Forward => forward;

        /// <summary>
        /// An extra crawling arm with its own socket, bone lengths, splay and cadence so it never
        /// mirrors the main pair.
        /// </summary>
        private class Limb
        {
            public float ForwardOffset;
            public float SideOffset;
            public float Side;
            public float Upper;
            public float Fore;
            public float Reach;
            public float ReachAngle;
            public float Period;
            public float Phase;
            public float PrevPhase;
            public int Thickness;
            public Vector2 HandWorld;
            public Vector2 PlantWorld;
        }

        public override void SetDefaults()
        {
            NPC.width = 220;
            NPC.height = 180;
            NPC.lifeMax = 1000;
            NPC.damage = 25;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = true;
            NPC.friendly = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
        }

        public override bool CheckActive() => false;

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                headWhoAmI = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<GhostHead>(), 0, NPC.whoAmI);
        }

        public override void OnKill()
        {
            if (headWhoAmI >= 0 && headWhoAmI < Main.maxNPCs)
            {
                NPC head = Main.npc[headWhoAmI];
                if (head.active && head.ModNPC is GhostHead)
                    head.active = false;
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player target = Main.player[NPC.target];
            NPC.velocity = Vector2.Zero;

            ArchiveLights.Disturb(NPC.Center);

            if (Main.dedServ) return;

            if (!gaitInitialized)
                InitGait();

            Vector2 toPlayer = target.Center - NPC.Center;
            UpdateGait(toPlayer);

            NPC.direction = NPC.spriteDirection = forward.X >= 0f ? 1 : -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (gaitInitialized)
                DrawSegments(spriteBatch);

            if (gaitInitialized && limbs != null)
                foreach (Limb limb in limbs)
                    DrawLimb(spriteBatch, limb);

            DrawBody(spriteBatch);

            if (gaitInitialized)
                for (int i = 0; i < 2; i++)
                    DrawArm(spriteBatch, i);

            return false;
        }

        private float HalfWidth() => TextureAssets.Npc[NPC.type].Value.Width / 2f;
        private float HalfLength() => TextureAssets.Npc[NPC.type].Value.Height / 2f;
        private static Vector2 Perp(Vector2 v) => new Vector2(-v.Y, v.X);
        private static float Side(int armIndex) => armIndex == 0 ? 1f : -1f;

        private void InitGait()
        {
            float halfWidth = HalfWidth();
            Vector2 perp = Perp(forward);
            for (int i = 0; i < 2; i++)
            {
                Vector2 shoulder = NPC.Center + perp * (Side(i) * halfWidth);
                handWorld[i] = plantWorld[i] = shoulder + forward * ReachDistance;
            }
            activeArm = 0;
            StartPull(forward);

            CreateLimbs();
            CreateSegments();
            gaitInitialized = true;
        }

        private void CreateSegments()
        {
            Vector2 pos = NPC.Center;
            for (int i = 0; i < SegmentCount; i++)
            {
                pos -= forward * SegmentSpacing(i);
                segmentPos[i] = pos;
            }
        }

        private float SegmentScale(int i) => MathHelper.Lerp(0.8f, 0.45f, i / (float)(SegmentCount - 1));
        private float SegmentSpacing(int i) => HalfLength() * SegmentScale(i) * 1.2f;

        /// <summary>
        /// Builds the extra arms
        /// </summary>
        private void CreateLimbs()
        {
            float halfWidth = HalfWidth();
            float halfLength = HalfLength();
            Vector2 perp = Perp(forward);
            float fwdAngle = forward.ToRotation();

            int count = 6;
            limbs = new Limb[count];

            float[] sides = new float[count];
            sides[0] = sides[1] = 1f;
            sides[2] = sides[3] = -1f;
            for (int r = 4; r < count; r++)
                sides[r] = Main.rand.NextBool() ? 1f : -1f;
            for (int r = count - 1; r > 0; r--)
            {
                int j = Main.rand.Next(r + 1);
                (sides[r], sides[j]) = (sides[j], sides[r]);
            }

            for (int i = 0; i < count; i++)
            {
                float side = sides[i];
                var limb = new Limb
                {
                    Side = side,
                    SideOffset = side * halfWidth * Main.rand.NextFloat(0.45f, 1f),
                    ForwardOffset = Main.rand.NextFloat(-halfLength, halfLength),
                    Upper = Main.rand.NextFloat(90f, 200f),
                    Fore = Main.rand.NextFloat(90f, 200f),
                    ReachAngle = side * Main.rand.NextFloat(0.15f, 0.7f),
                    Period = Main.rand.NextFloat(70f, 140f),
                    Phase = Main.rand.NextFloat(),
                };
                limb.Reach = (limb.Upper + limb.Fore) * Main.rand.NextFloat(0.78f, 0.9f);
                limb.PrevPhase = limb.Phase;
                limb.Thickness = (int)MathHelper.Clamp(limb.Upper / 22f, 4f, 11f);

                Vector2 socket = NPC.Center + forward * limb.ForwardOffset + perp * limb.SideOffset;
                Vector2 reachDir = (fwdAngle + limb.ReachAngle).ToRotationVector2();
                limb.HandWorld = limb.PlantWorld = socket + reachDir * limb.Reach;
                limbs[i] = limb;
            }
        }

        /// <summary>
        /// Begins a new pull for the active arm. A sharper turn leans the step more and makes it
        /// shorter and quicker.
        /// </summary>
        private void StartPull(Vector2 aim)
        {
            float fwdAngle = forward.ToRotation();
            float angleToAim = MathHelper.WrapAngle(aim.ToRotation() - fwdAngle);
            float turnFactor = MathHelper.Clamp(Math.Abs(angleToAim) / TurnRange, 0f, 1f);

            stepTurn = MathHelper.Clamp(angleToAim, -MaxStepTurn, MaxStepTurn);
            stepStartAngle = fwdAngle;
            stepDir = (fwdAngle + stepTurn).ToRotationVector2();

            pullDuration = MathHelper.Lerp(StraightPullDuration, TurnPullDuration, turnFactor);
            pullStride = MathHelper.Lerp(ReachDistance - DragEndDistance, MinStride, turnFactor);
            pullStartCenter = NPC.Center;
            pullProgress = 0f;

            plantWorld[activeArm] = handWorld[activeArm];
        }

        private void UpdateGait(Vector2 toPlayer)
        {
            Vector2 aim = forward;
            if (toPlayer.Length() > StopTurnDistance)
            {
                Vector2 candidate = toPlayer.SafeNormalize(forward);
                if (!float.IsNaN(candidate.X) && candidate != Vector2.Zero)
                    aim = candidate;
            }

            pullProgress += 1f / pullDuration;
            if (pullProgress >= 1f)
            {
                ApplyPull(1f);
                activeArm = 1 - activeArm;
                StartPull(aim);
            }

            float linear = MathHelper.Clamp(pullProgress, 0f, 1f);
            ApplyPull(Smooth(linear));

            float angleToAim = MathHelper.WrapAngle(aim.ToRotation() - forward.ToRotation());
            float turnFactor = MathHelper.Clamp(Math.Abs(angleToAim) / TurnRange, 0f, 1f);
            float turnSide = angleToAim >= 0f ? 1f : -1f;
            float reachLean = MathHelper.Clamp(angleToAim, -MaxReachAngle, MaxReachAngle);
            Vector2 reachDir = (forward.ToRotation() + reachLean).ToRotationVector2();

            UpdateReachArm(linear, reachDir, turnFactor, turnSide);
            UpdateLimbs();
            UpdateSegments();
        }

        /// <summary>
        /// Chains the trailing tail like a worm
        /// </summary>
        private void UpdateSegments()
        {
            Vector2 ahead = NPC.Center;
            for (int i = 0; i < SegmentCount; i++)
            {
                Vector2 dir = (ahead - segmentPos[i]).SafeNormalize(forward);
                segmentPos[i] = ahead - dir * SegmentSpacing(i);
                ahead = segmentPos[i];
            }
        }

        /// <summary>
        /// Runs each extra arm's own reach/plant cycle
        /// </summary>
        private void UpdateLimbs()
        {
            Vector2 perp = Perp(forward);
            float fwdAngle = forward.ToRotation();

            foreach (Limb limb in limbs)
            {
                limb.Phase = Frac(limb.Phase + 1f / limb.Period);

                Vector2 socket = NPC.Center + forward * limb.ForwardOffset + perp * limb.SideOffset;
                Vector2 reachDir = (fwdAngle + limb.ReachAngle).ToRotationVector2();

                bool inPull = limb.Phase >= 0.5f;
                bool wasPull = limb.PrevPhase >= 0.5f;

                if (inPull && !wasPull)
                    limb.PlantWorld = limb.HandWorld;

                if (inPull)
                {
                    limb.HandWorld = limb.PlantWorld;
                }
                else
                {
                    float t = Smooth(limb.Phase / 0.5f);
                    Vector2 target = socket + reachDir * limb.Reach;
                    limb.HandWorld = Vector2.Lerp(limb.PlantWorld, target, t);
                }

                limb.PrevPhase = limb.Phase;
            }
        }

        private void ApplyPull(float t)
        {
            forward = (stepStartAngle + stepTurn * t).ToRotationVector2();
            NPC.Center = pullStartCenter + stepDir * (pullStride * t);
            handWorld[activeArm] = plantWorld[activeArm];
        }

        private void UpdateReachArm(float linear, Vector2 reachDir, float turnFactor, float turnSide)
        {
            int reaching = 1 - activeArm;
            float halfWidth = HalfWidth();
            Vector2 perp = Perp(forward);
            Vector2 shoulder = NPC.Center + perp * (Side(reaching) * halfWidth);
            Vector2 target = ArmTarget(reaching, shoulder, perp, reachDir, turnFactor, turnSide);
            handWorld[reaching] = Vector2.Lerp(plantWorld[reaching], target, Smooth(linear));
        }

        private Vector2 ArmTarget(int armIndex, Vector2 shoulder, Vector2 perp, Vector2 reachDir, float turnFactor, float turnSide)
        {
            Vector2 ahead = shoulder + reachDir * ReachDistance + perp * (Side(armIndex) * ReachSpread);
            if (Side(armIndex) == turnSide)
                return ahead;

            Vector2 push = shoulder + forward * PushForward + perp * (Side(armIndex) * PushSpread);
            return Vector2.Lerp(ahead, push, turnFactor);
        }

        private void DrawArm(SpriteBatch spriteBatch, int armIndex)
        {
            float side = Side(armIndex);
            Vector2 perp = Perp(forward);
            Vector2 shoulder = NPC.Center + perp * (side * HalfWidth());

            SolveArm(shoulder, handWorld[armIndex], side, UpperArmLength, ForearmLength, out Vector2 elbow, out Vector2 hand);

            DrawBar(spriteBatch, shoulder, elbow, 16, new Color(120, 130, 165));
            DrawBar(spriteBatch, elbow, hand, 12, new Color(80, 88, 120));
        }

        private void DrawLimb(SpriteBatch spriteBatch, Limb limb)
        {
            Vector2 perp = Perp(forward);
            Vector2 socket = NPC.Center + forward * limb.ForwardOffset + perp * limb.SideOffset;

            SolveArm(socket, limb.HandWorld, limb.Side, limb.Upper, limb.Fore, out Vector2 elbow, out Vector2 hand);

            DrawBar(spriteBatch, socket, elbow, limb.Thickness, new Color(70, 76, 104));
            DrawBar(spriteBatch, elbow, hand, Math.Max(3, limb.Thickness - 2), new Color(50, 56, 82));
        }

        /// <summary>
        /// Places a fixed-length upper arm and forearm so the hand reaches toward the target,
        /// bending the elbow to a consistent side. Distance is clamped so the bones never stretch.
        /// </summary>
        private static void SolveArm(Vector2 shoulder, Vector2 target, float side, float upper, float fore, out Vector2 elbow, out Vector2 hand)
        {
            Vector2 toTarget = target - shoulder;
            float distance = MathHelper.Clamp(toTarget.Length(), Math.Abs(upper - fore) + 1f, upper + fore - 1f);
            float dirAngle = toTarget.ToRotation();

            float cosShoulder = (upper * upper + distance * distance - fore * fore) / (2f * upper * distance);
            float shoulderAngle = (float)Math.Acos(MathHelper.Clamp(cosShoulder, -1f, 1f));

            float upperAngle = dirAngle + shoulderAngle * side;
            elbow = shoulder + upperAngle.ToRotationVector2() * upper;

            float foreAngle = (target - elbow).ToRotation();
            hand = elbow + foreAngle.ToRotationVector2() * fore;
        }

        private void DrawBody(SpriteBatch spriteBatch)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, Color.White, forward.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        private void DrawSegments(SpriteBatch spriteBatch)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 origin = texture.Size() / 2f;

            for (int i = SegmentCount - 1; i >= 0; i--)
            {
                Vector2 ahead = i == 0 ? NPC.Center : segmentPos[i - 1];
                Vector2 dir = (ahead - segmentPos[i]).SafeNormalize(forward);
                float rotation = dir.ToRotation() + MathHelper.PiOver2;
                Color tint = Color.White * MathHelper.Lerp(0.9f, 0.6f, i / (float)(SegmentCount - 1));

                spriteBatch.Draw(texture, segmentPos[i] - Main.screenPosition, null, tint, rotation, origin, SegmentScale(i), SpriteEffects.None, 0f);
            }
        }

        private static void DrawBar(SpriteBatch spriteBatch, Vector2 from, Vector2 to, int thickness, Color color)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Vector2 edge = to - from;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            int length = (int)edge.Length();

            Vector2 perp = new Vector2(-(float)Math.Sin(angle), (float)Math.Cos(angle));
            Vector2 start = from - Main.screenPosition - perp * (thickness / 2f);

            spriteBatch.Draw(pixel, new Rectangle((int)start.X, (int)start.Y, length, thickness), null, color, angle, Vector2.Zero, SpriteEffects.None, 0f);
        }

        private static float Frac(float value) => value - (float)Math.Floor(value);

        private static float Smooth(float t) => t * t * (3f - 2f * t);
    }
}
