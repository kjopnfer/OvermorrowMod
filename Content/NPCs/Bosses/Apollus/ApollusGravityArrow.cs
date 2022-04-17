using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Apollus
{
    public class ApollusGravityArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.Magic + "MarbleBook/MarbleArrow";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 1800;
            Projectile.penetrate = -1;
            AIType = ProjectileID.BoneArrow;
            Projectile.aiStyle = 1;
        }
        public override void AI()
        {
            Dust dust = Dust.NewDustPerfect(Projectile.Center, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0);

        }
    }
}
