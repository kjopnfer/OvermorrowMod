using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class SpiderFire : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.FlamingScythe);
            aiType = ProjectileID.FlamingScythe;
            projectile.friendly = false;
            projectile.light = 3f;
            projectile.hostile = true;
            projectile.width = 50;
            projectile.height = 50;
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.CultistBossFireBall;
    }
}