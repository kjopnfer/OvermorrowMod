using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.MechWep.MechProj
{
    public class MechCircle : ModProjectile
    {
        readonly int Random = Main.rand.Next(0, 111);
        readonly int defence = Main.rand.Next(0, 60);
        readonly int zoppler = Main.rand.Next(10, 200);
        readonly int attckter = Main.rand.Next(10, 5000);
        readonly int scaleddd = Main.rand.Next(1, 5);

        private int timer = -1;

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.light = 0.5f;
            projectile.alpha = 2550;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 85;
        }
        public override void AI()
        {
            projectile.position.X = Main.MouseWorld.X;
            projectile.position.Y = Main.MouseWorld.Y;
            timer++;
            Vector2 value1 = new Vector2(0f, 0f);
            if (timer == 0)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 100, value1.X, value1.Y + 9, mod.ProjectileType("MechMageProj"), projectile.damage, 1f, projectile.owner, 0f);

                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y + 100, value1.X, value1.Y - 9, mod.ProjectileType("MechMageProj"), projectile.damage, 1f, projectile.owner, 0f);

                Projectile.NewProjectile(projectile.Center.X - 100, projectile.Center.Y, value1.X + 9, value1.Y, mod.ProjectileType("MechMageProj"), projectile.damage, 1f, projectile.owner, 0f);

                Projectile.NewProjectile(projectile.Center.X + 100, projectile.Center.Y, value1.X - 9, value1.Y, mod.ProjectileType("MechMageProj"), projectile.damage, 1f, projectile.owner, 0f);
                Vector2 position = projectile.Center;
                Main.PlaySound(SoundID.Item12, (int)position.X, (int)position.Y);
                projectile.Kill();
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cone");
        }
    }
}
