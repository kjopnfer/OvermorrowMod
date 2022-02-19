using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class ExplosiveTarget : ModProjectile
    {
        public override bool CanDamage() => false;
        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.penetrate = 5;
            projectile.timeLeft = 250;
            projectile.light = 0.5f;
            projectile.extraUpdates = 1;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosive Target");
        }
        public override void Kill(int timeLeft)
        {
            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(projectile.position.X, projectile.position.Y - 600, value1.X, value1.Y + 17, mod.ProjectileType("CustomExplosiveProj2"), projectile.damage, 1f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X, value1.Y, mod.ProjectileType("ExplosiveTargetafter"), projectile.damage - 5, 1f, projectile.owner, 0f);
        }
    }
}
