using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class ElectricBall : ModProjectile
    {
        Projectile parentProjectile;
        private int speedUpCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Ball");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);

            int num434 = Dust.NewDust(projectile.Center, 0, 0, 229, 0f, 0f, 100);
            Main.dust[num434].noLight = true;
            Main.dust[num434].noGravity = true;
            Main.dust[num434].velocity = projectile.velocity;
            Main.dust[num434].position -= Vector2.One * 4f;
            Main.dust[num434].scale = 0.8f;

            if (++projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

            // Parent projectile will have passed in the ID (projectile.whoAmI) for the projectile through AI fields when spawned
            for (int i = 0; i < Main.maxProjectiles; i++) // Loop through the projectile array
            {
                // Check that the projectile is the same as the parent projectile and it is active
                if (Main.projectile[i] == Main.projectile[(int)projectile.ai[0]] && Main.projectile[i].active)
                {
                    // Set the parent projectile
                    parentProjectile = Main.projectile[i];
                }
            }

            // Orbit around the parent projectile
            DoProjectile_OrbitPosition(projectile, parentProjectile.Center, 250);

            // Make projectiles gradually disappear
            if (projectile.timeLeft <= 60)
            {
                projectile.alpha += 5;
            }
        }

        public void DoProjectile_OrbitPosition(Projectile modProjectile, Vector2 position, double distance, double speed = 1.75)
        {
            Projectile projectile = modProjectile;
            double deg = speed * (double)projectile.ai[1];
            double rad = deg * (Math.PI / 180);

            // Controls how quickly the projectile rotates in a circle
            if(speedUpCounter > 180)
            {
                projectile.ai[1] += 4f;
            }else if (speedUpCounter > 135)
            {
                projectile.ai[1] += 3f;
            }
            else if(speedUpCounter > 90)
            {
                projectile.ai[1] += 2f;
            }else
            {
                projectile.ai[1] += 1f;
            }

            // Gradually increase speed
            speedUpCounter++;

            projectile.position.X = position.X - (int)(Math.Cos(rad) * distance) - projectile.width / 2;
            projectile.position.Y = position.Y - (int)(Math.Sin(rad) * distance) - projectile.height / 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft >= 60)
            {
                return Color.White;
            }
            else
            {
                return base.GetAlpha(lightColor);
            }
        }
    }
}