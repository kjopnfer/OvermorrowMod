using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class StormTalonSparks : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Sparks");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 65;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            projectile.melee = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                Dust.NewDustPerfect(projectile.Center, 206, null, 0, default, 1.5f);
            }
        }
    }
}