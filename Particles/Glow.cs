using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using System;

namespace OvermorrowMod.Particles
{
    public class Glow : CustomParticle
    {
        public float maxTime = 60f;
        public override void OnSpawn()
        {
            particle.customData[0] = particle.scale;
            particle.rotation += MathHelper.Pi / 2;
            particle.scale = 0f;
        }
        public override void Update()
        {
            // 0.05 == 20
            particle.velocity *= 0.98f;
            particle.alpha = Terraria.Utils.InverseLerp(0f, 0.05f, particle.activeTime / 60f, clamped: true) * Terraria.Utils.InverseLerp(1f, 0.9f, particle.activeTime / 60f, clamped: true);
            particle.scale = Terraria.Utils.InverseLerp(0f, 20f, particle.activeTime, clamped: true) * Terraria.Utils.InverseLerp(45f, 30f, particle.activeTime, clamped: true);
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.GetTexture("Terraria/Projectile_644");
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Color col = Color.White * particle.alpha * 0.9f;
            col.A /= 2;
            Color col1 = particle.color * particle.alpha * 0.5f; // used
            col1.A = 0;
            Color col2 = col * 0.5f; // used
            col1 *= particle.scale;
            col2 *= particle.scale;
            Vector2 scale1 = new Vector2(0.3f, 2f) * particle.scale * particle.customData[0];
            Vector2 scale2 = new Vector2(0.3f, 1f) * particle.scale * particle.customData[0];
            Vector2 pos = particle.position - Main.screenPosition;
            SpriteEffects effects = SpriteEffects.None;
            spriteBatch.Draw(texture, pos, null, col1, (float)Math.PI / 2f + particle.rotation, origin, scale1, effects, 0f);
            spriteBatch.Draw(texture, pos, null, col1, particle.rotation, origin, scale2, effects, 0f);
            spriteBatch.Draw(texture, pos, null, col2, (float)Math.PI / 2f + particle.rotation, origin, scale1 * 0.6f, effects, 0f);
            spriteBatch.Draw(texture, pos, null, col2, particle.rotation, origin, scale2 * 0.6f, effects, 0f);
        }
    }
}