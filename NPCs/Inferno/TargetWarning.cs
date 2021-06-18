using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class TargetWarning : ModProjectile
    {
        public override bool CanDamage() => false;
        private readonly int timer = 0;
        //int cooldown = 0;
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.friendly = true;
            projectile.light = 3f;
            projectile.alpha = 75;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 55;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 400;
        }
        public override void AI()
        {

            projectile.position.X = Main.player[projectile.owner].Center.X - 12f;
            projectile.position.Y = Main.player[projectile.owner].Center.Y - 265;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Worm Warning");
        }
    }
}