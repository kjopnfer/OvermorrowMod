using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Apollus
{
    public class HostileReflectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Shield");
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0f, 0.5f);

            // set npc as owner
            NPC ProjectileOwner = Main.npc[(int)Projectile.ai[0]];

            if (ProjectileOwner.active)
            {
                Projectile.timeLeft = 2;
            }

            //Factors for calculations
            double deg = (double)Projectile.ai[1]; // rotation rate
            double rad = deg * (Math.PI / 180); // convert degrees to radians
            double dist = 100; // distance from the owner


            Projectile.position.X = ProjectileOwner.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
            Projectile.position.Y = ProjectileOwner.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

            Projectile.ai[1] += 0.5f;

            // orient sprite based on rotation
            Projectile.rotation = (float)rad;


            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile incomingProjectile = Main.projectile[i];
                if (incomingProjectile.active && incomingProjectile.friendly)
                {
                    if (Projectile.Hitbox.Intersects(incomingProjectile.Hitbox))
                    {
                        incomingProjectile.velocity *= -1;
                        incomingProjectile.friendly = false;
                        incomingProjectile.hostile = true;
                    }
                }
            }
        }
    }
}