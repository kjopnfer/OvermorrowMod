using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class HolyLight : ModProjectile
    {
        private int length = 1;
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.timeLeft = 4000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.light = 5f;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.CloneDefaults(ProjectileID.RocketI);
            projectile.alpha = 50;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {

            timer++;
            if (timer == 1)
            {
                length++;
            }

            if (length == 220)
            {
                projectile.timeLeft = 0;
            }


            if (timer == 2)
            {
                timer = 0;
            }
            //float point = 0f;
            projectile.velocity.Y = 0f;
            //Vector2 endPoint;
            projectile.position.X = length * (float)Math.Cos(projectile.rotation) + Main.player[projectile.owner].Center.X - 16;
            projectile.position.Y = length * (float)Math.Sin(projectile.rotation) + Main.player[projectile.owner].Center.Y - 50;
            projectile.rotation += (float)((2 * Math.PI) / (Math.PI * 2 * 100 / 10)); // 200 is the speed, god only knows what dividing by 10 does
        }

        public override void Kill(int timeLeft)
        {

            if (length < 220)
            {
                Vector2 position = projectile.Center;
                Main.PlaySound(SoundID.Item14, (int)position.X, (int)position.Y);
                Main.PlaySound(SoundID.Item35, (int)position.X, (int)position.Y);
                int radius = 5;     //this is the explosion radius, the highter is the value the bigger is the explosion

                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {

                        if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                        {
                            Color alpha = Color.LightBlue;
                            Dust.NewDust(position, 5, 5, DustID.Enchanted_Gold, 0.0f, 0.0f, 120, alpha, 1f);
                        }
                    }
                }
            }
        }
    }
}
