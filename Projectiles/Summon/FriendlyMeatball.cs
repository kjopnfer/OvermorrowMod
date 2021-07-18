using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Summon;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Summon
{
    public class FriendlyMeatball : ModProjectile
    {
        Vector2 Rot;
        private int PosCheck = 0;
        private int PosPlay = 0;
        private int HasChecked = 0;
        private int NumProj = 0;
        private int CenterXPly = 5;

        public override bool CanDamage() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Friendly Meatball");
            Main.projFrames[projectile.type] = 13;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 40;
            projectile.minion = true;
            projectile.minionSlots = 0.5f;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.netImportant = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 200000;
        }
        

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<MeatBallBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<MeatBallBuff>()))
            {
                projectile.timeLeft = 2;
            }
            #endregion


            float distanceFromTarget = 100f;
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
                            Vector2 Rot = npc.Center;
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            NumProj = Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<FriendlyMeatball>()];
            PosCheck++;
            if(PosCheck == 1)
            {
                PosPlay = NumProj;
            }
            if(PosCheck == 2)
            {
                HasChecked = PosPlay;
            }

            if(PosPlay == 0 && PosCheck > 2)
            {
                double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / 180); //Convert degrees to radians
                if(NumProj > HasChecked + 1)
                {
                    HasChecked += 1;
                    projectile.ai[1] = 0;
                    deg = 0;
                    rad = 0;
                }
                else
                {
                    
                projectile.position.X = 75 * (float)Math.Cos(rad) + Main.player[projectile.owner].Center.X - projectile.width / 2 - CenterXPly;
                projectile.position.Y = 75 * (float)Math.Sin(rad) + Main.player[projectile.owner].Center.Y - projectile.height / 2;


                projectile.ai[1] += 2.5f;
                }
            }


            if(PosPlay == 1 && PosCheck > 2)
            {
                double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / 180); //Convert degrees to radians
                if(NumProj > HasChecked + 1)
                {
                    HasChecked += 1;
                    projectile.ai[1] = 0;
                    deg = 0;
                    rad = 0;
                }
                else
                {

                projectile.position.X = -75 * (float)Math.Cos(rad) + Main.player[projectile.owner].Center.X - projectile.width / 2 - CenterXPly;
                projectile.position.Y = -75 * (float)Math.Sin(rad) + Main.player[projectile.owner].Center.Y - projectile.height / 2;


                projectile.ai[1] += 2.5f;
                }
            }


            if(PosPlay == 2 && PosCheck == 2)
            {
                projectile.ai[1] = 90;
            }

            if(PosPlay == 2 && PosCheck > 2)
            {
                double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / -180); //Convert degrees to radians
                if(NumProj > HasChecked + 1)
                {
                    HasChecked += 1;
                    projectile.ai[1] = 90;
                    rad = 0;
                }
                else
                {

                projectile.position.X = -75 * (float)Math.Cos(rad) + Main.player[projectile.owner].Center.X - projectile.width / 2 - CenterXPly;
                projectile.position.Y = 75 * (float)Math.Sin(rad) + Main.player[projectile.owner].Center.Y - projectile.height / 2;


                projectile.ai[1] += 2.5f;
                }

            }

            if(PosPlay == 3 && PosCheck == 2)
            {
                projectile.ai[1] = 90;
            }

            if(PosPlay == 3 && PosCheck > 2)
            {
                double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / -180); //Convert degrees to radians
                if(NumProj > HasChecked + 1)
                {
                    HasChecked += 1;
                    projectile.ai[1] = 90;
                    deg = 0;
                    rad = 0;
                }
                else
                {

                projectile.position.X = 75 * (float)Math.Cos(rad) + Main.player[projectile.owner].Center.X - projectile.width / 2 - CenterXPly;
                projectile.position.Y = -75 * (float)Math.Sin(rad) + Main.player[projectile.owner].Center.Y - projectile.height / 2;

                projectile.ai[1] += 2.5f;
                }

            }



            if(PosPlay == 4 && PosCheck == 2)
            {
                projectile.ai[1] = 120;
            }

            if(PosPlay == 4 && PosCheck > 2)
            {
                double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / -180); //Convert degrees to radians
                if(NumProj > HasChecked + 1)
                {
                    HasChecked += 1;
                    projectile.ai[1] = 120;
                    deg = 0;
                    rad = 0;
                }
                else
                {

                projectile.position.X = 125 * (float)Math.Cos(rad) + Main.player[projectile.owner].Center.X - projectile.width / 2 - CenterXPly;
                projectile.position.Y = -125 * (float)Math.Sin(rad) + Main.player[projectile.owner].Center.Y - projectile.height / 2;

                projectile.ai[1] += 0.7f;
                }
            }

            if(PosPlay == 5 && PosCheck == 2)
            {
                projectile.ai[1] = 240;
            }

            if(PosPlay == 5 && PosCheck > 2)
            {
                double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / -180); //Convert degrees to radians
                if(NumProj > HasChecked + 1)
                {
                    HasChecked += 1;
                    projectile.ai[1] = 240;
                    deg = 0;
                    rad = 0;
                }
                else
                {

                projectile.position.X = 125 * (float)Math.Cos(rad) + Main.player[projectile.owner].Center.X - projectile.width / 2 - CenterXPly;
                projectile.position.Y = -125 * (float)Math.Sin(rad) + Main.player[projectile.owner].Center.Y - projectile.height / 2;

                projectile.ai[1] += 0.7f;
                }
            }


            if(PosPlay == 6 && PosCheck == 2)
            {
                projectile.ai[1] = 360;
            }

            if(PosPlay == 6 && PosCheck > 2)
            {
                double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / -180); //Convert degrees to radians
                if(NumProj > HasChecked + 1)
                {
                    HasChecked += 1;
                    projectile.ai[1] = 360;
                    deg = 0;
                    rad = 0;
                }
                else
                {

                projectile.position.X = 125 * (float)Math.Cos(rad) + Main.player[projectile.owner].Center.X - projectile.width / 2 - CenterXPly;
                projectile.position.Y = -125 * (float)Math.Sin(rad) + Main.player[projectile.owner].Center.Y - projectile.height / 2;

                projectile.ai[1] += 0.7f;
                }

            }

            if(PosPlay == 7 && PosCheck == 2)
            {
                projectile.ai[1] = 60;
            }

            if(PosPlay == 7 && PosCheck > 2)
            {
                double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / -180); //Convert degrees to radians
                if(NumProj > HasChecked + 1)
                {
                    HasChecked += 1;
                    projectile.ai[1] = 60;
                    deg = 0;
                    rad = 0;
                }
                else
                {

                projectile.position.X = 125 * (float)Math.Cos(rad) + Main.player[projectile.owner].Center.X - projectile.width / 2 - CenterXPly;
                projectile.position.Y = -125 * (float)Math.Sin(rad) + Main.player[projectile.owner].Center.Y - projectile.height / 2;

                projectile.ai[1] += 0.7f;
                }

            }



            if(PosPlay == 8 && PosCheck == 2)
            {
                projectile.ai[1] = 180;
            }
            if(PosPlay == 8 && PosCheck > 2)
            {
                double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / -180); //Convert degrees to radians
                if(NumProj > HasChecked + 1)
                {
                    HasChecked += 1;
                    projectile.ai[1] = 180;
                    deg = 0;
                    rad = 0;
                }
                else
                {

                projectile.position.X = 125 * (float)Math.Cos(rad) + Main.player[projectile.owner].Center.X - projectile.width / 2 - CenterXPly;
                projectile.position.Y = -125 * (float)Math.Sin(rad) + Main.player[projectile.owner].Center.Y - projectile.height / 2;

                projectile.ai[1] += 0.7f;
                }

            }

            if(PosPlay == 9 && PosCheck == 2)
            {
                projectile.ai[1] = 300;
            }
            if(PosPlay == 9 && PosCheck > 2)
            {
                double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / -180); //Convert degrees to radians
                if(NumProj > HasChecked + 1)
                {
                    HasChecked += 1;
                    projectile.ai[1] = 300;
                    deg = 0;
                    rad = 0;
                }
                else
                {

                projectile.position.X = 125 * (float)Math.Cos(rad) + Main.player[projectile.owner].Center.X - projectile.width / 2 - CenterXPly;
                projectile.position.Y = -125 * (float)Math.Sin(rad) + Main.player[projectile.owner].Center.Y - projectile.height / 2;

                projectile.ai[1] += 0.7f;
                }

            }

            if(PosPlay == 10 && PosCheck > 2)
            {
                projectile.Kill();
            }
            if (foundTarget)
            {
                projectile.ai[0]++;
                if (projectile.ai[0] == 110)
                {
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = Rot;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(360f));
                    float speed = 7f;
                    Projectile.NewProjectile(projectile.Center, newpoint2 * speed, ModContent.ProjectileType<BloodSeeker>(), projectile.damage, 1f, projectile.owner, 0f);
                    projectile.ai[0] = 0;
                }
            }

            // Loop through the 13 animation frames, spending 5 ticks on each.
            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }
    }
}
