using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Magic.CreepingD
{
    public class IronBolt : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.SlushBall;

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 150;
            projectile.width = 14;
            projectile.height = 14;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.localAI[0] += 1f;

            if (projectile.localAI[0] > 3f)
            {
                for (int num1202 = 0; num1202 < 4; num1202++)
                {
                    Vector2 vector304 = projectile.position;
                    vector304 -= projectile.velocity * ((float)num1202 * 0.25f);
                    projectile.alpha = 255;
                    int num1200 = Dust.NewDust(vector304, 1, 1, 8);
                    Main.dust[num1200].position = vector304;
                    Dust expr_140F1_cp_0 = Main.dust[num1200];
                    expr_140F1_cp_0.position.X = expr_140F1_cp_0.position.X + (float)(projectile.width / 2);
                    Dust expr_14115_cp_0 = Main.dust[num1200];
                    expr_14115_cp_0.position.Y = expr_14115_cp_0.position.Y + (float)(projectile.height / 2);
                    Main.dust[num1200].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Dust dust81 = Main.dust[num1200];
                    dust81.velocity *= 0.2f;
                }
            }
        }
    }
}