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

        // Anchor system - SIMPLE VERSION
        public Entity AnchorEntity { get; set; } = null;
        public Vector2 AnchorOffset { get; set; } = Vector2.Zero;
        public bool fadeIn = true;
        public bool floatUp = true;

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

            if (AnchorEntity != null && AnchorEntity.active)
            {
                particle.position = AnchorEntity.Center + AnchorOffset;
            }
        }

        public override void Update()
        {
            timeAlive++;

            // SIMPLE: If we have an anchor, just stick to it with offset
            if (AnchorEntity != null && AnchorEntity.active)
            {
                // Set position to anchor + offset, that's it!
                particle.position = AnchorEntity.Center + AnchorOffset;

                // Add upward float if enabled
                if (floatUp)
                    particle.position += -Vector2.UnitY * timeAlive * 0.5f;
            }
            else
            {
                // No anchor - move normally
                if (floatUp) particle.position += particle.velocity;
            }

            // Add sine wiggle
            particle.position += particle.velocity.RotatedBy(Math.PI / 2) * (float)Math.Sin(timeAlive * Math.PI / 10) * positionOffset;

            // Handle fading and scaling
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