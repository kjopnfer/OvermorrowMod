using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Particles
{
    public class Shockwave : CustomParticle
    {
        public override string Texture => "Textures/Empty";
        public float maxSize { get { return particle.customData[0]; } set { particle.customData[0] = value; } }
        float maxTime = 60f;
        public override void OnSpawn()
        {
            /*if (Main.rand.NextBool(3))
            {
                particle.customData[0] *= 2;
            }*/
            if (particle.customData[1] == 0) particle.customData[1] = 1;
            if (particle.customData[2] == 0) particle.customData[2] = 1;
            if (particle.customData[3] == 0) particle.customData[3] = 1;
            maxSize = particle.scale;
            particle.scale = 0f;
        }
        public override void Update()
        {
            particle.velocity = Vector2.Zero;
            float progress = (float)particle.activeTime / maxTime;
            particle.scale = MathHelper.Lerp(particle.scale, maxSize, progress);
            particle.alpha = MathHelper.Lerp(particle.alpha, 0, progress);
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.GetTexture("Terraria/Projectile_" + ProjectileID.StardustTowerMark);
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            float progress = (float)particle.activeTime / maxTime;
            float baseScale = 512f / (float)texture.Width;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, 0f, origin, baseScale * particle.scale, SpriteEffects.None, 0f);
        }
    }
}