using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class ManaWave : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        private float storeDirection;
        private float trigCounter = 0;
        private float period = 60;
        private float amplitude = 20;
        private float previousR = 0;

        private bool initProperties = true;
        private bool spawnedOnce = true;
        private Projectile childProjectile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana Storm");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 4;
            projectile.timeLeft = 410;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
 
            // This runs once when the projectile is created
            if (initProperties) 
            { 
                storeDirection = projectile.velocity.ToRotation();
                if(Main.netMode != NetmodeID.Server && projectile.owner == Main.myPlayer)
                {
                    // This spawns the child projectile that travels in the opposite direction
                    if(projectile.ai[0] == 0)
                    {
                        childProjectile = Main.projectile[Projectile.NewProjectile(projectile.Center, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 1, projectile.whoAmI)];
                    }
                    else // This is the check that the child projectile enters in
                    {
                        childProjectile = Main.projectile[(int)projectile.ai[1]];
                    }
                }
                initProperties = false;
            }

            trigCounter += 2 * (float)Math.PI / period;
            float r = amplitude * (float)Math.Sin(trigCounter) * (projectile.ai[0] == 0 ? 1 : -1);
            Vector2 instaVel = PolarVector(r - previousR, storeDirection + (float)Math.PI / 2);
            projectile.position += instaVel;
            previousR = r;
            projectile.rotation = (projectile.velocity + instaVel).ToRotation() + (float)Math.PI / 27;


            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 221, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 0.4f);
                switch (num1103)
                {
                    case 0:
                        Main.dust[num1106].position = (Main.dust[num1106].position + projectile.Center * 5f) / 6f;
                        break;
                    case 1:
                        Main.dust[num1106].position = (Main.dust[num1106].position + (projectile.Center + projectile.velocity / 2f) * 5f) / 6f;
                        break;
                }
                Dust dust81 = Main.dust[num1106];
                dust81.velocity *= 0.1f;
                Main.dust[num1106].noGravity = true;
                Main.dust[num1106].fadeIn = 1f;
            }

            float radius = 30;
            projectile.ai[1] += projectile.ai[0] == 0 ? 16 : -16;

            // Honestly after like 2 hours, I have no idea how to convert the movement of the dust to the movement of the projectiles
            // So what this does is it just spams projectiles, over and over and over again that act as the hitbox for the dust.
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustPos = projectile.Center + new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.ai[1]));
                Dust dust = Dust.NewDustPerfect(dustPos, 113, Vector2.Zero, 0, default, 1f);
                dust.noGravity = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (i == 0)
                    {
                        Projectile.NewProjectile(dustPos, Vector2.Zero, ModContent.ProjectileType<ManaDust>(), projectile.damage / 2, i, projectile.owner, projectile.whoAmI, 1);
                    }
                }
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 dustPos = projectile.Center - new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.ai[1]));
                Dust dust = Dust.NewDustPerfect(dustPos, 113, Vector2.Zero, 0, default, 1f);
                dust.noGravity = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (i == 0)
                    {
                        Projectile.NewProjectile(dustPos, Vector2.Zero, ModContent.ProjectileType<ManaDust>(), projectile.damage / 2, i, projectile.owner, projectile.whoAmI, 1);
                    }
                }
            }
        }

        public Vector2 PolarVector(float radius, float theta)
        {
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
        }
    }
}