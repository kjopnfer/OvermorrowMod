using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic.CreepingD
{
    public class BloodIronDraw : ModProjectile
    {

        public override bool CanDamage() => false;
        private int timer = 0;

        public override void SetDefaults()
        {
            projectile.width = 7;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 17;
            projectile.alpha = 255;
            projectile.light = 1.4f;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;

        }
        public override void AI()
        {
            projectile.ai[0]++;
            projectile.alpha = projectile.alpha + 15;

            if (projectile.ai[0] == 1)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                projectile.alpha = 255;
                projectile.velocity.Y = 0;
                projectile.velocity.X = 0;
            }

            if (projectile.ai[0] == 2)
            {
                projectile.alpha = 0;
            }
        }
    }
}
