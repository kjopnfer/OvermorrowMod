using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class Eggtop : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Glowstick);
            aiType = ProjectileID.Glowstick;
            projectile.scale = 2.4f;
            projectile.width = 18;
            projectile.height = 18;
        }
        public override void AI()
        {
            projectile.alpha = projectile.alpha + 1;
            if (projectile.alpha == 255)
            {
                projectile.timeLeft = 0;
            }
        }
    }
}