using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class DarkSmog : CustomParticle
    {
        public override string Texture => AssetDirectory.Textures + "Spotlight";
        public float maxTime = Main.rand.Next(8, 12) * 10;
        private int rotationDirection = 1;

        private Texture2D texture;
        public override void OnSpawn()
        {
            particle.customData[0] = particle.scale;
            //maxTime = particle.customData[1];
            particle.rotation += MathHelper.Pi / 2;
            rotationDirection = Main.rand.NextBool() ? 1 : -1;
            particle.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            string variant = Main.rand.NextBool() ? "Smoke2" : "Smoke2";
            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + variant).Value;
        }

        public override void Update()
        {
            // 0.05 == 20
            particle.velocity *= 0.95f;
            particle.rotation += 0.01f * rotationDirection;

            float progress = ModUtils.EaseOutQuad(particle.activeTime / maxTime);
            //particle.scale = MathHelper.SmoothStep(particle.customData[0], 0, progress);
            //particle.alpha = MathHelper.SmoothStep(particle.alpha, 0, particle.activeTime / maxTime);

            particle.alpha = Utils.GetLerpValue(0f, 0.05f, particle.activeTime / maxTime, clamped: true) * Utils.GetLerpValue(1f, 0.9f, particle.activeTime / maxTime, clamped: true);
            particle.scale += MathHelper.SmoothStep(0.075f, 0.01f, particle.activeTime / maxTime);

            if (particle.activeTime > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            //Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Smoke").Value;

            Color color = Color.Lerp(Color.Purple, Color.DarkRed, particle.activeTime / maxTime);

            //Color color = Color.Lerp(Color.Orange, Color.DarkRed, particle.activeTime / maxTime);
            //Color color = Color.Lerp(new Color(72, 50, 72), new Color(48, 25, 52), particle.activeTime / maxTime);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, color * particle.alpha, particle.rotation, texture.Size() / 2f, particle.scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}