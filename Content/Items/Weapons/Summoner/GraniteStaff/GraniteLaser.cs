using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.GraniteStaff
{
    public class GraniteLaser : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeBeam);
            aiType = ProjectileID.EyeBeam;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 150;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}