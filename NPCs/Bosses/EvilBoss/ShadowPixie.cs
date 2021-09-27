using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class ShadowPixie : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowFlame;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.ShadowFlame);
            aiType = ProjectileID.ShadowFlame;
            projectile.friendly = false;
            projectile.hostile = true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Tentacle");
        }
    }
}