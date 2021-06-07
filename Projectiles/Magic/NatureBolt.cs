using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class NatureBolt : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 2;
            projectile.timeLeft = 420;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            projectile.magic = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 1f, 0f);
            projectile.ai[0] += 4;
            projectile.ai[1] = 15; // Defines the radius

            /*for (int num1101 = 0; num1101 < 3; num1101++)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 1.2f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }*/

            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 0.4f);
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

            for (int i = 0; i < 2; i++)
            {
                Vector2 dustPos = projectile.Center + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.ai[0]));
                Dust dust = Dust.NewDustPerfect(dustPos, 107, Vector2.Zero, 0, default, 1f);
                dust.noGravity = true;
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 dustPos = projectile.Center - new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.ai[0]));
                Dust dust = Dust.NewDustPerfect(dustPos, 107, Vector2.Zero, 0, default, 1f);
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int projectiles = 8;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < projectiles; i++)
                {
                    Projectile.NewProjectile(projectile.Center + Utils.RotatedBy(new Vector2(40f, 0f), MathHelper.ToRadians(i * 45)), Vector2.Zero, ModContent.ProjectileType<NatureDust>(), projectile.damage / 2, 2, Main.myPlayer);
                    //Dust.NewDustPerfect(projectile.Center + Utils.RotatedBy(new Vector2(80f, 0f), MathHelper.ToRadians(i * 20)), 107, Vector2.Zero, 0, default, 1);
                }
            }
            projectile.Kill();
        }
    }
}