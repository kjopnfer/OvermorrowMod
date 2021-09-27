using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class SpearBomb : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SkullBomb");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.RocketI);
            projectile.hostile = true;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.width = 24;
            projectile.height = 24;
            projectile.penetrate = -1;
            projectile.aiStyle = 34;
            projectile.timeLeft = 0; //The amount of time the projectile is alive for
        }


        public override void Kill(int timeLeft)
        {

            Vector2 position = projectile.Center;
            Main.PlaySound(SoundID.Item14, (int)position.X, (int)position.Y);
            int radius = 6;     //this is the explosion radius, the highter is the value the bigger is the explosion

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {

                    if (Math.Sqrt(x * x + y * y) <= radius + 1)   //this make so the explosion radius is a circle
                    {
                        Color alpha = Color.Black;
                        Dust.NewDust(position, 5, 5, DustID.Smoke, 0.0f, 0.0f, 120, new Color(), 1f);  //this is the dust that will spawn after the explosion
                    }
                }
            }
        }
    }
}
