using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider
{
    public class RainbowTrailHostile : ModProjectile
    {


        private int timer = 0;

        public override void SetDefaults()
        {
            projectile.width = 7;
            projectile.height = 30;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 15;
            projectile.alpha = 255;
            projectile.light = 1.4f;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;

        }
        public override void AI()
        {
            timer++;
            projectile.alpha = projectile.alpha + 17;
            if(timer == 1)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                projectile.velocity.Y = 0;
                projectile.velocity.X = 0;
            }
            if(timer == 2)
            {
                projectile.alpha = 0;
            }
        }
    }
}
