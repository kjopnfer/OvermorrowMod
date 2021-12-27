using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Misc
{
    public class ShockwaveExplosion: ModProjectile
    {
        private int rippleCount = 3;
        private int rippleSize = 5;
        private int rippleSpeed = 45;
        private float distortStrength = 100f;

        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override bool CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shockwave");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            if (projectile.ai[0]++ == 0)
            {
                if (Main.netMode != NetmodeID.Server && !Filters.Scene["Shockwave"].IsActive())
                {
                    Filters.Scene.Activate("Shockwave", projectile.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(projectile.Center);
                }
            }

            if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive())
            {
                float progress = (180f - projectile.timeLeft) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive())
            {
                Filters.Scene["Shockwave"].Deactivate();
            }
        }
    }
}