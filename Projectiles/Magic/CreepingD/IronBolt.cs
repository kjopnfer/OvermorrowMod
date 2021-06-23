using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Magic.CreepingD
{
    public class IronBolt : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.SlushBall;

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 150;
            projectile.width = 14;
            projectile.height = 14;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.localAI[0] += 1f;

            float num116 = 16f;
            for (int num117 = 0; (float)num117 < 16; num117++)
            {
                Vector2 spinningpoint7 = Vector2.UnitX * 0f;
                spinningpoint7 += -Vector2.UnitY.RotatedBy((float)num117 * ((float)Math.PI * 2f / num116)) * new Vector2(1f, 4f);
                spinningpoint7 = spinningpoint7.RotatedBy(projectile.velocity.ToRotation());
                Vector2 position = projectile.Center;
                Dust dust = Terraria.Dust.NewDustPerfect(position, 8, new Vector2(0f, 0f), 0, default, 0.75f);
                dust.noLight = true;
                dust.noGravity = true;
                dust.position = projectile.Center + spinningpoint7;
            }
        }
    }
}