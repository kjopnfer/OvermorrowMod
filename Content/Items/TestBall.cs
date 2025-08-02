using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
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

        private List<ChainLink> chainLinks;
        private Vector2 lastAnchorPoint;
        private int chainLinkCount = 12;          // Number of chain segments
        private float linkDistance = 14f;        // Distance between each link
        private float gravityFactor = 0.25f;           // Downward force (higher = heavier feel)
        private float airResistance = 0.95f;           // Air resistance (lower = more drag)
        private float tileFriction = 0.05f;      // Ground friction (lower = more sliding)
        private int constraintIterations = 3;    // Physics stability (higher = stiffer)

        private float dragEnergyLoss = 0.3f;      // Energy RETENTION when dragging (lower = more loss)
        private float maxDragDistance = 20f;       // Distance at which full drag kicks in

        private float movementSensitivity = 0.03f;   // Reduced from 0.2f to 0.1f
        public override void OnSpawn(IEntitySource source)
        {
            InitializeChain();
        }

        private void InitializeChain()
        {
            chainLinks = new List<ChainLink>();
            Vector2 anchorPoint = Main.MouseWorld;
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

            Projectile.Center = chainLinks[chainLinks.Count - 1].Position;
        }

        public ref float AICounter => ref Projectile.ai[0];

        public override void AI()
        {
            AICounter++;
            Vector2 anchorPoint = Main.MouseWorld;

            UpdateChainPhysics(anchorPoint);

            Projectile.Center = chainLinks[chainLinks.Count - 1].Position;
            //Projectile.rotation += chainLinks[chainLinks.Count - 1].Velocity.X * 0.05f;

            lastAnchorPoint = anchorPoint;
        }

        private void UpdateChainPhysics(Vector2 anchorPoint)
        {
            Vector2 anchorMovement = anchorPoint - lastAnchorPoint;

            for (int i = 0; i < chainLinks.Count; i++)
            {
                ChainLink link = chainLinks[i];

                link.OldPosition = link.Position;

                // Check if link should be considered resting
                bool wasResting = link.IsResting;
                link.IsResting = IsLinkResting(link.Position, link.Velocity);

                // Only apply gravity if not resting or if disturbed by anchor movement
                if (!link.IsResting || Vector2.Distance(anchorPoint, lastAnchorPoint) > 2f)
                {
                    link.Velocity.Y += gravityFactor;
                    link.IsResting = false; // Movement breaks resting state
                }

                if (i == 0)
                {
                    // Calculate distance from anchor to first link
                    float distanceFromAnchor = Vector2.Distance(anchorPoint, link.Position);
                    float dragFactor = Math.Min(distanceFromAnchor / maxDragDistance, 1f);

                    // Reduce responsiveness when dragging far behind - much lower values
                    float responsiveness = MathHelper.Lerp(0.02f, 0.01f, dragFactor); // Was 0.3f, 0.1f
                    link.Velocity += anchorMovement * responsiveness;

                    if (anchorMovement.LengthSquared() > 1f)
                    {
                        link.IsResting = false;
                    }
                }
                else
                {
                    // links further down are heavier
                    float weightMultiplier = 1f + (i * 0.1f);
                    link.Velocity.Y += gravityFactor * weightMultiplier;
                }

                Vector2 newPosition = link.Position + link.Velocity;

                // apply friction
                bool touchingTile = false;
                Vector2 adjustedPosition = HandleTileCollisionWithFriction(newPosition, link.Position, 8f, out touchingTile, ref link);
                link.Position = adjustedPosition;

                Vector2 actualMovement = link.Position - link.OldPosition;

                float frictionMultiplier = touchingTile ? tileFriction : airResistance;

                // Additional energy loss when being dragged along ground
                if (touchingTile && i == chainLinks.Count - 1) // Only for the ball (last link)
                {
                    float distanceFromAnchor = Vector2.Distance(Main.MouseWorld, link.Position);
                    float dragFactor = Math.Min(distanceFromAnchor / maxDragDistance, 1f);

                    // More energy loss when dragging far behind
                    float additionalDrag = MathHelper.Lerp(1f, dragEnergyLoss, dragFactor);
                    frictionMultiplier *= additionalDrag;
                }

                link.Velocity = actualMovement * frictionMultiplier;

                // Additional damping for resting links and heavy ball
                if (link.IsResting)
                {
                    link.Velocity *= 0.4f; // Much stronger damping when resting

                    // Stop very small movements to prevent sliding
                    if (link.Velocity.LengthSquared() < 0.01f)
                    {
                        link.Velocity = Vector2.Zero;
                    }
                }

                // Extra resistance for the ball when being dragged
                if (i == chainLinks.Count - 1 && touchingTile)
                {
                    float ballWeight = 0.01f; // Heavy ball resistance
                    link.Velocity *= ballWeight;

                    // Additional scraping resistance
                    if (link.Velocity.LengthSquared() > 0.5f)
                    {
                        link.Velocity *= 0.1f; // Extra drag for fast movement
                    }
                }

                chainLinks[i] = link;

                ChainLink dampedLink = chainLinks[i];
                dampedLink.Velocity *= 0.02f;
                chainLinks[i] = dampedLink;
            }

            for (int iteration = 0; iteration < constraintIterations; iteration++)
            {
                ApplyDistanceConstraints(anchorPoint);
            }

            // Update velocities after constraint solving
            for (int i = 0; i < chainLinks.Count; i++)
            {
                ChainLink link = chainLinks[i];
                link.Velocity = link.Position - link.OldPosition;
                chainLinks[i] = link;
            }
        }

        private Vector2 HandleTileCollisionWithFriction(Vector2 newPosition, Vector2 oldPosition, float radius, out bool touchingTile, ref ChainLink link)
        {
            Point newTile = newPosition.ToTileCoordinates();
            touchingTile = false;

            // Check if we're near or inside a solid tile
            if (WorldGen.SolidTile(newTile.X, newTile.Y) || IsNearTile(newPosition, radius))
            {
                touchingTile = true;

                if (WorldGen.SolidTile(newTile.X, newTile.Y))
                {
                    Vector2 movement = newPosition - oldPosition;
                    Vector2 correctedPosition = newPosition;
                    Point oldTile = oldPosition.ToTileCoordinates();

                    // Horizontal collision - preserve momentum but slide along surface
                    if (WorldGen.SolidTile(newTile.X, oldTile.Y))
                    {
                        correctedPosition.X = oldPosition.X;
                        if (Math.Abs(movement.X) > 0.1f)
                        {
                            correctedPosition.X += movement.X * 0.1f;
                        }
                        link.IsResting = false; // Wall contact isn't resting
                    }

                    // Vertical collision - check if this is a floor (resting surface)
                    if (WorldGen.SolidTile(oldTile.X, newTile.Y))
                    {
                        correctedPosition.Y = oldPosition.Y;

                        // Calculate bounce based on speed - slower = less bounce
                        float speed = Math.Abs(movement.Y);
                        float bounceAmount = 0f;

                        if (speed < 0.5f)
                        {
                            // Very slow - no bounce, just rest
                            link.IsResting = true;
                            bounceAmount = 0f;
                        }
                        else if (speed < 1.5f)
                        {
                            // Slow - minimal bounce
                            bounceAmount = speed * 0.1f;
                            link.IsResting = false;
                        }
                        else
                        {
                            // Fast - normal bounce
                            bounceAmount = speed * 0.2f;
                            link.IsResting = false;
                        }

                        correctedPosition.Y += movement.Y > 0 ? -bounceAmount : bounceAmount;
                    }

                    // Final safety check
                    Point finalTile = correctedPosition.ToTileCoordinates();
                    if (WorldGen.SolidTile(finalTile.X, finalTile.Y))
                    {
                        Vector2 tileCenter = finalTile.ToWorldCoordinates(8f, 8f);
                        Vector2 pushDirection = Vector2.Normalize(correctedPosition - tileCenter);
                        correctedPosition = tileCenter + pushDirection * (radius + 4f);
                    }

                    return correctedPosition;
                }
            }

            return newPosition;
        }

        private bool IsLinkResting(Vector2 position, Vector2 velocity)
        {
            // Link is resting if it's moving very slowly and there's a solid tile below it
            if (velocity.LengthSquared() > 0.25f) return false; // Too much movement

            Point belowTile = new Point((int)(position.X / 16), (int)((position.Y + 12f) / 16));
            return WorldGen.SolidTile(belowTile.X, belowTile.Y);
        }

        private bool IsNearTile(Vector2 position, float radius)
        {
            // Check a small area around the position for nearby solid tiles
            int checkRadius = 1;
            Point centerTile = position.ToTileCoordinates();

            for (int x = -checkRadius; x <= checkRadius; x++)
            {
                for (int y = -checkRadius; y <= checkRadius; y++)
                {
                    Point checkTile = new Point(centerTile.X + x, centerTile.Y + y);
                    if (WorldGen.SolidTile(checkTile.X, checkTile.Y))
                    {
                        Vector2 tileCenter = checkTile.ToWorldCoordinates(8f, 8f);
                        float distance = Vector2.Distance(position, tileCenter);
                        if (distance < radius + 8f) // 8f is half tile size
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void ApplyDistanceConstraints(Vector2 anchorPoint)
        {
            for (int i = 0; i < chainLinks.Count; i++)
            {
                Vector2 targetPosition = (i == 0) ? anchorPoint : chainLinks[i - 1].Position;
                Vector2 currentPosition = chainLinks[i].Position;

                Vector2 difference = currentPosition - targetPosition;
                float distance = difference.Length();

                if (distance > 0.01f) // Avoid division by zero
                {
                    float stretchRatio = distance / linkDistance;

                    // Clamp the maximum stretch to prevent excessive elongation
                    if (stretchRatio > 1f) // Allow 20% stretch maximum
                    {
                        stretchRatio = 1f;
                    }

                    float targetDistance = linkDistance * stretchRatio;
                    Vector2 direction = difference / distance;
                    Vector2 correction = difference - (direction * targetDistance);

                    if (i == 0)
                    {
                        // First link only moves away from anchor
                        chainLinks[i] = new ChainLink
                        {
                            Position = targetPosition + direction * targetDistance,
                            OldPosition = chainLinks[i].OldPosition,
                            Velocity = chainLinks[i].Velocity
                        };
                    }
                    else
                    {
                        // Distribute correction between both links
                        float correctionStrength = 0.5f;

                        chainLinks[i - 1] = new ChainLink
                        {
                            Position = chainLinks[i - 1].Position + correction * correctionStrength,
                            OldPosition = chainLinks[i - 1].OldPosition,
                            Velocity = chainLinks[i - 1].Velocity
                        };

                        chainLinks[i] = new ChainLink
                        {
                            Position = currentPosition - correction * correctionStrength,
                            OldPosition = chainLinks[i].OldPosition,
                            Velocity = chainLinks[i].Velocity
                        };
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (chainLinks == null || chainLinks.Count == 0) return false;

            Vector2 anchorPoint = Main.MouseWorld;

            Texture2D chainTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadChain").Value;
            Texture2D ballTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadFlail").Value;

            // Draw chain links
            for (int i = 0; i < chainLinks.Count; i++)
            {
                Vector2 startPos = (i == 0) ? anchorPoint : chainLinks[i - 1].Position;
                Vector2 endPos = chainLinks[i].Position;

                Vector2 screenStart = startPos - Main.screenPosition;
                Vector2 screenEnd = endPos - Main.screenPosition;

                Vector2 linkDir = screenEnd - screenStart;
                float rotation = linkDir.ToRotation() + MathHelper.PiOver2;

                // Draw chain link sprite
                Main.spriteBatch.Draw(chainTexture, screenStart, null, Color.White, rotation,
                    new Vector2(chainTexture.Width / 2f, chainTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
            }

            // Draw ball at end
            Vector2 ballPos = chainLinks[chainLinks.Count - 1].Position - Main.screenPosition;
            Vector2 chainDirection = Vector2.Zero;
            if (chainLinks.Count > 1)
            {
                chainDirection = chainLinks[chainLinks.Count - 1].Position - chainLinks[chainLinks.Count - 2].Position;
                Projectile.rotation = chainDirection.ToRotation() + MathHelper.PiOver2; // +90° so ball hangs naturally
            }

            Main.spriteBatch.Draw(ballTexture, ballPos, null, Color.White, Projectile.rotation,
                ballTexture.Size() / 2f, 1f, SpriteEffects.FlipVertically, 0f);

            return false;
        }

        private struct ChainLink
        {
            public Vector2 Position;
            public Vector2 OldPosition;
            public Vector2 Velocity;
            public bool IsResting; // Track if this link is resting on ground
        }
    }
}