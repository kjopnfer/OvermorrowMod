using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Content.Projectiles.Misc
{
    public class ghostProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = -1;
            Projectile.width = 12;
            Projectile.height = 12;
        }
    }
}