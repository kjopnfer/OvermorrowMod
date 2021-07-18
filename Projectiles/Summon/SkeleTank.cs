using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs.Summon;

namespace OvermorrowMod.Projectiles.Summon
{
	public class SkeleTank : ModProjectile
	{

        Vector2 NPCtarget;
		bool targetjump;

		bool flying = false;

		bool PosCheck = false;
		int PosPlay = 0;
        private int NumProj = 0;
		
        int Randompos = Main.rand.Next(150, 300);

        int rockettimer = 0;
        float NPCtargetWidth = 0;
        int postimer = 0;


		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skeletank"); 
			Main.projFrames[projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			projectile.width = 22;
			projectile.height = 46;
            projectile.timeLeft = 200000;
		}


		public override void AI()
		{




            NumProj = Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<SkeleTank>()];
            if(!PosCheck)
            {
                PosPlay = NumProj;
				PosCheck = true;
            }





            projectile.frame = 0;
			if(!flying)
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


            Player player = Main.player[projectile.owner];

            float distanceFromTarget = 300f;
            Vector2 targetCenter = projectile.position;
            bool foundTarget = false;
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
			if(!flying)
			{
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

					if(Vector2.Distance(new Vector2(projectile.Center.X, projectile.Center.X), new Vector2(NPCtarget.X, NPCtarget.X)) > Randompos + NPCtargetWidth)
					{
						if(NPCtarget.X > projectile.Center.X)
						{
							projectile.velocity.X += 0.07f;
						}

						if(NPCtarget.X < projectile.Center.X)
						{
							projectile.velocity.X -= 0.07f;
						}
					}
					else
					{
						if(NPCtarget.X > projectile.Center.X)
						{
							projectile.velocity.X -= 0.07f;
						}

						if(NPCtarget.X < projectile.Center.X)
						{
							projectile.velocity.X += 0.07f;
						}
					}

					postimer++;
					rockettimer++;
					if(rockettimer > 49)
					{
						Vector2 position = projectile.Center;
						Vector2 targetPosition = NPCtarget;
						Vector2 direction = targetPosition - position;
						direction.Normalize();
						float speed = 10f;
						Projectile.NewProjectile(projectile.Center, direction * speed, ModContent.ProjectileType<GraniteLaser>(), projectile.damage, 1f, projectile.owner, 0f);
						rockettimer = 0;
					}

					if(postimer > 199)
					{
						Randompos = Main.rand.Next(150, 300);
						postimer = 0;
					}
				}



			}
            else
            {
				if(!flying)
				{
					targetjump = true;
					Vector2 position = projectile.Center;
					Vector2 targetPosition = Main.player[projectile.owner].Center + new Vector2((PosPlay * 50 + 50) * -Main.player[projectile.owner].direction, 0);
					Vector2 direction = targetPosition - position;
					projectile.velocity.X = direction.X / 10;
				}
				if(Main.player[projectile.owner].Center.Y < projectile.Center.Y - 20f && !foundTarget)
				{
					flying = true;
				}



				if(Vector2.Distance(player.Center,  projectile.Center) > 1300f)
				{
					projectile.position = player.Center;
				}


				if(flying)
				{
					Vector2 idlePosition = player.Center;
					float speed = 15f;
					float inertia = 20f;
					Vector2 vectorToIdlePosition = idlePosition - projectile.Center;
					float distanceToIdlePosition = vectorToIdlePosition.Length();

					// Minion doesn't have a target: return to player and idle
					if (distanceToIdlePosition > 100f)
					{
						// Speed up the minion if it's away from the player
						speed = 40f;
						inertia = 60f;
					}
					else
					{
						// Slow down the minion if closer to the player
						speed = 20f;
						inertia = 80f;
					}
					if (distanceToIdlePosition > 20f)
					{
						// The immediate range around the player (when it passively floats about)

						// This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
						vectorToIdlePosition.Normalize();
						vectorToIdlePosition *= speed;
						projectile.velocity = (projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
					}
					else if (projectile.velocity == Vector2.Zero)
					{
						// If there is a case where it's not moving at all, give it a little "poke"
						projectile.velocity.X = -0.15f;
						projectile.velocity.Y = -0.15f;
					}
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