using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace OvermorrowMod.Content.Projectiles
{
    public class GlimsporeBomb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.height = 14;
            Projectile.width = 14;
            Projectile.friendly = true;
            Projectile.maxPenetrate = 1;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 480;
        }
        public override void AI()
        {
            Projectile.velocity /= 1.05f;
            Projectile.ai[0]++;
        }
        const int HitboxExtent = 8;
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = new Rectangle(hitbox.X - HitboxExtent, hitbox.Y - HitboxExtent, hitbox.Width + HitboxExtent * 2, hitbox.Height + HitboxExtent * 2);
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}