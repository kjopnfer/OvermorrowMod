using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public class FloatingSeeds : ModProjectile
    {
        private bool CanDescend = false;
        private bool GoLeft = true;
        public override bool CanHitPlayer(Player target) => CanDescend ? true : false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drifting Leaf"); 
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 16;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            if (projectile.ai[0]++ == 0)
            {
                projectile.spriteDirection = Main.rand.NextBool(2) ? -1 : 1;
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (projectile.getRect().Intersects(Main.projectile[i].getRect()) && Main.projectile[i].friendly)
                {
                    projectile.Kill();
                }
            }

            if (!CanDescend)
            {
                projectile.rotation += 1f;

                // Be affected by the shoot initial velocity
                if (projectile.ai[0] % 180 == 0) // After 3 seconds, set velocity to zero
                {
                    projectile.velocity = Vector2.Zero;
                    CanDescend = true;

                    if (Main.rand.NextBool(2)) // Go left
                    {
                        projectile.velocity.X = 5f;
                        GoLeft = true;
                    }
                    else // Go right
                    {
                        GoLeft = false;
                        projectile.velocity.X = -5f;
                    }
                }
            }
            else
            {
                // Start descending
                projectile.velocity.Y = 4.5f;

                // Float left and right
                if (GoLeft)
                {
                    projectile.rotation = projectile.velocity.X * 0.5f + (MathHelper.PiOver2 * 3);

                    projectile.velocity.X -= 0.25f;
                    if (projectile.velocity.X == 0)
                    {
                        GoLeft = false;
                        projectile.velocity.X = -5f;
                    }
                }
                else
                {
                    projectile.rotation = projectile.velocity.X * 0.5f - (MathHelper.PiOver2 * 3);

                    projectile.velocity.X += 0.25f;
                    if (projectile.velocity.X == 0)
                    {
                        GoLeft = true;
                        projectile.velocity.X = 5f;
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Grass, projectile.Center);

            Vector2 vector23 = projectile.Center + Vector2.One * -20f;
            int num137 = 40;
            int num138 = num137;
            for (int num139 = 0; num139 < 4; num139++)
            {
                int num140 = Dust.NewDust(vector23, num137, num138, DustID.Grass, 0f, 0f, 100, default(Color), 0.25f);
                Main.dust[num140].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
            }
        }
    }
}