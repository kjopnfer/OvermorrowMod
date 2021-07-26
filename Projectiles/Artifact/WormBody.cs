using Microsoft.Xna.Framework;
using OvermorrowMod.WardenClass;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Artifact
{
    public class WormBody : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 85;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            projectile.ai[0]++;
            projectile.alpha = projectile.alpha + 3;
            if(projectile.ai[0] == 1)
            {
                projectile.alpha = 255;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                projectile.velocity.Y = 0;
                projectile.velocity.X = 0;
            }
            if(projectile.ai[0] == 2)
            {
                projectile.alpha = 0;
            }
        }
    }
}
