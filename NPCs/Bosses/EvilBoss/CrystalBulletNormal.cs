using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class CrystalBulletNormal : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 63;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Bullet");
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.velocity.Y = 6;
        }

        public override void AI()
        {
            projectile.damage = 25;
            projectile.knockBack = 0;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void Kill(int timeLeft)
        {
            Vector2 position = projectile.Center;
            Vector2 targetPosition = Main.player[projectile.owner].position;
            Vector2 direction = targetPosition - position;
            direction.Normalize();
            float speed = 11.7f;
            int type = mod.ProjectileType("CrystalBulletRang");
            int damage = projectile.damage - 3;
            Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
        }
    }
}