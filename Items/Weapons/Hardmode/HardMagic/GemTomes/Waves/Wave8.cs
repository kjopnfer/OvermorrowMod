using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardMagic.GemTomes.Waves
{
    public class Wave8 : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 80;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.penetrate = 2;
            projectile.timeLeft = 100;
            projectile.light = 0.5f;
            projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            projectile.velocity.X = projectile.velocity.X - 0.06f;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cone");
        }
    }
}