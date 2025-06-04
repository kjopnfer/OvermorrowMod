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

        private bool rotateWithVelocity;
        private float maxTime;
        private float timeAlive = 0f;


        private readonly Texture2D texture;

        /// <summary>
        /// Stores the initial scale of the particle, used for scaling calculations.
        /// If canGrow is true, this is the maximum scale the particle will reach.
        /// Otherwise, it is the initial scale from which the particle shrinks to 0.
        /// </summary>
        private float initialScale;

        /// <summary>
        /// 0 = no rotation, positive/negative = rotation speed
        /// </summary>
        private float rotationDirection = 0f;

        /// <summary>
        /// The color the spark will fade to at the end of its lifetime.
        /// </summary>
        public Color endColor = Color.Red;

        public Spark(Texture2D texture, float maxTime = 0f, bool rotateWithVelocity = false, float rotationDirection = 0f)
        {
            this.texture = texture;

            //this.initialScale = initialScale > 0 ? initialScale : Main.rand.NextFloat(0.05f, 0.15f);
            this.rotateWithVelocity = rotateWithVelocity;
            this.maxTime = maxTime > 0 ? maxTime : Main.rand.Next(4, 8) * 10;
            this.rotationDirection = rotationDirection;
        }

        public override void OnSpawn()
        {
            this.initialScale = particle.scale;

            particle.rotation += MathHelper.Pi / 2;
            particle.scale = 0f;
            particle.alpha = 0f;

            if (rotationDirection != 0f)
            {
                particle.alpha = 0f;
                //maxTime = Main.rand.Next(8, 10) * 10;
            }
        }

        public override void Update()
        {
            timeAlive++;
            particle.rotation = particle.velocity.ToRotation() + MathHelper.PiOver2;

            if (rotationDirection != 0f)
            {
                particle.velocity = particle.velocity.RotatedBy(MathHelper.ToRadians(3 * rotationDirection));
                particle.rotation = particle.velocity.ToRotation();

                particle.alpha = (float)(Math.Sin((1f - timeAlive / maxTime) * Math.PI));
            }
            else
            {
                particle.velocity *= 0.95f;

                particle.alpha = Utils.GetLerpValue(0f, 0.05f, particle.activeTime / maxTime, clamped: true) *
                               Utils.GetLerpValue(1f, 0.9f, particle.activeTime / maxTime, clamped: true);
            }

            // Add lighting if scale/time is small enough (original condition)
            var lightTime = maxTime * 0.6f;
            if (timeAlive < lightTime)
            {
                Lighting.AddLight(particle.position, particle.color.ToVector3() * 0.5f);
            }

            if (timeAlive > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

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