using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic.CreepingD
{
    public class IronBloodCircle : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 200;
            projectile.height = 200;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.light = 0.5f;
            projectile.alpha = 0;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 51;
        }
        public override void AI()
        {
            projectile.position.X = Main.player[projectile.owner].Center.X - 100;
            projectile.position.Y = Main.player[projectile.owner].Center.Y - 100;
            projectile.scale = projectile.scale - 0.002f;
            projectile.rotation = projectile.rotation + 1;
            projectile.alpha = projectile.alpha + 5;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cone");
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(target.Center.X, target.Center.Y, -projectile.velocity.X, -projectile.velocity.Y, mod.ProjectileType("BloodIronReal"), projectile.damage, 1f, projectile.owner, 0f);
        }
    }
}
