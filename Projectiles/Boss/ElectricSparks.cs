using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class ElectricSparks : ModProjectile
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
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 690;
            projectile.alpha = 255;
            projectile.tileCollide = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);

            // Make projectile slow down until stationary
            if (projectile.timeLeft <= 525)
            {
                if (projectile.ai[0] % 40 == 0)
                {
                    if (projectile.velocity.X != 0)
                    {
                        if (projectile.velocity.X < 0)
                        {
                            projectile.velocity.X += 1;
                        }
                        else
                        {
                            projectile.velocity.X -= 1;
                        }
                    }

                    if (projectile.velocity.Y != 0)
                    {
                        if (projectile.velocity.Y < 0)
                        {
                            projectile.velocity.Y += 1;
                        }
                        else
                        {
                            projectile.velocity.Y -= 1;
                        }
                    }
                }
            }

            projectile.ai[0]++;
            Dust.NewDust(projectile.position, 1, 1, 206);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 90);
        }
    }
}