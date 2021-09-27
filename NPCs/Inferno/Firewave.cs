using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace OvermorrowMod.NPCs.Inferno
{
    public class Firewave : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 14;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.penetrate = 5;
            projectile.timeLeft = 80;
            projectile.light = 0.5f;
            projectile.extraUpdates = 1;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire Wave");
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.Fire, projectile.oldVelocity.X * 0.2f, projectile.oldVelocity.Y * 0.2f, 5, new Color(), 0.7f);
            }
        }
    }
}