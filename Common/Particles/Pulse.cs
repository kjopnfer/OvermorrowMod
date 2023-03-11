using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Pulse : Particle
    {
        public float maxSize { get { return CustomData[0]; } set { CustomData[0] = value; } }
        public float maxTime { get { return CustomData[1]; } set { CustomData[1] = value; } }
        public override void OnSpawn()
        {
            if (CustomData[1] == 0) CustomData[1] = 60;
            if (CustomData[2] == 0) CustomData[2] = 1;
            if (CustomData[3] == 0) CustomData[3] = 1;

            maxTime = CustomData[1] == 0 ? 60 : CustomData[1];
            maxSize = Scale;
            Scale = 0f;
        }

        public override void Update()
        {
            Velocity = Vector2.Zero;
            //float progress = ActiveTime / maxTime;

            float progress = ModUtils.EaseOutQuad(ActiveTime / maxTime);
            Scale = MathHelper.SmoothStep(Scale, maxSize, progress);
            Alpha = MathHelper.SmoothStep(Alpha, 0, ActiveTime / maxTime);
            if (ActiveTime > maxTime) Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, Rotation, origin, Scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}