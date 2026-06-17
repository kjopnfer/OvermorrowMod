using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Tiles.Archives;
using System;
using Terraria;
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

        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 80;
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
            DrawBody(spriteBatch);

            if (gaitInitialized)
                for (int i = 0; i < 2; i++)
                    DrawArm(spriteBatch, i);

            return false;
        }

        private float HalfWidth() => TextureAssets.Npc[NPC.type].Value.Width / 2f;
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
            gaitInitialized = true;
        }

        /// <summary>
        /// Begins a new pull for the active arm. The step leans the body toward the aim direction,
        /// capped per pull, and a sharper turn shortens and quickens the step so the arms crank the
        /// body around rather than the body rotating on its own.
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

        /// <summary>
        /// The world point an arm sets up for. The inside arm of a turn reaches ahead to grab and pull;
        /// the outside arm braces behind and out to the side to push, blended in by how sharp the turn is.
        /// </summary>
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

            SolveArm(shoulder, handWorld[armIndex], side, out Vector2 elbow, out Vector2 hand);

            DrawBar(spriteBatch, shoulder, elbow, 16, new Color(120, 130, 165));
            DrawBar(spriteBatch, elbow, hand, 12, new Color(80, 88, 120));
        }

        /// <summary>
        /// Places a fixed-length upper arm and forearm so the hand reaches toward the target,
        /// bending the elbow to a consistent side. Distance is clamped so the bones never stretch.
        /// </summary>
        private static void SolveArm(Vector2 shoulder, Vector2 target, float side, out Vector2 elbow, out Vector2 hand)
        {
            Vector2 toTarget = target - shoulder;
            float distance = MathHelper.Clamp(toTarget.Length(), Math.Abs(UpperArmLength - ForearmLength) + 1f, UpperArmLength + ForearmLength - 1f);
            float dirAngle = toTarget.ToRotation();

            float cosShoulder = (UpperArmLength * UpperArmLength + distance * distance - ForearmLength * ForearmLength) / (2f * UpperArmLength * distance);
            float shoulderAngle = (float)Math.Acos(MathHelper.Clamp(cosShoulder, -1f, 1f));

            float upperAngle = dirAngle + shoulderAngle * side;
            elbow = shoulder + upperAngle.ToRotationVector2() * UpperArmLength;

            float foreAngle = (target - elbow).ToRotation();
            hand = elbow + foreAngle.ToRotationVector2() * ForearmLength;
        }

        private void DrawBody(SpriteBatch spriteBatch)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, Color.White, forward.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
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

        private static float Smooth(float t) => t * t * (3f - 2f * t);
    }
}
