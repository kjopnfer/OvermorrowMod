using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Smoke : Particle
    {
        float maxTime = 420;
        public override void OnSpawn()
        {
            Color = Color.Lerp(Color.Purple, Color.Violet, Scale);
            CustomData[0] = Main.rand.Next(3, 6);
            if (Main.rand.NextBool(3))
            {
                CustomData[0] *= 2;
            }

            Rotation = MathHelper.ToRadians(Main.rand.Next(0, 90));
            Scale = 0;
        }
        public override void Update()
        {
            if (ActiveTime > maxTime) Kill();
            /*if (ActiveTime < 10)
            {
                float progress = (float)ActiveTime / 10f;
                Scale = MathHelper.Lerp(0, CustomData[0], progress);
                Alpha = progress;
            }
            if (ActiveTime > 35)
            {
                float progress = (float)(ActiveTime - 35) / 10f;
                Scale = MathHelper.Lerp(CustomData[0], 0f, progress);
                Alpha = 1f - progress;
            }*/

            float progress = (float)(ActiveTime) / maxTime;
            //Scale = MathHelper.Lerp(0f, CustomData[0], progress);
            Scale += 0.05f;
            Alpha = 1f - progress;

            Rotation += 0.06f;
            Velocity.Y -= 0.05f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "smoke").Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, Rotation, new Vector2(texture.Width, texture.Height) / 2, Scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}