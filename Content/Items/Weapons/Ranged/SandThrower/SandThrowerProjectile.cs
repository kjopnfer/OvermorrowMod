using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.SandThrower
{
    public class SandThrowerProjectile : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 90;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 60)
            {
                Projectile.timeLeft = 60;
            }

            if (Projectile.ai[0] > 7f)
            {
                float num919 = 1f;
                if (Projectile.ai[0] == 8f)
                {
                    num919 = 0.25f;
                }
                else if (Projectile.ai[0] == 9f)
                {
                    num919 = 0.5f;
                }
                else if (Projectile.ai[0] == 10f)
                {
                    num919 = 0.75f;
                }
                Projectile.ai[0] += 1f;
                int num920 = 6;
                /*if (type == 101)
				{
					num920 = 75;
				}*/
                if (num920 == 6 || Main.rand.NextBool(2))
                {
                    for (int num927 = 0; num927 < 1; num927++)
                    {
                        int num929 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Sand, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
                        Dust dust81;
                        if (!Main.rand.NextBool(3)|| (num920 == 75 && Main.rand.NextBool(3)))
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
                            dust81.velocity += Projectile.velocity;
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
                Projectile.ai[0] += 1f;
            }
            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }
    }
}