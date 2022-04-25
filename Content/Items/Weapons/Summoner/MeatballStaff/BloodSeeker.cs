using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.MeatballStaff
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
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            float distanceFromTarget = 130f;
            Vector2 targetCenter = Projectile.Center;
            bool foundTarget = false;
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (target.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(target.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;

                        if (between < 21 && Projectile.ai[0] > 0)
                        {
                            target.StrikeNPC(Projectile.damage, 1, 0);
                            Projectile.ai[0] = -25;
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
                Projectile.rotation = Projectile.velocity.X * 0.03f;

                if (NPCtargetX > Projectile.Center.X)
                {
                    Projectile.velocity.X += 0.5f;
                }

                if (NPCtargetX < Projectile.Center.X)
                {
                    Projectile.velocity.X -= 0.5f;
                }

                if (NPCtargetY > Projectile.Center.Y)
                {
                    Projectile.velocity.Y += 0.5f;
                }
                if (NPCtargetY < Projectile.Center.Y)
                {
                    Projectile.velocity.Y -= 0.5f;
                }

                if (Projectile.velocity.Y < -6.5f)
                {
                    Projectile.velocity.Y = -6.5f;
                }

                if (Projectile.velocity.Y > 6.5f)
                {
                    Projectile.velocity.Y = 6.5f;
                }


                if (Projectile.velocity.X < -6.5f)
                {
                    Projectile.velocity.X = -6.5f;
                }

                if (Projectile.velocity.X > 6.5f)
                {
                    Projectile.velocity.X = 6.5f;
                }
            }
            else
            {
                Projectile.rotation = 0;
                Projectile.velocity.Y -= 0.9f;
            }
        }
    }
}