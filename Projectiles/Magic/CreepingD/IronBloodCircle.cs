using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic.CreepingD
{
    public class IronBloodCircle : ModProjectile
    {
        const int radius = 204;
        const int width = 40;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iron Blood Circle");
        }

        public override void SetDefaults()
        {
            projectile.width = radius * 2; // from 200
            projectile.height = radius * 2;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.magic = true;
            projectile.alpha = 0;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 51;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.65f, 0f, 0f);

            projectile.Center = Main.player[projectile.owner].Center;

            projectile.scale = projectile.scale - 0.002f;
            projectile.rotation = projectile.rotation + 1;
            projectile.alpha = projectile.alpha + 5;
        }

        public override bool? CanHitNPC(NPC target)
        {
            float dist = (target.Center - projectile.Center).Length();
            return dist <= radius && dist >= radius - width;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, -projectile.velocity.X, -projectile.velocity.Y, mod.ProjectileType("BloodIronReal"), projectile.damage, 1f, projectile.owner, 0f);
        }
    }
}
