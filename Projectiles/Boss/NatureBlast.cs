using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class NatureBlast : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        private int randDelay;
        private bool foundTarget;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 540;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                randDelay = Main.rand.Next(90, 200);
            }

            for (int num1101 = 0; num1101 < 3; num1101++)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 1.2f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }

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

            projectile.ai[0]++;

            if (projectile.ai[0] == 65)
            {
                projectile.velocity = Vector2.Zero;
            }

            if (projectile.ai[0] == randDelay)
            {
                Vector2 move = Vector2.Zero;
                float distance = 6000f; // Search distance
                for (int k = 0; k < Main.maxPlayers; k++) // Loop through the player array
                {
                    if (Main.player[k].active && !Main.player[k].dead)
                    {
                        Vector2 newMove = Main.player[k].Center - projectile.Center;
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance)
                        {
                            move = newMove;
                            distance = distanceTo;
                            float launchSpeed = 100f;
                            if(distance < 50)
                            {
                                launchSpeed /= 1.5f;
                            }
                            foundTarget = true;
                            projectile.velocity = (move) / launchSpeed;
                        }
                    }
                }
            }

            if (foundTarget)
            {
                if(projectile.ai[0] % 20 == 0)
                {
                    projectile.velocity *= Main.expertMode ? 1.55f : 1.33f;
                }
            }
        }
    }
}