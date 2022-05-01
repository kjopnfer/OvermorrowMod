using Terraria;
using Terraria.ID;
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
            Projectile.width = 408; // from 200
            Projectile.height = 408;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 0;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 51;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.65f, 0f, 0f);

            Projectile.Center = Main.player[Projectile.owner].Center;

            Projectile.scale = Projectile.scale - 0.002f;
            Projectile.rotation = Projectile.rotation + 1;
            Projectile.alpha = Projectile.alpha + 5;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center.X, target.Center.Y, -Projectile.velocity.X, -Projectile.velocity.Y, ModContent.ProjectileType<BloodIronReal>(), Projectile.damage, 1f, Projectile.owner, 0f);
        }
    }
}
