using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.Summon;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class SkeleTank : ModProjectile
    {
        public override bool CanDamage() => false;

        Vector2 NPCtarget;
        bool targetjump;

        private bool flying = false;

        bool PosCheck = false;
        int PosPlay = 0;
        private int NumProj = 0;

        int Randompos = Main.rand.Next(150, 275);

        int rockettimer = 0;
        float NPCtargetWidth = 0;
        int postimer = 0;



        bool straight = false;
        bool angle = false;
        bool up = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeletank");
            Main.projFrames[projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 40;
            projectile.minion = true;
            projectile.minionSlots = 1f;
        }


        public override void AI()
        {


            Player player = Main.player[projectile.owner];
            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<TankBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<TankBuff>()))
            {
                projectile.timeLeft = 2;
            }
            #endregion






            NumProj = Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<SkeleTank>()];
            if (!PosCheck)
            {
                PosPlay = NumProj;
                PosCheck = true;
            }





            if (!flying)
            {
                if (projectile.velocity.Y < 7f)
                {
                    projectile.velocity.Y += 0.4f;
                }


                if (projectile.velocity.Y > 7f)
                {
                    projectile.velocity.Y = 7f;
                }
            }



            float distanceFromTarget = 100f;
            Vector2 targetCenter = projectile.position;
            bool foundTarget = false;
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy() && Vector2.Distance(Main.MouseWorld, npc.Center) < 220f)
                    {
                        float between = Vector2.Distance(npc.Center, projectile.Center);
                        bool closest = Vector2.Distance(Main.MouseWorld, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;

                        if (((closest && inRange) || !foundTarget))
                        {
                            NPCtargetWidth = npc.width / 2;
                            NPCtarget = npc.Center;
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
            if (foundTarget)
            {
                if (!flying)
                {
                    projectile.rotation = 0f;
                    if (NPCtarget.Y < projectile.Center.Y - 60)
                    {
                        angle = true;
                    }
                    else
                    {
                        angle = false;
                    }

                    if (NPCtarget.Y < projectile.Center.Y - 100 && Vector2.Distance(new Vector2(projectile.Center.X, projectile.Center.X), new Vector2(NPCtarget.X, NPCtarget.X)) < 50f)
                    {
                        up = true;
                    }
                    else
                    {
                        up = false;
                    }

                    if (!angle && !up)
                    {
                        straight = true;
                    }
                    else
                    {
                        straight = false;
                    }

                    if (projectile.velocity.Y < -7f)
                    {
                        projectile.velocity.Y = -7f;
                    }

                    if (projectile.velocity.X > 7f)
                    {
                        projectile.velocity.X = 7f;
                    }

                    if (projectile.velocity.X < -7f)
                    {
                        projectile.velocity.X = -7f;
                    }




                    targetjump = true;

                    if (Vector2.Distance(new Vector2(projectile.Center.X, projectile.Center.X), new Vector2(NPCtarget.X, NPCtarget.X)) > Randompos + NPCtargetWidth)
                    {
                        if (NPCtarget.X > projectile.Center.X)
                        {
                            projectile.spriteDirection = -1;
                            projectile.velocity.X += 0.07f;
                        }

                        if (NPCtarget.X < projectile.Center.X)
                        {
                            projectile.spriteDirection = 1;
                            projectile.velocity.X -= 0.07f;
                        }
                    }
                    else
                    {
                        if (NPCtarget.X > projectile.Center.X)
                        {
                            projectile.spriteDirection = -1;
                            projectile.velocity.X -= 0.07f;
                        }

                        if (NPCtarget.X < projectile.Center.X)
                        {
                            projectile.spriteDirection = 1;
                            projectile.velocity.X += 0.07f;
                        }
                    }

                    postimer++;
                    rockettimer++;
                    if (rockettimer > 49)
                    {
                        Vector2 position = projectile.Center;
                        Vector2 targetPosition = NPCtarget;
                        Vector2 direction = targetPosition - position;
                        direction.Normalize();
                        float speed2 = 14f;
                        Projectile.NewProjectile(projectile.Center, direction * speed2, ModContent.ProjectileType<Tankrocket>(), projectile.damage, 1f, projectile.owner, 0f);
                        rockettimer = 0;
                    }

                    if (postimer > 199)
                    {
                        Randompos = Main.rand.Next(150, 300);
                        postimer = 0;
                    }
                }



            }
            else
            {
                targetjump = true;

                projectile.spriteDirection = -Main.player[projectile.owner].direction;
                if (!flying)
                {
                    projectile.rotation = 0f;
                    straight = true;
                    angle = false;
                    up = false;
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = Main.player[projectile.owner].Center + new Vector2((PosPlay * 50 + 50) * -Main.player[projectile.owner].direction, 0);
                    Vector2 direction = targetPosition - position;
                    projectile.velocity.X = direction.X / 10;
                }
                if (Main.player[projectile.owner].Center.Y < projectile.Center.Y - 150f && !foundTarget)
                {
                    flying = true;
                }



                if (Vector2.Distance(player.Center, projectile.Center) > 1300f)
                {
                    projectile.position = player.Center;
                }


            }

            if (flying)
            {
                straight = false;
                angle = false;
                up = false;
                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.player[projectile.owner].Center + new Vector2((PosPlay * 50 + 50) * -Main.player[projectile.owner].direction, 10);
                Vector2 direction = targetPosition - position;
                projectile.velocity.X = direction.X / 10;

                projectile.velocity.Y = direction.Y / 10;
                projectile.rotation = projectile.velocity.X * 0.02f;



                projectile.frameCounter++;
                if (projectile.frameCounter > 4) // Ticks per frame
                {
                    projectile.frameCounter = 0;
                    projectile.frame += 1;
                }
                if (projectile.frame > 9) // 6 is max # of frames
                {
                    projectile.frame = 7; // Reset back to default
                }
                if (projectile.frame < 7) // 6 is max # of frames
                {
                    projectile.frame = 7; // Reset back to default
                }
            }



            if (straight)
            {
                projectile.frameCounter++;
                if (projectile.frameCounter > 4) // Ticks per frame
                {
                    projectile.frameCounter = 0;
                    projectile.frame += 1;
                }
                if (projectile.frame > 1) // 6 is max # of frames
                {
                    projectile.frame = 0; // Reset back to default
                }
                if (projectile.frame < 0) // 6 is max # of frames
                {
                    projectile.frame = 0; // Reset back to default
                }
            }


            if (angle)
            {
                projectile.frameCounter++;
                if (projectile.frameCounter > 4) // Ticks per frame
                {
                    projectile.frameCounter = 0;
                    projectile.frame += 1;
                }
                if (projectile.frame > 3) // 6 is max # of frames
                {
                    projectile.frame = 2; // Reset back to default
                }
                if (projectile.frame < 2) // 6 is max # of frames
                {
                    projectile.frame = 2; // Reset back to default
                }
            }


            if (up)
            {
                projectile.frameCounter++;
                if (projectile.frameCounter > 4) // Ticks per frame
                {
                    projectile.frameCounter = 0;
                    projectile.frame += 1;
                }
                if (projectile.frame > 5) // 6 is max # of frames
                {
                    projectile.frame = 4; // Reset back to default
                }
                if (projectile.frame < 4) // 6 is max # of frames
                {
                    projectile.frame = 4; // Reset back to default
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.X != projectile.velocity.X && targetjump)
            {
                projectile.velocity.Y = -7f;
            }
            flying = false;
            return false;
        }
    }
}