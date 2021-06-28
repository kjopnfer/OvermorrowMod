using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class FlameCone : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 2;
            projectile.scale = 0.7f;
            projectile.timeLeft = 45;
            projectile.extraUpdates = 1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cone");
        }

        public override void Kill(int timeLeft)
        {
            Vector2 perturbedSpeed = new Vector2(projectile.velocity.X / -4f, projectile.velocity.Y / -4f).RotatedBy(MathHelper.ToRadians(90));
            Vector2 FlamePlay = projectile.Center;
            Main.PlaySound(SoundID.Item20, (int)FlamePlay.X, (int)FlamePlay.Y);;
            Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X / -4f, projectile.velocity.Y / -4f, 85, projectile.damage - 10, 1f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, 85, projectile.damage - 7, 1f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.position.X, projectile.position.Y, perturbedSpeed.X * -1, perturbedSpeed.Y * -1, 85, projectile.damage - 10, 1f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.position.X, projectile.position.Y, perturbedSpeed.X, perturbedSpeed.Y, 85, projectile.damage - 10, 1f, projectile.owner, 0f);
        }
    }
}