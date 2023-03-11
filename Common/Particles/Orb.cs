using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Orb : Particle
    {
        float maxTime = 120;
        public override void OnSpawn()
        {
            Color = Color.Lerp(Color.Yellow, Color.Orange, Scale);
            Rotation = MathHelper.ToRadians(Main.rand.Next(0, 90));
            maxTime = CustomData[0];
        }
        public override void Update()
        {
            if (ActiveTime > maxTime) Kill();
            float progress = (float)(ActiveTime) / maxTime;

            Alpha = 1f - progress;
            Rotation += 0.06f;
            Velocity *= 0.98f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Orb").Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, Rotation, new Vector2(texture.Width, texture.Height) / 2, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.White * Alpha, Rotation, new Vector2(texture.Width, texture.Height) / 2, Scale / 2, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}