using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardRanged
{
    public class katyusharocket : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.RocketI);
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.width = 20;
            projectile.height = 24;
            projectile.penetrate = 1;
            projectile.aiStyle = 34;
            projectile.timeLeft = 120; //The amount of time the projectile is alive for
        }



        public override void AI()
        {
            int Random = Main.rand.Next(-4, 4);

            if (projectile.timeLeft == 120)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.velocity.Y = -19 + Random;
            }
            else
            {
                projectile.velocity.Y += 0.4f;
            }
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
                        Dust.NewDust(position, 5, 5, DustID.HeatRay, 0.0f, 0.0f, 120, new Color(), 3.3f);  //this is the dust that will spawn after the explosion
                    }
                }
            }
        }
    }
}
