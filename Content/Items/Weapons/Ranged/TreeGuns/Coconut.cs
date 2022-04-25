using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.TreeGuns
{
    public class Coconut : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.timeLeft = 200;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.velocity.Y += 0.13f;

            if (Projectile.ai[0] > 3f)
            {
                Projectile.ai[0] += Projectile.ai[1];
                if (Projectile.ai[0] > 30f)
                {
                    Projectile.velocity.Y += 0.1f;
                }

                int num508 = 175;
                Color newColor2 = new Color(255, 255, 255, 100);
                for (int num509 = 0; num509 < 6; num509++)
                {
                    Vector2 vector41 = Projectile.velocity * num509 / 6f;
                    int num510 = 6;
                    int num511 = Dust.NewDust(Projectile.position + Vector2.One * 6f, Projectile.width - num510 * 2, Projectile.height - num510 * 2, DustID.t_Slime, 0f, 0f, num508, newColor2, 1.2f);
                    Dust dust;
                    if (Main.rand.Next(2) == 0)
                    {
                        dust = Main.dust[num511];
                        dust.alpha += 25;
                    }
                    if (Main.rand.Next(2) == 0)
                    {
                        dust = Main.dust[num511];
                        dust.alpha += 25;
                    }
                    if (Main.rand.Next(2) == 0)
                    {
                        dust = Main.dust[num511];
                        dust.alpha += 25;
                    }
                    Main.dust[num511].noGravity = true;
                    dust = Main.dust[num511];
                    dust.velocity *= 0.3f;
                    dust = Main.dust[num511];
                    dust.velocity += Projectile.velocity * 0.5f;
                    Main.dust[num511].position = Projectile.Center;
                    Main.dust[num511].position.X -= vector41.X;
                    Main.dust[num511].position.Y -= vector41.Y;
                    dust = Main.dust[num511];
                    dust.velocity *= 0.2f;
                }
                if (Main.rand.Next(4) == 0)
                {
                    int num512 = 6;
                    int num513 = Dust.NewDust(Projectile.position + Vector2.One * 6f, Projectile.width - num512 * 2, Projectile.height - num512 * 2, DustID.t_Slime, 0f, 0f, num508, newColor2, 1.2f);
                    Dust dust = Main.dust[num513];
                    dust.velocity *= 0.5f;
                    dust = Main.dust[num513];
                    dust.velocity += Projectile.velocity * 0.5f;
                }
            }
            else
            {
                Projectile.ai[0] += 1f;
            }
        }
    }
}