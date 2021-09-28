using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles
{
    public class TestProj : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Effects/TrailTextures/Trail3";
        public override void SetDefaults()
        {
            projectile.width = 320;
            projectile.height = 10;
            projectile.tileCollide = false;
            projectile.timeLeft = 960;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.ignoreWater = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ModUtils.PointInShape(targetHitbox.Center.ToVector2(), projHitbox.TopLeft(), projHitbox.TopRight(), projHitbox.BottomRight(), projHitbox.BottomLeft());
        }
    }
}