using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Armor.Marble
{
    public class PlayerMoveWave : ModProjectile
    {


        private int timer = 0;

        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.light = 1f;
            projectile.penetrate = -1;
            projectile.timeLeft = 128;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;

        }
        public override void AI()
        {
            timer++;
            projectile.alpha = projectile.alpha + 2;
            if(timer == 1)
            {
                projectile.alpha = 255;
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