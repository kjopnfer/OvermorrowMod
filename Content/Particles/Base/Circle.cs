using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace OvermorrowMod.Content.Particles
{
    public class Circle : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;

        // Convert customData to proper fields
        private float timeAlive = 0f;
        private float maxTime;
        private float positionOffset;
        private bool useSineFade;

        /// <summary>
        /// The color the spark will fade to at the end of its lifetime.
        /// </summary>
        public Color? endColor = null;

        /// <summary>
        /// false = shrink (default), true = grow
        /// </summary>
        private bool canGrow;

        /// <summary>
        /// Stores the initial scale of the particle, used for scaling calculations.
        /// If canGrow is true, this is the maximum scale the particle will reach.
        /// Otherwise, it is the initial scale from which the particle shrinks to 0.
        /// </summary>
        private float initialScale;

        private float initialAlpha;

        private readonly Texture2D texture;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxTime">The max life of the particle measured in ticks. If zero, defaults to 1 second.</param>
        /// <param name="canGrow">Whether the particle grows or shrinks</param>
        /// <param name="useSineFade"></param>
        public Circle(Texture2D texture, float maxTime = 0f, bool canGrow = false, bool useSineFade = true)
        {
            this.texture = texture;
            this.maxTime = maxTime > 0 ? maxTime : ModUtils.SecondsToTicks(1);
            this.canGrow = canGrow;

            positionOffset = Main.rand.NextFloat(0.1f, 0.2f) * (Main.rand.NextBool() ? 1 : -1);
            
            this.useSineFade = useSineFade;
        }

        public override void OnSpawn()
        {
            this.initialScale = particle.scale;
            this.initialAlpha = particle.alpha;

            // This is needed so that particles spawning in don't start flickering
            particle.alpha = 0f;
            
            if (canGrow) particle.scale = 0;
            if (!endColor.HasValue) endColor = particle.color;
        }

        public override void Update()
        {
            Lighting.AddLight(particle.position, particle.color.ToVector3() / 255f);

            timeAlive++;
            particle.position += particle.velocity;
            particle.position += particle.velocity.RotatedBy(Math.PI / 2) * (float)Math.Sin(timeAlive * Math.PI / 10) * positionOffset;

            float lifeProgress = timeAlive / maxTime;

            // Alpha behavior
            float fadeProgress = 1f - lifeProgress;
            particle.alpha = useSineFade ? initialAlpha * (float)(Math.Sin(fadeProgress * Math.PI)) : initialAlpha * fadeProgress;

            // Scale behavior
            if (canGrow)
            {
                // Grows from 0 to initialScale over lifetime
                particle.scale = lifeProgress * initialScale;
            }
            else
            {
                // Shrinks from initialScale to 0 over lifetime (original behavior)
                particle.scale = fadeProgress * initialScale;
            }

            particle.rotation += rotationAmount;

            if (timeAlive > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;
            float progress = particle.activeTime / maxTime;
            Color drawColor = Color.Lerp(particle.color, endColor.Value, progress);

            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, drawColor * particle.alpha, particle.rotation, texture.Size() / 2, particle.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, drawColor * particle.alpha * 0.7f, 0f, texture.Size() / 2, particle.scale * 1.5f, SpriteEffects.None, 0f);
        }
    }
}