using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.MechWep.MechProj
{
    public class LaserMechWepProj : ModProjectile
    {

        private int timer = 0;
        public override bool CanDamage() => false;

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 110;
            projectile.aiStyle = 20;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hide = true;
            projectile.ownerHitCheck = true; //so you can't hit enemies through walls
            projectile.melee = true;
        }

        public override void AI()
        {
            timer++;
            if (timer == 5)
            {
                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.MouseWorld;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 20f;
                int type = 606;
                int damage = projectile.damage;
                Vector2 perturbedSpeed = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(10f));
                Projectile.NewProjectile(position, perturbedSpeed * speed, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item12, projectile.Center);
                timer = 0;
            }
        }
    }
}