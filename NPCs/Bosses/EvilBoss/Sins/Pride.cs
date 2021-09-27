using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.NPCs.Bosses.EvilBoss.Sins
{
    public class Pride : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;
            projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            projectile.rotation += 0.5f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            int RandomPosX = Main.rand.Next(-360, 361);
            int RandomPosY = Main.rand.Next(-360, 361);
            target.position.X += RandomPosX;
            target.position.Y += RandomPosY;
            target.velocity.X = 0;
            target.velocity.Y = 0;
            Main.PlaySound(SoundID.Item6, projectile.Center);


            Vector2 position = target.Center;
            int radius = 9;     //this is the explosion radius, the highter is the value the bigger is the explosion

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {

                    if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                    {
                        Color alpha = Color.White;
                        int dust = Dust.NewDust(position, projectile.width, projectile.height, DustID.FrostHydra, 0.0f, 0.0f, 120, alpha, 2f);
                        Main.dust[dust].noGravity = true;
                    }
                }
            }

        }


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sin of Pride");
        }
    }
}