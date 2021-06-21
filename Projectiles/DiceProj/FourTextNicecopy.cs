using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.DiceProj
{
    public class FourTextNicecopy : ModProjectile
    {

        //private int timer = 0;
        //int Random = Main.rand.Next(0, 111);
        //int defence = Main.rand.Next(0, 60);
        //int zoppler = Main.rand.Next(10, 200);
        //int attckter = Main.rand.Next(10, 5000);
        //int scaleddd = Main.rand.Next(1, 5);

        public override void SetDefaults()
        {
            projectile.width = 31;
            projectile.height = 44;
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
            projectile.scale = projectile.scale + 0.02f;
            projectile.alpha = projectile.alpha + 1;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cone");
        }
    }
}
