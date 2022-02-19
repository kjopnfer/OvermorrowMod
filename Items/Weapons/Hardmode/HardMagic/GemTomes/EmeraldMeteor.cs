/*using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardMagic.GemTomes
{
    class EmeraldMeteor : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.ChlorophyteArrow);
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.width = 24;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.light = 3f;
            projectile.penetrate = 3;
            projectile.timeLeft = 900;
            projectile.tileCollide = false;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.velocity.Y = projectile.velocity.Y - 0.3f; // 0.1f for arrow gravity, 0.4f for knife gravity
            if (projectile.velocity.Y < -5f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
            {
                projectile.velocity.Y += 10f;
            }
        }
    }
}*/

