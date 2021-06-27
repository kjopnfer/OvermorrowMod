using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Summon;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Summon
{
    public class GraniteSummon : ModProjectile
    {

        int colorcooldown = 0;
        readonly int frame = 1;
        Vector2 Rot;
        int Random2 = Main.rand.Next(-15, 12);
        int Random = Main.rand.Next(1, 3);
        public override bool CanDamage() => false;
        private readonly int timer2 = 0;
        private int timer = 0;
        private int PosCheck = 0;
        private int PosPlay = 0;
        private int Pos = 0;
        private int movement = 0;
        private int NumProj = 0;
        private int movement2 = 0;
        float NPCtargetX = 0;
        float NPCtargetY = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 38;
            projectile.minionSlots = 1f;
            projectile.light = 2f;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.netImportant = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 200000;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Elemental");
            Main.projFrames[base.projectile.type] = 4;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<GraniteEleBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<GraniteEleBuff>()))
            {
                projectile.timeLeft = 2;
            }
            #endregion


            NumProj = Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<GraniteSummon>()];
            PosCheck++;
            if(PosCheck == 2)
            {
                PosPlay = NumProj;
            }

            if (PosCheck == 5)
            {
                Pos = PosPlay * 30;
            }
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
                            NPCtargetY = npc.Center.Y;
                            Vector2 Rot = npc.Center;
                            //projectile.rotation = (Rot - projectile.Center).ToRotation();
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }


            if (foundTarget && Main.player[projectile.owner].channel)
			{
            movement = 1;
            movement2++;

                if (movement2 == 70)
                {
                    mrand2 = Main.rand.Next(-170, 171);
                    mrand = Main.rand.Next(-170, 171);
                    movement2 = 0;
                }

                if(NPCtargetX > projectile.Center.X)
                {
                    projectile.spriteDirection = -1;
                }
                else
                {
                    projectile.spriteDirection = 1;
                }

                if(NPCtargetX + mrand > projectile.Center.X)
                {
                    projectile.velocity.X += 0.9f;
                }

                if(NPCtargetX + mrand < projectile.Center.X)
                {
                    projectile.velocity.X -= 0.9f;
                }

                if(NPCtargetY + mrand2 > projectile.Center.Y)
                {
                    projectile.velocity.Y += 2f;
                }
                if(NPCtargetY + mrand2 < projectile.Center.Y)
                {
                    projectile.velocity.Y -= 2f;
                }


                if(projectile.velocity.Y < -9f)
                {
                    projectile.velocity.Y = -9f;
                }

                if(projectile.velocity.Y > 9f)
                {
                    projectile.velocity.Y = 9f;
                }


                if(projectile.velocity.X < -9f)
                {
                    projectile.velocity.X = -9f;
                }

                if(projectile.velocity.X > 9f)
                {
                    projectile.velocity.X = 9f;
                }

			}
            else
            {
                projectile.spriteDirection = -Main.player[projectile.owner].direction;


                if (Main.player[projectile.owner].direction == -1)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X + Pos;
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - 32;
                }

                if (Main.player[projectile.owner].direction == 1)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - Pos - 32;
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - 32;
                }

                projectile.velocity.Y = 0f;
                projectile.velocity.X = 0f;

            }

            if (Main.player[projectile.owner].channel && foundTarget)
            {
                timer++;
                if (timer == 45 + Random2)
                {
                    Random2 = Main.rand.Next(-15, 12);
                    Random = Main.rand.Next(-2, 3);
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = Main.MouseWorld;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(1.5f));
                    float speed = Random + 20f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint2.X * speed, newpoint2.Y * speed, ModContent.ProjectileType<GraniteLaser>(), projectile.damage, 1f, projectile.owner, 0f);
                    timer = 0;
                }
            }

            // Loop through the 4 animation frames, spending 5 ticks on each.
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
