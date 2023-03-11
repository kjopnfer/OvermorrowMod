using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Common.Particles
{
    public class Spark : CustomParticle
    {
        float maxTime = 70;
        public override void OnSpawn()
        {
            float ff = MathHelper.ToRadians(30);
            particle.velocity = particle.velocity.RotatedBy(Main.rand.NextFloat(-ff, ff));
        }
        public override void Update()
        {
            particle.rotation = particle.velocity.ToRotation();
            particle.alpha = (maxTime - particle.activeTime) / maxTime;
            particle.scale = MathHelper.Lerp(0, particle.scale, particle.alpha);
            particle.velocity += Vector2.UnitY * 0.5f;
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Particle.GetTexture(particle.type);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height), new Vector2(particle.scale, 0.05f), SpriteEffects.None, 0f);
        }
    }
}