using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.CreepingDeath
{
    public class BloodIronReal : ModProjectile
    {

        public override bool? CanDamage() => false;

        int RandomHeal = Main.rand.Next(1, 3);


        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenBoomerang);
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 10;
            Projectile.scale = 1f;
            Projectile.alpha = 255;
            Projectile.timeLeft = 500;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            RandomHeal = Main.rand.Next(1, 3);

            Projectile.ai[0]++;
            if (Projectile.ai[0] == 2)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X / 7, Projectile.velocity.Y / 7, ModContent.ProjectileType<BloodIronDraw>(), Projectile.damage, 1f, Projectile.owner, 0f);
                Projectile.ai[0] = 0;
            }
        }
        public override void Kill(int timeLeft)
        {
            Vector2 position = Main.player[Projectile.owner].Center;
            Vector2 targetPosition = Main.MouseWorld;
            Vector2 direction = targetPosition - position;
            direction.Normalize();
            float speed = 8f;
            int type = ModContent.ProjectileType<IronBolt>();
            int damage = Projectile.damage;
            Projectile.NewProjectile(Projectile.GetSource_Death(), position, direction * speed, type, damage, 4f, Main.myPlayer);
        }
    }
}