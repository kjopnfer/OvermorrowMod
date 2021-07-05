using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    public class ElectricBallRadialLightning : ModProjectile
    {
        //int wait;
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamer Ball");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);

            int num434 = Dust.NewDust(projectile.Center, 0, 0, 229, 0f, 0f, 100);
            Main.dust[num434].noLight = true;
            Main.dust[num434].noGravity = true;
            Main.dust[num434].velocity = projectile.velocity;
            Main.dust[num434].position -= Vector2.One * 4f;
            Main.dust[num434].scale = 0.8f;

            if (++projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

            switch (projectile.ai[0])
            {
                case 0:
                    {
                        if (projectile.ai[1] > 60 && projectile.velocity != Vector2.Zero)
                        {
                            projectile.velocity = Vector2.SmoothStep(projectile.velocity, Vector2.Zero, 0.1f);//0.2f);//0.1f);//0.075f);
                            if (projectile.velocity.X < 0.05 || projectile.velocity.Y < 0.05)
                            {
                                projectile.velocity = Vector2.Zero;
                            }
                            //if (projectile.velocity.X < 2)//1.5)//0.8)//0.5)//0.25)//0.1)//0.025)
                            //{
                            //    projectile.velocity.X = 0;
                            //}
                            //if (projectile.velocity.Y < 2)//1.5)//0.8)//0.5)//0.25)//0.1)//0.025)
                            //{
                            //    projectile.velocity.Y = 0;
                            //}
                        }
                        if (projectile.velocity == Vector2.Zero)
                        {
                            projectile.ai[1] = 0;
                            projectile.ai[0]++;
                        }
                    }
                    break;
                case 1:
                    {
                        if (projectile.ai[1] % /*5*/ 4 == 0 && projectile.ai[1] <= 144/*180*/)
                        {
                            //if (/*projectile.ai[1] != 4 && projectile.ai[1] != 8*/ /*wait != 0*/)
                            //{
                            int proj = Projectile.NewProjectile(projectile.Center, new Vector2(0.01f, 0.01f).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<LaserWarning>(), projectile.damage, projectile.knockBack, projectile.owner);
                            ((LaserWarning)Main.projectile[proj].modProjectile).killearly = true;
                            ((LaserWarning)Main.projectile[proj].modProjectile).waittime = 100; 
                            /*(int)projectile.ai[1] * -1*/
                            //((LaserWarning)Main.projectile[proj].modProjectile).waittime = (wait * 4) * -1/*(int)projectile.ai[1] * -1*/;
                            //}
                            //wait++;
                        }
                        //Main.NewText(projectile.ai[1]);

                        if (projectile.ai[1] > /*180*/ 144 && projectile.timeLeft > 90)
                        {
                            projectile.timeLeft = 90;
                        }
                        if (projectile.ai[1] > 200)
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && Main.projectile[i].type == mod.ProjectileType("LaserWarning") && ((LaserWarning)Main.projectile[i].modProjectile).killearly == true)
                                {
                                    ((LaserWarning)Main.projectile[i].modProjectile).killnow = true;
                                    Main.projectile[i].Kill();
                                }
                            }
                        }   
                    }
                    break;
            }
            projectile.ai[1]++;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft >= 60)
            {
                return Color.White;
            }
            else
            {
                return null;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, Main.expertMode ? 360 : 180);
        }
    }
}