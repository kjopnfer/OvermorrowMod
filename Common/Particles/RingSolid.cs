using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class RingSolid : Particle
    {
        public float maxSize;
        public float maxTime;
        public override void OnSpawn()
        {
            if (CustomData[0] == 0) CustomData[0] = 60;

            maxTime = CustomData[1] == 0 ? 60 : CustomData[1];
            maxSize = Scale;
            Scale = 0f;
        }

        public override void Update()
        {
            Velocity = Vector2.Zero;

            float fadeTime = maxTime - 10;
;
            float progress = ModUtils.EaseOutCirc(ActiveTime / maxTime);
            Scale = MathHelper.Lerp(Scale, maxSize, progress);
            //Alpha = MathHelper.SmoothStep(Alpha, 0, Utils.Clamp(ActiveTime - 10, 0, fadeTime) / fadeTime);
            Alpha = MathHelper.Lerp(Alpha, 0, ActiveTime / maxTime);

            if (ActiveTime > maxTime) Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "RingSolid").Value;
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, Rotation, origin, Scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}