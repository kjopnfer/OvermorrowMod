using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class Bones : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.Bone;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bouncing Bones");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            //projectile.aiStyle = 2;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 360;
        }

        public override void AI()
        {
            // copy aiStyle = 2
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.03f * (float)projectile.direction;
            projectile.ai[0] += 1f;

            if (projectile.ai[0] >= 20f)
            {
                projectile.velocity.Y = projectile.velocity.Y + 0.4f;
            }

            if (projectile.velocity.Y > 16f) // Maximum acceleration
            {
                projectile.velocity.Y = 16f;
            }

            // Make projectiles gradually disappear
            if(projectile.timeLeft <= 60)
            {
                projectile.alpha += 5;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            // Make projectiles bounce on impact

            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
    }
}