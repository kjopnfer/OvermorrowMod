using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class FlamingLight : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Fireball);
            projectile.width = 63;
            projectile.height = 52;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.magic = true;
            projectile.alpha = 0;
            projectile.tileCollide = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 300;
            projectile.light = 0.5f;
            projectile.extraUpdates = 1;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darkwave");
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 6, projectile.oldVelocity.X * 0.2f, projectile.oldVelocity.Y * 0.2f, 5, new Color(), 0.7f);
            }

            if(Main.player[projectile.owner].Center.X > projectile.position.X)
            {
                projectile.velocity.X = 0.7f;
            }

            if(Main.player[projectile.owner].Center.X < projectile.position.X)
            {
                projectile.velocity.X = -0.7f;
            }
        }
    }
}