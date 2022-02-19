using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class StarAxe : ModProjectile
    {

        public override void SetDefaults()
        {

            projectile.CloneDefaults(ProjectileID.Mushroom);
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 1;
            projectile.light = 1f;
        }
    }
}
