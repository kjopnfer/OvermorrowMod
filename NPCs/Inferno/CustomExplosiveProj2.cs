using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class CustomExplosiveProj2 : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 26;   //This defines the hitbox width
            projectile.height = 48;    //This defines the hitbox height //How the projectile works, 16 is the aistyle Used for: Grenades, Dynamite, Bombs, Sticky Bomb.
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1; //Tells the game how many enemies it can hit before being destroyed
            projectile.timeLeft = 200; //The amount of time the projectile is alive for
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skull Bomb");
        }

        public override void Kill(int timeLeft)
        {

            Vector2 position = projectile.Center;
            Main.PlaySound(SoundID.Item14, (int)position.X, (int)position.Y);
            int radius = 7;     //this is the explosion radius, the highter is the value the bigger is the explosion

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {

                    if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                    {
                        Dust.NewDust(position, 22, 22, DustID.Smoke, 0.0f, 0.0f, 120, new Color(), 1f);  //this is the dust that will spawn after the explosion
                    }
                }
            }
        }
    }
}
