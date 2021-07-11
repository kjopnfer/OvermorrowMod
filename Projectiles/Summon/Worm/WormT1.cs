using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon.Worm
{
    public class WormT1 : ModProjectile
    {
        
        private int Wtimer = 0;
        private bool didHit = false;
        private int timer = 0;
        private int SaveVeloX = 0;
        private int SaveVeloY = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 18;
            projectile.height = 18;
            projectile.timeLeft = 700;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Wtimer++;
            if(Wtimer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT2"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }

            if(!didHit)
            {
            Player player = Main.player[projectile.owner];
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 400f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5)
                {
                    Vector2 newMove = Main.npc[k].Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }
            if (target)
            {
                AdjustMagnitude(ref move);
                projectile.velocity += (10 * projectile.velocity + move) / 11f;
                AdjustMagnitude(ref projectile.velocity);
            }
            }
            if(projectile.velocity.X > 11)
            {
                projectile.velocity.X = 11;
            }
            if(projectile.velocity.X < -11)
            {
                projectile.velocity.X = -11;
            }

            if(projectile.velocity.Y > 11)
            {
                projectile.velocity.Y = 11;
            }
            if(projectile.velocity.Y < -11)
            {
                projectile.velocity.Y = -11;
            }

            projectile.velocity.Y = projectile.velocity.Y + 0.06f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            didHit = true;
            projectile.tileCollide = false;
        }
    }
    
    public class WormT2 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/WormT3";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 18;
            projectile.height = 18;
            projectile.timeLeft = 700;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }



        public override void AI()
        {

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT3"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }


            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT1"))
            {
                // set rotation to the parent segment
                projectile.rotation = projectile.DirectionTo(projectile2.Center).ToRotation();
                // check if distance is over segment size (ps: adjust height to right value)
                    // direction from parent to me
                    Vector2 dir = projectile2.DirectionTo(projectile.Center);
                    // position where the distance between parent and me is exactly the segment length
                    projectile.Center = projectile2.Center + new Vector2(dir.X * projectile2.height, dir.Y * projectile2.width);
            }
                else
                {
                    // kil
                    projectile.Kill();
                }
        }
	}   

    public class WormT3 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 18;
            projectile.height = 18;
            projectile.timeLeft = 700;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }



        public override void AI()
        {

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT4"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }

            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT2"))
            {
                // set rotation to the parent segment
                projectile.rotation = projectile.DirectionTo(projectile2.Center).ToRotation();
                // check if distance is over segment size (ps: adjust height to right value)
                    // direction from parent to me
                    Vector2 dir = projectile2.DirectionTo(projectile.Center);
                    // position where the distance between parent and me is exactly the segment length
                    projectile.Center = projectile2.Center + new Vector2(dir.X * projectile2.height, dir.Y * projectile2.width);
            }
                else
                {
                    // kil
                    projectile.Kill();
                }
        }
	}   
    public class WormT4 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/WormT3";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 18;
            projectile.height = 18;
            projectile.timeLeft = 700;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }



        public override void AI()
        {

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT5"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }
            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT3"))
            {
                // set rotation to the parent segment
                projectile.rotation = projectile.DirectionTo(projectile2.Center).ToRotation();
                // check if distance is over segment size (ps: adjust height to right value)
                    // direction from parent to me
                    Vector2 dir = projectile2.DirectionTo(projectile.Center);
                    // position where the distance between parent and me is exactly the segment length
                    projectile.Center = projectile2.Center + new Vector2(dir.X * projectile2.height, dir.Y * projectile2.width);
            }
                else
                {
                    // kil
                    projectile.Kill();
                }
        }
	} 

    public class WormT5 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/WormT5";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 18;
            projectile.height = 18;
            projectile.timeLeft = 700;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }



        public override void AI()
        {
            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT4"))
            {
                // set rotation to the parent segment
                projectile.rotation = projectile.DirectionTo(projectile2.Center).ToRotation();
                // check if distance is over segment size (ps: adjust height to right value)
                    // direction from parent to me
                    Vector2 dir = projectile2.DirectionTo(projectile.Center);
                    // position where the distance between parent and me is exactly the segment length
                    projectile.Center = projectile2.Center + new Vector2(dir.X * projectile2.height, dir.Y * projectile2.width);
            }
                else
                {
                    // kil
                    projectile.Kill();
                }
        }
	}   
}