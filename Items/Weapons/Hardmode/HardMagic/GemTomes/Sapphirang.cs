using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardMagic.GemTomes
{
    class Sapphirang : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.LightDisc);
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1000;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (projectile.timeLeft > 990)
            {
                projectile.position = Main.MouseWorld;
            }
        }
    }

}

