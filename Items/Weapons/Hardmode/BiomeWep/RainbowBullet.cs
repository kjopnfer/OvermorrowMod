using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace OvermorrowMod.Items.Weapons.Hardmode.BiomeWep
{
    public class RainbowBullet : ModProjectile
    {
        
        public override bool CanDamage() => false;
        private int timer = 0; 

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 10;
            projectile.scale = 1f;
            projectile.alpha = 255;
            projectile.timeLeft = 200;
            projectile.extraUpdates = 1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 1)
            {
                projectile.Kill();
            }
            else
            {
                Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Item10, projectile.position);
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override void AI()
        {

            timer++;
            if(timer == 2)
            {
            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X / 7, projectile.velocity.Y / 7, mod.ProjectileType("RainbowTrail"), projectile.damage, 1f, projectile.owner, 0f);
            timer = 0;
            }
        }
    }
}