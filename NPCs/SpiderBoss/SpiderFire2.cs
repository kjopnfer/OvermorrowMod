using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class SpiderFire2 : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.FlamingScythe);
            aiType = ProjectileID.FlamingScythe;
            projectile.hostile = true;
            projectile.width = 18;
            projectile.light = 3f;
            projectile.height = 18;
        }
    }
}