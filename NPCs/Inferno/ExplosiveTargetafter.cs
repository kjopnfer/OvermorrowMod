using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class ExplosiveTargetafter : ModProjectile
    {

        public override bool CanDamage() => false;

        //private int timer = 0;
        //int Random = Main.rand.Next(0, 111);
        //int defence = Main.rand.Next(0, 60);
        //int zoppler = Main.rand.Next(10, 200);
        //int attckter = Main.rand.Next(10, 5000);
        //int scaleddd = Main.rand.Next(1, 5);

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.light = 0.5f;
            projectile.alpha = 0;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 126;
        }

        public override void AI()
        {
            projectile.scale = projectile.scale + 0.01f;
            projectile.alpha = projectile.alpha + 2;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cone");
        }
    }
}
