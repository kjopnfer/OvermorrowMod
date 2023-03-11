using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;

namespace OvermorrowMod.Common.Particles
{

    public class Smoke : CustomParticle
    {
        float maxTime = 420;
        public override void OnSpawn()
        {
            particle.color = Color.Lerp(Color.Purple, Color.Violet, particle.scale);
            particle.customData[0] = Main.rand.Next(3, 6);
            if (Main.rand.NextBool(3))
            {
                particle.customData[0] *= 2;
            }

            particle.rotation = MathHelper.ToRadians(Main.rand.Next(0, 90));
            particle.scale = 0;
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

            float progress = (float)(particle.activeTime) / maxTime;
            //particle.scale = MathHelper.Lerp(0f, particle.customData[0], progress);
            particle.scale += 0.05f;
            particle.alpha = 1f - progress;

            particle.rotation += 0.06f;
            particle.velocity.Y -= 0.05f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = Particle.GetTexture(particle.type);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}