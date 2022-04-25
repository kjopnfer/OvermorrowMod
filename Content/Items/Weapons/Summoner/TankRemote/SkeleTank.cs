using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.TankRemote
{
    public class SkeleTank : ModProjectile
    {
        public override bool? CanDamage() => false;

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
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 40;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
        }


        public override void AI()
        {


            Player player = Main.player[Projectile.owner];
            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<TankBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<TankBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            #endregion

            NumProj = Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<SkeleTank>()];
            if (!PosCheck)
            {
                PosPlay = NumProj;
                PosCheck = true;
            }


            if (!flying)
            {
                if (Projectile.velocity.Y < 7f)
                {
                    Projectile.velocity.Y += 0.4f;
                }


                if (Projectile.velocity.Y > 7f)
                {
                    Projectile.velocity.Y = 7f;
                }
            }

            float distanceFromTarget = 100f;
            Vector2 targetCenter = Projectile.position;
            bool foundTarget = false;
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy() && Vector2.Distance(Main.MouseWorld, npc.Center) < 220f)
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
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
                    Projectile.rotation = 0f;
                    if (NPCtarget.Y < Projectile.Center.Y - 60)
                    {
                        angle = true;
                    }
                    else
                    {
                        angle = false;
                    }

                    if (NPCtarget.Y < Projectile.Center.Y - 100 && Vector2.Distance(new Vector2(Projectile.Center.X, Projectile.Center.X), new Vector2(NPCtarget.X, NPCtarget.X)) < 50f)
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

                    if (Projectile.velocity.Y < -7f)
                    {
                        Projectile.velocity.Y = -7f;
                    }

                    if (Projectile.velocity.X > 7f)
                    {
                        Projectile.velocity.X = 7f;
                    }

                    if (Projectile.velocity.X < -7f)
                    {
                        Projectile.velocity.X = -7f;
                    }




                    targetjump = true;

                    if (Vector2.Distance(new Vector2(Projectile.Center.X, Projectile.Center.X), new Vector2(NPCtarget.X, NPCtarget.X)) > Randompos + NPCtargetWidth)
                    {
                        if (NPCtarget.X > Projectile.Center.X)
                        {
                            Projectile.spriteDirection = -1;
                            Projectile.velocity.X += 0.07f;
                        }

                        if (NPCtarget.X < Projectile.Center.X)
                        {
                            Projectile.spriteDirection = 1;
                            Projectile.velocity.X -= 0.07f;
                        }
                    }
                    else
                    {
                        if (NPCtarget.X > Projectile.Center.X)
                        {
                            Projectile.spriteDirection = -1;
                            Projectile.velocity.X -= 0.07f;
                        }

                        if (NPCtarget.X < Projectile.Center.X)
                        {
                            Projectile.spriteDirection = 1;
                            Projectile.velocity.X += 0.07f;
                        }
                    }

                    postimer++;
                    rockettimer++;
                    if (rockettimer > 49)
                    {
                        Vector2 position = Projectile.Center;
                        Vector2 targetPosition = NPCtarget;
                        Vector2 direction = targetPosition - position;
                        direction.Normalize();
                        float speed2 = 14f;
                        Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, direction * speed2, ModContent.ProjectileType<Tankrocket>(), Projectile.damage, 1f, Projectile.owner, 0f);
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

                Projectile.spriteDirection = -Main.player[Projectile.owner].direction;
                if (!flying)
                {
                    Projectile.rotation = 0f;
                    straight = true;
                    angle = false;
                    up = false;
                    Vector2 position = Projectile.Center;
                    Vector2 targetPosition = Main.player[Projectile.owner].Center + new Vector2((PosPlay * 50 + 50) * -Main.player[Projectile.owner].direction, 0);
                    Vector2 direction = targetPosition - position;
                    Projectile.velocity.X = direction.X / 10;
                }
                if (Main.player[Projectile.owner].Center.Y < Projectile.Center.Y - 150f && !foundTarget)
                {
                    flying = true;
                }



                if (Vector2.Distance(player.Center, Projectile.Center) > 1300f)
                {
                    Projectile.position = player.Center;
                }


            }

            if (flying)
            {
                straight = false;
                angle = false;
                up = false;
                Vector2 position = Projectile.Center;
                Vector2 targetPosition = Main.player[Projectile.owner].Center + new Vector2((PosPlay * 50 + 50) * -Main.player[Projectile.owner].direction, 10);
                Vector2 direction = targetPosition - position;
                Projectile.velocity.X = direction.X / 10;

                Projectile.velocity.Y = direction.Y / 10;
                Projectile.rotation = Projectile.velocity.X * 0.02f;



                Projectile.frameCounter++;
                if (Projectile.frameCounter > 4) // Ticks per frame
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame += 1;
                }
                if (Projectile.frame > 9) // 6 is max # of frames
                {
                    Projectile.frame = 7; // Reset back to default
                }
                if (Projectile.frame < 7) // 6 is max # of frames
                {
                    Projectile.frame = 7; // Reset back to default
                }
            }



            if (straight)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 4) // Ticks per frame
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame += 1;
                }
                if (Projectile.frame > 1) // 6 is max # of frames
                {
                    Projectile.frame = 0; // Reset back to default
                }
                if (Projectile.frame < 0) // 6 is max # of frames
                {
                    Projectile.frame = 0; // Reset back to default
                }
            }


            if (angle)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 4) // Ticks per frame
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame += 1;
                }
                if (Projectile.frame > 3) // 6 is max # of frames
                {
                    Projectile.frame = 2; // Reset back to default
                }
                if (Projectile.frame < 2) // 6 is max # of frames
                {
                    Projectile.frame = 2; // Reset back to default
                }
            }


            if (up)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 4) // Ticks per frame
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame += 1;
                }
                if (Projectile.frame > 5) // 6 is max # of frames
                {
                    Projectile.frame = 4; // Reset back to default
                }
                if (Projectile.frame < 4) // 6 is max # of frames
                {
                    Projectile.frame = 4; // Reset back to default
                }
            }
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.X != Projectile.velocity.X && targetjump)
            {
                Projectile.velocity.Y = -7f;
            }
            flying = false;
            return false;
        }
    }
}