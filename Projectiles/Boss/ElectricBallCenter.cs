using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class ElectricBallCenter : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        private bool spawnedProjectiles = false;
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
            projectile.timeLeft = 720;
            projectile.alpha = 255;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0) // Make this projectile deal no damage
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

            if (projectile.ai[0] < 240) // Stay still for 120 seconds
            {
                projectile.velocity = new Vector2(0, 0);

                // Get the ID of the Parent NPC that was passed in via AI[1]
                NPC parent = Main.npc[(int)projectile.ai[1]];
                projectile.Center = parent.Center;
                projectile.netUpdate = true;

                if (projectile.ai[0] == 239)
                {
                    Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)projectile.position.X, (int)projectile.position.Y);
                }
            }
            else // Launch at the nearest player
            {
                Vector2 move = Vector2.Zero;
                float distance = 6000f; // Search distance
                if (projectile.ai[0] < 710/*!launchedProjectile*/)
                {
                    Vector2 direction = Main.player[Main.npc[(int)projectile.ai[1]].target].Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
                    if (distanceTo < distance)
                    {
                        move = direction;
                        direction.SafeNormalize(Vector2.Zero);
                        float launchSpeed = projectile.ai[0] > 480 ? 0.05f : 0.025f /*75f : 100f*/;
                        direction *= launchSpeed;
                        distance = distanceTo;
                        //projectile.velocity = (move) / launchSpeed;
                        float inertia = 150f;
                        projectile.velocity = (projectile.velocity * (inertia - 1) + direction) / inertia;
                    }
                }
            }

            if (projectile.ai[0] == 710)
            {
                projectile.velocity *= 1.5f;
            }


            if (!spawnedProjectiles)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        // AI[0] is the ID of the parent projectile, AI[1] is the degree of the initial position in a circle 
                        Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ElectricBall>(), storeDamage, 1, Main.myPlayer, projectile.whoAmI, 30f * i);
                    }
                }
                spawnedProjectiles = true;
                projectile.netUpdate = true;
            }

            projectile.ai[0]++;
        }
    }
}