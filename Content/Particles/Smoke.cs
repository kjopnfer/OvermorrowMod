using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common;
using Terraria.ModLoader;
using Terraria;

namespace OvermorrowMod.Content.Particles
{
    public class Smoke : CustomParticle
    {
        float maxTime = 60;
        int smokeVariant = 0;
        float scaleRate = 0.005f;

        // customData[0] = startScale
        // customData[1] = maxTime
        // customData[2] = alpha
        // customData[3] = scaleRate

        public override void OnSpawn()
        {
            maxTime = 30;
            if (particle.customData[1] != 0) maxTime = particle.customData[1];
            if (particle.customData[3] != 0) scaleRate = particle.customData[3];

            particle.rotation = MathHelper.ToRadians(Main.rand.Next(0, 90));
            particle.scale = particle.customData[0];
            smokeVariant = Main.rand.Next(1, 8);
        }
        public override void Update()
        {

            if (particle.activeTime > maxTime) particle.Kill();
            /*if (particle.activeTime < 10)
            {
                float progress = (float)particle.activeTime / 10f;
                particle.scale = MathHelper.Lerp(0, particle.customData[0], progress);
                particle.alpha = progress;
            }
            if (particle.activeTime > 35)
            {
                float progress = (float)(particle.activeTime - 35) / 10f;
                particle.scale = MathHelper.Lerp(particle.customData[0], 0f, progress);
                particle.alpha = 1f - progress;
            }*/

            float progress = particle.activeTime / (float)maxTime;
            //particle.scale = MathHelper.Lerp(0f, particle.customData[0], progress);
            particle.scale += scaleRate;
            particle.alpha = 1f - progress;
            particle.rotation += 0.04f;
            particle.velocity.Y -= 0.03f;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            float alpha = particle.customData[2] != 0 ? particle.customData[2] : 1;

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "smoke_0" + smokeVariant).Value;
            //Color color = Color.Lerp(Color.Orange, Color.Black, particle.activeTime / maxTime);
            //spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, color * particle.alpha * alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale, SpriteEffects.None, 0f);
        }
    }
}