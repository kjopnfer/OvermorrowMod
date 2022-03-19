using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class StormTalonSparks : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Sparks");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 65;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.melee = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);
            if (projectile.localAI[0]++ >= 2f)
            {
                Dust.NewDustPerfect(projectile.Center, 206, null, 0, default, 1.5f);
                projectile.localAI[0] = 0f;
            }
        }
    }
}