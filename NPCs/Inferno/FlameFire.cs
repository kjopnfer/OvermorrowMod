using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class FlameFire : ModProjectile
    {
        private int Flamevelo = 0;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Flames);
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 60;
        }
        public override void AI()
        {
            if (Flamevelo == 0)
            {
                projectile.velocity.X *= 2f;
                projectile.velocity.Y *= 2f;
                Flamevelo++;
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flames");
        }
    }
}