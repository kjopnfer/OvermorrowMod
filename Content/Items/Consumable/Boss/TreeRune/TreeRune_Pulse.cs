using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.Boss.TreeRune
{
    public class TreeRune_Pulse : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich Pulse");
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 25;
            Projectile.penetrate = -1;
            Projectile.scale = 1;
        }
        public override void AI()
        {
            // Invert the scale
            if (Projectile.ai[0] == 1)
            {
                Projectile.scale = MathHelper.Lerp(0, 8, Projectile.timeLeft / 25f);
            }
            else
            {
                Projectile.scale = MathHelper.Lerp(4, 0, Projectile.timeLeft / 25f);
            }
            Projectile.alpha = (int)MathHelper.Lerp(255, 0, Utils.Clamp(Projectile.timeLeft, 0, 25) / 25f);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * (MathHelper.Lerp(1, 0, Projectile.alpha / 255f));
    }
}
