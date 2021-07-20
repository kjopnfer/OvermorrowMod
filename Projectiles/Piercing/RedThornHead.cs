using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class RedThornHead : ModProjectile
    {
        private bool canGrow = false;
        private int storeDamage;
        private Vector2 storeVelocity;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Thorn");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 240;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1f;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Projectile special property initialization
            if (projectile.ai[0] == 0) // Store the initial damage value when first spawned, but make it deal no damage
            {
                storeDamage = projectile.damage;
                storeVelocity = projectile.velocity;
                projectile.damage = 0;
                projectile.velocity = Vector2.Zero;                
            }

            projectile.ai[0]++;

            // Initial dust spawning to show where it is spawning
            if (!canGrow)
            {
                projectile.velocity = Vector2.Zero;
                projectile.alpha = 255;

                if (projectile.ai[0] % 20 == 0) // Spend 1/3 seconds doing nothing
                {
                    canGrow = true;
                    Main.PlaySound(new LegacySoundStyle(SoundID.Grass, 0)); // Grass
                }
            }
            else // Start spawning the projectile
            {
                projectile.damage = storeDamage;
                projectile.velocity = storeVelocity;
                projectile.alpha = 0;

                if (projectile.ai[0] < 110)
                {
                    if (projectile.ai[0] % 2 == 0)
                    {
                        projectile.ai[1]++;
                        if (projectile.ai[1] % 2 == 0)
                        {
                            Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<RedThornBody1>(), projectile.damage, 2f, projectile.owner, projectile.whoAmI);
                        }
                        else
                        {
                            Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<RedThornBody2>(), projectile.damage, 2f, projectile.owner, projectile.whoAmI);
                        }
                    }
                }
                else
                {
                    projectile.velocity = Vector2.Zero;
                }
            }
        }
    }
}