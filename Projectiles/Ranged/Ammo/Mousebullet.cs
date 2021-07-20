using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged.Ammo
{
    public class Mousebullet : ModProjectile
    {
        private int length = 1;
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 2;
            projectile.timeLeft = 100;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Vector2 GuidePos4 = projectile.Center;
            Vector2 PlayerPosition4 = Main.MouseWorld;
            Vector2 GuideDirection4 = PlayerPosition4 - GuidePos4;
            GuideDirection4.Normalize();
            projectile.velocity = GuideDirection4 * 15f;  
            projectile.rotation = projectile.velocity.ToRotation();
                
            float BetweenKill = Vector2.Distance(Main.MouseWorld, projectile.Center);

            if(BetweenKill < 16)
            {
				projectile.Kill();    
            }

        }
    }
}
