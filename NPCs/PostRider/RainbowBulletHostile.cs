using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider
{
    public class RainbowBulletHostile : ModProjectile
    {

        public override bool CanDamage() => false;
        private int timer = 0;

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.penetrate = 1;
            projectile.scale = 1f;
            projectile.alpha = 255;
            projectile.timeLeft = 75;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {

            timer++;
            if (timer == 2)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X / 7, projectile.velocity.Y / 7, mod.ProjectileType("RainbowTrailHostile"), projectile.damage, 1f, projectile.owner, 0f);
                timer = 0;
            }
        }
    }
}