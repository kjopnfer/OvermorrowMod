using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs.Summon;

namespace OvermorrowMod.Projectiles.Summon
{
    public class MeteorSumm : ModProjectile
    {
        bool dive = false;
        private int divetimer = 0;
        private readonly int timer2 = 0;
        private int timer = 0;
        private int struck = 0;
        private int movement = 0;
        private int movement2 = 0;
        float NPCtargetX = 0;
        float NPCtargetY = 0;
        int mrand = Main.rand.Next(-100, 101);
        private int PosCheck = 0;
        private int PosPlay = 0;
        private int Pos = 0;
        private int NumProj = 0;
        float NPCtargetHeight = 0;
        public override bool CanDamage() => false;
        
        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 24;
            projectile.minionSlots = 1f;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.netImportant = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 200000;
            projectile.light = 0.8f;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor");
            Main.projFrames[base.projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<DemEyeBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<DemEyeBuff>()))
            {
                projectile.timeLeft = 2;
            }
            #endregion

            NumProj = Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MeteorSumm>()];
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
            struck--;
            projectile.tileCollide = false;
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Main.MouseWorld);
                        bool closest = Vector2.Distance(Main.MouseWorld, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;

                        if (((closest && inRange) || !foundTarget))
                        {
                            NPCtargetX = npc.Center.X;
                            NPCtargetY = npc.Center.Y;
                            NPCtargetHeight = npc.height / 2;
                            projectile.rotation = (projectile.Center - npc.Center).ToRotation();
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                            float between2 = Vector2.Distance(npc.Center, projectile.Center);
                            if(between2 < 38f && struck < 1)
                            {
                            	npc.StrikeNPC(projectile.damage, 0, 0);
                                struck = 35;
                            }
                        }
                    }
                }
            }



			if (foundTarget) 
			{
            if(!dive)
            {
                divetimer = 0;
                movement = 1;
			    timer++;
                movement2--;
			    if(timer == 20)
			    {
                    dive = true;
			    }
                if(movement2 == 50)
                {
                    mrand = Main.rand.Next(-100, 100);
                }

                if(NPCtargetX + mrand > projectile.Center.X)
                {
                    projectile.velocity.X += 1.1f;
                }

                if(NPCtargetX + mrand < projectile.Center.X)
                {
                    projectile.velocity.X -= 1.1f;
                }

                if(NPCtargetY - 50 - NPCtargetHeight > projectile.Center.Y)
                {
                    projectile.velocity.Y += 1.1f;
                }
                if(NPCtargetY - 50 - NPCtargetHeight < projectile.Center.Y)
                {
                    projectile.velocity.Y -= 1.1f;
                }
            }
            if(dive)
            {
                divetimer++;
                if(divetimer < 6)
                {
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = new Vector2(NPCtargetX, NPCtargetY);
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    projectile.velocity = direction * 11;
                    
                }
                if(divetimer == 15)
                {
                    timer = 0;
                    dive = false;
                    movement2 = 51;
                }
            }


                if(projectile.velocity.Y < -17f)
                {
                    projectile.velocity.Y = -17f;
                }

                if(projectile.velocity.Y > 17f)
                {
                    projectile.velocity.Y = 17f;
                }


                if(projectile.velocity.X < -17f)
                {
                    projectile.velocity.X = -17f;
                }

                if(projectile.velocity.X > 7f)
                {
                    projectile.velocity.X = 7f;
                }
			}
            else
            {
                projectile.rotation = (projectile.Center - Main.player[projectile.owner].Center).ToRotation();

                if (Main.player[projectile.owner].direction == -1)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X + Pos;
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - 20;
                }

                if (Main.player[projectile.owner].direction == 1)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - Pos - 32;
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - 20;
                }

                projectile.velocity.Y = 0f;
                projectile.velocity.X = 0f;

            }

            if (++projectile.frameCounter >= 8)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Texture2D texture = mod.GetTexture("Projectiles/Summon/EyeDraw");

                Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
                for (int k = 0; k < projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                    Color color = projectile.GetAlpha(Color.White) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                    spriteBatch.Draw(texture, drawPos, new Rectangle?(), color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
                }
            return true;
        }
    }
}
