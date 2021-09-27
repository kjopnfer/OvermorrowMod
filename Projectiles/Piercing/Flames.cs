using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class Flames : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flames");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Flames);
            aiType = ProjectileID.Flames;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
        }
    }
}