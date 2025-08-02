using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.InverseKinematics;
using OvermorrowMod.Common.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Tools
{
    public class ChainBall : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = ModUtils.SecondsToTicks(20);
            Projectile.penetrate = -1;
        }

        private InverseKinematicLimb armLimb;
        private Vector2 shoulderPosition;
        private bool armInitialized = false;

        // Chain physics fields
        private List<ChainLink> chainLinks;
        private Vector2 lastAnchorPoint;
        private int chainLinkCount = 12;
        private float linkDistance = 14f;
        private float gravityFactor = 0.25f;
        private float airResistance = 0.95f;
        private float tileFriction = 0.05f;
        private int constraintIterations = 3;
        private float dragEnergyLoss = 0.3f;
        private float maxDragDistance = 20f;

        public override void OnSpawn(IEntitySource source)
        {
            shoulderPosition = Projectile.Center;
        }

        private void InitializeArm()
        {
            try
            {
                Texture2D upperArmTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassArm1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Texture2D forearmTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassArm2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                float[] segmentLengths = new float[] { upperArmTexture.Height - 16, forearmTexture.Height };
                Texture2D[] segmentTextures = new Texture2D[] { upperArmTexture, forearmTexture };
                Vector2[] origins = new Vector2[]
                {
                   new Vector2(upperArmTexture.Width / 2f, 0),    // Left side, middle for upper arm
                   new Vector2(forearmTexture.Width / 2f, 0)      // Left side, middle for forearm
                };

                armLimb = new InverseKinematicLimb(shoulderPosition.X, shoulderPosition.Y, 2, segmentLengths, 0f, segmentTextures, origins);

                armInitialized = true;
                InitializeChain();
            }
            catch (Exception ex)
            {
                Main.NewText("Error initializing arm: " + ex.Message);
            }
        }

        private void InitializeChain()
        {
            chainLinks = new List<ChainLink>();
            Vector2 anchorPoint = shoulderPosition;
            lastAnchorPoint = anchorPoint;

            for (int i = 0; i < chainLinkCount; i++)
            {
                Vector2 position = anchorPoint + new Vector2(0, (i + 1) * linkDistance);
                chainLinks.Add(new ChainLink
                {
                    Position = position,
                    OldPosition = position,
                    Velocity = Vector2.Zero,
                    IsResting = false
                });
            }
        }

        public ref float AICounter => ref Projectile.ai[0];

        public override void AI()
        {
            AICounter++;

            // Initialize arm on first frame after spawn
            if (!armInitialized && AICounter == 1)
            {
                InitializeArm();
            }

            if (armInitialized && armLimb != null)
            {
                // Update arm to point toward mouse
                Vector2 targetPosition = Main.MouseWorld;
                armLimb.Update(targetPosition);

                armLimb.Segments[0].MinAngle = MathHelper.ToRadians(0f);
                armLimb.Segments[0].MaxAngle = MathHelper.ToRadians(360f);

                // Set angle constraints for the second segment (lower limb/forearm)
                // Prevent it from rotating fully backwards when facing backwards
                armLimb.Segments[1].MinAngle = armLimb.Segments[0].Angle;
                armLimb.Segments[1].MaxAngle = MathHelper.ToRadians(180f);
                if (armLimb.Segments[1].Angle >= MathHelper.ToRadians(355))
                    armLimb.Segments[1].MaxAngle = MathHelper.ToRadians(360f);

                float upperArmAngle = MathHelper.ToDegrees(armLimb.Segments[0].Angle);
                float forearmAngle = MathHelper.ToDegrees(armLimb.Segments[1].Angle);
                float forearmMinAngle = MathHelper.ToDegrees(armLimb.Segments[1].MinAngle);
                float forearmMaxAngle = MathHelper.ToDegrees(armLimb.Segments[1].MaxAngle);

                Main.NewText($"Upper Arm: {upperArmAngle:F1}°");
                Main.NewText($"Forearm: {forearmAngle:F1}° (Range: {forearmMinAngle:F1}° to {forearmMaxAngle:F1}°)");

                // Update chain physics anchored to the end of the arm
                Vector2 anchorPoint = armLimb.GetEndPosition();
                UpdateChainPhysics(anchorPoint);

                // Keep projectile center at the end of the chain (ball)
                if (chainLinks.Count > 0)
                {
                    Projectile.Center = chainLinks[chainLinks.Count - 1].Position;
                }

                lastAnchorPoint = anchorPoint;
            }
        }

        private void UpdateChainPhysics(Vector2 anchorPoint)
        {
            Vector2 anchorMovement = anchorPoint - lastAnchorPoint;

            for (int i = 0; i < chainLinks.Count; i++)
            {
                ChainLink link = chainLinks[i];
                link.OldPosition = link.Position;

                // Apply gravity
                link.Velocity.Y += gravityFactor;

                if (i == 0)
                {
                    // First link follows the arm end
                    float responsiveness = 0.02f;
                    link.Velocity += anchorMovement * responsiveness;
                }
                else
                {
                    // Links further down are heavier
                    float weightMultiplier = 1f + (i * 0.1f);
                    link.Velocity.Y += gravityFactor * weightMultiplier;
                }

                Vector2 newPosition = link.Position + link.Velocity;

                // Apply basic tile collision (simplified)
                bool touchingTile = false;
                Point tileCheck = newPosition.ToTileCoordinates();
                if (WorldGen.SolidTile(tileCheck.X, tileCheck.Y))
                {
                    newPosition.Y = link.Position.Y;
                    link.Velocity.Y *= -0.3f; // Bounce
                    touchingTile = true;
                }

                link.Position = newPosition;

                // Apply friction
                float frictionMultiplier = touchingTile ? tileFriction : airResistance;
                link.Velocity *= frictionMultiplier;

                chainLinks[i] = link;
            }

            // Apply distance constraints
            for (int iteration = 0; iteration < constraintIterations; iteration++)
            {
                ApplyDistanceConstraints(anchorPoint);
            }
        }

        private void ApplyDistanceConstraints(Vector2 anchorPoint)
        {
            for (int i = 0; i < chainLinks.Count; i++)
            {
                Vector2 targetPosition = (i == 0) ? anchorPoint : chainLinks[i - 1].Position;
                Vector2 currentPosition = chainLinks[i].Position;

                Vector2 difference = currentPosition - targetPosition;
                float distance = difference.Length();

                if (distance > linkDistance)
                {
                    Vector2 direction = difference / distance;
                    chainLinks[i] = new ChainLink
                    {
                        Position = targetPosition + direction * linkDistance,
                        OldPosition = chainLinks[i].OldPosition,
                        Velocity = chainLinks[i].Velocity,
                        IsResting = chainLinks[i].IsResting
                    };
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Debug circle at projectile center
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Vector2 screenPos = Projectile.Center - Main.screenPosition;
            Rectangle rect = new Rectangle((int)screenPos.X - 10, (int)screenPos.Y - 10, 20, 20);
            Main.spriteBatch.Draw(pixel, rect, Color.Red);

            if (armInitialized && armLimb != null)
            {
                try
                {
                    armLimb.Draw(Main.spriteBatch, lightColor);

                    // Draw chain links
                    if (chainLinks != null && chainLinks.Count > 0)
                    {
                        Vector2 anchorPoint = armLimb.GetEndPosition();
                        Texture2D chainTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadChain").Value;
                        Texture2D ballTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadFlail").Value;

                        // Draw chain links
                        for (int i = 0; i < chainLinks.Count; i++)
                        {
                            Vector2 startPos = (i == 0) ? anchorPoint : chainLinks[i - 1].Position;
                            Vector2 endPos = chainLinks[i].Position;

                            Vector2 screenStart = startPos - Main.screenPosition;
                            Vector2 linkDir = endPos - startPos;
                            float rotation = linkDir.ToRotation() + MathHelper.PiOver2;

                            Main.spriteBatch.Draw(chainTexture, screenStart, null, Color.White, rotation,
                                new Vector2(chainTexture.Width / 2f, chainTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
                        }

                        // Draw ball at end
                        Vector2 ballPos = chainLinks[chainLinks.Count - 1].Position - Main.screenPosition;
                        Main.spriteBatch.Draw(ballTexture, ballPos, null, Color.White, 0f,
                            ballTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
                    }

                    Vector2 armEndPos = armLimb.GetEndPosition() - Main.screenPosition;
                    Rectangle endRect = new Rectangle((int)armEndPos.X - 5, (int)armEndPos.Y - 5, 10, 10);
                    Main.spriteBatch.Draw(pixel, endRect, Color.Blue);
                }
                catch (Exception ex)
                {
                    Main.NewText("Error drawing arm: " + ex.Message);
                }
            }

            return false;
        }

        private struct ChainLink
        {
            public Vector2 Position;
            public Vector2 OldPosition;
            public Vector2 Velocity;
            public bool IsResting;
        }
    }
}