using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class ShotSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 18;
            projectile.timeLeft = 100;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Dust dust;
            // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
            dust = Terraria.Dust.NewDustPerfect(projectile.Center, 1, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            dust.noGravity = true;

            float num116 = 16f;
            for (int num117 = 0; (float)num117 < 16; num117++)
            {
                Vector2 spinningpoint7 = Vector2.UnitX * 0f;
                spinningpoint7 += -Vector2.UnitY.RotatedBy((float)num117 * ((float)Math.PI * 2f / num116)) * new Vector2(1f, 4f);
                spinningpoint7 = spinningpoint7.RotatedBy(projectile.velocity.ToRotation());
                Vector2 position = projectile.Center;
                Dust dust2 = Terraria.Dust.NewDustPerfect(position, 1, new Vector2(0f, 0f), 0, default, 1f);
                dust2.noLight = true;
                dust2.noGravity = true;
                dust2.position = projectile.Center + spinningpoint7;
            }

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            projectile.ai[0]++;
            if (projectile.ai[0] > 20)
            {
                projectile.tileCollide = true;
            }
        }
    }
}
