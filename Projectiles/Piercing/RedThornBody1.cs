using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class RedThornBody1 : ModProjectile
    {
        private Projectile parentProjectile;
        private Vector2 storeVelocity;
        public override void SetStaticDefaults()
        {
            // Left facing thorn
            DisplayName.SetDefault("Crimson Thorn");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1f;
        }

        public override void AI()
        {
            // Get parent projectile
            if (Main.projectile[(int)projectile.ai[0]].active)
            {
                parentProjectile = Main.projectile[(int)projectile.ai[0]];
                projectile.timeLeft = 2;
            }
            else
            {
                projectile.Kill();
            }

            // Store velocity for rotation
            if (projectile.ai[1] == 0)
            {
                storeVelocity = projectile.velocity;
                projectile.velocity = Vector2.Zero;
                projectile.ai[1]++;
            }

            // Rotate projectile
            projectile.rotation = storeVelocity.ToRotation() + MathHelper.PiOver2;

            if (parentProjectile.timeLeft < 20)
            {
                projectile.alpha += 15;
                parentProjectile.alpha = projectile.alpha;
            }
        }
    }
}