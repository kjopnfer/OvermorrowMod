using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
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
        private Vector2 anchorPoint;
        private Vector2 extendDirection;
        private float currentChainLength = 30f;

        private float chainLength = 200f;
        private float maxChainLength = 600f;
        private float waitTime = ModUtils.SecondsToTicks(3);
        private float extendTime = ModUtils.SecondsToTicks(0.5f);
        private float extendedWaitTime = ModUtils.SecondsToTicks(0.3f);
        private float retractTime = ModUtils.SecondsToTicks(0.5f);

        private float groundHitDelay = ModUtils.SecondsToTicks(1f);
        private bool hitGround = false;

        public int ParentID
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public ref float AICounter => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
            NPC npc = Main.npc[ParentID];

            if (!npc.active)
            {
                Projectile.Kill();
            }

            ballVelocity = Vector2.Zero;
        }

        private Vector2 retractStartPosition;
        public override void AI()
        {
            NPC npc = Main.npc[ParentID];
            if (!npc.active)
                Projectile.Kill();
            Projectile.timeLeft = 5;

            if (npc.ModNPC is ChainArm arm)
            {
                anchorPoint = arm.GetArmEndPosition();

                float armEndRotation = 0f;
                if (arm.armLimb != null && arm.armLimb.Segments.Length > 1)
                {
                    armEndRotation = arm.armLimb.Segments[arm.armLimb.Segments.Length - 1].Angle;
                }

                extendDirection = new Vector2(1, 0).RotatedBy(armEndRotation);

                stateTimer++;

                switch (currentState)
                {
                    case ChainState.Waiting:
                        HandleWaitingState();
                        break;
                    case ChainState.Extending:
                        HandleExtendingState();
                        break;
                    case ChainState.Extended:
                        HandleExtendedState();
                        break;
                    case ChainState.Retracting:
                        HandleRetractingState();
                        break;
                }

                if (currentState == ChainState.Waiting || currentState == ChainState.Retracting)
                {
                    Projectile.rotation = armEndRotation;
                }
                else if (currentState == ChainState.Extending)
                {
                    if (ballVelocity.LengthSquared() > 0.1f)
                    {
                        float targetRotation = ballVelocity.ToRotation();
                        float rotationDifference = MathHelper.WrapAngle(targetRotation - Projectile.rotation);
                        Projectile.rotation += rotationDifference * 0.1f;
                    }
                }
            }
        }

        private void HandleWaitingState()
        {
            Projectile.Center = anchorPoint + extendDirection * 30f;
            ballVelocity = Vector2.Zero;

            if (stateTimer >= waitTime)
            {
                currentState = ChainState.Extending;
                stateTimer = 0f;
            }
        }

        private bool hasBeenShot = false;
        private void HandleExtendingState()
        {
            if (!hasBeenShot)
            {
                ballVelocity = extendDirection * 30f;
                hasBeenShot = true;
                hitGround = false;
            }

            UpdateBallPhysics(anchorPoint);

            Point tileCheck = Projectile.Center.ToTileCoordinates();
            if (WorldGen.SolidTile(tileCheck.X, tileCheck.Y))
            {
                currentState = ChainState.Extended;
                stateTimer = 0f;
                ballVelocity = Vector2.Zero;
                hitGround = true;
                return;
            }

            Vector2 chainVector = Projectile.Center - anchorPoint;
            float currentDistance = chainVector.Length();
            if (currentDistance >= maxChainLength)
            {
                currentState = ChainState.Retracting;
                stateTimer = 0f;
                ballVelocity = Vector2.Zero;
                hitGround = false;
            }
        }

        private void HandleExtendedState()
        {
            ballVelocity = Vector2.Zero;

            float waitDuration = hitGround ? groundHitDelay : extendedWaitTime;

            if (stateTimer >= waitDuration)
            {
                currentState = ChainState.Retracting;
                stateTimer = 0f;
            }
        }

        private void HandleRetractingState()
        {
            if (stateTimer == 1f)
            {
                retractStartPosition = Projectile.Center;
            }

            float progress = Math.Min(stateTimer / retractTime, 1f);
            Vector2 targetPosition = anchorPoint + extendDirection * 30f;
            float easedProgress = EasingUtils.EaseInQuad(progress);
            Projectile.Center = Vector2.Lerp(retractStartPosition, targetPosition, easedProgress);
            ballVelocity = Vector2.Zero;

            if (progress >= 1f)
            {
                currentState = ChainState.Waiting;
                stateTimer = 0f;
                hasBeenShot = false;
                currentChainLength = 30f;
            }
        }

        private void UpdateBallPhysics(Vector2 anchorPoint)
        {
            float currentMaxLength = currentState == ChainState.Extending ? maxChainLength : chainLength;
            Vector2 chainVector = Projectile.Center - anchorPoint;
            float currentLength = chainVector.Length();

            if (currentLength > currentMaxLength)
            {
                Vector2 constraintDirection = chainVector / currentLength;
                Projectile.Center = anchorPoint + constraintDirection * currentMaxLength;

                Vector2 velocityAlongChain = Vector2.Dot(ballVelocity, constraintDirection) * constraintDirection;
                if (Vector2.Dot(velocityAlongChain, constraintDirection) > 0)
                {
                    ballVelocity -= velocityAlongChain;
                }
            }

            ballVelocity *= 0.995f;
            Projectile.Center += ballVelocity;

            Point tileCheck = Projectile.Center.ToTileCoordinates();
            if (WorldGen.SolidTile(tileCheck.X, tileCheck.Y))
            {
                ballVelocity.Y *= -0.4f;
                ballVelocity.X *= 0.8f;
            }
        }

        private void DrawChain()
        {
            Texture2D chainTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadChain").Value;
            Vector2 chainDirection = Projectile.Center - anchorPoint;
            float chainDistance = chainDirection.Length();
            float chainRotation = chainDirection.ToRotation();
            int chainSegmentHeight = chainTexture.Height;
            int numSegments = (int)(chainDistance / chainSegmentHeight) + 1;

            for (int i = 0; i < numSegments; i++)
            {
                float progress = (float)i / numSegments;
                Vector2 segmentPosition = Vector2.Lerp(anchorPoint, Projectile.Center, progress);
                Vector2 segmentScreenPos = segmentPosition - Main.screenPosition;
                Rectangle sourceRect = new Rectangle(0, 0, chainTexture.Width, chainSegmentHeight);
                Vector2 origin = new Vector2(chainTexture.Width / 2f, 0);

                Main.spriteBatch.Draw(chainTexture, segmentScreenPos, sourceRect, Color.White,
                    chainRotation + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawChain();

            Texture2D ballTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadFlail").Value;
            Vector2 ballScreenPos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(ballTexture, ballScreenPos, null, Color.White, Projectile.rotation + MathHelper.PiOver2, ballTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}