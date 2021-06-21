using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.DiceProj
{
    public class BoomDiceShow : ModProjectile
    {
        private int timer = 0;
        readonly int xpos = Main.rand.Next(-100, 100);
        readonly int ypos = Main.rand.Next(-100, 100);
        readonly int xpos2 = Main.rand.Next(-100, 100);
        readonly int ypos2 = Main.rand.Next(-100, 100);
        readonly int xpos3 = Main.rand.Next(-100, 100);
        readonly int ypos3 = Main.rand.Next(-100, 100);
        readonly int xpos4 = Main.rand.Next(-100, 100);
        readonly int ypos4 = Main.rand.Next(-100, 100);
        readonly int xpos5 = Main.rand.Next(-100, 100);
        readonly int ypos5 = Main.rand.Next(-100, 100);
        readonly int xpos6 = Main.rand.Next(-100, 100);
        readonly int ypos6 = Main.rand.Next(-100, 100);
        readonly int xpos7 = Main.rand.Next(-100, 100);
        readonly int ypos7 = Main.rand.Next(-100, 100);
        readonly int xpos8 = Main.rand.Next(-100, 100);
        readonly int ypos8 = Main.rand.Next(-100, 100);
        readonly int xpos9 = Main.rand.Next(-100, 100);
        readonly int ypos9 = Main.rand.Next(-100, 100);
        readonly int xpos10 = Main.rand.Next(-100, 100);
        readonly int ypos10 = Main.rand.Next(-100, 100);
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.light = 0.5f;
            projectile.alpha = 0;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 255;
        }
        public override void AI()
        {
            timer++;
            if (timer == 20)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.position.X + xpos, projectile.position.Y + ypos, value1.X = 0, value1.Y = 0, mod.ProjectileType("BoomDiecProj"), projectile.damage + 100, 1f, projectile.owner, 0f);
            }

            if (timer == 40)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.position.X + xpos2, projectile.position.Y + ypos2, value1.X = 0, value1.Y = 0, mod.ProjectileType("BoomDiecProj"), projectile.damage + 100, 1f, projectile.owner, 0f);
            }

            if (timer == 60)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.position.X + xpos3, projectile.position.Y + ypos3, value1.X = 0, value1.Y = 0, mod.ProjectileType("BoomDiecProj"), projectile.damage + 100, 1f, projectile.owner, 0f);
            }

            if (timer == 80)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.position.X + xpos4, projectile.position.Y + ypos4, value1.X = 0, value1.Y = 0, mod.ProjectileType("BoomDiecProj"), projectile.damage + 100, 1f, projectile.owner, 0f);
            }

            if (timer == 100)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.position.X + xpos5, projectile.position.Y + ypos5, value1.X = 0, value1.Y = 0, mod.ProjectileType("BoomDiecProj"), projectile.damage + 100, 1f, projectile.owner, 0f);
            }

            if (timer == 120)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.position.X + xpos6, projectile.position.Y + ypos6, value1.X = 0, value1.Y = 0, mod.ProjectileType("BoomDiecProj"), projectile.damage + 100, 1f, projectile.owner, 0f);
            }

            if (timer == 140)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.position.X + xpos7, projectile.position.Y + ypos7, value1.X = 0, value1.Y = 0, mod.ProjectileType("BoomDiecProj"), projectile.damage + 100, 1f, projectile.owner, 0f);
            }

            if (timer == 160)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.position.X + xpos8, projectile.position.Y + ypos8, value1.X = 0, value1.Y = 0, mod.ProjectileType("BoomDiecProj"), projectile.damage + 100, 1f, projectile.owner, 0f);
            }

            if (timer == 180)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.position.X + xpos9, projectile.position.Y + ypos9, value1.X = 0, value1.Y = 0, mod.ProjectileType("BoomDiecProj"), projectile.damage + 100, 1f, projectile.owner, 0f);
            }

            if (timer == 200)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.position.X + xpos10, projectile.position.Y + ypos10, value1.X = 0, value1.Y = 0, mod.ProjectileType("BoomDiecProj"), projectile.damage + 100, 1f, projectile.owner, 0f);
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cone");
        }
    }
}
