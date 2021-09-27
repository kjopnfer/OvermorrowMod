using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class SapSpurt : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulHostile;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sap Spurt");
        }
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.friendly = true;
        }
        public override void AI()
        {
            for (int num1101 = 0; num1101 < 3; num1101++)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Honey, projectile.velocity.X, projectile.velocity.Y, 50, default, 2f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }

            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.HoneyBubbles, projectile.velocity.X, projectile.velocity.Y, 50, default, 0.4f);
                switch (num1103)
                {
                    case 0:
                        Main.dust[num1106].position = (Main.dust[num1106].position + projectile.Center * 5f) / 6f;
                        break;
                    case 1:
                        Main.dust[num1106].position = (Main.dust[num1106].position + (projectile.Center + projectile.velocity / 2f) * 5f) / 6f;
                        break;
                }
                Dust dust81 = Main.dust[num1106];
                dust81.velocity *= 0.1f;
                Main.dust[num1106].noGravity = true;
                Main.dust[num1106].fadeIn = 1f;
            }

            if (projectile.ai[0] > 8)
            {
                projectile.velocity.Y += 0.17f;
            }

            projectile.ai[0]++;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(3, 5); i++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ModContent.ProjectileType<SapSpurt2>(), (projectile.damage / 2) - 1, 10f, Main.myPlayer);
                }
            }
        }
    }
}