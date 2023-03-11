using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
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
            particle.alpha = Utils.GetLerpValue(0f, 0.05f, particle.activeTime / 60f, clamped: true) * Utils.GetLerpValue(1f, 0.9f, particle.activeTime / 60f, clamped: true);
            particle.scale = Utils.GetLerpValue(0f, 20f, particle.activeTime, clamped: true) * Utils.GetLerpValue(45f, 30f, particle.activeTime, clamped: true);
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_644").Value;
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

    public class Glow1 : CustomParticle
    {
        public override string Texture => AssetDirectory.Textures + "Spotlight";
        public override void Update()
        {
            particle.velocity.X += Main.windSpeedCurrent;
            particle.velocity.Y -= 0.4f;
            float progress = Utils.GetLerpValue(0, particle.customData[0], particle.activeTime);
            if (progress > 0.8f)
                particle.alpha = (progress - 0.5f) * 2;
            if (particle.activeTime > particle.customData[0])
                particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Particle.ParticleTextures[particle.type];
            //Texture2D texture2 = ModContent.GetTexture("Terraria/Projectile_" + ProjectileID.StardustTowerMark);
            spriteBatch.Reload(BlendState.Additive);
            //spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, 0f, texture.Size() / 2, particle.scale / 4, SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.8f, 0f, texture.Size() / 2, particle.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.5f, 0f, texture.Size() / 2, particle.scale * 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.2f, 0f, texture.Size() / 2, particle.scale * 3, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.01f, 0f, texture.Size() / 2, particle.scale * 7, SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture2, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.5f, 0f, texture2.Size() / 2, particle.scale / 4, SpriteEffects.None, 0f);
            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}