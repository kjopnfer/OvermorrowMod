using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class IorichRune : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich's Prominence");
        }

        public override void SetDefaults()
        {
            projectile.width = 408; 
            projectile.height = 408;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.alpha = 0;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 900;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.65f, 0f);

            projectile.scale = projectile.ai[0];
            
            projectile.rotation = projectile.ai[0] < 1 ? projectile.rotation - 0.02f : projectile.rotation + 0.02f;
            //projectile.alpha = projectile.alpha + 5;
        }
    }
}
