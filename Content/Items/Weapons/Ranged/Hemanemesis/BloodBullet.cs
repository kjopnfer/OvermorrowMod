using Microsoft.Xna.Framework;
using OvermorrowMod.Content.NPCs.Bosses.DripplerBoss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Hemanemesis
{
    public class BloodBullet : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 20;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 240;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0f, 0f);
            Projectile.damage = 17;
            Projectile.ai[0]++;
            Color Bloodc = Color.Red;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.HeartCrystal, Projectile.velocity.X, Projectile.velocity.Y, 50, Bloodc, 1.6f);
                Main.dust[num1110].position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.life < Projectile.damage - (target.defense * 1.1))
            {
                for (int i = 0; i < Main.rand.Next(3, 5); i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ProjectileType<BloodyBallGravity>(), 12, 10f, Main.myPlayer, 0, 10);
                    }
                }
            }
        }
    }
}