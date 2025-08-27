using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items.Daggers;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Effects.Slash;
using OvermorrowMod.Core.Items.Daggers;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Daggers
{
    public abstract class HeldDagger : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public virtual Color SlashColor => Color.White;
        public virtual SoundStyle? SlashSound => new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/DaggerSlash");

        public virtual int TotalTime => 22;

        private int totalTime;
        private int playerDirection = 1;
        private SlashRenderer slashRenderer;
        private SlashPath fullSlashPath;
        private Vector2 baseOffset;

        Player player => Main.player[Projectile.owner];
        public ref float SwingDirection => ref Projectile.ai[0];
        public ref float OffhandFlag => ref Projectile.ai[1];
        private bool swingForward => SwingDirection == 1;

        public override void SetDefaults()
        {
            totalTime = TotalTime;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = totalTime;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ModDaggerBase item = player.HeldItem.ModItem as ModDaggerBase;
            if (OffhandFlag == 1)
            {
                SwingDirection = item.ComboCount == 3 ? -SwingDirection : SwingDirection;
                if (item.ComboCount != 3) totalTime += 3;
            }

            if (item.ComboCount == 3)
            {
                Projectile.CritChance += 100;
                Projectile.damage = (int)(Projectile.damage * 1.5f);
            }

            playerDirection = Main.MouseWorld.X < player.Center.X ? -1 : 1;

            InitializeSlash();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (OffhandFlag == 1)
            {
                Projectile.hide = true;
                behindProjectiles.Add(index);
            }
        }

        public override void AI()
        {
            if (OffhandFlag != 1)
                player.heldProj = Projectile.whoAmI;

            player.ChangeDir(playerDirection);
            Projectile.Center = player.MountedCenter;

            UpdateSlash();
        }

        private void InitializeSlash()
        {
            ModDaggerBase item = player.HeldItem.ModItem as ModDaggerBase;

            Vector2 center = player.MountedCenter;
            float radiusX = Main.rand.Next(8, 10) * 5f;
            float radiusY = Main.rand.Next(4, 9) * 5f;
            float ellipseRotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation();

            if (item.ComboCount == 3)
            {
                radiusX = 40f;
                radiusY = OffhandFlag == 1 ? 45f : 20f;
            }

            if (OffhandFlag == 1 && item.ComboCount < 3)
            {
                ellipseRotation += MathHelper.ToRadians(45f * playerDirection);

                radiusX *= 0.7f;
                radiusY *= 0.9f;

                Vector2 mouseDirection = player.Center.DirectionTo(Main.MouseWorld);
                baseOffset = new Vector2(-mouseDirection.Y, mouseDirection.X) * -5f * playerDirection;
            }

            center += baseOffset;

            // Flip the ellipse rotation for left swings
            if (playerDirection == -1)
            {
                ellipseRotation += MathHelper.Pi;
            }

            float startAngle = MathHelper.PiOver2 * 2;
            float endAngle = -MathHelper.PiOver2;

            // Flip angles for left direction
            if (playerDirection == -1)
            {
                startAngle = MathHelper.Pi - startAngle;
                endAngle = MathHelper.Pi - endAngle;
                (startAngle, endAngle) = (endAngle, startAngle);
            }

            if (!swingForward)
            {
                (startAngle, endAngle) = (endAngle, startAngle);
            }

            fullSlashPath = new SlashPath(center, radiusX, radiusY, ellipseRotation, startAngle, endAngle);

            slashRenderer = new SlashRenderer(fullSlashPath, baseWidth: 35f, segments: 40);
            SetupLayers();
        }

        protected virtual void SetupLayers()
        {
            Texture2D dissolvedTexture = ModContent.Request<Texture2D>(AssetDirectory.SlashTrails + "Blurred", AssetRequestMode.ImmediateLoad).Value;
            Texture2D laserTexture = ModContent.Request<Texture2D>(AssetDirectory.SlashTrails + "Edge", AssetRequestMode.ImmediateLoad).Value;

            float opacity = 0.3f;
            slashRenderer.AddLayer(new SlashLayer(dissolvedTexture, SlashColor * opacity * 0.5f, 1f, 1f)
            {
                Opacity = 0.5f,
                WidthScale = 0.5f,
                StartTaper = 0f,
                EndTaper = 1f,
                TaperLength = 0.5f
            });

            slashRenderer.AddLayer(new SlashLayer(laserTexture, Color.White * opacity, 1f, 1f)
            {
                Opacity = 0.5f,
                StartTaper = 0f,
                EndTaper = 1f,
                WidthScale = 0.5f,
                TaperLength = 0.5f,
                BlendState = BlendState.Additive,
                SpriteEffects = SpriteEffects.FlipHorizontally
            });
        }

        private float GetCurrentDrawingAngle()
        {
            int elapsedTicks = totalTime - Projectile.timeLeft;
            float progress = elapsedTicks / (float)totalTime;

            // During windup, use starting position
            if (progress <= windupPhase)
            {
                return slashRenderer.Path.GetDirectionAt(0f).ToRotation();
            }

            // Calculate drawing progress (excluding windup)
            float drawProgress = 0f;
            if (progress <= windupPhase + drawPhase)
            {
                drawProgress = (progress - windupPhase) / drawPhase;
            }
            else
            {
                drawProgress = 1f;
            }

            float easedProgress = EasingUtils.EaseOutQuint(drawProgress);
            Vector2 currentDrawDirection = slashRenderer.Path.GetDirectionAt(easedProgress);

            return currentDrawDirection.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Find the collision point on the slash path
            Vector2 strikePoint = GetStrikePoint(target);
            CreateSlashHitEffects(strikePoint);
            OnDaggerHit(target, hit, damageDone);

            // Trigger modifier events
            var modifiers = new NPC.HitModifiers();
            DaggerModifierHandler.TriggerSlashHit(this, player, target, ref modifiers);
        }

        /// <summary>
        /// Used for custom hit behavior.
        /// </summary>
        protected virtual void OnDaggerHit(NPC target, NPC.HitInfo hit, int damageDone) { }

        /// <summary>
        /// Used for custom hit particles
        /// </summary>
        protected virtual void CreateSlashHitEffects(Vector2 strikePoint)
        {
            Vector2 tangentDirection = GetTangentAtPoint(strikePoint);
            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;

            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                float randomScale = Main.rand.NextFloat(0.1f, 0.25f);

                // Create particles tangent to the strike point
                float tangentSpread = MathHelper.ToRadians(45);
                float randomAngle = Main.rand.NextFloat(-tangentSpread * 0.5f, tangentSpread * 0.5f);
                Vector2 particleDirection = tangentDirection.RotatedBy(randomAngle);
                Vector2 particleVelocity = particleDirection * Main.rand.NextFloat(2f, 4f);

                var lightSpark = new Spark(sparkTexture, maxTime: 30, false, 0f)
                {
                    endColor = SlashColor
                };
                ParticleManager.CreateParticleDirect(lightSpark, strikePoint, particleVelocity * 2, SlashColor, 1f, randomScale, 0f, ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true);
            }
        }

        private Vector2 GetStrikePoint(NPC target)
        {
            if (slashRenderer?.Path == null)
                return Projectile.Center;

            SlashPath currentPath = slashRenderer.Path;
            Vector2 targetCenter = target.Center;
            Vector2 closestPoint = currentPath.GetPointAt(0f);
            float closestDistance = Vector2.DistanceSquared(targetCenter, closestPoint);

            // Sample along the slash path to find the closest point to the target
            int samplePoints = 50;
            for (int i = 0; i <= samplePoints; i++)
            {
                float t = i / (float)samplePoints;
                Vector2 pathPoint = currentPath.GetPointAt(t);
                float distance = Vector2.DistanceSquared(targetCenter, pathPoint);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = pathPoint;
                }
            }

            return closestPoint;
        }

        private Vector2 GetTangentAtPoint(Vector2 strikePoint)
        {
            if (slashRenderer?.Path == null)
                return Vector2.UnitX;

            SlashPath currentPath = slashRenderer.Path;

            // Instead of finding the closest point, let's use the current slash progress
            int elapsedTicks = totalTime - Projectile.timeLeft;
            float progress = elapsedTicks / (float)totalTime;

            // Calculate current drawing progress
            float drawProgress = 0f;
            if (progress > windupPhase && progress <= windupPhase + drawPhase)
            {
                drawProgress = (progress - windupPhase) / drawPhase;
            }
            else if (progress > windupPhase + drawPhase)
            {
                drawProgress = 1f;
            }

            // Use the current progress to get the tangent direction
            float easedProgress = EasingUtils.EaseOutQuint(drawProgress);
            Vector2 tangent = currentPath.GetDirectionAt(easedProgress);

            // Account for both swing direction and player direction
            if (!swingForward)
            {
                tangent = -tangent;
            }

            // Also account for player direction (left vs right facing)
            if (playerDirection == -1)
            {
                tangent = -tangent;
            }
            else
            {
                tangent = -tangent;
            }

            return tangent;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int elapsedTicks = totalTime - Projectile.timeLeft;
            float progress = elapsedTicks / (float)totalTime;

            // Only damage during draw and hold phases (not windup, follow-through, or fade)
            if (progress <= windupPhase || progress > windupPhase + drawPhase + holdPhase || slashRenderer == null)
                return false;

            SlashPath currentPath = slashRenderer.Path;
            float slashWidth = slashRenderer.BaseWidth;

            // Sample points along the slash to check for collision
            int samplePoints = 30;

            for (int i = 0; i <= samplePoints; i++)
            {
                float t = i / (float)samplePoints;
                Vector2 slashPoint = currentPath.GetPointAt(t);

                Rectangle slashSegment = new Rectangle(
                    (int)(slashPoint.X - slashWidth * 0.5f),
                    (int)(slashPoint.Y - slashWidth * 0.5f),
                    (int)slashWidth,
                    (int)slashWidth
                );

                if (slashSegment.Intersects(targetHitbox))
                    return true;
            }

            return false;
        }

        private float windupPhase = 0.25f;
        private float drawPhase = 0.35f;
        private float holdPhase = 0.15f;
        private float fadePhase = 0.25f;

        private void UpdateSlash()
        {
            int elapsedTicks = totalTime - Projectile.timeLeft;
            float progress = elapsedTicks / (float)totalTime;

            float windupProgress = 0f;
            float drawProgress = 0f;
            float fadeProgress = 0f;

            if (progress <= windupPhase)
            {
                windupProgress = progress / windupPhase;
            }
            else if (progress <= windupPhase + drawPhase)
            {
                windupProgress = 1f;
                drawProgress = (progress - windupPhase) / drawPhase;

                if (drawProgress > 0f && elapsedTicks == (int)(windupPhase * totalTime) + 1)
                {
                    if (SlashSound.HasValue)
                        SoundEngine.PlaySound(SlashSound.Value, player.Center);
                }
            }
            else if (progress <= windupPhase + drawPhase + holdPhase)
            {
                // Hold phase
                windupProgress = 1f;
                drawProgress = 1f;
            }
            else
            {
                // Fade phase
                windupProgress = 1f;
                drawProgress = 1f;
                float fadeStart = windupPhase + drawPhase + holdPhase;
                fadeProgress = (progress - fadeStart) / fadePhase;
                fadeProgress = Math.Min(fadeProgress, 1f);
            }

            Vector2 newCenter = player.MountedCenter + baseOffset;
            fullSlashPath = new SlashPath(
                newCenter,
                fullSlashPath.RadiusX,
                fullSlashPath.RadiusY,
                fullSlashPath.EllipseRotation,
                fullSlashPath.StartAngle,
                fullSlashPath.EndAngle
            );

            Vector2 offsetCenter = player.MountedCenter + baseOffset;
            if (slashRenderer.Path.Center != newCenter)
            {
                SlashPath currentPath = slashRenderer.Path;
                slashRenderer.UpdatePath(new SlashPath(
                    offsetCenter,
                    currentPath.RadiusX,
                    currentPath.RadiusY,
                    currentPath.EllipseRotation,
                    currentPath.StartAngle,
                    currentPath.EndAngle
                ));
            }

            UpdateSlashPath(drawProgress);
            UpdateLayerProperties(drawProgress, fadeProgress);
            UpdateArmAnimation(windupProgress, drawProgress);
        }

        private void UpdateArmAnimation(float windupProgress, float drawProgress)
        {
            if (slashRenderer == null) return;

            float currentDrawAngle = GetCurrentDrawingAngle();
            float baseAngle = currentDrawAngle + MathHelper.Pi;

            // Calculate arm adjustment based on phase
            float armAdjustment = 0f;

            if (windupProgress < 1f)
            {
                // Pull back more prominently on windup (opposite direction)
                float windupAmount = EasingUtils.EaseInOutQuad(windupProgress);
                armAdjustment = MathHelper.Lerp(swingForward ? -1.6f : 1.6f, 0f, windupAmount);
            }

            float finalAngle = baseAngle + armAdjustment;
            Projectile.rotation = finalAngle;

            if (OffhandFlag == 1)
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, finalAngle);
            else
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, finalAngle);
        }

        private void UpdateSlashPath(float drawProgress)
        {
            if (drawProgress >= 1f)
            {
                // Full slash drawn
                slashRenderer.Path = fullSlashPath;
                return;
            }

            // Create partial slash path
            float totalAngleSpan = fullSlashPath.EndAngle - fullSlashPath.StartAngle;
            float currentAngleSpan = totalAngleSpan * EasingUtils.EaseOutQuint(drawProgress);

            SlashPath partialPath = new SlashPath(
                fullSlashPath.Center,
                fullSlashPath.RadiusX,
                fullSlashPath.RadiusY,
                fullSlashPath.EllipseRotation,
                fullSlashPath.StartAngle,
                fullSlashPath.StartAngle + currentAngleSpan
            );

            slashRenderer.UpdatePath(partialPath);
        }

        private void UpdateLayerProperties(float drawProgress, float fadeProgress)
        {
            // Set the fade progress on the renderer
            slashRenderer.FadeProgress = fadeProgress;

            for (int i = 0; i < slashRenderer.Layers.Count; i++)
            {
                var layer = slashRenderer.Layers[i];

                if (i == 0) // Main layer
                {
                    layer.Opacity = 1f;
                }
                else if (i == 1) // Highlight layer
                {
                    float intensity = drawProgress < 1f ? 1.5f : 1f;
                    layer.Opacity = intensity;
                }
                else if (i == 2) // Support layer
                {
                    layer.Opacity = 1f;
                }

                slashRenderer.Layers[i] = layer;
            }
        }

        protected virtual string GetDaggerTexture()
        {
            return AssetDirectory.ArchiveItems + "CarvingKnife";
        }

        private void DrawIntersectionSlash(float progress)
        {
            Vector2 offset = new Vector2(50 * playerDirection, 0);
            Vector2 center = player.MountedCenter + offset.RotatedBy(fullSlashPath.EllipseRotation);
            Texture2D slashTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_06").Value;

            float alpha = 0f;
            float scaleX = 1f;
            float midDrawPhase = windupPhase + drawPhase * 0.5f; // Midpoint of draw phase

            if (progress <= midDrawPhase)
            {
                alpha = 0f;
                scaleX = 0f;
            }
            else if (progress <= windupPhase + drawPhase)
            {
                // Fade in from midpoint to end of draw phase
                float fadeInProgress = (progress - midDrawPhase) / (drawPhase * 0.5f);
                alpha = fadeInProgress * 0.8f;
                scaleX = fadeInProgress;
            }
            else if (progress <= windupPhase + drawPhase + holdPhase * 0.3f)
            {
                alpha = 0.8f;
                scaleX = 1f;
            }
            else
            {
                float fadeStart = windupPhase + drawPhase + holdPhase * 0.3f;
                float remainingTime = 1f - fadeStart;
                float fadeProgress = (progress - fadeStart) / remainingTime;
                alpha = (1f - fadeProgress) * 0.8f;
                scaleX = 1f - fadeProgress;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(slashTexture, center - Main.screenPosition, null, Color.White * alpha, fullSlashPath.EllipseRotation + MathHelper.PiOver4, slashTexture.Size() * 0.5f, new Vector2(0.05f * scaleX, 0.35f), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(slashTexture, center - Main.screenPosition, null, Color.White * alpha, fullSlashPath.EllipseRotation + -MathHelper.PiOver4, slashTexture.Size() * 0.5f, new Vector2(0.05f * scaleX, 0.25f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (slashRenderer != null)
            {
                slashRenderer.Draw(Main.spriteBatch);
            }

            SpriteEffects spriteEffects = swingForward ? SpriteEffects.FlipVertically : SpriteEffects.None;
            var rotationOffset = swingForward ? MathHelper.ToRadians(40) : MathHelper.ToRadians(140);
            Vector2 off = new Vector2(swingForward ? -55 : -10, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            Texture2D texture = ModContent.Request<Texture2D>(GetDaggerTexture()).Value;
            Vector2 drawOrigin = swingForward ? new Vector2(texture.Width, texture.Height) : new Vector2(0, texture.Height);

            Main.spriteBatch.Draw(texture, player.Center - Main.screenPosition + off + new Vector2(0, Main.player[Projectile.owner].gfxOffY), null, lightColor, Projectile.rotation + rotationOffset, drawOrigin, Projectile.scale, spriteEffects, 0);

            ModDaggerBase item = player.HeldItem.ModItem as ModDaggerBase;
            if (item.ComboCount == 0 && OffhandFlag != 1 && player.ownedProjectileCounts[this.Type] == 2) // Only draw once from main hand
            {
                int elapsedTicks = totalTime - Projectile.timeLeft;
                float progress = elapsedTicks / (float)totalTime;
                DrawIntersectionSlash(progress);
            }

            // These need to be here otherwise the player arm gets drawn additively for some reason
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}