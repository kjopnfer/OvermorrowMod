using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.NPCs.Town
{
    public class MerchantSpike : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.MagnetSphereBolt;
        public override void SetDefaults()
        {
            //projectile.CloneDefaults(ProjectileID.MagnetSphereBolt);
            //aiType = ProjectileID.MagnetSphereBolt;
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            for (int num353 = 0; num353 < 4; num353++)
            {
                Vector2 vector29 = projectile.position;
                vector29 -= projectile.velocity * ((float)num353 * 0.25f);
                projectile.alpha = 255;
                //int num354 = Dust.NewDust(vector29, 1, 1, 160);
                //Main.dust[num354].position = vector29;
                //Main.dust[num354].position.X += projectile.width / 2;
                //Main.dust[num354].position.Y += projectile.height / 2;
                //Main.dust[num354].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                //Dust dust = Main.dust[num354];
                //dust.velocity *= 0.2f;

                Particle.CreateParticle(Particle.ParticleType<BlackFlame>(), vector29, Vector2.Zero, Main.DiscoColor, 1, (float)Main.rand.Next(70, 110) * 0.013f, 0, 1f);
            }

            
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 400f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5)
                {
                    Vector2 newMove = Main.npc[k].Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }

            if (target)
            {
                AdjustMagnitude(ref move);
                projectile.velocity = (10 * projectile.velocity + move) / 11f;
                AdjustMagnitude(ref projectile.velocity);
            }
        }
        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Particle.CreateParticle(Particle.ParticleType<Shockwave>(), projectile.Center, Vector2.Zero, Color.Black, 1, 1, 0, 1f);
        }
    }
}