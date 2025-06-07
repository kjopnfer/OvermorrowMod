using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Particles
{
    public class Light : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;

        private float timeAlive = 0f;
        private float maxTime;
        private float maxScale;
        private Vector2 relativeOffset;

        /// <summary>
        /// The entity this light ray is anchored to (required)
        /// </summary>
        public Entity AnchorEntity { get; set; }

        /// <summary>
        /// The color the light will fade to at the end of its lifetime.
        /// </summary>
        public Color? endColor = null;

        private readonly Texture2D texture;

        /// <summary>
        /// Creates a light ray particle that grows and shrinks while anchored to an entity
        /// </summary>
        /// <param name="texture">The texture to use for the light ray</param>
        /// <param name="maxTime">The max life of the particle measured in ticks. If zero, defaults to 1 second.</param>
        /// <param name="anchorEntity">The entity to anchor this light ray to (required)</param>
        /// <param name="offset">Offset from the anchor entity's position</param>
        public Light(Texture2D texture, float maxTime = 0f, Entity anchorEntity = null, Vector2 offset = default)
        {
            this.texture = texture ?? throw new ArgumentNullException(nameof(texture));
            this.maxTime = maxTime > 0 ? maxTime : ModUtils.SecondsToTicks(1);
            this.AnchorEntity = anchorEntity ?? throw new ArgumentNullException(nameof(anchorEntity), "Light particles must be anchored to an entity");
            this.relativeOffset = offset;
        }

        public override void OnSpawn()
        {
            this.maxScale = particle.scale;

            // Start with no scale and no alpha
            particle.scale = 0f;
            particle.alpha = 0f;

            if (!endColor.HasValue) endColor = particle.color;

            // Set initial position relative to anchor entity
            UpdateAnchoredPosition();
        }

        public override void Update()
        {
            // Kill particle if anchor entity is no longer active
            if (AnchorEntity == null || !AnchorEntity.active)
            {
                particle.Kill();
                return;
            }

            // Update position to stay anchored to entity
            UpdateAnchoredPosition();

            timeAlive++;
            float lifeProgress = timeAlive / maxTime;

            // Alpha follows the same vertical extension curve
            if (lifeProgress <= 0.5f)
            {
                // First half: fade in as ray extends
                float growProgress = lifeProgress * 2f;
                particle.alpha = EasingUtils.EaseOutQuad(growProgress);
            }
            else
            {
                // Second half: fade out as ray retracts
                float shrinkProgress = (lifeProgress - 0.5f) * 2f;
                particle.alpha = 1f - EasingUtils.EaseOutQuad(shrinkProgress);
            }

            // Add some lighting effect
            Lighting.AddLight(particle.position, particle.color.ToVector3() * particle.alpha / 255f);

            if (timeAlive >= maxTime) particle.Kill();
        }

        private void UpdateAnchoredPosition()
        {
            particle.position = AnchorEntity.Center + relativeOffset;
        }

        public override bool ShouldUpdatePosition()
        {
            // Don't update position normally since we're anchored
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (texture == null) return;

            spriteBatch.Reload(BlendState.Additive);

            float progress = timeAlive / maxTime;
            Color drawColor = Color.Lerp(particle.color, endColor.Value, progress);

            // Calculate height and width lerps for vertical light ray extension
            float heightLerp, widthLerp;

            if (progress <= 0.5f)
            {
                // First half: extend vertically from 0 to max
                float growProgress = progress * 2f;
                heightLerp = MathHelper.Lerp(0f, maxScale, EasingUtils.EaseOutQuad(growProgress));
                widthLerp = MathHelper.Lerp(0.1f, 0.3f, EasingUtils.EaseOutQuad(growProgress)); // Thin width
            }
            else
            {
                // Second half: retract vertically from max to 0
                float shrinkProgress = (progress - 0.5f) * 2f;
                heightLerp = MathHelper.Lerp(maxScale, 0f, EasingUtils.EaseOutQuad(shrinkProgress));
                widthLerp = MathHelper.Lerp(0.3f, 0.1f, EasingUtils.EaseOutQuad(shrinkProgress));
            }

            // Draw the light ray with vertical scaling
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null,
                drawColor * particle.alpha,
                particle.rotation,
                new Vector2(texture.Width / 2, 0),
                new Vector2(widthLerp, heightLerp), // Width, Height
                SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}