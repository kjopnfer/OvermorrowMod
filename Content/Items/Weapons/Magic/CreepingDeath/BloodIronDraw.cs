using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.CreepingDeath
{
    public class BloodIronDraw : ModProjectile
    {

        public override bool? CanDamage() => false;

        public override void SetDefaults()
        {
            Projectile.width = 7;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 17;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;

        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.65f, 0f, 0f);

            Projectile.ai[0]++;

            Dust dust;
            // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
            Vector2 position = Projectile.Center;
            dust = Terraria.Dust.NewDustPerfect(position, 183, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            dust.noGravity = true;


            if (Projectile.ai[0] == 1)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                Projectile.alpha = 255;
                Projectile.velocity.Y = 0;
                Projectile.velocity.X = 0;
            }
        }
    }
}
