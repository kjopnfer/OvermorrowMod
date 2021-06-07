using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class BoltStreamBolt : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        private bool canAccelerate = false;
        private Vector2 storeVelocity;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt Stream");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            projectile.magic = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);

            if (projectile.ai[0] == 0) // Store the projectile's velocity when it was fired from the weapon
            {
                storeVelocity = projectile.velocity;
            }

            if(projectile.ai[0] >= 5 && projectile.ai[0] <= 25) // Make the projectile stops momentarily for 40 ticks
            {
                projectile.velocity = new Vector2(0, 0);
                if(projectile.ai[0] == 25) // Allow the projectile to accelerate
                {
                    canAccelerate = true;
                }
            }

            if(projectile.ai[0] <= 25) // Let's the 3rd projectile not get stuck in the ground
            {
                projectile.tileCollide = false;
            }
            else
            {
                projectile.tileCollide = true;
            }

            if (canAccelerate)
            {
                if (projectile.ai[0] == 25)
                {
                    projectile.velocity = storeVelocity;
                }
                else
                {
                    projectile.localAI[0] += 1f;
                    if (projectile.localAI[0] > 3f)
                    {
                        for (int num1202 = 0; num1202 < 4; num1202++)
                        {
                            Vector2 vector304 = projectile.position;
                            vector304 -= projectile.velocity * ((float)num1202 * 0.25f);
                            projectile.alpha = 255;
                            int num1200 = Dust.NewDust(vector304, 1, 1, 206);
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

                    if (projectile.ai[0] % 4 == 0)
                    {
                        projectile.velocity *= 1.47f;
                    }
                }
            }

            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(projectile.position, 1, 1, 206);
            }
            projectile.ai[0]++;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
        }
    }
}