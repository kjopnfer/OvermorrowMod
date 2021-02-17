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
        private int storeDamage = 0;

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
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if(projectile.ai[0] == 0) // Make this projectile deal no damage
            {
                storeDamage = projectile.damage;
                projectile.damage = 0;
            }

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

                // Get the ID of the Parent NPC that was passed in via AI[1]
                NPC parent = Main.npc[(int)projectile.ai[1]];
                projectile.Center = parent.Center;
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
                                float launchSpeed = Main.expertMode ? 75f : 100f;
                                projectile.velocity = (move) / launchSpeed;
                                launchedProjectile = true;
                                Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)projectile.position.X, (int)projectile.position.Y);
                            }
                        }
                    }
                }
            }

            if(projectile.ai[0] == 480)
            {
                projectile.velocity *= Main.expertMode ? 2 : 3;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!spawnedProjectiles)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        // AI[0] is the ID of the parent projectile, AI[1] is the degree of the initial position in a circle 
                        Projectile.NewProjectile(projectile.Center, new Vector2(0, 0), ModContent.ProjectileType<ElectricBall>(), storeDamage, 1, Main.myPlayer, projectile.whoAmI, 30f * i);
                    }
                    spawnedProjectiles = true;
                }
            }

            projectile.ai[0]++;
        }
    }
}