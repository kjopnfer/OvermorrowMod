using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class FloatingSeeds : ModProjectile
    {
        private int storeDamage;
        private bool canDescend = false;
        private bool goLeft = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Floating Seed");
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 16;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1200;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                storeDamage = projectile.damage;
                projectile.spriteDirection = Main.rand.NextBool(2) ? -1 : 1;
            }
            projectile.ai[0]++;
            if (!canDescend)
            {
                projectile.damage = 0;
                projectile.rotation += 1f;
                // Be affected by the shoot initial velocity
                if (projectile.ai[0] % 180 == 0) // After 3 seconds, set velocity to zero
                {
                    projectile.velocity = Vector2.Zero;
                    canDescend = true;
                    int chooseDirection = Main.rand.Next(2);
                    if (chooseDirection == 0) // Go left
                    {
                        projectile.velocity.X = 5f;
                        goLeft = true;
                    }
                    else // Go right
                    {
                        goLeft = false;
                        projectile.velocity.X = -5f;
                    }
                }
            }
            else
            {
                projectile.damage = storeDamage;

                // Start descending
                projectile.velocity.Y = 4.5f;
                projectile.rotation = 0.0f;

                // Float left and right
                if (goLeft)
                {
                    projectile.velocity.X -= 0.25f;
                    if (projectile.velocity.X == 0)
                    {
                        goLeft = false;
                        projectile.velocity.X = -5f;
                    }
                }
                else
                {
                    projectile.velocity.X += 0.25f;
                    if (projectile.velocity.X == 0)
                    {
                        goLeft = true;
                        projectile.velocity.X = 5f;
                    }
                }
            }
        }
    }
}