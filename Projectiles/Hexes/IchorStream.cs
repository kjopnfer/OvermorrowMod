using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Hexes;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Effects.Prim.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Hexes
{
    public class IchorStream : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.IchorSplash;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ichor Stream");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.extraUpdates = 2;
            projectile.alpha = 255;
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

            for (int num110 = 0; num110 < 3; num110++)
            {
                float num111 = projectile.velocity.X / 3f * (float)num110;
                float num112 = projectile.velocity.Y / 3f * (float)num110;
                int num113 = 14;
                int num114 = Dust.NewDust(new Vector2(projectile.position.X + (float)num113, projectile.position.Y + (float)num113), projectile.width - num113 * 2, projectile.height - num113 * 2, DustID.Ichor, 0f, 0f, 100);
                Main.dust[num114].noGravity = true;
                Dust dust = Main.dust[num114];
                dust.velocity *= 0.1f;
                dust = Main.dust[num114];
                dust.velocity += projectile.velocity * 0.5f;
                Main.dust[num114].position.X -= num111;
                Main.dust[num114].position.Y -= num112;
            }

            if (Main.rand.Next(8) == 0)
            {
                int num115 = 16;
                int num116 = Dust.NewDust(new Vector2(projectile.position.X + (float)num115, projectile.position.Y + (float)num115), projectile.width - num115 * 2, projectile.height - num115 * 2, DustID.Ichor, 0f, 0f, 100, default(Color), 0.5f);
                Dust dust = Main.dust[num116];
                dust.velocity *= 0.25f;
                dust = Main.dust[num116];
                dust.velocity += projectile.velocity * 0.5f;
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
            target.AddHex(Hex.HexType<LesserIchor>(), 60 * 15);

            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}