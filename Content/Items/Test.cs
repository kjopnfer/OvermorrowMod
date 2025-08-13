using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Effects.Slash;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Test
{
    public class TestSlashProjectile : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        int totalTime = 22;
        public override void SetDefaults()
        {
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

        private SlashRenderer slashRenderer;
        private SlashPath fullSlashPath;

        public override void OnSpawn(IEntitySource source)
        {
            InitializeSlash();
        }

        public override void AI()
        {
            Main.LocalPlayer.heldProj = Projectile.whoAmI;
            Projectile.Center = Main.LocalPlayer.MountedCenter;

            Projectile.damage = 30;

            UpdateSlash();
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
                Vector2 direction = currentPath.GetDirectionAt(t);
                Vector2 perpendicular = new Vector2(-direction.Y, direction.X);

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

        bool swingForward = true;
        private void InitializeSlash()
        {
            Vector2 center = Main.LocalPlayer.MountedCenter;
            float radiusX = Main.rand.Next(8, 10) * 5f;
            float radiusY = Main.rand.Next(4, 9) * 5f;
            float ellipseRotation = Main.LocalPlayer.Center.DirectionTo(Main.MouseWorld).ToRotation();

            float startAngle = MathHelper.PiOver2 * 2;
            float endAngle = -MathHelper.PiOver2;
            if (Main.rand.NextBool())
            {
                swingForward = false;
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

            float opacity = 0.6f;

            // Sharp sword-like slash
            slashRenderer.AddLayer(new SlashLayer(dissolvedTexture, Color.LightBlue * opacity, 1f, 1f)
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

        private float windupPhase = 0.25f;    // 25% windup (increased for prominence)
        private float drawPhase = 0.35f;      // 35% drawing  
        private float holdPhase = 0.15f;      // 15% hold
        private float fadePhase = 0.25f;      // 25% fade (no follow-through phase)

        private void UpdateSlash()
        {
            int elapsedTicks = totalTime - Projectile.timeLeft;
            float progress = elapsedTicks / (float)totalTime;

            // Calculate each phase
            float windupProgress = 0f;
            float drawProgress = 0f;
            float fadeProgress = 0f;

            if (progress <= windupPhase)
            {
                // Wind-up phase
                windupProgress = progress / windupPhase;
            }
            else if (progress <= windupPhase + drawPhase)
            {
                // Drawing phase
                windupProgress = 1f; // Windup complete
                drawProgress = (progress - windupPhase) / drawPhase;
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

            // Update slash position
            Vector2 newCenter = Main.LocalPlayer.MountedCenter;
            fullSlashPath = new SlashPath(
                newCenter,
                fullSlashPath.RadiusX,
                fullSlashPath.RadiusY,
                fullSlashPath.EllipseRotation,
                fullSlashPath.StartAngle,
                fullSlashPath.EndAngle
            );

            if (slashRenderer.Path.Center != newCenter)
            {
                SlashPath currentPath = slashRenderer.Path;
                slashRenderer.UpdatePath(new SlashPath(
                    newCenter,
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
                // Wind-up: pull back more prominently (opposite direction)
                float windupAmount = EasingUtils.EaseInOutQuad(windupProgress);
                armAdjustment = MathHelper.Lerp(swingForward ? -1.6f : 1.6f, 0f, windupAmount); // -0.6 radians back (more prominent)
            }
            // No follow-through phase - arm stays at normal position after drawing

            float finalAngle = baseAngle + armAdjustment;
            Projectile.rotation = finalAngle;
            Main.LocalPlayer.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, finalAngle);
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
                    layer.Opacity = 1f; // Let vertex colors handle the fade
                }
                else if (i == 1) // Highlight layer
                {
                    float intensity = drawProgress < 1f ? 1.5f : 1f;
                    layer.Opacity = intensity;
                }
                else if (i == 2) // Support layer
                {
                    layer.Opacity = 1f; // Let vertex colors handle the fade
                }

                slashRenderer.Layers[i] = layer;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (slashRenderer != null)
            {
                slashRenderer.Draw(Main.spriteBatch);
            }

            // These need to be here otherwise the player arm gets drawn additively for some reason
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            SpriteEffects spriteEffects = swingForward ? SpriteEffects.FlipVertically : SpriteEffects.None;
            var rotationOffset = swingForward ? MathHelper.ToRadians(40) : MathHelper.ToRadians(140);
            Vector2 off = new Vector2(swingForward ? -55 : -10, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveItems + "CarvingKnife").Value;
            Vector2 drawOrigin = swingForward ? new Vector2(texture.Width, texture.Height) : new Vector2(0, texture.Height);

            Main.spriteBatch.Draw(texture, Main.LocalPlayer.Center - Main.screenPosition + off + new Vector2(0, Main.player[Projectile.owner].gfxOffY), null, Color.White, Projectile.rotation + rotationOffset, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}