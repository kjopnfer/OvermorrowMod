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
    public class Circle : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;

        private float timeAlive = 0f;
        private float maxTime;
        private float positionOffset;
        private bool useSineFade;
        private bool canGrow;
        private float initialScale;
        private float initialAlpha;

        private readonly Texture2D texture;
        public Color? endColor = null;

        // Anchor system
        private Entity anchorEntity = null;
        private Vector2 lastAnchorPosition;
        private Vector2 relativeOffset;
        private bool hasAnchor = false;

        public Entity AnchorEntity
        {
            get => anchorEntity;
            set
            {
                if (value != null && value.active)
                {
                    anchorEntity = value;
                    if (particle != null)
                    {
                        if (!hasAnchor)
                        {
                            relativeOffset = particle.position - (value.Center + AnchorOffset);
                            hasAnchor = true;
                        }
                        lastAnchorPosition = value.Center + AnchorOffset;
                    }
                }
                else
                {
                    anchorEntity = null;
                    hasAnchor = false;
                }
            }
        }

        public Vector2 AnchorOffset { get; set; } = Vector2.Zero;
        public bool fadeIn = true;
        public Circle(Texture2D texture, float maxTime = 0f, bool canGrow = false, bool useSineFade = true)
        {
            this.texture = texture;
            this.maxTime = maxTime > 0 ? maxTime : ModUtils.SecondsToTicks(1);
            this.canGrow = canGrow;
            this.useSineFade = useSineFade;

            positionOffset = Main.rand.NextFloat(0.1f, 0.2f) * (Main.rand.NextBool() ? 1 : -1);
        }

        public override void OnSpawn()
        {
            initialScale = particle.scale;
            initialAlpha = particle.alpha;
            if (fadeIn) particle.alpha = 0f;

            if (canGrow) particle.scale = 0f;
            if (!endColor.HasValue) endColor = particle.color;

            // Setup anchor if already assigned
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

            // Anchor tracking logic
            if (hasAnchor && anchorEntity != null)
            {
                if (!anchorEntity.active)
                {
                    anchorEntity = null;
                    hasAnchor = false;
                }
                else
                {
                    Vector2 currentAnchorPos = anchorEntity.Center + AnchorOffset;
                    Vector2 anchorDelta = currentAnchorPos - lastAnchorPosition;

                    particle.position += anchorDelta;
                    relativeOffset += particle.velocity;
                    particle.position = currentAnchorPos + relativeOffset;
                    lastAnchorPosition = currentAnchorPos;
                }
            }
            else
            {
                particle.position += particle.velocity;
            }

            // Sine wiggle motion
            particle.position += particle.velocity.RotatedBy(Math.PI / 2) * (float)Math.Sin(timeAlive * Math.PI / 10) * positionOffset;

            float lifeProgress = timeAlive / maxTime;
            float fadeProgress = 1f - lifeProgress;

            particle.alpha = useSineFade ? initialAlpha * (float)(Math.Sin(fadeProgress * Math.PI)) : initialAlpha * fadeProgress;

            if (canGrow)
                particle.scale = lifeProgress * initialScale;
            else
                particle.scale = fadeProgress * initialScale;

            particle.rotation += rotationAmount;

            Lighting.AddLight(particle.position, particle.color.ToVector3() / 255f);

            if (timeAlive > maxTime)
                particle.Kill();
        }

        public override bool ShouldUpdatePosition() => false;

        public override void Draw(SpriteBatch spriteBatch)
        {
            float progress = particle.activeTime / maxTime;
            Color drawColor = Color.Lerp(particle.color, endColor.Value, progress);

            Vector2 origin = texture.Size() / 2f;

            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null,
                drawColor * particle.alpha, particle.rotation, origin, particle.scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null,
                drawColor * particle.alpha * 0.7f, 0f, origin, particle.scale * 1.5f, SpriteEffects.None, 0f);
        }
    }
}
