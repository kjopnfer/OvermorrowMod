using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic.Gems
{
    public class EmeraldProj : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        Vector2 targetPosition;

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 18;
            projectile.timeLeft = 200;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            timer++;
            if(timer == 1)
            {
                targetPosition = Main.MouseWorld;
            }
            if(timer < 10)
            {
                Vector2 position = projectile.Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                projectile.velocity += direction * 1.5f;
            }
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), 2, 2, 2, projectile.velocity.X, projectile.velocity.Y, 10, Color.Green, 1.2f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }

            
        }
    }
}
