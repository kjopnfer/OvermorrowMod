using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class SandThrowerProjectile : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 2;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 90;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            if (projectile.timeLeft > 60)
            {
                projectile.timeLeft = 60;
            }

            if (projectile.ai[0] > 7f)
            {
                float num919 = 1f;
                if (projectile.ai[0] == 8f)
                {
                    num919 = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    num919 = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    num919 = 0.75f;
                }
                projectile.ai[0] += 1f;
                int num920 = 6;
                /*if (type == 101)
				{
					num920 = 75;
				}*/
                if (num920 == 6 || Main.rand.Next(2) == 0)
                {
                    for (int num927 = 0; num927 < 1; num927++)
                    {
                        int num929 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Sand, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100);
                        Dust dust81;
                        if (Main.rand.Next(3) != 0 || (num920 == 75 && Main.rand.Next(3) == 0))
                        {
                            Main.dust[num929].noGravity = true;
                            dust81 = Main.dust[num929];
                            dust81.scale *= 1.5f;
                            Dust expr_DD41_cp_0 = Main.dust[num929];
                            expr_DD41_cp_0.velocity.X = expr_DD41_cp_0.velocity.X * 2f;
                            Dust expr_DD61_cp_0 = Main.dust[num929];
                            expr_DD61_cp_0.velocity.Y = expr_DD61_cp_0.velocity.Y * 2f;
                        }

                        dust81 = Main.dust[num929];
                        dust81.scale *= 1.5f;

                        Dust expr_DDC6_cp_0 = Main.dust[num929];
                        expr_DDC6_cp_0.velocity.X = expr_DDC6_cp_0.velocity.X * 1.2f;
                        Dust expr_DDE6_cp_0 = Main.dust[num929];
                        expr_DDE6_cp_0.velocity.Y = expr_DDE6_cp_0.velocity.Y * 1.2f;
                        dust81 = Main.dust[num929];
                        dust81.scale *= num919;
                        if (num920 == 75)
                        {
                            dust81 = Main.dust[num929];
                            dust81.velocity += projectile.velocity;
                            if (!Main.dust[num929].noGravity)
                            {
                                dust81 = Main.dust[num929];
                                dust81.velocity *= 0.5f;
                            }
                        }
                    }
                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }
    }
}