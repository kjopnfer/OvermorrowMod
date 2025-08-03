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
        private int chainLinkCount = 20;
        private float linkDistance = 14f;
        private float gravityFactor = 0.2f;
        private float airResistance = 0.95f;
        private float tileFriction = 0.05f;
        private int constraintIterations = 4;
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


                // Update chain physics anchored to the end of the arm
                Vector2 anchorPoint = armLimb.GetEndPosition();
                UpdateChainPhysics(anchorPoint);

                // Keep projectile center at the end of the chain (ball)
                if (chainLinks.Count > 0)
                {
                    Projectile.Center = chainLinks[chainLinks.Count - 1].Position;
                }

                lastAnchorPoint = anchorPoint;

                if (AICounter % 10 == 0) // Every 10 frames
                {
                    Vector2 armTip = armLimb.GetEndPosition();
                    Vector2 mousePos = Main.MouseWorld;
                    Vector2 firstChainLink = chainLinks[0].Position;

                    float armToMouse = Vector2.Distance(armTip, mousePos);
                    float armToChain = Vector2.Distance(armTip, firstChainLink);
                    float chainVelocity = chainLinks[0].Velocity.Length();

                    Core.OvermorrowModFile.Instance.Logger.Debug($"ArmLag: {armToMouse:F1}, ChainLag: {armToChain:F1}, ChainVel: {chainVelocity:F2}");
                }
            }
        }

        private void UpdateChainPhysics(Vector2 anchorPoint)
        {
            Vector2 anchorMovement = anchorPoint - lastAnchorPoint;
            float armSpeed = anchorMovement.Length();
            bool armIsMoving = armSpeed > 1f; // Only consider significant movement

            for (int i = 0; i < chainLinks.Count; i++)
            {
                ChainLink link = chainLinks[i];
                link.OldPosition = link.Position;

                // Apply gravity - normal when still, reduced when swinging
                float dynamicGravity = gravityFactor;
                if (armIsMoving && armSpeed > 3f)
                {
                    dynamicGravity *= 0.7f; // Less aggressive reduction
                }
                link.Velocity.Y += dynamicGravity;

                if (i == 0)
                {
                    if (armIsMoving)
                    {
                        // Only boost responsiveness for actual movement
                        float responsiveness = 0.2f + (armSpeed * 0.05f); // Scales with speed
                        responsiveness = Math.Min(responsiveness, 0.5f); // Cap it

                        link.Velocity += anchorMovement * responsiveness;

                        // Add swinging force only for fast movement
                        if (armSpeed > 4f)
                        {
                            Vector2 centerToLink = link.Position - anchorPoint;
                            Vector2 tangential = new Vector2(-centerToLink.Y, centerToLink.X).SafeNormalize(Vector2.Zero);
                            link.Velocity += tangential * armSpeed * 0.1f; // Reduced from 0.3f
                        }
                    }
                    else
                    {
                        // When arm is still, apply settling force
                        link.Velocity *= 0.95f; // Dampen when not moving
                    }
                }
                else
                {
                    // Only propagate energy if previous link is actually moving
                    Vector2 prevMovement = chainLinks[i - 1].Position - chainLinks[i - 1].OldPosition;
                    float prevSpeed = prevMovement.Length();

                    if (prevSpeed > 0.5f) // Only propagate significant movement
                    {
                        float propagation = 0.3f - (i * 0.05f); // Reduced from 0.6f
                        link.Velocity += prevMovement * propagation;
                    }

                    // Weight
                    float weightMultiplier = 1f + (i * 0.08f);
                    link.Velocity.Y += dynamicGravity * (weightMultiplier - 1f);

                    // Extra damping for links further down when arm is still
                    if (!armIsMoving)
                    {
                        link.Velocity *= 0.92f - (i * 0.01f); // More damping down the chain
                    }
                }

                Vector2 newPosition = link.Position + link.Velocity;

                // Tile collision
                Point tileCheck = newPosition.ToTileCoordinates();
                if (WorldGen.SolidTile(tileCheck.X, tileCheck.Y))
                {
                    newPosition.Y = link.Position.Y;
                    link.Velocity.Y *= -0.3f;
                }

                link.Position = newPosition;

                // Adaptive friction - more when still, less when swinging
                float friction = armIsMoving ? 0.98f : 0.94f;
                link.Velocity *= friction;

                chainLinks[i] = link;
            }

            // Stronger constraints to prevent excessive stretching
            for (int iteration = 0; iteration < 4; iteration++) // Increased from 2
            {
                ApplyDistanceConstraints(anchorPoint);
            }

            // More conservative velocity update
            for (int i = 0; i < chainLinks.Count; i++)
            {
                ChainLink link = chainLinks[i];
                Vector2 constraintVelocity = link.Position - link.OldPosition;

                // Blend conservatively to prevent instability
                link.Velocity = Vector2.Lerp(link.Velocity, constraintVelocity, 0.5f); // Reduced from 0.7f

                // Velocity limiting to prevent explosions
                if (link.Velocity.LengthSquared() > 100f) // Max 10 pixels per frame
                {
                    link.Velocity = Vector2.Normalize(link.Velocity) * 10f;
                }

                chainLinks[i] = link;
            }

            // Nearly instant settling when arm stops moving
            if (armSpeed < 1f) // Arm is essentially still
            {
                float totalEnergy = 0f;
                for (int i = 0; i < chainLinks.Count; i++)
                {
                    totalEnergy += chainLinks[i].Velocity.LengthSquared();
                }

                // Extremely aggressive damping for instant settling
                float energyDamping = 0.95f; // Base: remove only 5% per frame for very low energy

                if (totalEnergy > 2f) energyDamping = 0.3f;   // Remove 70% per frame
                if (totalEnergy > 10f) energyDamping = 0.1f;   // Remove 90% per frame  
                if (totalEnergy > 50f) energyDamping = 0.02f;  // Remove 98% per frame
                if (totalEnergy > 100f) energyDamping = 0.01f;  // Remove 99% per frame

                for (int i = 0; i < chainLinks.Count; i++)
                {
                    ChainLink link = chainLinks[i];

                    // Apply exponential damping
                    link.Velocity *= energyDamping;

                    // Much stronger positional correction toward natural hanging position
                    Vector2 expectedPos = anchorPoint + new Vector2(0, (i + 1) * linkDistance);
                    Vector2 toRest = expectedPos - link.Position;

                    // Strong correction force that increases with distance from rest
                    float correctionStrength = Math.Min(toRest.Length() * 0.1f, 2f); // Cap at 2 pixels per frame
                    if (totalEnergy > 1f)
                    {
                        link.Velocity += Vector2.Normalize(toRest) * correctionStrength;
                    }

                    // Stop tiny movements completely with lower threshold
                    if (link.Velocity.LengthSquared() < 0.1f) // Much more aggressive stopping
                    {
                        link.Velocity = Vector2.Zero;
                    }

                    chainLinks[i] = link;
                }

                // If total energy is very low, force all links to hang naturally
                if (totalEnergy < 1f)
                {
                    for (int i = 0; i < chainLinks.Count; i++)
                    {
                        ChainLink link = chainLinks[i];
                        Vector2 naturalPos = anchorPoint + new Vector2(0, (i + 1) * linkDistance);

                        // Lerp toward natural position very quickly
                        link.Position = Vector2.Lerp(link.Position, naturalPos, 0.2f); // 20% per frame
                        link.Velocity = Vector2.Zero; // Kill all velocity

                        chainLinks[i] = link;
                    }
                }
            }

            // Also add oscillation prevention
            for (int i = 1; i < chainLinks.Count; i++)
            {
                ChainLink currentLink = chainLinks[i];
                ChainLink prevLink = chainLinks[i - 1];

                Vector2 currentVel = currentLink.Velocity;
                Vector2 prevVel = prevLink.Velocity;

                // If adjacent links are moving in opposite directions, dampen both
                if (Vector2.Dot(currentVel, prevVel) < -0.5f) // Opposing motion
                {
                    currentLink.Velocity *= 0.6f;
                    prevLink.Velocity *= 0.6f;

                    // Put the modified structs back into the list
                    chainLinks[i] = currentLink;
                    chainLinks[i - 1] = prevLink;
                }
            }

            // In UpdateChainPhysics(), add this at the end:
            if (AICounter % 15 == 0)
            {
                float totalEnergy = 0f;
                float maxVelocity = 0f;
                int movingLinks = 0;

                for (int i = 0; i < chainLinks.Count; i++)
                {
                    float linkEnergy = chainLinks[i].Velocity.LengthSquared();
                    totalEnergy += linkEnergy;
                    maxVelocity = Math.Max(maxVelocity, chainLinks[i].Velocity.Length());
                    if (chainLinks[i].Velocity.Length() > 0.5f) movingLinks++;
                }

                Core.OvermorrowModFile.Instance.Logger.Debug($"TotalEnergy: {totalEnergy:F1}, MaxVel: {maxVelocity:F1}, MovingLinks: {movingLinks}, ArmSpeed: {armSpeed:F1}");
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