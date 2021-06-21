using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace OvermorrowMod.Projectiles.DiceProj
{
    public class BoomDiecProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.hostile = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.width = 70;
            projectile.height = 70;
            projectile.CloneDefaults(ProjectileID.Bomb);
            projectile.timeLeft = 0; //The amount of time the projectile is alive for
        }


        public override void Kill(int timeLeft)
        {

            Vector2 position = projectile.Center;
            Main.PlaySound(SoundID.Item14, (int)position.X, (int)position.Y);
            int radius = 5;     //this is the explosion radius, the highter is the value the bigger is the explosion

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {

                    if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                    {
                        Dust.NewDust(position, 5, 5, DustID.Smoke, 0.0f, 0.0f, 120, new Color(), 0.6f);  //this is the dust that will spawn after the explosion
                    }
                }
            }
        }
    }
}
