using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Glow : Particle
    {
        public float maxTime = 60f;
        public override void OnSpawn()
        {
            CustomData[0] = Scale;
            Rotation += MathHelper.Pi / 2;
            Scale = 0f;
        }
        public override void Update()
        {
            // 0.05 == 20
            Velocity *= 0.98f;
            Alpha = Utils.GetLerpValue(0f, 0.05f, ActiveTime / 60f, clamped: true) * Utils.GetLerpValue(1f, 0.9f, ActiveTime / 60f, clamped: true);
            Scale = Utils.GetLerpValue(0f, 20f, ActiveTime, clamped: true) * Utils.GetLerpValue(45f, 30f, ActiveTime, clamped: true);
            if (ActiveTime > maxTime) Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_644").Value;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Color col = Color.White * Alpha * 0.9f;
            col.A /= 2;
            Color col1 = Color * Alpha * 0.5f; // used
            col1.A = 0;
            Color col2 = col * 0.5f; // used
            col1 *= Scale;
            col2 *= Scale;
            Vector2 scale1 = new Vector2(0.3f, 2f) * Scale * CustomData[0];
            Vector2 scale2 = new Vector2(0.3f, 1f) * Scale * CustomData[0];
            Vector2 pos = Position - Main.screenPosition;
            SpriteEffects effects = SpriteEffects.None;
            spriteBatch.Draw(texture, pos, null, col1, (float)Math.PI / 2f + Rotation, origin, scale1, effects, 0f);
            spriteBatch.Draw(texture, pos, null, col1, Rotation, origin, scale2, effects, 0f);
            spriteBatch.Draw(texture, pos, null, col2, (float)Math.PI / 2f + Rotation, origin, scale1 * 0.6f, effects, 0f);
            spriteBatch.Draw(texture, pos, null, col2, Rotation, origin, scale2 * 0.6f, effects, 0f);
        }
    }

    public class Glow1 : Particle
    {
        public override void Update()
        {
            Velocity.X += Main.windSpeedCurrent;
            Velocity.Y -= 0.4f;
            float progress = Utils.GetLerpValue(0, CustomData[0], ActiveTime);
            if (progress > 0.8f)
                Alpha = (progress - 0.5f) * 2;
            if (ActiveTime > CustomData[0])
                Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
            //Texture2D texture2 = ModContent.GetTexture("Terraria/Projectile_" + ProjectileID.StardustTowerMark);
            spriteBatch.Reload(BlendState.Additive);
            //spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, 0f, texture.Size() / 2, Scale / 4, SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha * 0.8f, 0f, texture.Size() / 2, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha * 0.5f, 0f, texture.Size() / 2, Scale * 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha * 0.2f, 0f, texture.Size() / 2, Scale * 3, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha * 0.01f, 0f, texture.Size() / 2, Scale * 7, SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture2, Position - Main.screenPosition, null, Color * Alpha * 0.5f, 0f, texture2.Size() / 2, Scale / 4, SpriteEffects.None, 0f);
            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}