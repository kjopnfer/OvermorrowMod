using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Effects.Slash;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Test
{
    public class TestSlashProjectile : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        int totalTime = 25;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = totalTime; // 2 seconds
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        private SlashRenderer slashRenderer;
        private SlashPath fullSlashPath;
        private bool initialized = false;

        // Timing variables - customize these for different effects
        private int drawDuration = 20; // Ticks to draw the full slash
        private int totalDuration = 80; // Ticks before fading starts
        private int fadeDuration = 40;  // Ticks to fade out

        public override void AI()
        {
            Projectile.damage = 30;
            if (!initialized)
            {
                InitializeSlash();
                initialized = true;
            }

            // Update the slash based on timing
            UpdateSlash();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int elapsedTicks = totalTime - Projectile.timeLeft;
            float progress = elapsedTicks / (float)totalTime;

            if (progress > drawPhase + holdPhase || !initialized || slashRenderer == null)
                return false;

            // Get the current slash path
            SlashPath currentPath = slashRenderer.Path;
            float slashWidth = slashRenderer.BaseWidth;

            // Sample points along the slash to check for collision
            int samplePoints = 30;

            for (int i = 0; i <= samplePoints; i++)
            {
                float t = i / (float)samplePoints;
                Vector2 slashPoint = currentPath.GetPointAt(t);
                Vector2 direction = currentPath.GetDirectionAt(t);
                Vector2 perpendicular = new Vector2(-direction.Y, direction.X);

                // Create a rectangle at this point along the slash
                Rectangle slashSegment = new Rectangle(
                    (int)(slashPoint.X - slashWidth * 0.5f),
                    (int)(slashPoint.Y - slashWidth * 0.5f),
                    (int)slashWidth,
                    (int)slashWidth
                );

                // Check if this segment intersects the target
                if (slashSegment.Intersects(targetHitbox))
                    return true;
            }

            return false;
        }

        private void InitializeSlash()
        {
            Vector2 center = Projectile.Center;
            float radiusX = Main.rand.Next(8, 12) * 10f;
            float radiusY = Main.rand.Next(4, 9) * 10f;
            float ellipseRotation = Projectile.rotation + MathHelper.ToRadians(Main.rand.NextFloat(0, 10) * 10);
            float startAngle = MathHelper.PiOver2 * 2;
            float endAngle = -MathHelper.PiOver2;
            if (Main.rand.NextBool())
            {
                (startAngle, endAngle) = (endAngle, startAngle);
            }

            fullSlashPath = new SlashPath(center, radiusX, radiusY, ellipseRotation, startAngle, endAngle);

            slashRenderer = new SlashRenderer(fullSlashPath, baseWidth: 35f, segments: 40);
            SetupLayers();
        }

        private void SetupLayers()
        {
            Texture2D dissolvedTexture = ModContent.Request<Texture2D>(AssetDirectory.SlashTrails + "Blurred").Value;
            Texture2D laserTexture = ModContent.Request<Texture2D>(AssetDirectory.SlashTrails + "Edge").Value;
            Texture2D supportTexture = ModContent.Request<Texture2D>(AssetDirectory.Trails + "Jagged").Value;

            // Sharp sword-like slash
            slashRenderer.AddLayer(new SlashLayer(dissolvedTexture, Color.LightBlue, 1f, 1f)
            {
                Opacity = 0.8f,
                WidthScale = 1f,
                StartTaper = 0f,
                EndTaper = 1f,
                TaperLength = 0.5f  // Start taper lasts 20% of slash length,
            });

            slashRenderer.AddLayer(new SlashLayer(laserTexture, Color.White, 1f, 1f)
            {
                StartTaper = 0f,
                EndTaper = 1f,
                TaperLength = 0.5f,
                BlendState = BlendState.Additive,
                SpriteEffects = SpriteEffects.FlipHorizontally

            });

            //slashRenderer.AddLayer(new SlashLayer(supportTexture, Color.White, 1f, 1f)
            //{
            //    Opacity = 1f,
            //    WidthScale = 2f,
            //    StartTaper = 0f,
            //    EndTaper = 1f,
            //    TaperLength = 0.5f,
            //    SpriteEffects = SpriteEffects.FlipVertically,
            //    Offset = -15
            //});

            // Highlight layer - thin, bright, additive
            //slashRenderer.AddLayer(new SlashLayer(
            //    laserTexture,
            //    Color.White,
            //    widthScale: 1f,
            //    opacity: 1f,
            //    BlendState.Additive,
            //    SpriteEffects.FlipHorizontally
            //));
        }

        private float drawPhase = 0.25f;
        private float holdPhase = 0.33f;
        private float fadePhase = 0.42f;

        private void UpdateSlash()
        {
            int elapsedTicks = totalTime - Projectile.timeLeft;
            float progress = elapsedTicks / (float)totalTime; // Overall progress (0 to 1)

            // Calculate each phase
            float drawProgress = 0f;
            float fadeProgress = 0f;

            if (progress <= drawPhase)
            {
                // Drawing phase
                drawProgress = progress / drawPhase;
            }
            else if (progress <= drawPhase + holdPhase)
            {
                // Hold phase - drawing complete, no fade yet
                drawProgress = 1f;
                fadeProgress = 0f;
            }
            else
            {
                // Fade phase
                drawProgress = 1f;
                float fadeStart = drawPhase + holdPhase;
                fadeProgress = (progress - fadeStart) / fadePhase;
                fadeProgress = Math.Min(fadeProgress, 1f);
            }

            // Update the slash path and properties
            UpdateSlashPath(drawProgress);
            UpdateLayerProperties(drawProgress, fadeProgress);
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
            float baseOpacity = 1f - fadeProgress;

            for (int i = 0; i < slashRenderer.Layers.Count; i++)
            {
                var layer = slashRenderer.Layers[i];

                if (i == 0) // Main layer
                {
                    layer.Opacity = baseOpacity;
                }
                else if (i == 1) // Highlight layer
                {
                    float intensity = drawProgress < 1f ? 1.5f : 1f;
                    layer.Opacity = intensity * baseOpacity;
                }

                slashRenderer.Layers[i] = layer;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (initialized && slashRenderer != null)
            {
                slashRenderer.Draw(Main.spriteBatch);
            }

            return false;
        }
    }
}