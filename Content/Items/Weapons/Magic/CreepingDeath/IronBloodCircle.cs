using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.CreepingDeath
{
    public class IronBloodCircle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iron Blood Circle");
        }

        public override void SetDefaults()
        {
            projectile.width = 408; // from 200
            projectile.height = 408;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, -projectile.velocity.X, -projectile.velocity.Y, mod.ProjectileType("BloodIronReal"), projectile.damage, 1f, projectile.owner, 0f);
        }
    }
}
