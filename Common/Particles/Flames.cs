using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Flames : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;

        private float scaleRate;
        private float initialScale;

        int variant = Main.rand.Next(1, 3);

        // customData[0] = scaleRate
        public override void OnSpawn()
        {
            scaleRate = particle.customData[0];
            initialScale = particle.scale;
        }

        public override void Update()
        {
            particle.velocity *= 0.9f;
            particle.scale -= scaleRate;

            if (particle.scale <= 0) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "fire_0" + variant).Value;

            Texture2D glowTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
            spriteBatch.Draw(glowTexture, particle.position - Main.screenPosition, null, Color.Red * particle.alpha * 0.75f, particle.rotation, glowTexture.Size() / 2f, particle.scale * 6f, SpriteEffects.None, 0f);

            Color fadeColor = particle.scale < initialScale * 0.65f ? particle.color : Color.White;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, fadeColor * particle.alpha, particle.rotation, texture.Size() / 2f, particle.scale * 0.6f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, texture.Size() / 2f, particle.scale, SpriteEffects.None, 0f);


            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}