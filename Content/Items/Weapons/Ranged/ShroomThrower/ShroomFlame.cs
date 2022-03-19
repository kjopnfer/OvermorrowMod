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
            projectile.width = 24;
            projectile.height = 24;
            projectile.timeLeft = 18;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {

            projectile.rotation += 0.3f;

            projectile.ai[0]++;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width + Main.rand.Next(-25, 26), projectile.height + Main.rand.Next(-25, 26), DustID.GlowingMushroom, projectile.velocity.X, projectile.velocity.Y, 75, new Color(), 2.7f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
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
            projectile.damage /= 2;
        }
    }
}
