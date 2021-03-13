using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class ThornHeadFriendly : ModProjectile
    {
        private bool canGrow = false;
        private int storeDamage;
        private Vector2 storeVelocity;
        private enum spawnDirection { left, right }
        private spawnDirection chooseDirection;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn of Iorich");
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1f;
        }

        public override void AI()
        {
            // Projectile special property initialization
            if (projectile.ai[0] == 0) // Store the initial damage value when first spawned, but make it deal no damage
            {
                storeDamage = projectile.damage;
                storeVelocity = projectile.velocity;
                projectile.damage = 0;
                projectile.velocity = Vector2.Zero;

                int direction = Main.rand.Next(2);

                if (direction == 0)
                {
                    chooseDirection = spawnDirection.left;
                }
                else
                {
                    chooseDirection = spawnDirection.right;
                }
            }
            projectile.ai[0]++;

            // Initial dust spawning to show where it is spawning
            if (!canGrow)
            {
                projectile.velocity = Vector2.Zero;
                projectile.alpha = 255;

                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > 3f)
                {
                    // Change dust to green/brown
                    int randChoice = Main.rand.Next(2);
                    if (randChoice == 0)
                    {
                        int dust = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 0, 0, 0, 0, default, 1.14f);
                        Main.dust[dust].noGravity = true;
                    }
                    else
                    {
                        int dust = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 2, 0, 0, 0, default, 1.14f);
                        Main.dust[dust].noGravity = true;
                    }
                }

                if (projectile.ai[0] % 30 == 0) // Spend half a second doing nothing
                {
                    canGrow = true;
                    Main.PlaySound(new LegacySoundStyle(SoundID.Grass, 0)); // Grass
                    projectile.alpha = 0;
                }
            }
            else // Start spawning the projectile
            {
                projectile.damage = storeDamage;
                projectile.velocity = storeVelocity;

                if (projectile.ai[0] >= 120) // Stop spawning the thorn body
                {
                    projectile.velocity = Vector2.Zero;
                }
                else
                {
                    if (projectile.ai[0] % 4 == 0)
                    {
                        // Spawn body, this randomizes which part goes first to make it non-monotonous visually
                        // Left if even, right if odd
                        if (chooseDirection == spawnDirection.left)
                        {
                            if (projectile.ai[1] % 0.75f == 0) // Left facing
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(new Vector2(projectile.position.X + (projectile.width / 2) + 5, projectile.position.Y + (projectile.height / 2)), Vector2.Zero, ModContent.ProjectileType<ThornBody1Friendly>(), projectile.damage, 2.5f, Main.myPlayer, projectile.whoAmI, 0f);
                                }
                            }
                            else // Right facing
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(new Vector2(projectile.position.X + (projectile.width / 2) + 5, projectile.position.Y + (projectile.height / 2)), Vector2.Zero, ModContent.ProjectileType<ThornBody2Friendly>(), projectile.damage, 2.5f, Main.myPlayer, projectile.whoAmI, 0f);
                                }
                            }
                        }
                        else
                        {
                            if (projectile.ai[1] % 0.75f == 0) // Right facing
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(new Vector2(projectile.position.X + (projectile.width / 2) + 5, projectile.position.Y + (projectile.height / 2)), Vector2.Zero, ModContent.ProjectileType<ThornBody2Friendly>(), projectile.damage, 2.5f, Main.myPlayer, projectile.whoAmI, 0f);
                                }
                            }
                            else // left facing
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(new Vector2(projectile.position.X + (projectile.width / 2) + 5, projectile.position.Y + (projectile.height / 2)), Vector2.Zero, ModContent.ProjectileType<ThornBody1Friendly>(), projectile.damage, 2.5f, Main.myPlayer, projectile.whoAmI, 0f);
                                }
                            }
                        }
                        projectile.ai[1]++;
                    }
                }
            }

            // Make projectiles gradually disappear
            if (projectile.timeLeft <= 60)
            {
                projectile.alpha += 5;
            }
        }
    }
}