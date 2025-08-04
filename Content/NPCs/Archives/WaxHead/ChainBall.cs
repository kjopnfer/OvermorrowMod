using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class ChainBall : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + "WaxheadFlail";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = ModUtils.SecondsToTicks(300);
            Projectile.penetrate = -1;
        }

        private Vector2 ballVelocity;
        private enum ChainState
        {
            Waiting = 0,
            Extending = 1,
            Extended = 2,
            Retracting = 3
        }
        private ChainState currentState = ChainState.Waiting;
        private float stateTimer = 0f;

        private float waitTime = ModUtils.SecondsToTicks(3);
        private float extendedWaitTime = ModUtils.SecondsToTicks(0.3f);
        private float retractTime = ModUtils.SecondsToTicks(0.5f);
        private float maxChainLength = 600f;

        public int ParentID
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public ref float AICounter => ref Projectile.ai[1];

        private Vector2 retractStartPosition;

        public override void OnSpawn(IEntitySource source)
        {
            NPC npc = Main.npc[ParentID];
            if (!npc.active)
            {
                Projectile.Kill();
            }
            ballVelocity = Vector2.Zero;
        }

        public override void AI()
        {
            NPC npc = Main.npc[ParentID];
            if (!npc.active)
                Projectile.Kill();
            Projectile.timeLeft = 5;

            if (npc.ModNPC is ChainArm arm)
            {
                HandleRecoil(arm, recoilMaxBend, recoilDuration);

                stateTimer++;

                switch (currentState)
                {
                    case ChainState.Waiting:
                        HandleWaitingState(arm);
                        break;
                    case ChainState.Extending:
                        HandleExtendingState(arm);
                        break;
                    case ChainState.Extended:
                        HandleExtendedState();
                        break;
                    case ChainState.Retracting:
                        HandleRetractingState(arm);
                        break;
                }

                if (currentState == ChainState.Waiting || currentState == ChainState.Retracting)
                {
                    Projectile.rotation = arm.GetForearmAngle();
                }
                else if (currentState == ChainState.Extending && ballVelocity.LengthSquared() > 0.1f)
                {
                    Projectile.rotation = ballVelocity.ToRotation();
                }
            }
        }

        private void HandleWaitingState(ChainArm arm)
        {
            float offsetDistance = MathHelper.Lerp(8f, 0f, Math.Abs(arm.BendOffset / 40f));
            Vector2 forearmDirection = new Vector2((float)Math.Cos(arm.GetForearmAngle()), (float)Math.Sin(arm.GetForearmAngle()));
            Projectile.Center = arm.GetHandPosition() + forearmDirection * offsetDistance;
            ballVelocity = Vector2.Zero;

            if (stateTimer >= waitTime)
            {
                currentState = ChainState.Extending;
                stateTimer = 0f;
            }
        }

        private float recoilDuration = 60f;
        private float recoilMaxBend = -40f;
        private void HandleRecoil(ChainArm arm, float maxBend, float duration)
        {
            if (recoilTimer > 0)
            {
                float recoilBend = MathHelper.Lerp(maxBend, 0f, EasingUtils.EaseOutExpo((duration - recoilTimer) / duration));
                arm.BendOffset = recoilBend;
                recoilTimer--;
            }
            else
            {
                arm.BendOffset = 0f;
            }
        }

        private bool hasBeenShot = false;
        private float recoilTimer = 0f;
        private void HandleExtendingState(ChainArm arm)
        {
            if (!hasBeenShot)
            {
                Vector2 fireDirection = Vector2.Normalize(Main.LocalPlayer.Center - Projectile.Center);
                ballVelocity = fireDirection * 30f;
                hasBeenShot = true;

                recoilTimer = 60f;
                recoilDuration = 60f;
                recoilMaxBend = -40f;
            }

            UpdateBallPhysics(arm.GetHandPosition());

            Point tileCheck = Projectile.Center.ToTileCoordinates();
            if (WorldGen.SolidTile(tileCheck.X, tileCheck.Y))
            {
                currentState = ChainState.Extended;
                stateTimer = 0f;
                ballVelocity = Vector2.Zero;
                return;
            }

            Vector2 chainVector = Projectile.Center - arm.GetHandPosition();
            float currentDistance = chainVector.Length();
            if (currentDistance >= maxChainLength)
            {
                currentState = ChainState.Retracting;
                stateTimer = 0f;
                ballVelocity = Vector2.Zero;
            }
        }

        private void HandleExtendedState()
        {
            ballVelocity = Vector2.Zero;

            if (stateTimer >= extendedWaitTime)
            {
                currentState = ChainState.Retracting;
                stateTimer = 0f;
            }
        }

        private void HandleRetractingState(ChainArm arm)
        {
            if (stateTimer == 1f)
            {
                retractStartPosition = Projectile.Center;
            }

            float progress = Math.Min(stateTimer / retractTime, 1f);
            float offsetDistance = MathHelper.Lerp(8f, 0f, Math.Abs(arm.BendOffset / 40f));
            Vector2 forearmDirection = new Vector2((float)Math.Cos(arm.GetForearmAngle()), (float)Math.Sin(arm.GetForearmAngle()));
            Vector2 targetPosition = arm.GetHandPosition() + forearmDirection * offsetDistance;

            Projectile.Center = Vector2.Lerp(retractStartPosition, targetPosition, progress);
            ballVelocity = Vector2.Zero;

            if (progress >= 1f)
            {
                currentState = ChainState.Waiting;
                stateTimer = 0f;
                hasBeenShot = false;

                recoilTimer = 90f;
                recoilDuration = 90f;
                recoilMaxBend = -40f;
            }
        }

        private void UpdateBallPhysics(Vector2 anchorPoint)
        {
            Vector2 chainVector = Projectile.Center - anchorPoint;
            float currentLength = chainVector.Length();

            if (currentLength > maxChainLength)
            {
                Vector2 constraintDirection = chainVector / currentLength;
                Projectile.Center = anchorPoint + constraintDirection * maxChainLength;

                Vector2 velocityAlongChain = Vector2.Dot(ballVelocity, constraintDirection) * constraintDirection;
                if (Vector2.Dot(velocityAlongChain, constraintDirection) > 0)
                {
                    ballVelocity -= velocityAlongChain;
                }
            }

            ballVelocity *= 0.995f;
            Projectile.Center += ballVelocity;
        }

        private void DrawChain(ChainArm arm)
        {
            if (currentState == ChainState.Waiting) return; // No chain when waiting

            Texture2D chainTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadChain").Value;
            Vector2 chainDirection = Projectile.Center - arm.GetHandPosition();
            float chainDistance = chainDirection.Length();
            float chainRotation = chainDirection.ToRotation();
            int chainSegmentHeight = chainTexture.Height;
            int numSegments = (int)(chainDistance / chainSegmentHeight) + 1;

            for (int i = 0; i < numSegments; i++)
            {
                float progress = (float)i / numSegments;
                Vector2 segmentPosition = Vector2.Lerp(arm.GetHandPosition(), Projectile.Center, progress);
                Vector2 segmentScreenPos = segmentPosition - Main.screenPosition;
                Rectangle sourceRect = new Rectangle(0, 0, chainTexture.Width, chainSegmentHeight);
                Vector2 origin = new Vector2(chainTexture.Width / 2f, 0);

                Main.spriteBatch.Draw(chainTexture, segmentScreenPos, sourceRect, Color.White,
                    chainRotation + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            NPC npc = Main.npc[ParentID];
            if (npc.active && npc.ModNPC is ChainArm arm)
            {
                DrawChain(arm);
            }

            Texture2D ballTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadFlail").Value;
            Vector2 ballScreenPos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(ballTexture, ballScreenPos, null, Color.White, Projectile.rotation + MathHelper.PiOver2, ballTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}