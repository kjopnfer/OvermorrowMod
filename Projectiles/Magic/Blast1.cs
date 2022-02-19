using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class Blast1 : ModProjectile
    {
        private int timer = 0;
        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 14;
            projectile.timeLeft = 200;
            projectile.penetrate = 1;
            projectile.scale = 0.8f;
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
                projectile.position.X -= 5;
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


        }
    }
}
