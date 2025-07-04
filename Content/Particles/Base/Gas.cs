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
    public enum GasBehavior
    {
        Grow,
        Shrink,
        None
    }

    public class Gas : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;

        // Convert customData to proper fields
        private float timeAlive = 0f;
        private float maxTime;
        private float initialScale;
        private float positionOffset;
        private Texture2D selectedTexture;
        private Texture2D[] textures;

        // Configurable behavior properties
        public bool hasLight = false;
        public bool hasWaveMovement = false;
        public bool hasAdditiveLayers = false;
        public GasBehavior gasBehavior = GasBehavior.Grow;
        public bool rotatesOverTime = false;
        public bool driftsUpward = false;
        public float scaleRate = 0.005f;
        public float customAlpha = 1f;
        public float lightIntensity = 0.5f;

        public Gas(Texture2D[] textures, float maxTimeOverride = 0f, float scaleOverride = 0f)
        {
            this.textures = textures;
            this.maxTime = maxTimeOverride > 0 ? maxTimeOverride : Main.rand.Next(4, 5) * 10;

            // Select random texture variant
            this.selectedTexture = textures[Main.rand.Next(textures.Length)];

            // Generate random values
            this.positionOffset = Main.rand.NextFloat(0.1f, 0.2f) * (Main.rand.NextBool() ? 1 : -1);
        }

        public override void OnSpawn()
        {
            particle.alpha = 0f;
            particle.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            this.initialScale = particle.scale;

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
                particle.position += particle.velocity.RotatedBy(Math.PI / 2) * (float)Math.Sin(timeAlive * Math.PI / 10) * positionOffset;
            }

            if (driftsUpward)
            {
                // Smoke movement - rises upward
                particle.velocity.Y -= 0.1f;
            }

            if (rotatesOverTime)
            {
                particle.rotation += 0.04f;
            }

            // Scale behavior
            switch (gasBehavior)
            {
                case GasBehavior.Grow:
                    particle.scale += scaleRate;
                    break;
                case GasBehavior.Shrink:
                    particle.scale = (1f - timeAlive / maxTime) * initialScale;
                    break;
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

            spriteBatch.Draw(selectedTexture, particle.position - Main.screenPosition, null,
                particle.color * finalAlpha * 0.7f, particle.rotation, origin, particle.scale * 0.7f, SpriteEffects.None, 0f);

            //if (Main.rand.NextBool())
            //{
            //    spriteBatch.Draw(selectedTexture, particle.position - Main.screenPosition, null,
            //        particle.color * finalAlpha * 0.4f, particle.rotation, origin, particle.scale * 2f, SpriteEffects.None, 0f);
            //}
        }
    }
}
