using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class HeatCircle : ModProjectile
    {
        private const int RADIUS = 204;
        private const int WIDTH = 40;

        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crosshair");
        }

        public override void SetDefaults()
        {
            projectile.width = RADIUS * 2;
            projectile.height = RADIUS * 2;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 640;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (projectile.velocity != Vector2.Zero && projectile.ai[0]++ == 1200)
            {
                projectile.velocity = Vector2.Zero;
            }
        }

        public override bool CanDamage()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                
                float dist = (Main.player[i].Center - projectile.Center).Length();
                return dist <= RADIUS && dist >= RADIUS - WIDTH;
            }

            return base.CanDamage();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/PulseCircle");
            Color color = Color.Lerp(Color.Orange, Color.Red, (float)Math.Sin(Main.GlobalTime * 2));

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
