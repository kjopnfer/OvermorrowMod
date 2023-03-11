using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class LightSpark : Particle
    {
        public float maxTime = Main.rand.Next(4, 8) * 10;
        public override void OnSpawn()
        {
            CustomData[0] = Scale;
            //maxTime = CustomData[1];
            Rotation += MathHelper.Pi / 2;
            Scale = 0f;
        }

        public override void Update()
        {
            // 0.05 == 20
            Velocity *= 0.95f;
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;

            float progress = ModUtils.EaseOutQuad(ActiveTime / maxTime);
            //Scale = MathHelper.SmoothStep(CustomData[0], 0, progress);
            //Alpha = MathHelper.SmoothStep(Alpha, 0, ActiveTime / maxTime);
            Alpha = Utils.GetLerpValue(0f, 0.05f, ActiveTime / maxTime, clamped: true) * Utils.GetLerpValue(1f, 0.9f, ActiveTime / maxTime, clamped: true);
            //Scale = Utils.GetLerpValue(0f, 20f, ActiveTime, clamped: true) * Utils.GetLerpValue(45f, 30f, ActiveTime, clamped: true);

            if (ActiveTime > maxTime) Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;
            //Vector2 scale = new Vector2(0.3f, 2f) * Scale * CustomData[0];
            float heightLerp = MathHelper.Lerp(CustomData[0], 0, ModUtils.EaseOutQuad(Utils.Clamp(ActiveTime, 0, maxTime) / maxTime));
            float widthLerp = MathHelper.Lerp(0.25f, 0, ModUtils.EaseOutQuad(Utils.Clamp(ActiveTime, 0, maxTime) / maxTime));
            Color color = Color.Lerp(Color, Color.White, ActiveTime / maxTime);

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, color * Alpha, Rotation, texture.Size() / 2f, new Vector2(heightLerp, widthLerp), SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, Rotation, texture.Size() / 2f, scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}