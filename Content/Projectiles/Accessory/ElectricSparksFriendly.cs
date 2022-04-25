using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Accessory
{
    public class ElectricSparksFriendly : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Sparks");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 290;
            Projectile.alpha = 255;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0, 0.5f, 0.5f);

            // Make projectile slow down until stationary
            if (Projectile.timeLeft <= 125 && ++Projectile.ai[0] % 40 == 0)
            {
                if (Projectile.velocity.X != 0)
                {
                    if (Projectile.velocity.X < 0)
                    {
                        Projectile.velocity.X += 1;
                    }
                    else
                    {
                        Projectile.velocity.X -= 1;
                    }
                }

                if (Projectile.velocity.Y != 0)
                {
                    if (Projectile.velocity.Y < 0)
                    {
                        Projectile.velocity.Y += 1;
                    }
                    else
                    {
                        Projectile.velocity.Y -= 1;
                    }
                }
            }

            Dust.NewDust(Projectile.position, 1, 1, DustID.UnusedWhiteBluePurple);
        }
    }
}