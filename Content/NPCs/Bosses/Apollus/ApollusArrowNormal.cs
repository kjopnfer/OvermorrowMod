using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Apollus
{
    public class ApollusArrowNormal : ModProjectile
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
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 30;
            Projectile.timeLeft = 240;
            Projectile.light = 0.75f;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            AIType = ProjectileID.WoodenArrowHostile;
        }
        public override void AI()
        {
            Dust dust = Dust.NewDustPerfect(Projectile.Center, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0);
            Projectile.rotation = Projectile.velocity.RotatedBy(MathHelper.ToRadians(90)).ToRotation();
        }
    }
}
