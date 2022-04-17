using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.StormTalon
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
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 65;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0, 0.5f, 0.5f);
            if (Projectile.localAI[0]++ >= 2f)
            {
                Dust.NewDustPerfect(Projectile.Center, 206, null, 0, default, 1.5f);
                Projectile.localAI[0] = 0f;
            }
        }
    }
}