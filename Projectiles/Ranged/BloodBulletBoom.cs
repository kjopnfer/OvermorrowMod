using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class BloodBulletBoom : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.width = 100;
            projectile.height = 100;
            projectile.penetrate = -1;
            projectile.timeLeft = 1; //The amount of time the projectile is alive for
        }

        public override void Kill(int timeLeft)
        {

            Vector2 position = projectile.Center;
            Main.PlaySound(SoundID.Item14, (int)position.X, (int)position.Y);
            int radius = 20;     //this is the explosion radius, the highter is the value the bigger is the explosion

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {

                    if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                    {
                        Color color = Color.Red;
                        Dust.NewDust(position, 5, 5, DustID.HeatRay, 0.0f, 0.0f, 120, color, 3.3f);  //this is the dust that will spawn after the explosion
                    }
                }
            }
        }
    }
}
