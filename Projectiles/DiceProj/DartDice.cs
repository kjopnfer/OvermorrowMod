using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace OvermorrowMod.Projectiles.DiceProj
{
    public class DartDice : ModProjectile
    {
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
            Projectile.NewProjectile(projectile.position.X, projectile.position.Y, value1.X = 0, value1.Y = 0, mod.ProjectileType("ThreeTextNicecopy"), projectile.damage - 10, 1f, projectile.owner, 0f);

            Vector2 position = projectile.Center;
            Vector2 targetPosition = Main.player[projectile.owner].Center;
            Vector2 direction = targetPosition - position;
            direction.Normalize();
            float speed = 9f;
            int type = ProjectileID.PoisonDart;
            int damage = projectile.damage + 25;
            Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dice");
            Main.projFrames[base.projectile.type] = 6;
        }
    }
}
