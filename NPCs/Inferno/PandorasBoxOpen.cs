using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class PandorasBoxOpen : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 18;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.penetrate = 5;
            projectile.timeLeft = 150;
            projectile.light = 0.5f;
            projectile.extraUpdates = 1;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pandoras Box");
        }
        public override void Kill(int timeLeft)
        {
            Item.NewItem((int)projectile.position.X, (int)projectile.position.Y + 5, projectile.width, projectile.height, 58, 2, false, 0, false, false);
        }
    }
}