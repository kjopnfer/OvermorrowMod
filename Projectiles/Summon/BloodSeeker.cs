using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class BloodSeeker : ModProjectile
    {
        float NPCtargetX = 0;
        float NPCtargetY = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Seeker");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.tileCollide = true;
        }

        public override void AI()
        {
            projectile.ai[0]++;

            float distanceFromTarget = 130f;
            Vector2 targetCenter = projectile.Center;
            bool foundTarget = false;
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (target.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(target.Center, projectile.Center);
                        bool closest = Vector2.Distance(projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;

                        if (between < 21 && projectile.ai[0] > 0)
                        {
                            target.StrikeNPC(projectile.damage, 1, 0);
                            projectile.ai[0] = -25;
                        }

                        if (((closest && inRange) || !foundTarget))
                        {
                            NPCtargetX = target.Center.X;
                            NPCtargetY = target.Center.Y;
                            distanceFromTarget = between;
                            targetCenter = target.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            if (foundTarget)
            {
                projectile.rotation = projectile.velocity.X * 0.03f;

                if (NPCtargetX > projectile.Center.X)
                {
                    projectile.velocity.X += 0.5f;
                }

                if (NPCtargetX < projectile.Center.X)
                {
                    projectile.velocity.X -= 0.5f;
                }

                if (NPCtargetY > projectile.Center.Y)
                {
                    projectile.velocity.Y += 0.5f;
                }
                if (NPCtargetY < projectile.Center.Y)
                {
                    projectile.velocity.Y -= 0.5f;
                }

                if (projectile.velocity.Y < -6.5f)
                {
                    projectile.velocity.Y = -6.5f;
                }

                if (projectile.velocity.Y > 6.5f)
                {
                    projectile.velocity.Y = 6.5f;
                }


                if (projectile.velocity.X < -6.5f)
                {
                    projectile.velocity.X = -6.5f;
                }

                if (projectile.velocity.X > 6.5f)
                {
                    projectile.velocity.X = 6.5f;
                }
            }
            else
            {
                projectile.rotation = 0;
                projectile.velocity.Y -= 0.9f;
            }
        }
    }
}