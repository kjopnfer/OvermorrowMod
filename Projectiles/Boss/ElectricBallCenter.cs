using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class ElectricBallCenter : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        private bool spawnedProjectiles = false;
        private bool launchedProjectile = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Ball");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            // Unnecessary code
            if (++projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

            if (projectile.ai[0] < 180) // Stay still for 120 seconds
            {
                projectile.velocity = new Vector2(0, 0);
            }
            else // Launch at the nearest player
            {
                bool target = false;
                Vector2 move = Vector2.Zero;
                float distance = 6000f; // Search distance
                if (!launchedProjectile)
                {
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
                                target = true;
                                projectile.velocity = (move) / 100f;
                                launchedProjectile = true;
                            }
                        }
                    }
                }
            }

            if (!spawnedProjectiles)
            {
                for (int i = 0; i < 7; i++)
                {
                    // AI[0] is the ID of the parent projectile, AI[1] is the degree of the initial position in a circle 
                    Projectile.NewProjectile(projectile.Center, new Vector2(0, 0), ModContent.ProjectileType<ElectricBall>(), (int)projectile.ai[1], 1, Main.myPlayer, projectile.whoAmI, 30f * i);
                }
                spawnedProjectiles = true;
            }

            projectile.ai[0]++;
        }
    }
}