using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Particles
{
    public class Spark : CustomParticle
    {
        public override string Texture => AssetDirectory.Textures + "spotlight";

        private float initialScale;
        private bool rotateWithVelocity;
        private float maxTime;
        private float timeAlive = 0f;

        /// <summary>
        /// 0 = no rotation, positive/negative = rotation speed
        /// </summary>
        private float rotationDirection = 0f;

        /// <summary>
        /// The color the spark will fade to at the end of its lifetime.
        /// </summary>
        public Color endColor = Color.Red;
        public Spark(float initialScale = 0f, bool rotateWithVelocity = false, float maxTimeOverride = 0f, float rotationDirection = 0f)
        {
            this.initialScale = initialScale > 0 ? initialScale : Main.rand.NextFloat(0.05f, 0.15f);
            this.rotateWithVelocity = rotateWithVelocity;
            this.maxTime = maxTimeOverride > 0 ? maxTimeOverride : Main.rand.Next(4, 8) * 10;
            this.rotationDirection = rotationDirection;
        }

        public override void OnSpawn()
        {
            particle.rotation += MathHelper.Pi / 2;
            particle.scale = 0f;

            if (rotationDirection != 0f)
            {
                particle.alpha = 0f;
                //maxTime = Main.rand.Next(8, 10) * 10;
            }
        }

        public override void Update()
        {
            timeAlive++;

            if (rotationDirection != 0f)
            {
                particle.velocity = particle.velocity.RotatedBy(MathHelper.ToRadians(3 * rotationDirection));
                particle.rotation = particle.velocity.ToRotation();

                particle.alpha = (float)(Math.Sin((1f - timeAlive / maxTime) * Math.PI));
            }
            else
            {
                particle.velocity *= 0.95f;

                if (rotateWithVelocity)
                    particle.rotation = particle.velocity.ToRotation() + MathHelper.PiOver2;

                // Complex alpha calculation from original
                particle.alpha = Utils.GetLerpValue(0f, 0.05f, particle.activeTime / maxTime, clamped: true) *
                               Utils.GetLerpValue(1f, 0.9f, particle.activeTime / maxTime, clamped: true);
            }

            // Add lighting if scale/time is small enough (original condition)
            if (initialScale < 55 || timeAlive < 55)
            {
                Lighting.AddLight(particle.position, new Vector3(0.6f, 0.35f, 0));
            }

            if (timeAlive > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;

            // Calculate scale lerps - different for rotating vs regular sparks
            float heightLerp, widthLerp;
            if (rotationDirection != 0f)
            {
                // RotatingEmber scale behavior
                heightLerp = MathHelper.Lerp(1f, 0, EasingUtils.EaseOutQuad(Utils.Clamp(particle.activeTime, 0, maxTime) / maxTime));
                widthLerp = MathHelper.Lerp(0.25f, 0, EasingUtils.EaseOutQuad(Utils.Clamp(particle.activeTime, 0, maxTime) / maxTime));
            }
            else
            {
                // Regular spark scale behavior
                heightLerp = MathHelper.Lerp(initialScale, 0, EasingUtils.EaseOutQuad(Utils.Clamp(particle.activeTime, 0, maxTime) / maxTime));
                widthLerp = MathHelper.Lerp(0.25f, 0, EasingUtils.EaseOutQuad(Utils.Clamp(particle.activeTime, 0, maxTime) / maxTime));
            }

            // Use particle color with red lerp
            float progress = particle.activeTime / maxTime;
            Color drawColor = Color.Lerp(particle.color, endColor, progress);

            float rotationOffset = (rotationDirection != 0f) ? MathHelper.PiOver2 :
                                 (rotateWithVelocity ? MathHelper.PiOver2 : 0);

            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null,
                drawColor * particle.alpha,
                particle.rotation + rotationOffset,
                texture.Size() / 2f,
                new Vector2(heightLerp, widthLerp),
                SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}