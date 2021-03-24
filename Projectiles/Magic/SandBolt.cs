using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class SandBolt : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        private int delay = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 3;
            projectile.timeLeft = 240;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            projectile.magic = true;
        }

        public override void AI()
        {
            // The distance from the projectile's center to the mouse position
            Vector2 mousePosition = new Vector2(projectile.ai[0], projectile.ai[1]) - projectile.Center;

            if (delay == 0)
            {
                // Move towards the mouse
                if (mousePosition.Length() > projectile.velocity.Length())
                {
                    mousePosition.Normalize();
                    projectile.velocity = Vector2.Lerp(projectile.velocity.RotatedBy(Math.PI / 180), mousePosition * 14, 0.1f);
                }
                else // The projectile has reached where the mouse was
                {
                    delay = 3;
                    projectile.velocity = projectile.velocity.RotatedByRandom(Math.PI) * 1.5f;
                }
            }
            else
            {
                delay--;
            }


            projectile.netUpdate = true;
            projectile.localAI[0] += 1f;

            if (projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 32, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 1.5f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;

                Dust dustTrail = Dust.NewDustPerfect(new Vector2(projectile.position.X, projectile.position.Y), 32, projectile.velocity);
                dustTrail.position = (Main.dust[num1110].position + projectile.Center) / 2f;
                dustTrail.noGravity = true;
            }
        }
    }
}