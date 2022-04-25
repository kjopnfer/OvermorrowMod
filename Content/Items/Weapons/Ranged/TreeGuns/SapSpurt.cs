using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.TreeGuns
{
    public class SapSpurt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LostSoulHostile;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sap Spurt");
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
        }
        public override void AI()
        {
            for (int num1101 = 0; num1101 < 3; num1101++)
            {
                int num1110 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Honey, Projectile.velocity.X, Projectile.velocity.Y, 50, default, 2f);
                Main.dust[num1110].position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }

            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Honey, Projectile.velocity.X, Projectile.velocity.Y, 50, default, 0.4f);
                switch (num1103)
                {
                    case 0:
                        Main.dust[num1106].position = (Main.dust[num1106].position + Projectile.Center * 5f) / 6f;
                        break;
                    case 1:
                        Main.dust[num1106].position = (Main.dust[num1106].position + (Projectile.Center + Projectile.velocity / 2f) * 5f) / 6f;
                        break;
                }
                Dust dust81 = Main.dust[num1106];
                dust81.velocity *= 0.1f;
                Main.dust[num1106].noGravity = true;
                Main.dust[num1106].fadeIn = 1f;
            }

            if (Projectile.ai[0] > 8)
            {
                Projectile.velocity.Y += 0.17f;
            }

            Projectile.ai[0]++;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(3, 5); i++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ModContent.ProjectileType<SapSpurt2>(), (Projectile.damage / 2) - 1, 10f, Main.myPlayer);
                }
            }
        }
    }

    public class SapSpurt2 : ModProjectile
    {
        int storeDamage;
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LostSoulHostile;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sap Spurt");
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                storeDamage = Projectile.damage;
                Projectile.damage = 0;
            }

            for (int num1101 = 0; num1101 < 3; num1101++)
            {
                int num1110 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Honey, Projectile.velocity.X, Projectile.velocity.Y, 50, default, 0.85f);
                Main.dust[num1110].position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }

            if (Projectile.ai[0] > 6)
            {
                Projectile.damage = storeDamage;
                Projectile.velocity.Y += 0.23f;
            }

            Projectile.ai[0]++;
        }
    }
}