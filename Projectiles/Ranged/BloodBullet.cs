using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class BloodBullet : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 20;
            projectile.penetrate = 1;
            projectile.friendly = true;
            projectile.hostile = false;
        }

		public override void AI() 
		{
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if(target.life < projectile.damage - target.defense)
            {
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<BloodBulletBoom>(), projectile.damage * 2, 1f, projectile.owner, 0f);
            }
        }
    }
}