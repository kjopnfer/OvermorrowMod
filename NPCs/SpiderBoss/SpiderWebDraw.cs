using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class SpiderWebDraw : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 700;
        }
    }
}