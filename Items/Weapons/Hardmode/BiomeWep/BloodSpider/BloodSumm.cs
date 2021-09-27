using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.BiomeWep.BloodSpider
{
    public class BloodSumm : ModProjectile
    {
        public override bool CanDamage() => false;
        private int timer = 0;
        private int movement = 0;
        float NPCtargetX = 0;
        float NPCtargetY = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);
        int mrand3 = Main.rand.Next(-170, -39);

        private bool go = false;

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Raven);
            projectile.width = 54;
            projectile.height = 26;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.netImportant = true;
            aiType = ProjectileID.Raven;
            projectile.penetrate = -1;
            projectile.timeLeft = 200000;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harpy");
            Main.projFrames[base.projectile.type] = 8;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<BloodCrawlerBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<BloodCrawlerBuff>()))
            {
                projectile.timeLeft = 2;
            }
            #endregion




            float distanceFromTarget = 500f;
            Vector2 targetCenter = projectile.position;
            bool foundTarget = false;
            projectile.tileCollide = false;
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Main.player[projectile.owner].Center);
                        bool closest = Vector2.Distance(projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;

                        if (((closest && inRange) || !foundTarget))
                        {
                            NPCtargetX = npc.Center.X;
                            NPCtargetY = npc.Center.Y - 140;
                            Vector2 Rot = npc.Center;
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }




            if (foundTarget)
            {
                projectile.velocity.Y = 0f;
                movement = 1;
                timer++;
                movement++;
                if (timer == 15)
                {
                    Projectile.NewProjectile(projectile.Center.X + 5, projectile.Center.Y, 0, 10, mod.ProjectileType("RottingEgg"), projectile.damage, 1f, projectile.owner, 0f);
                }
                if (timer == 30)
                {
                    Projectile.NewProjectile(projectile.Center.X - 5, projectile.Center.Y, 0, 10, mod.ProjectileType("RottingEgg"), projectile.damage, 1f, projectile.owner, 0f);
                    timer = 0;
                }


                if (movement == 50 && !go)
                {
                    go = true;
                    mrand2 = Main.rand.Next(-170, -39);
                    mrand = Main.rand.Next(40, 171);
                    mrand3 = Main.rand.Next(-25, 50);
                    movement = 0;
                }


                if (movement == 50 && go)
                {
                    go = false;
                    mrand2 = Main.rand.Next(-170, -39);
                    mrand = Main.rand.Next(40, 171);
                    mrand3 = Main.rand.Next(-25, 50);
                    movement = 0;
                }

                if (go)
                {
                    if (NPCtargetX + mrand > projectile.Center.X)
                    {
                        projectile.velocity.X += 0.9f;
                    }

                    if (NPCtargetX + mrand < projectile.Center.X)
                    {
                        projectile.velocity.X -= 0.9f;
                    }

                }

                if (!go)
                {
                    if (NPCtargetX + mrand2 > projectile.Center.X)
                    {
                        projectile.velocity.X += 0.9f;
                    }

                    if (NPCtargetX + mrand2 < projectile.Center.X)
                    {
                        projectile.velocity.X -= 0.9f;
                    }

                }

                if (NPCtargetY + mrand3 > projectile.Center.Y)
                {
                    projectile.velocity.Y += 2f;
                }
                if (NPCtargetY + mrand3 < projectile.Center.Y)
                {
                    projectile.velocity.Y -= 2f;
                }


                if (projectile.velocity.Y < -18f)
                {
                    projectile.velocity.Y = -18f;
                }

                if (projectile.velocity.Y > 18f)
                {
                    projectile.velocity.Y = 18f;
                }


                if (projectile.velocity.X < -9f)
                {
                    projectile.velocity.X = -9f;
                }

                if (projectile.velocity.X > 9f)
                {
                    projectile.velocity.X = 9f;
                }

            }
        }
    }
}
