using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class GraniteSpearElectricity : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        Projectile parentProjectile;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electricity");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 900;
            projectile.alpha = 255;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            // Parent projectile will have passed in the ID (projectile.whoAmI) for the projectile through AI fields when spawned
            for (int i = 0; i < Main.maxProjectiles; i++) // Loop through the projectile array
            {
                // Check that the projectile is the same as the parent projectile and it is active
                if (Main.projectile[i] == Main.projectile[(int)projectile.ai[0]] && Main.projectile[i].active)
                {
                    // Set the parent projectile
                    parentProjectile = Main.projectile[i];
                    projectile.netUpdate = true;
                }
            }

            if (parentProjectile.active)
            {
                projectile.timeLeft = 2;
            }

            // Orbit around the parent projectile
            DoProjectile_OrbitPosition(projectile, parentProjectile.Center, 25);

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 206, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 2f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;

                Dust dustTrail = Dust.NewDustPerfect(new Vector2(projectile.position.X, projectile.position.Y), 206, projectile.velocity);
                dustTrail.position = (Main.dust[num1110].position + projectile.Center) / 2f;
                dustTrail.noGravity = true;
            }
        }

        public void DoProjectile_OrbitPosition(Projectile modProjectile, Vector2 position, double distance, double speed = 1.75)
        {
            Projectile projectile = modProjectile;
            double deg = speed * (double)projectile.ai[1];
            double rad = deg * (Math.PI / 180);

            projectile.ai[1] += 6f;

            projectile.position.X = position.X - (int)(Math.Cos(rad) * distance) - projectile.width / 2;
            projectile.position.Y = position.Y - (int)(Math.Sin(rad) * distance) - projectile.height / 2;
            projectile.netUpdate = true;
        }
    }
}