using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class GraniteLaser : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.EyeBeam);
            aiType = ProjectileID.EyeBeam;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 150;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}