using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
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
            projectile.width = 96;
            projectile.height = 96;
            projectile.tileCollide = false;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.timeLeft = 25;
            projectile.penetrate = -1;
            projectile.scale = 1;
        }
        public override void AI()
        {
            // Invert the scale
            if (projectile.ai[0] == 1)
            {
                projectile.scale = MathHelper.Lerp(0, 8, projectile.timeLeft / 25f);
            }
            else
            {
                projectile.scale = MathHelper.Lerp(4, 0, projectile.timeLeft / 25f);
            }
            projectile.alpha = (int)MathHelper.Lerp(255, 0, Utils.Clamp(projectile.timeLeft, 0, 25) / 25f);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * (MathHelper.Lerp(1, 0, projectile.alpha / 255f));
    }
}
