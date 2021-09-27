using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class ChoreographLaser : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Choreograph Lightning Laser");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 200;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.extraUpdates = 100;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                for (int num1202 = 0; num1202 < 4; num1202++)
                {
                    Vector2 vector304 = projectile.position;
                    vector304 -= projectile.velocity * ((float)num1202 * 0.25f);
                    projectile.alpha = 255;
                    int num1200 = Dust.NewDust(vector304, 1, 1, DustID.UnusedWhiteBluePurple);
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