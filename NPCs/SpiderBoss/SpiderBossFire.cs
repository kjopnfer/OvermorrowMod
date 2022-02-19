using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class SpiderBossFire : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Flames);
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.light = 3f;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell Fire");
        }
    }
}