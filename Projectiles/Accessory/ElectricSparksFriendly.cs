using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Accessory
{
    public class ElectricSparksFriendly : ModProjectile
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
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 290;
            projectile.alpha = 255;
            projectile.tileCollide = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);

            // Make projectile slow down until stationary
            if (projectile.timeLeft <= 125 && ++projectile.ai[0] % 40 == 0)
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

            Dust.NewDust(projectile.position, 1, 1, 206);
        }
    }
}