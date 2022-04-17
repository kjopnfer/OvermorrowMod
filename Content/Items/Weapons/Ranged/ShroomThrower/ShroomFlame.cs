using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.ShroomThrower
{
    public class ShroomFlame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shroomfire");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 18;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {

            Projectile.rotation += 0.3f;

            Projectile.ai[0]++;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width + Main.rand.Next(-25, 26), Projectile.height + Main.rand.Next(-25, 26), DustID.GlowingMushroom, Projectile.velocity.X, Projectile.velocity.Y, 75, new Color(), 2.7f);
                Main.dust[num1110].position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(7) == 3)
            {
                target.AddBuff(ModContent.BuffType<FungalInfection>(), 400);
            }
            Projectile.damage /= 2;
        }
    }
}
