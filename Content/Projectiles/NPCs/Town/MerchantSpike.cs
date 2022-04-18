using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.NPCs.Town
{
    public class MerchantSpike : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.MagnetSphereBolt;
        public override void SetDefaults()
        {
            //projectile.CloneDefaults(ProjectileID.MagnetSphereBolt);
            //aiType = ProjectileID.MagnetSphereBolt;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            for (int num353 = 0; num353 < 4; num353++)
            {
                Vector2 vector29 = Projectile.position;
                vector29 -= Projectile.velocity * ((float)num353 * 0.25f);
                Projectile.alpha = 255;
                //int num354 = Dust.NewDust(vector29, 1, 1, 160);
                //Main.dust[num354].position = vector29;
                //Main.dust[num354].position.X += projectile.width / 2;
                //Main.dust[num354].position.Y += projectile.height / 2;
                //Main.dust[num354].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                //Dust dust = Main.dust[num354];
                //dust.velocity *= 0.2f;

                Particle.CreateParticle(Particle.ParticleType<BlackFlame>(), vector29, Vector2.Zero, Main.DiscoColor, 1, (float)Main.rand.Next(70, 110) * 0.013f, 0, 1f);
            }


            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                Projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 400f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5)
                {
                    Vector2 newMove = Main.npc[k].Center - Projectile.Center;
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
                Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
                AdjustMagnitude(ref Projectile.velocity);
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
            Particle.CreateParticle(Particle.ParticleType<Pulse>(), Projectile.Center, Vector2.Zero, Color.Black, 1, 1, 0, 1f);
        }
    }
}