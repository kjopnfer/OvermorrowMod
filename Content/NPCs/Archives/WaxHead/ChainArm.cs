using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public partial class Waxhead
    {
        // ChainArm specific fields
        private Texture2D upperArmTexture;
        private Texture2D forearmTexture;
        private int ballID = -1;
        private Vector2 currentDirection = Vector2.UnitX;

        private float _bendOffset;
        public float BendOffset
        {
            get => _bendOffset;
            set => _bendOffset = MathHelper.Clamp(value, -40f, 40f);
        }

        public Vector2 AnchorPoint { get; private set; }
        public Vector2 ElbowJoint { get; private set; }
        public Vector2 HandJoint { get; private set; }

        private float currentAngle = 0f;
        private int spinDirection = 1;
        private void InitializeChainArm(IEntitySource source)
        {
            upperArmTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassArm1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            forearmTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassArm2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            ballID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ChainBall>(), NPC.damage, 0, Main.myPlayer, NPC.whoAmI);
        }

        private void UpdateChainArm()
        {
            if (ballID == -1) return;

            ChainBall chainBall = Main.projectile[ballID].ModProjectile as ChainBall;

            var frameOffsets = new Dictionary<int, int>
            {
                [0] = -48,
                [1] = -56,
                [2] = -64,
                [3] = -56,
                [4] = -52,
                [5] = -52,
                [6] = -46,
                [7] = -58,
                [8] = -64,
                [9] = -54,
                [10] = -52,
                [11] = -48,
                [12] = -46
            };

            int yOffset = frameOffsets.TryGetValue(yFrame, out int offset) ? offset - 4 : -54;
            AnchorPoint = NPC.Center + new Vector2(-8 * NPC.direction, yOffset);

            if (CurrentState == WaxheadState.Idle)
            {
                // Calculate target angle for idle animation
                var frameAngles = new Dictionary<int, float>
                {
                    [0] = 120,
                    [1] = 100,
                    [2] = 90,
                    [3] = 80,
                    [4] = 70,
                    [5] = 60,
                    [6] = 50,
                    [7] = 70,
                    [8] = 80,
                    [9] = 90,
                    [10] = 100,
                    [11] = 120,
                    [12] = 130,
                };
                float frameAngle = frameAngles.TryGetValue(yFrame, out float angle) ? angle : 130;

                if (NPC.direction == 1)
                    frameAngle = 180f - frameAngle;

                float targetAngle = MathHelper.ToRadians(frameAngle);

                // Smoothly rotate towards target angle
                if (chainBall.CurrentState == ChainBall.ChainState.Waiting)
                {
                    currentAngle = MathHelper.Lerp(currentAngle, targetAngle, 0.08f);
                }
            }
            else if (CurrentState == WaxheadState.SpinAttack)
            {
                AnchorPoint += new Vector2(4 * NPC.direction, 2);
                float totalSpinTime = attackTime * 2;
                float windupTime = totalSpinTime * 0.25f;
                float mainSpinTime = totalSpinTime * 0.5f;
                float winddownTime = totalSpinTime * 0.25f;

                float spinSpeed = 0.2f;

                if (AICounter == 1)
                    spinDirection = NPC.direction;


                if (AICounter <= windupTime)
                {
                    float windupProgress = AICounter / windupTime;
                    float currentSpinSpeed = spinSpeed * windupProgress * windupProgress;
                    currentAngle += currentSpinSpeed * spinDirection;
                }
                else if (AICounter <= windupTime + mainSpinTime)
                {
                    currentAngle += spinSpeed * spinDirection;
                }
                else
                {
                    float winddownProgress = (AICounter - windupTime - mainSpinTime) / winddownTime;
                    float slowdownFactor = 1f - (winddownProgress * winddownProgress);
                    float currentSpinSpeed = spinSpeed * slowdownFactor;
                    currentAngle += currentSpinSpeed * spinDirection;
                }

                // Normalize angle to prevent spinning issues when transitioning to other states
                currentAngle = MathHelper.WrapAngle(currentAngle);
            }
            else // Attack state
            {
                float targetAngle = (Main.LocalPlayer.Center - AnchorPoint).ToRotation();

                if (chainBall.CurrentState == ChainBall.ChainState.Waiting)
                {
                    currentAngle = MathHelper.Lerp(currentAngle, targetAngle, 0.05f);
                }
            }

            // Convert angle to direction for all calculations
            currentDirection = new Vector2((float)Math.Cos(currentAngle), (float)Math.Sin(currentAngle));

            float armLength = 130f;
            HandJoint = AnchorPoint + currentDirection * armLength;

            Vector2 straightElbow = Vector2.Lerp(AnchorPoint, HandJoint, 0.45f);
            Vector2 perpendicular = new Vector2(-currentDirection.Y, currentDirection.X);
            Vector2 backwardDirection = -currentDirection;
            float backwardAmount = Math.Abs(BendOffset) * 0.5f;

            ElbowJoint = straightElbow + perpendicular * BendOffset + backwardDirection * backwardAmount;
            HandJoint += perpendicular * -BendOffset * 2;
        }

        private void DrawChainArmDebugDust()
        {
            return;
            int shoulder = Dust.NewDust(AnchorPoint, 1, 1, DustID.Torch);
            Main.dust[shoulder].noGravity = true;

            int elbow = Dust.NewDust(ElbowJoint, 1, 1, DustID.RedTorch);
            Main.dust[elbow].noGravity = true;

            int hand = Dust.NewDust(HandJoint, 1, 1, DustID.BlueTorch);
            Main.dust[hand].noGravity = true;
        }

        private void DrawChain(SpriteBatch spriteBatch, ChainBall chainBall)
        {
            if (chainBall.CurrentState == ChainBall.ChainState.Waiting) return; // No chain when waiting

            Texture2D chainTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadChain").Value;
            Vector2 forearmAnchor = GetForearmAnchor(chainBall);
            Vector2 chainDirection = chainBall.Projectile.Center - forearmAnchor;
            float chainDistance = chainDirection.Length();
            float chainRotation = chainDirection.ToRotation();
            int chainSegmentHeight = chainTexture.Height;
            int numSegments = (int)(chainDistance / chainSegmentHeight) + 1;

            for (int i = 0; i < numSegments; i++)
            {
                float progress = (float)i / numSegments;
                Vector2 segmentPosition = Vector2.Lerp(forearmAnchor, chainBall.Projectile.Center, progress);
                Vector2 segmentScreenPos = segmentPosition - Main.screenPosition;
                Rectangle sourceRect = new Rectangle(0, 0, chainTexture.Width, chainSegmentHeight);
                Vector2 origin = new Vector2(chainTexture.Width / 2f, 0);

                spriteBatch.Draw(chainTexture, segmentScreenPos, sourceRect, Color.White,
                    chainRotation + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        private Vector2 GetForearmAnchor(ChainBall chainBall)
        {
            float recoilProgress = chainBall.recoilTimer > 0 ? chainBall.recoilTimer / chainBall.recoilDuration : 0f;
            float lerpValue = MathHelper.Lerp(1f, 0.7f, recoilProgress);
            return Vector2.Lerp(ElbowJoint, HandJoint, lerpValue);
        }

        private void DrawChainArm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (upperArmTexture == null || forearmTexture == null) return;

            // Draw the chain first (behind the arm)
            if (ballID != -1 && Main.projectile[ballID].active)
            {
                ChainBall chainBall = Main.projectile[ballID].ModProjectile as ChainBall;
                if (chainBall != null)
                {
                    DrawChain(spriteBatch, chainBall);
                }
            }

            // Then draw the arm parts
            Vector2 anchorScreen = AnchorPoint - Main.screenPosition;
            Vector2 elbowScreen = ElbowJoint - Main.screenPosition;

            float upperArmAngle = (ElbowJoint - AnchorPoint).ToRotation();
            float forearmAngle = (HandJoint - ElbowJoint).ToRotation();

            var spriteEffects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(upperArmTexture, anchorScreen, null, drawColor, upperArmAngle - MathHelper.PiOver2, new Vector2(upperArmTexture.Width / 2f, 8), 1f, spriteEffects, 0f);
            spriteBatch.Draw(forearmTexture, elbowScreen, null, drawColor, forearmAngle - MathHelper.PiOver2, new Vector2(forearmTexture.Width / 2f, 0), 1f, spriteEffects, 0f);
        }

        public Vector2 GetHandPosition()
        {
            return HandJoint;
        }

        public float GetForearmAngle()
        {
            return (HandJoint - ElbowJoint).ToRotation();
        }

        public Vector2 GetArmDirection()
        {
            return Vector2.Normalize(Main.LocalPlayer.Center - AnchorPoint);
        }
    }
}