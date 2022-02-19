using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic.Upgrades
{
    public class EyeBlast : ModProjectile
    {
        private int timer = 0;
        bool HasHit = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[base.projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 18;
            projectile.timeLeft = 300;
            projectile.penetrate = 8;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            timer++;
            if (timer == 1)
            {
                if (Main.MouseWorld.X > Main.player[projectile.owner].Center.X)
                {
                    projectile.velocity.X = 10;
                    projectile.velocity.Y = 0;
                }
                else
                {
                    projectile.velocity.X = -10;
                    projectile.velocity.Y = 0;
                }
            }


            if (!HasHit)
            {
                if (++projectile.frameCounter >= 4)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= 2)
                    {
                        projectile.frame = 0;
                    }
                }
            }
            else
            {
                if (++projectile.frameCounter >= 4)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= 4)
                    {
                        projectile.frame = 2;
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.frame = 2;
            HasHit = true;
            projectile.damage += 5;
            if (projectile.velocity.X > 0)
            {
                projectile.velocity.X += 1.5f;
            }
            else
            {
                projectile.velocity.X -= 1.5f;
            }
        }
    }
}
