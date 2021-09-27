using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider
{
    public class JungleBoom : ModProjectile
    {

        int Anitimer = 0;

        public override void SetDefaults()
        {
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.width = 156;
            projectile.height = 200;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.light = 6f;
            projectile.timeLeft = 24; //The amount of time the projectile is alive for
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jungle Boom");
            Main.projFrames[base.projectile.type] = 12;
        }

        public override void AI()
        {

            Anitimer++;
            if (Anitimer == 1)
            {
                projectile.frame += 1;
                Anitimer = 0;
            }
        }
    }
}
