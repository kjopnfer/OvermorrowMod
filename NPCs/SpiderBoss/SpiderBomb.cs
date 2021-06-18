using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class SpiderBomb : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.RocketI);
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.tileCollide = true;
            projectile.width = 20;
            projectile.height = 20;
            projectile.penetrate = -1;
            projectile.aiStyle = 34;
            projectile.timeLeft = 1; //The amount of time the projectile is alive for
        }



        public override void AI()
        {
            if(projectile.timeLeft == 1)
            {
                projectile.hostile = true;
                projectile.friendly = false;
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
                        Dust.NewDust(position, 5, 5, 162, 0.0f, 0.0f, 120, new Color(), 3.3f);  //this is the dust that will spawn after the explosion
                    }
                }
            }
        }
    }
}
