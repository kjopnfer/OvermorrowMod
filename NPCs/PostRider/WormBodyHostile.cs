using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider
{
    public class WormBodyHostile : ModProjectile
    {


        private int timer = 0;

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 24;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 51;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;

        }
        public override void AI()
        {
            timer++;
            projectile.alpha = projectile.alpha + 5;
            if (timer == 1)
            {
                projectile.alpha = 255;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                projectile.velocity.Y = 0;
                projectile.velocity.X = 0;
            }
            if (timer == 2)
            {
                projectile.alpha = 0;
            }
        }
    }
}
