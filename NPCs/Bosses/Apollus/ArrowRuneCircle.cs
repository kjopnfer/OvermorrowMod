using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.NPCs.Bosses.Apollus
{
    public class ArrowRuneCircle : ModProjectile
    {
        public override string Texture => "OvermorrowMod/WardenClass/RuneCircles/temp2";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Circle");
        }
        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 70;
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
        }
        public int timer = 0;
        public bool kill = false;
        public override void AI()
        {
            if (++timer % 45 == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ProjectileType<ApollusGravityArrow>(), 2, 10f, Main.myPlayer);
                }
            }
            if (kill == true)
            {
                projectile.Kill();
            }
        }
    }
}
