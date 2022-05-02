using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Projectiles.Accessory;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.BloodStaff
{
    public class BloodSplit : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Splitting Blood Ball");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 420;
            Projectile.alpha = 255;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0f, 0f);
            Projectile.ai[0]++;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y, 50, default(Color), 2.5f);
                Main.dust[num1110].position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int Projectiles = Main.rand.Next(3, 5);
            for (int i = 0; i < Projectiles; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, new Vector2(7).RotatedBy(MathHelper.ToRadians((360 / Projectiles) * i + i)), ModContent.ProjectileType<BouncingBlood>(), Projectile.damage / 2, 2, Main.myPlayer);
            }
            Projectile.Kill();
        }
    }
}