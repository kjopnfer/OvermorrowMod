using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Particles
{
    public class Gas : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;

        // Convert customData to proper fields
        private float timeAlive = 0f;
        private float maxTime;
        private float initialScale;
        private float flameOffset;
        private Texture2D selectedTexture;
        private Texture2D[] textures;

        // Configurable behavior properties
        public bool hasLight = false;
        public bool hasWaveMovement = false;
        public bool hasAdditiveLayers = false;
        public bool growsOverTime = false;
        public bool rotatesOverTime = false;
        public bool driftsUpward = false;
        public float scaleRate = 0.005f;
        public float customAlpha = 1f;
        public float lightIntensity = 0.5f;

        public Gas(Texture2D[] textures, float maxTimeOverride = 0f, float scaleOverride = 0f)
        {
            this.textures = textures;
            this.maxTime = maxTimeOverride > 0 ? maxTimeOverride : Main.rand.Next(4, 5) * 10;
            this.initialScale = scaleOverride > 0 ? scaleOverride : Main.rand.NextFloat(0.2f, 0.3f);

            // Select random texture variant
            this.selectedTexture = textures[Main.rand.Next(textures.Length)];

            // Generate random values
            this.flameOffset = Main.rand.NextFloat(0.1f, 0.2f) * (Main.rand.NextBool() ? 1 : -1);
        }

        public override void OnSpawn()
        {
            particle.alpha = 0f;
            particle.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            particle.scale = initialScale;
        }

        public override void Update()
        {
            timeAlive++;

            // Movement
            particle.position += particle.velocity;

            if (hasWaveMovement)
            {
                // PlantGas wave movement
                particle.position += particle.velocity.RotatedBy(Math.PI / 2) * (float)Math.Sin(timeAlive * Math.PI / 10) * flameOffset;
            }

            if (driftsUpward)
            {
                // Smoke movement - rises upward
                particle.velocity.Y -= 0.03f;
            }

            if (rotatesOverTime)
            {
                particle.rotation += 0.04f;
            }

            // Scale behavior
            if (growsOverTime)
            {
                // Smoke grows over time
                particle.scale += scaleRate;
            }
            else
            {
                // PlantGas shrinks over time
                particle.scale = (1f - timeAlive / maxTime) * initialScale;
            }

            // Alpha behavior
            float lifeProgress = 1f - timeAlive / maxTime;
            particle.alpha = lifeProgress;

            // Lighting
            if (hasLight)
            {
                Lighting.AddLight(particle.position, particle.color.ToVector3() * lightIntensity * lifeProgress);
            }

            if (timeAlive > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = selectedTexture.Size() / 2f;
            float finalAlpha = particle.alpha * customAlpha;

            if (!hasAdditiveLayers)
            {
                spriteBatch.Draw(selectedTexture, particle.position - Main.screenPosition, null,
                    particle.color * finalAlpha, particle.rotation, origin, particle.scale, SpriteEffects.None, 0f);
            }
            else
            {
                // Complex drawing with additive layers
                // Base layer
                spriteBatch.Draw(selectedTexture, particle.position - Main.screenPosition, null,
                    particle.color * finalAlpha, 0f, origin, particle.scale * 0.125f, SpriteEffects.None, 0f);

                spriteBatch.Reload(BlendState.Additive);

                // Additive layer 1
                spriteBatch.Draw(selectedTexture, particle.position - Main.screenPosition, null,
                    particle.color * finalAlpha * 0.7f, particle.rotation, origin, particle.scale * 1.5f, SpriteEffects.None, 0f);

                // Additive layer 2
                if (Main.rand.NextBool())
                {
                    spriteBatch.Draw(selectedTexture, particle.position - Main.screenPosition, null,
                        particle.color * finalAlpha * 0.4f, particle.rotation, origin, particle.scale * 3f, SpriteEffects.None, 0f);
                }

                spriteBatch.Reload(BlendState.AlphaBlend);
            }
        }
    }
}
