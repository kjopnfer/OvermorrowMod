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

        // Anchor system fields
        private Entity anchorEntity = null;
        private Vector2 lastAnchorPosition;
        private Vector2 relativeOffset;
        private bool hasAnchor = false;

        /// <summary>
        /// Sets an entity that this particle will follow while still allowing velocity-based movement.
        /// The particle will automatically track the entity's Center position.
        /// </summary>
        public Entity AnchorEntity
        {
            get => anchorEntity;
            set
            {
                if (value != null && value.active)
                {
                    anchorEntity = value;

                    // Only calculate relative offset if particle is already initialized
                    if (particle != null)
                    {
                        if (!hasAnchor)
                        {
                            // First time setting anchor - calculate initial relative offset
                            relativeOffset = particle.position - (value.Center + AnchorOffset);
                            hasAnchor = true;
                        }
                        lastAnchorPosition = value.Center + AnchorOffset;
                    }
                    // If particle is null, we'll calculate offset in OnSpawn()
                }
                else
                {
                    // Remove anchor
                    anchorEntity = null;
                    hasAnchor = false;
                }
            }
        }

        /// <summary>
        /// Optional offset from the entity's center. Useful for attaching to specific parts of an entity.
        /// </summary>
        public Vector2 AnchorOffset { get; set; } = Vector2.Zero;

        public Spark(Texture2D texture, float maxTime = 0f, bool rotateWithVelocity = false, float rotationDirection = 0f)
        {
            this.texture = texture;
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
            }

            // Initialize anchor system if anchor was set before spawn
            if (anchorEntity != null && anchorEntity.active)
            {
                Vector2 anchorPos = anchorEntity.Center + AnchorOffset;
                relativeOffset = particle.position - anchorPos;
                lastAnchorPosition = anchorPos;
                hasAnchor = true;
            }
        }

        public override void Update()
        {
            timeAlive++;

            // Handle anchor movement first
            if (hasAnchor && anchorEntity != null)
            {
                // Check if anchor entity is still valid
                if (!anchorEntity.active)
                {
                    // Entity died/became inactive, remove anchor
                    anchorEntity = null;
                    hasAnchor = false;
                }
                else
                {
                    Vector2 currentAnchorPos = anchorEntity.Center + AnchorOffset;

                    // Calculate how much the anchor moved
                    Vector2 anchorDelta = currentAnchorPos - lastAnchorPosition;

                    // Move the particle by the same amount the anchor moved
                    particle.position += anchorDelta;

                    // Update relative offset with velocity-based movement
                    relativeOffset += particle.velocity;

                    // Set final position as anchor + relative offset
                    particle.position = currentAnchorPos + relativeOffset;

                    // Update last anchor position for next frame
                    lastAnchorPosition = currentAnchorPos;
                }
            }

            if (!hasAnchor)
            {
                // Normal movement when no anchor
                particle.position += particle.velocity;
            }

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

        public override bool ShouldUpdatePosition()
        {
            // Don't let the particle manager update position since we handle it manually
            return false;
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