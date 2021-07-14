using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon.Worm
{
    public class WormT10 : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/Devourer";
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
			projectile.width = 26;
            projectile.height = 26;
            projectile.timeLeft = 2000;
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
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT11"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }


            Player player = Main.player[projectile.owner];
            projectile.position.X = Main.player[projectile.owner].Center.X - 9;
            projectile.position.Y = Main.player[projectile.owner].Center.Y - 9;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
            didHit = true;
            projectile.tileCollide = false;
        }
    }
    
    public class WormT11 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/Devourer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 26;
            projectile.height = 26;
            projectile.timeLeft = 2000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }
        public override void AI()
        {

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT12"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }


            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT10"))
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

    public class WormT12 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/Devourer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 26;
            projectile.height = 26;
            projectile.timeLeft = 2000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }

        public override void AI()
        {

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT13"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }

            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT11"))
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
    public class WormT13 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/Devourer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 26;
            projectile.height = 26;
            projectile.timeLeft = 2000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }

        public override void AI()
        {
            
            if(Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) > 2000)
            {
                projectile.Kill();
            }

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT14"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }
            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT12"))
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

    public class WormT14 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/Devourer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 26;
            projectile.height = 26;
            projectile.timeLeft = 2000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }

        public override void AI()
        {
            
            if(Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) > 2000)
            {
                projectile.Kill();
            }

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT15"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }
            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT13"))
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

    public class WormT15 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/Devourer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 26;
            projectile.height = 26;
            projectile.timeLeft = 2000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }

        public override void AI()
        {
            
            if(Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) > 2000)
            {
                projectile.Kill();
            }

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT16"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }
            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT14"))
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

    public class WormT16 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/Devourer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 26;
            projectile.height = 26;
            projectile.timeLeft = 2000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }

        public override void AI()
        {
            projectile.timeLeft = 2000;
            if(Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) > 2000)
            {
                projectile.Kill();
            }

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT17"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }
            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT15"))
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

    public class WormT17 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/Devourer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 26;
            projectile.height = 26;
            projectile.timeLeft = 2000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }

        public override void AI()
        {
            
            if(Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) > 2000)
            {
                projectile.Kill();
            }

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT18"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }
            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT16"))
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

    public class WormT18 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/Devourer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 26;
            projectile.height = 26;
            projectile.timeLeft = 2000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }

        public override void AI()
        {
            
            if(Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) > 2000)
            {
                projectile.Kill();
            }

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT19"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }
            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT17"))
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

    public class WormT19 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/Devourer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 26;
            projectile.height = 26;
            projectile.timeLeft = 2000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }

        public override void AI()
        {
            
            if(Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) > 2000)
            {
                projectile.Kill();
            }

            timer++;
            if(timer == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("WormT20"), 10, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }
            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT18"))
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

    public class WormT20 : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        public override string Texture => "OvermorrowMod/Projectiles/Summon/Worm/DevourerT";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
			projectile.width = 26;
            projectile.height = 26;
            projectile.timeLeft = 2000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }

        public override void AI()
        {
            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == mod.ProjectileType("WormT19"))
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
