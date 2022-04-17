using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.TreeGuns
{
    public class MeatMissileProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.timeLeft = 200;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            DrawOffsetX = -6;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Terraria.Dust.NewDustPerfect(Projectile.Center, 5, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodExplosion>(), Projectile.damage, 10f, Projectile.owner);
        }
    }
}