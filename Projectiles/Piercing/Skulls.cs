using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class Skulls : ModProjectile
    {
        private float storeDirection;
        private float trigCounter = 0;
        private float period = 30;
        private float amplitude = 10;
        private float previousR = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skull");
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 420;
            projectile.tileCollide = true;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                storeDirection = projectile.velocity.ToRotation();

                Vector2 origin = projectile.Center;
                float radius = 15;
                int numLocations = 30;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, 160, dustvelocity.X, dustvelocity.Y, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }
            }

            trigCounter += 2 * (float)Math.PI / period;
            float r = amplitude * (float)Math.Sin(trigCounter) * (projectile.ai[1] == 0 ? 1 : -1);
            Vector2 instaVel = PolarVector(r - previousR, storeDirection + (float)Math.PI / 2);
            projectile.position += instaVel;
            previousR = r;
            //projectile.rotation = (projectile.velocity + instaVel).ToRotation() + (float)Math.PI / 27;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            projectile.spriteDirection = projectile.direction;

            /*if (projectile.ai[0] >= 61)
            {
                projectile.alpha = 0;
                Vector2 position = projectile.Center;
                Dust dust = Dust.NewDustPerfect(position, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            }*/

            //projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);

            projectile.ai[0]++;
        }

        public Vector2 PolarVector(float radius, float theta)
        {
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
        }
    }
}