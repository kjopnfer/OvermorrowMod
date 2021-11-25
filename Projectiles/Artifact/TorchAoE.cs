using Microsoft.Xna.Framework;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Effects.Prim.Trails;
using OvermorrowMod.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Artifact
{
    public class TorchAoE : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.CursedFlameFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Torch God's Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 1;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            /*int num651 = Main.rand.Next(4, 10);
            for (int num652 = 0; num652 < num651; num652++)
            {
                int num653 = Dust.NewDust(projectile.Center, 0, 0, Main.rand.NextBool(2) ? 180 : 6, 0f, 0f, 100, default, 3f);
                Dust dust = Main.dust[num653];
                dust.velocity *= 1.6f;
                Main.dust[num653].velocity.Y -= 1f;
                dust = Main.dust[num653];
                dust.velocity += -projectile.velocity * (Main.rand.NextFloat() * 2f - 1f);
                Main.dust[num653].scale = 2f;
                Main.dust[num653].fadeIn = 0.5f;
                Main.dust[num653].noGravity = true;
            }
            */
            for (int num369 = 0; num369 < 10; num369++)
            {
                int num370 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 174, 0f, 0f, 100, default(Color), 1.2f);
                Main.dust[num370].noGravity = true;
                Dust dust = Main.dust[num370];
                dust.velocity *= 0.5f;
                dust = Main.dust[num370];
                dust.velocity += projectile.velocity * 0.1f;
            }

            float num373 = 25f;
            for (int num374 = 0; (float)num374 < num373; num374++)
            {
                float num375 = Main.rand.Next(-10, 11);
                float num376 = Main.rand.Next(-10, 11);
                float num377 = Main.rand.Next(3, 9);
                float num378 = (float)Math.Sqrt(num375 * num375 + num376 * num376);
                num378 = num377 / num378;
                num375 *= num378;
                num376 *= num378;
                int num379 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 174, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num379].noGravity = true;
                Main.dust[num379].position.X = projectile.Center.X;
                Main.dust[num379].position.Y = projectile.Center.Y;
                Main.dust[num379].position.X += Main.rand.Next(-10, 11);
                Main.dust[num379].position.Y += Main.rand.Next(-10, 11);
                Main.dust[num379].velocity.X = num375;
                Main.dust[num379].velocity.Y = num376;
            }
            
            // Not the explosion from the smaller fireball
            if (projectile.ai[0] != 1)
            {
                int numLocations = 6;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(360f / numLocations * i)) * 15;
                    Vector2 dustvelocity = new Vector2(0f, Main.rand.Next(3, 5)).RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * Main.rand.Next(1, 3);

                    Particle.CreateParticle(Particle.ParticleType<Glow>(), position, dustvelocity, Main.rand.NextBool(2) ? Color.LightBlue : Color.Orange, 1, 0.5f, 0, 1f);
                }
            }
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 60 * 8);

            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}