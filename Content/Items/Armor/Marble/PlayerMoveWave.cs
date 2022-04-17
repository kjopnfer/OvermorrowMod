using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Marble
{
    public class PlayerMoveWave : ModProjectile
    {


        private int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.light = 1f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 128;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;

        }
        public override void AI()
        {
            timer++;
            Projectile.alpha = Projectile.alpha + 2;
            if (timer == 1)
            {
                Projectile.alpha = 255;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                Projectile.velocity.Y = 0;
                Projectile.velocity.X = 0;
            }
            if (timer == 2)
            {
                Projectile.alpha = 0;
            }
        }
    }
}
