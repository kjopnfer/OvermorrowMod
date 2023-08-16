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

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heat Circle");
        }

        public override void SetDefaults()
        {
            Projectile.width = RADIUS * 2;
            Projectile.height = RADIUS * 2;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 640;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero && Projectile.ai[0]++ == 1200)
            {
                Projectile.velocity = Vector2.Zero;
            }
        }

        public override bool? CanDamage()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                float dist = (Main.player[i].Center - Projectile.Center).Length();
                if (dist <= RADIUS) return true;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;
            Color color = Color.Lerp(Color.Transparent, Color.Lerp(Color.Orange, Color.Red, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2)), Utils.Clamp(Projectile.timeLeft, 0, 120) / 120f);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, 0.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
