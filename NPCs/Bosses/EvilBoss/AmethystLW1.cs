using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class AmethystLW1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evil Ray");
            Main.projFrames[base.projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.scale = 2;
            projectile.timeLeft = 199;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile parentProjectile = Main.projectile[(int)projectile.ai[0]];
            projectile.Center = parentProjectile.Center;
            projectile.position.X = parentProjectile.Center.X - 20;
            projectile.position.Y = parentProjectile.Center.Y;



            if (projectile.timeLeft == 75)
            {
                projectile.frame = 1;
            }

            if (projectile.timeLeft == 50)
            {
                projectile.frame = 2;
            }

            if (projectile.timeLeft == 25)
            {
                projectile.frame = 3;
            }
        }
    }
}
