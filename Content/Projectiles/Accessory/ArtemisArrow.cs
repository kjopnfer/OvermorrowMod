using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Accessory
{
    public class ArtemisArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.Magic + "MarbleBook/MarbleArrow";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.penetrate = 30;
            projectile.timeLeft = 240;
            projectile.light = 0.75f;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
            aiType = ProjectileID.BoneArrow;
            projectile.aiStyle = 1;
        }

        public override void AI()
        {
            Dust dust = Dust.NewDustPerfect(projectile.Center, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi / 2;
        }
    }
}