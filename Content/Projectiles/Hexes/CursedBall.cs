using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Hexes
{
    public class CursedBall : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.CursedFlameFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Fireball");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            projectile.spriteDirection = projectile.direction;

            if (projectile.ai[0] > 60)
            {
                Vector2 move = Vector2.Zero;
                float distance = 1000f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].CanBeChasedBy(this))
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
                    projectile.velocity = (20 * projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref projectile.velocity);
                }
            }

            projectile.ai[0]++;

            int num98 = Dust.NewDust(new Vector2(projectile.position.X + projectile.velocity.X, projectile.position.Y + projectile.velocity.Y), projectile.width, projectile.height, DustID.CursedTorch, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 2f * projectile.scale);
            Main.dust[num98].noGravity = true;

            projectile.localAI[0]++;
            if (projectile.localAI[0] % 15 == 0)
            {
                DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
                Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * 10f, 8f, DelegateMethods.CastLightOpen);
                float num108 = 16f;
                for (int num109 = 0; (float)num109 < num108; num109++)
                {
                    Vector2 spinningpoint5 = Vector2.UnitX * 0f;
                    spinningpoint5 += -Vector2.UnitY.RotatedBy((float)num109 * ((float)Math.PI * 2f / num108)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(projectile.velocity.ToRotation());
                    int num110 = Dust.NewDust(projectile.Center, 0, 0, DustID.CursedTorch);
                    Main.dust[num110].scale = 1.5f;
                    Main.dust[num110].noGravity = true;
                    Main.dust[num110].position = projectile.Center + spinningpoint5;
                    Main.dust[num110].velocity = projectile.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * 1f;
                }
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

        public override bool CanDamage()
        {
            if (projectile.ai[0] < 60)
            {
                return false;
            }

            return base.CanDamage();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 60 * 8);

            base.OnHitNPC(target, damage, knockback, crit);
        }

        public override void Kill(int timeLeft)
        {
            Vector2 origin = projectile.Center;
            float radius = 15;
            int numLocations = 30;
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                int dust = Dust.NewDust(position, 2, 2, DustID.CursedTorch, dustvelocity.X, dustvelocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}