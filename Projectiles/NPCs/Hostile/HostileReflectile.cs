using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.NPCs.Hostile
{
    public class HostileReflectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Shield");
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0f, 0.5f);

            // set npc as owner
            NPC projectileOwner = Main.npc[(int)projectile.ai[0]];

            if (projectileOwner.active)
            {
                projectile.timeLeft = 2;
            }

            //Factors for calculations
            double deg = (double)projectile.ai[1]; // rotation rate
            double rad = deg * (Math.PI / 180); // convert degrees to radians
            double dist = 100; // distance from the owner

            
            projectile.position.X = projectileOwner.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
            projectile.position.Y = projectileOwner.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;

            projectile.ai[1] += 0.5f;

            // orient sprite based on rotation
            projectile.rotation = (float)rad; 
            

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile incomingProjectile = Main.projectile[i];
                if (incomingProjectile.active && incomingProjectile.friendly)
                {
                    if (projectile.Hitbox.Intersects(incomingProjectile.Hitbox))
                    {
                        incomingProjectile.velocity *= -1;
                        incomingProjectile.friendly = false;
                        incomingProjectile.hostile = true;
                        Main.NewText("hitbox intersect");
                    }
                }
            }
        }
    }
}