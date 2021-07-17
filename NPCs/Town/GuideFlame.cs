using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Town
{
    public class GuideFlame : ModProjectile
    {

        private int SavedDMG = 0;
        private int timer = 0;
        private bool ComingBack = false;
        private int flametimer = 0;

        public override string Texture => "Terraria/Projectile_" + ProjectileID.DesertDjinnCurse;

        public override void SetStaticDefaults()
        {
            Main.projFrames[base.projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.PhantasmalEye);
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 330; 
        }


        public override void AI()
        {
            if(Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center) < 60f)
            {
                int bomb = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, 102, projectile.damage, 0f, Main.myPlayer);  
                Main.projectile[bomb].timeLeft = 1;
                projectile.Kill();
            }

                        float speed = 6f;
                        if(projectile.velocity.Y < -speed)
                        {
                            projectile.velocity.Y = -speed;
                        }

                        if(projectile.velocity.Y > speed)
                        {
                            projectile.velocity.Y = speed;
                        }


                        if(projectile.velocity.X < -speed)
                        {
                            projectile.velocity.X = -speed;
                        }

                        if(projectile.velocity.X > speed)
                        {
                            projectile.velocity.X = speed;
                        }





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
