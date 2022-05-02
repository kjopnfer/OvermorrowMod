using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GraniteBook
{
    public class GraniteSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Spike");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            float num116 = 16f;
            for (int num117 = 0; (float)num117 < 16; num117++)
            {
                Vector2 spinningpoint7 = Vector2.UnitX * 0f;
                spinningpoint7 += -Vector2.UnitY.RotatedBy((float)num117 * ((float)Math.PI * 2f / num116)) * new Vector2(1f, 4f);
                spinningpoint7 = spinningpoint7.RotatedBy(Projectile.velocity.ToRotation());
                Vector2 position = Projectile.Center;
                Dust dust = Terraria.Dust.NewDustPerfect(position, 63, new Vector2(0f, 0f), 0, new Color(0, 242, 255), 1f);
                dust.noLight = true;
                dust.noGravity = true;
                dust.position = Projectile.Center + spinningpoint7;
            }

        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Main.player[Projectile.owner].GetArmorPenetration(DamageClass.Generic) += 5;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[Projectile.owner].GetArmorPenetration(DamageClass.Generic) -= 5;
        }
    }
}