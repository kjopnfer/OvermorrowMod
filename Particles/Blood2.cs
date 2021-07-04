
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects;

namespace OvermorrowMod.Particles
{
    public class Blood2 : CustomParticle
    {
        public override string Texture => "Textures/Empty";
        public override void Update()
        {
            if (particle.activeTime > 120) particle.Kill();
            particle.velocity.Y += 0.1f;
            
            particle.oldPos[0] = particle.position;
            for (int i = (particle.oldPos.Length - 1); i > 0; i--)
                particle.oldPos[i] = particle.oldPos[i - 1];
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.GetTexture("Terraria/Projectile_" + ProjectileID.StardustTowerMark);
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            /*for (int i = 0; i < particle.oldPos.Length - 1; i++)
            {
                if (particle.oldPos[i] == Vector2.Zero || particle.oldPos[i + 1] == Vector2.Zero) return;
                float len = 5;
                float scale1 = (float)i / (float)particle.oldPos.Length;
                float scale2 = (float)(i + 1) / (float)particle.oldPos.Length;
                for (int j = 0; j < len; j++)
                {
                    float progress = (float)(j / len);
                    float scale = MathHelper.Lerp(scale1, scale2, progress);
                    Vector2 pos = Vector2.Lerp(particle.oldPos[i], particle.oldPos[i+1], progress);
                    float progress2 = (float)i / (float)particle.oldPos.Length;
                    Color color = Color.Lerp(Color.DarkRed, Color.Red, progress2);
                    spriteBatch.Draw(texture, particle.oldPos[i] - Main.screenPosition, null, color * particle.alpha * (1f - progress2), 0f, origin, (1f - scale1) / texture.Width * 5f, SpriteEffects.None, 0f);
                }
            }*/
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, Color.DarkRed * particle.alpha, 0f, origin, particle.scale / texture.Width * 10f, SpriteEffects.None, 0f);
            TrailHelper helper = new TrailHelper(particle.oldPos, new TrailConfig{
                color = delegate(float progress)
                {
                    return Color.Lerp(Color.DarkRed, Color.Red, progress) * particle.alpha * MathHelper.Clamp((1f - progress) + 0.5f, 0f, 1f);
                },
                size = delegate(float progress)
                {
                    return (particle.scale * texture.Width) / 20 * (1f - progress);
                },
                Length = 10
            });
            helper.Draw();
        }
    }
}