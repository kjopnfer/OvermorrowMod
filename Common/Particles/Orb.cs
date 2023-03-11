using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;

namespace OvermorrowMod.Common.Particles
{
    public class Orb : CustomParticle
    {
        float maxTime = 120;
        public override void OnSpawn()
        {
            particle.color = Color.Lerp(Color.Yellow, Color.Orange, particle.scale);

            particle.rotation = MathHelper.ToRadians(Main.rand.Next(0, 90));
            maxTime = particle.customData[0];
        }
        public override void Update()
        {
            if (particle.activeTime > maxTime) particle.Kill();
            float progress = (float)(particle.activeTime) / maxTime;

            particle.alpha = 1f - progress;
            particle.rotation += 0.06f;
            particle.velocity *= 0.98f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = Particle.GetTexture(particle.type);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, Color.White * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale / 2, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}