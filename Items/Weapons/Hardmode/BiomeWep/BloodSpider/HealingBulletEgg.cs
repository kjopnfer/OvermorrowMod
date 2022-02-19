using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.BiomeWep.BloodSpider
{
    public class HealingBulletEgg : ModProjectile
    {

        public override bool CanDamage() => false;
        private int timer = 0;
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


            timer++;
            if (timer == 2)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X / 7, projectile.velocity.Y / 7, mod.ProjectileType("BloodTrail"), projectile.damage, 1f, projectile.owner, 0f);
                timer = 0;
            }
        }
        public override void Kill(int timeLeft)
        {

            Main.player[projectile.owner].statLife += RandomHeal;
            CombatText.NewText(Main.player[projectile.owner].getRect(), Color.LightGreen, RandomHeal);
        }
    }
}