using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class WormWarning : ModProjectile
    {
        public override bool CanDamage() => false;

        //int cooldown = 0;
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.light = 3f;
            projectile.scale = 2f;
            projectile.alpha = 75;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 55;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 400;
        }
        public override void AI()
        {

            projectile.position.X = Main.player[projectile.owner].Center.X - 1;
            projectile.position.Y = Main.player[projectile.owner].Center.Y - 247;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Worm Warning");
        }
    }
}