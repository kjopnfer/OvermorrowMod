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
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;

        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.65f, 0f, 0f);

            projectile.ai[0]++;

            Dust dust;
            // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
            Vector2 position = projectile.Center;
            dust = Terraria.Dust.NewDustPerfect(position, 183, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            dust.noGravity = true;


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
        }
    }
}
