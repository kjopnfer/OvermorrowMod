using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class InfernoSpear : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 115;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 30;
        }
        public override void AI()
        {
            if(projectile.timeLeft > 15)
            {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            if(projectile.timeLeft == 15)
            {
            projectile.velocity *= -1f;
            }
        }
    }
}