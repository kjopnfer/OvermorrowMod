using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;



namespace OvermorrowMod.Items.Weapons.Hardmode.HardSummon
{
    public class SkeleSummon : ModProjectile
    {
        private float NPCtargetX = 0;
        private float NPCtargetY = 0;
        private int timer = 0;
		private bool target = false;
        private int frame = 6;
        private int frametimer = 0;
        private int frametimer2 = 0;
        private float distanceTo;
        private int penet = 0;
        private int penet2 = 0;
        private int savedDMG = 0;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeleton");
            Main.projFrames[base.projectile.type] = 15;
        }

        public override void AI()
        {
            projectile.ignoreWater = true;
            projectile.penetrate = 2;
            projectile.height = 46;
            projectile.width = 24;
            projectile.light = 1f;
            projectile.friendly = true;
            projectile.scale = 0.8f;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.minion = true;
            projectile.minionSlots = 0.5f;
            projectile.spriteDirection = projectile.direction;
            timer++;


            if(timer == 600)
            {
				projectile.Kill();
            }



            if(timer == 1)
            {
                savedDMG = projectile.damage;
                projectile.position.Y = projectile.position.Y - projectile.height;
            }



            float distanceFromTarget = 450f;
            Vector2 targetCenter = projectile.position;
            bool Target = false;
            projectile.tileCollide = false;
            if (!Target)
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

                        if (((closest && inRange) || !Target))
                        {
                            NPCtargetX = npc.Center.X;
                            NPCtargetY = npc.Center.Y;
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            Target = true;
                        }
                    }
                }
            }


			if (Target) 
			{
                projectile.tileCollide = false;
                projectile.rotation = projectile.velocity.X * 0.03f;


                if(NPCtargetY > projectile.Center.Y)
                {
                    projectile.velocity.Y += 0.7f;
                }

                if(NPCtargetY < projectile.Center.Y)
                {
                    projectile.velocity.Y -= 0.7f;
                }



                if(NPCtargetX > projectile.Center.X)
                {
                    projectile.velocity.X += 0.7f;
                }

                if(NPCtargetX < projectile.Center.X)
                {
                    projectile.velocity.X -= 0.7f;
                }


                if(projectile.velocity.Y < -7f)
                {
                    projectile.velocity.Y = -7f;
                }

                if(projectile.velocity.Y > 7f)
                {
                    projectile.velocity.Y = 7f;
                }


                if(projectile.velocity.X < -10f)
                {
                    projectile.velocity.X = -10f;
                }

                if(projectile.velocity.X > 10f)
                {
                    projectile.velocity.X = 10f;
                }

            }
        else
        {
            projectile.rotation = 0f;
            projectile.tileCollide = true;
            projectile.velocity.Y += 0.3f;
            if (Main.player[projectile.owner].Center.X + 95 < projectile.Center.X)
            {
                projectile.velocity.X -= 0.07f;
            }


            if (Main.player[projectile.owner].Center.X - 95 > projectile.Center.X)
            {
                projectile.velocity.X += 0.07f;
            }

                if(projectile.velocity.X < -4f)
                {
                    projectile.velocity.X = -4f;
                }

                if(projectile.velocity.X > 4f)
                {
                    projectile.velocity.X = 4f;
                }
            }


            if(projectile.velocity.Y > 0 || projectile.velocity.Y < 0)
            {
                frametimer2++;
                frametimer++;
                if(frametimer == 1)
                {
                    projectile.frame = 10;
                }

                if(frametimer2 == 4)
                {
                    frametimer2 = 0;
                    projectile.frame = projectile.frame + 1;
                }

                if(projectile.frame > 13)
                {
                    projectile.frame = 10;
                }
            }


            if(penet > 0)
            {
			    projectile.damage = 0;
                penet++;
            }
            
            if(projectile.damage == 0)
            {
                penet2++;
            }

            if(penet2 > 25)
            {
                penet = 0;
                penet2 = 0;
                projectile.damage = savedDMG;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            penet++;
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            {
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = 0;
                }
            }
            return false;
        }
    }
}
