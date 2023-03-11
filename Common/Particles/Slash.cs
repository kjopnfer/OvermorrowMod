using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Common.Particles
{
    public class Slash : CustomParticle
    {
        float maxTime = 30;
        public override void OnSpawn()
        {
            //particle.color = Color.Lerp(Color.Yellow, Color.Orange, particle.scale);

            //particle.rotation = MathHelper.ToRadians(Main.rand.Next(0, 90));
            particle.rotation = particle.velocity.ToRotation() + MathHelper.Pi;
            //maxTime = particle.customData[0];
            //particle.scale = 0.5f;
        }
        public override void Update()
        {
            if (particle.activeTime > maxTime) particle.Kill();
            float progress = (float)(particle.activeTime) / maxTime;
            //particle.scale = MathHelper.Lerp(0f, particle.customData[0], progress);
            //particle.scale += 0.025f;

            particle.alpha = 1f - progress;
            //particle.rotation += 0.06f;
            particle.velocity *= 0.98f;
            particle.rotation = particle.velocity.ToRotation() + MathHelper.Pi;

            /*if (particle.velocity.X > 0)
            {
                particle.velocity.X -= 0.05f;
            }
            if (particle.velocity.X < 0)
            {
                particle.velocity.X += 0.05f;
            }
            particle.velocity.Y -= 0.05f;*/
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = Particle.GetTexture(particle.type);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, Color.White * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale / 2, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}