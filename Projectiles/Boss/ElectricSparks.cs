using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class ElectricSparks : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Sparks");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 690;
            projectile.alpha = 255;
            projectile.tileCollide = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);

            // Make projectile slow down until stationary
            if (projectile.timeLeft <= 525)
            {
                if (projectile.ai[0] % 40 == 0)
                {
                    if (projectile.velocity.X != 0)
                    {
                        if (projectile.velocity.X < 0)
                        {
                            projectile.velocity.X += 1;
                        }
                        else
                        {
                            projectile.velocity.X -= 1;
                        }
                    }

                    if (projectile.velocity.Y != 0)
                    {
                        if (projectile.velocity.Y < 0)
                        {
                            projectile.velocity.Y += 1;
                        }
                        else
                        {
                            projectile.velocity.Y -= 1;
                        }
                    }
                }
            }

            projectile.ai[0]++;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                for (int num1202 = 0; num1202 < 2; num1202++)
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

                Dust.NewDustPerfect(projectile.Center, 206, null, 0, default, 1.5f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 90);
        }
    }
}