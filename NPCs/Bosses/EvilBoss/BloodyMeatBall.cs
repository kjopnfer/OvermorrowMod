using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class BloodyMeatBall : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.TheMeatball;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.FlamingScythe);
            aiType = ProjectileID.FlamingScythe;
            projectile.tileCollide = false;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 50;
            projectile.width = 34;
            projectile.height = 34;
            projectile.penetrate = -1;
        }
    }
}