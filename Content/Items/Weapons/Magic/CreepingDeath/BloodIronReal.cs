using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.CreepingDeath
{
    public class BloodIronReal : ModProjectile
    {

        public override bool CanDamage() => false;

        int RandomHeal = Main.rand.Next(1, 3);


        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.WoodenBoomerang);
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 10;
            projectile.scale = 1f;
            projectile.alpha = 255;
            projectile.timeLeft = 500;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {

            RandomHeal = Main.rand.Next(1, 3);

            projectile.ai[0]++;
            if (projectile.ai[0] == 2)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X / 7, projectile.velocity.Y / 7, mod.ProjectileType("BloodIronDraw"), projectile.damage, 1f, projectile.owner, 0f);
                projectile.ai[0] = 0;
            }
        }
        public override void Kill(int timeLeft)
        {
            Vector2 position = Main.player[projectile.owner].Center;
            Vector2 targetPosition = Main.MouseWorld;
            Vector2 direction = targetPosition - position;
            direction.Normalize();
            float speed = 8f;
            int type = mod.ProjectileType("IronBolt");
            int damage = projectile.damage;
            Projectile.NewProjectile(position, direction * speed, type, damage, 4f, Main.myPlayer);
        }
    }
}