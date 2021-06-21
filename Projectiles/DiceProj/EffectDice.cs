using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace OvermorrowMod.Projectiles.DiceProj
{
    public class EffectDice : ModProjectile
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
            Projectile.NewProjectile(projectile.position.X, projectile.position.Y, value1.X = 0, value1.Y = 0, mod.ProjectileType("FiveTextNicecopy"), projectile.damage - 10, 1f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.position.X - 1000, projectile.position.Y - 1000, value1.X = 0, value1.Y = 0, mod.ProjectileType("DiceIchorFlame"), projectile.damage, 1f, projectile.owner, 0f);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dice");
            Main.projFrames[base.projectile.type] = 6;
        }
    }
}
