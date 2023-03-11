using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Flames : Particle
    {
        private float scaleRate;
        private float initialScale;
        public override void OnSpawn()
        {
            scaleRate = CustomData[0];
            initialScale = Scale;
        }

        public override void Update()
        {
            Velocity *= 0.95f;
            Scale -= scaleRate;

            if (Scale <= 0) Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            int variant = Main.rand.Next(1, 3);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "fire_0" + variant).Value;

            Texture2D glowTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
            spriteBatch.Draw(glowTexture, Position - Main.screenPosition, null, Color.Red * Alpha * 0.75f, Rotation, glowTexture.Size() / 2f, Scale * 6f, SpriteEffects.None, 0f);

            Color fadeColor = Scale < initialScale * 0.65f ? Color : Color.White;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, fadeColor * Alpha, Rotation, texture.Size() / 2f, Scale * 0.6f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, Rotation, texture.Size() / 2f, Scale, SpriteEffects.None, 0f);


            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}