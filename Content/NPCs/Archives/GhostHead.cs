using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class GhostHead : ModNPC
    {
        public override string Texture => $"Terraria/Images/NPC_{NPCID.Zombie}";

        private const float NeckBaseOffset = 40f;
        private const float MinNeckLength = 240f;
        private const float MaxNeckLength = 440f;
        private const float SegmentSpacing = 24f;
        private const int MaxSegments = 30;

        private const float ExtendSpeed = 0.02f;
        private const float PeekSway = 70f;
        private const float SwaySpeed = 0.025f;
        private const float WaveAmp = 16f;
        private const float WaveSpeed = 0.035f;
        private const float Phase1 = 0f;
        private const float Phase2 = 2.2f;
        private const float EndPhase = 3.6f;
        private const float HandleLen = 90f;
        private const float HeadEase = 0.15f;
        private const float RotEase = 0.12f;
        private const float AimEase = 0.08f;
        private const float MaxPeekAngle = 1.2f;
        private const float MaxLookAngle = 0.8f;

        private const float HeadLength = 64f;
        private const int HeadThickness = 48;
        private const int NeckBaseThickness = 26;
        private const int NeckTipThickness = 12;

        private float time;
        private float neckLength;
        private float aimAngle;
        private bool initialized;
        private Vector2[] neckPoints;
        private int neckCount;

        private int ParentId => (int)NPC.ai[0];

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 60;
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
            if (ParentId < 0 || ParentId >= Main.maxNPCs)
            {
                NPC.active = false;
                return;
            }

            NPC parent = Main.npc[ParentId];
            if (!parent.active || parent.ModNPC is not GhostCrawler body)
            {
                NPC.active = false;
                return;
            }

            NPC.velocity = Vector2.Zero;
            time += 1f;

            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            Vector2 parentForward = body.Forward;
            Vector2 anchor = parent.Center + parentForward * NeckBaseOffset;

            if (!initialized)
            {
                aimAngle = parentForward.ToRotation();
                NPC.Center = anchor + parentForward * MinNeckLength;
                NPC.rotation = aimAngle;
                initialized = true;
            }

            float baseAngle = parentForward.ToRotation();
            Vector2 toPlayer = (player.Center - anchor).SafeNormalize(parentForward);
            float lean = MathHelper.Clamp(MathHelper.WrapAngle(toPlayer.ToRotation() - baseAngle), -MaxPeekAngle, MaxPeekAngle);
            aimAngle = aimAngle.AngleLerp(baseAngle + lean, AimEase);
            Vector2 aim = aimAngle.ToRotationVector2();
            Vector2 perpAim = new Vector2(-aim.Y, aim.X);

            UpdatePeek(anchor, aim, perpAim, player);

            if (!Main.dedServ)
                BuildNeck(anchor, parentForward, aim, perpAim);
        }

        /// <summary>
        /// Eases the head toward its peek position and turns it to face the player.
        /// </summary>
        private void UpdatePeek(Vector2 anchor, Vector2 aim, Vector2 perpAim, Player player)
        {
            float lenT = 0.5f + 0.5f * (float)Math.Sin(time * ExtendSpeed);
            float neckLengthTarget = MathHelper.Lerp(MinNeckLength, MaxNeckLength, lenT);
            neckLength = MathHelper.Lerp(neckLength, neckLengthTarget, 0.08f);

            float sway = PeekSway * (float)Math.Sin(time * SwaySpeed);
            float endWave = WaveAmp * WaveScale() * (float)Math.Sin(time * WaveSpeed + EndPhase);

            Vector2 headTarget = anchor + aim * neckLength + perpAim * (sway + endWave);
            NPC.Center = Vector2.Lerp(NPC.Center, headTarget, HeadEase);

            float lookTarget = (player.Center - NPC.Center).ToRotation();
            float lookDelta = MathHelper.Clamp(MathHelper.WrapAngle(lookTarget - aimAngle), -MaxLookAngle, MaxLookAngle);
            NPC.rotation = NPC.rotation.AngleLerp(aimAngle + lookDelta, RotEase);
            NPC.direction = NPC.spriteDirection = player.Center.X >= NPC.Center.X ? 1 : -1;
        }

        /// <summary>
        /// Wave strength fades as the neck extends, so it reads straighter when stretched out.
        /// </summary>
        private float WaveScale()
        {
            float extend = MathHelper.Clamp((neckLength - MinNeckLength) / (MaxNeckLength - MinNeckLength), 0f, 1f);
            return MathHelper.Lerp(1f, 0.3f, extend);
        }

        /// <summary>
        /// Samples the neck into points along a cubic Bezier whose end tangents follow the body and head
        /// facings, with a traveling wave offsetting the interior controls.
        /// </summary>
        private void BuildNeck(Vector2 anchor, Vector2 parentForward, Vector2 aim, Vector2 perpAim)
        {
            Vector2 head = NPC.Center;
            Vector2 headForward = NPC.rotation.ToRotationVector2();

            float waveScale = WaveScale();
            float w1 = WaveAmp * waveScale * (float)Math.Sin(time * WaveSpeed + Phase1);
            float w2 = WaveAmp * waveScale * (float)Math.Sin(time * WaveSpeed + Phase2);

            Vector2 p0 = anchor;
            Vector2 p1 = anchor + parentForward * HandleLen + perpAim * w1;
            Vector2 p2 = head - headForward * HandleLen + perpAim * w2;
            Vector2 p3 = head;

            int n = (int)MathHelper.Clamp((float)Math.Round(neckLength / SegmentSpacing), 1f, MaxSegments);
            if (neckPoints == null || neckPoints.Length != n + 1)
                neckPoints = new Vector2[n + 1];

            for (int i = 0; i <= n; i++)
                neckPoints[i] = Bezier(p0, p1, p2, p3, i / (float)n);

            neckCount = n + 1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (neckPoints != null && neckCount >= 2)
                DrawNeck(spriteBatch);

            DrawHead(spriteBatch);
            return false;
        }

        private void DrawNeck(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < neckCount - 1; i++)
            {
                float t = i / (float)(neckCount - 1);
                int thickness = (int)MathHelper.Lerp(NeckBaseThickness, NeckTipThickness, t);
                Color color = Color.Lerp(new Color(60, 66, 96), new Color(92, 100, 138), t);
                DrawBar(spriteBatch, neckPoints[i], neckPoints[i + 1], thickness, color);
            }
        }

        private void DrawHead(SpriteBatch spriteBatch)
        {
            Vector2 fwd = NPC.rotation.ToRotationVector2();
            Vector2 back = NPC.Center - fwd * (HeadLength * 0.5f);
            Vector2 front = NPC.Center + fwd * (HeadLength * 0.5f);
            DrawBar(spriteBatch, back, front, HeadThickness, new Color(110, 120, 162));
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

        private static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
        {
            float u = 1f - t;
            return u * u * u * a + 3f * u * u * t * b + 3f * u * t * t * c + t * t * t * d;
        }
    }
}
