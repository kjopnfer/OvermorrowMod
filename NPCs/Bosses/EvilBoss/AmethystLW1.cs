using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class AmethystLW1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evil Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = 94;
            projectile.height = 94;
            projectile.timeLeft = 99;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile parentProjectile = Main.projectile[(int)projectile.ai[0]];
            projectile.Center = parentProjectile.Center;
            projectile.position.X = parentProjectile.Center.X - 47;
            projectile.position.Y = parentProjectile.Center.Y - 47;
        }
    }
}
