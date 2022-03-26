using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class HeatCircle : ModProjectile
    {
        private const int RADIUS = 102;

        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heat Circle");
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
                return dist <= RADIUS;
            }

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.GetTexture(AssetDirectory.Textures + "PulseCircle");
            Color color = Color.Lerp(Color.Transparent, Color.Lerp(Color.Orange, Color.Red, (float)Math.Sin(Main.GlobalTime * 2)), Utils.Clamp(projectile.timeLeft, 0, 120) / 120f);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
