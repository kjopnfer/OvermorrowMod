using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Apollus
{
    public class ApollusArrowNormal : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Magic/MarbleArrow";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = 30;
            projectile.timeLeft = 240;
            projectile.light = 0.75f;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
            aiType = ProjectileID.WoodenArrowHostile;
        }
        public override void AI()
        {
            Dust dust = Dust.NewDustPerfect(projectile.Center, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0);
            projectile.rotation = projectile.velocity.RotatedBy(MathHelper.ToRadians(90)).ToRotation();
        }
    }
}
