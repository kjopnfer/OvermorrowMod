using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace OvermorrowMod.Projectiles.DiceProj
{
    public class HealDice : ModProjectile
    {

        readonly int xposrandy = Main.rand.Next(-25, 25);
        int xposrandy2 = Main.rand.Next(-25, 25);
        int xposrandy3 = Main.rand.Next(-25, 25);
        int xposrandy4 = Main.rand.Next(-25, 25);
        int xposrandy5 = Main.rand.Next(-25, 25);
        int xposrandy6 = Main.rand.Next(-25, 25);
        int xposrandy7 = Main.rand.Next(-25, 25);
        int xposrandy8 = Main.rand.Next(-25, 25);
        int xposrandy9 = Main.rand.Next(-25, 25);
        int xposrandy10 = Main.rand.Next(-25, 25);
        public override void SetDefaults()
        {
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.width = 32;
            projectile.height = 31;
            projectile.CloneDefaults(ProjectileID.SpikyBall);
            projectile.timeLeft = 150; //The amount of time the projectile is alive for
        }
        public override void AI()
        {

            projectile.velocity.Y = projectile.velocity.Y + 0.00001f;
            if (++projectile.frameCounter >= 7)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 6)
                {
                    projectile.frame = 0;
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(projectile.position.X, projectile.position.Y, value1.X = 0, value1.Y = 0, mod.ProjectileType("SixTextNicecopy"), projectile.damage - 10, 1f, projectile.owner, 0f);
            Item.NewItem((int)projectile.position.X + xposrandy, (int)projectile.position.Y, projectile.width, projectile.height, 58, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy, (int)projectile.position.Y, projectile.width, projectile.height, 184, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy2, (int)projectile.position.Y, projectile.width, projectile.height, 58, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy2, (int)projectile.position.Y, projectile.width, projectile.height, 184, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy3, (int)projectile.position.Y, projectile.width, projectile.height, 58, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy3, (int)projectile.position.Y, projectile.width, projectile.height, 184, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy4, (int)projectile.position.Y, projectile.width, projectile.height, 58, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy4, (int)projectile.position.Y, projectile.width, projectile.height, 184, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy5, (int)projectile.position.Y, projectile.width, projectile.height, 58, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy5, (int)projectile.position.Y, projectile.width, projectile.height, 184, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy6, (int)projectile.position.Y, projectile.width, projectile.height, 58, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy6, (int)projectile.position.Y, projectile.width, projectile.height, 184, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy7, (int)projectile.position.Y, projectile.width, projectile.height, 58, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy7, (int)projectile.position.Y, projectile.width, projectile.height, 184, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy8, (int)projectile.position.Y, projectile.width, projectile.height, 58, 4, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy8, (int)projectile.position.Y, projectile.width, projectile.height, 184, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy9, (int)projectile.position.Y, projectile.width, projectile.height, 58, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy9, (int)projectile.position.Y, projectile.width, projectile.height, 184, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy10, (int)projectile.position.Y, projectile.width, projectile.height, 58, 2, false, 0, false, false);
            Item.NewItem((int)projectile.position.X + xposrandy10, (int)projectile.position.Y, projectile.width, projectile.height, 184, 2, false, 0, false, false);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dice");
            Main.projFrames[base.projectile.type] = 6;
        }
    }
}
