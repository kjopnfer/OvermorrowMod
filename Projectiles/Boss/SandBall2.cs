using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class SandBall2 : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                projectile.netUpdate = true;
                projectile.ai[0]++;
            }

            projectile.ai[1]++;

            if (projectile.ai[1] >= 120 && projectile.ai[1] % 40 == 0)
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


            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 32, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 1f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;

                Dust dustTrail = Dust.NewDustPerfect(new Vector2(projectile.position.X, projectile.position.Y), 32, projectile.velocity);
                dustTrail.position = (Main.dust[num1110].position + projectile.Center) / 2f;
                dustTrail.noGravity = true;
            }
        }
    }
}