using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class AxeWarning : ModProjectile
    {
        public override bool CanDamage() => false;

        //int cooldown = 0;
        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 1;
            projectile.friendly = true;
            projectile.light = 3f;
            projectile.alpha = 75;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 55;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 550;
        }
        public override void AI()
        {

            projectile.position.X = Main.player[projectile.owner].Center.X - 7.5f;
            projectile.position.Y = Main.player[projectile.owner].Center.Y - 265;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Worm Warning");
        }
    }
}