using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Juvenation.Projectiles
{
    public class FrostWaveFriend2 : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.width = 80;
            projectile.height = 46;
            projectile.timeLeft = 30;
            projectile.tileCollide = false;
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.FrostWave;

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}