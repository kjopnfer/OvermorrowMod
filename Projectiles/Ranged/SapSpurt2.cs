using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class SapSpurt2 : ModProjectile
    {
        int storeDamage;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulHostile;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sap Spurt");
        }
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.friendly = true;
        }
        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                storeDamage = projectile.damage;
                projectile.damage = 0;
            }

            for (int num1101 = 0; num1101 < 3; num1101++)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Honey, projectile.velocity.X, projectile.velocity.Y, 50, default, 0.85f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }

            if (projectile.ai[0] > 6)
            {
                projectile.damage = storeDamage;
                projectile.velocity.Y += 0.23f;
            }

            projectile.ai[0]++;
        }
    }
}