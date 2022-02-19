using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardMagic.GemTomes
{
    class RubyPro : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.light = 3f;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.magic = true;
        }

    }

}

