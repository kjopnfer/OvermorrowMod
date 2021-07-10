using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class Blast1 : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 14;
            projectile.timeLeft = 200;
            projectile.penetrate = 1;
            projectile.scale = 0.8f;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
        }
    }
}
