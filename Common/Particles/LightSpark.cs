using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class LightSpark : CustomParticle
    {
        public override string Texture => AssetDirectory.Textures + "Spotlight";
        public float maxTime = Main.rand.Next(4, 8) * 10;
        public override void OnSpawn()
        {
            particle.customData[0] = particle.scale;
            //maxTime = particle.customData[1];
            particle.rotation += MathHelper.Pi / 2;
            particle.scale = 0f;
        }

        public override void Update()
        {
            // 0.05 == 20
            particle.velocity *= 0.95f;
            particle.rotation = particle.velocity.ToRotation() + MathHelper.PiOver2;

            float progress = ModUtils.EaseOutQuad(particle.activeTime / maxTime);
            //particle.scale = MathHelper.SmoothStep(particle.customData[0], 0, progress);
            //particle.alpha = MathHelper.SmoothStep(particle.alpha, 0, particle.activeTime / maxTime);
            particle.alpha = Utils.GetLerpValue(0f, 0.05f, particle.activeTime / maxTime, clamped: true) * Utils.GetLerpValue(1f, 0.9f, particle.activeTime / maxTime, clamped: true);
            //particle.scale = Utils.GetLerpValue(0f, 20f, particle.activeTime, clamped: true) * Utils.GetLerpValue(45f, 30f, particle.activeTime, clamped: true);

            if (particle.activeTime > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;
            //Vector2 scale = new Vector2(0.3f, 2f) * particle.scale * particle.customData[0];
            float heightLerp = MathHelper.Lerp(particle.customData[0], 0, ModUtils.EaseOutQuad(Utils.Clamp(particle.activeTime, 0, maxTime) / maxTime));
            float widthLerp = MathHelper.Lerp(0.25f, 0, ModUtils.EaseOutQuad(Utils.Clamp(particle.activeTime, 0, maxTime) / maxTime));
            Color color = Color.Lerp(particle.color, Color.Red, particle.activeTime / maxTime);

            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, color * particle.alpha, particle.rotation + MathHelper.PiOver2, texture.Size() / 2f, new Vector2(heightLerp, widthLerp), SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}