using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class SpiderBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.HeatRay);
            aiType = ProjectileID.HeatRay;
            projectile.friendly = false;
            projectile.hostile = true;
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.HeatRay;
    }
}