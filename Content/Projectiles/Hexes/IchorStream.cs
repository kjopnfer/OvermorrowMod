using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Hexes;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Hexes
{
    public class IchorStream : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.IchorSplash;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ichor Stream");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            Projectile.spriteDirection = Projectile.direction;

            if (Projectile.ai[0] > 60)
            {
                Vector2 move = Vector2.Zero;
                float distance = 1000f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].CanBeChasedBy(this))
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
                    Projectile.velocity = (20 * Projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref Projectile.velocity);
                }
            }

            Projectile.ai[0]++;

            for (int num110 = 0; num110 < 3; num110++)
            {
                float num111 = Projectile.velocity.X / 3f * (float)num110;
                float num112 = Projectile.velocity.Y / 3f * (float)num110;
                int num113 = 14;
                int num114 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num113, Projectile.position.Y + (float)num113), Projectile.width - num113 * 2, Projectile.height - num113 * 2, DustID.Ichor, 0f, 0f, 100);
                Main.dust[num114].noGravity = true;
                Dust dust = Main.dust[num114];
                dust.velocity *= 0.1f;
                dust = Main.dust[num114];
                dust.velocity += Projectile.velocity * 0.5f;
                Main.dust[num114].position.X -= num111;
                Main.dust[num114].position.Y -= num112;
            }

            if (Main.rand.Next(8) == 0)
            {
                int num115 = 16;
                int num116 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num115, Projectile.position.Y + (float)num115), Projectile.width - num115 * 2, Projectile.height - num115 * 2, DustID.Ichor, 0f, 0f, 100, default(Color), 0.5f);
                Dust dust = Main.dust[num116];
                dust.velocity *= 0.25f;
                dust = Main.dust[num116];
                dust.velocity += Projectile.velocity * 0.5f;
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

        public override bool? CanDamage()
        {
            if (Projectile.ai[0] < 60)
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