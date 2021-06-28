using Terraria.ModLoader;
using Terraria.ID;

namespace OvermorrowMod.NPCs.Bosses.Apollus
{
    public class ApollusGravityArrow : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.BoneArrow;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }
        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 70;
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 1800;
            projectile.penetrate = -1;
            aiType = ProjectileID.BoneArrow;
            projectile.aiStyle = 1;
        }
    }
}
