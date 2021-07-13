using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class BloodBullet : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 20;
            projectile.penetrate = -1;//2;//1;
            projectile.timeLeft = 240;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.alpha = 255;
        }
        public override void AI() 
		{
            Lighting.AddLight(projectile.Center, 1f, 0f, 0f);
            projectile.damage = 17;
            projectile.ai[0]++;
            Color Bloodc = Color.Red;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 12, projectile.velocity.X, projectile.velocity.Y, 50, Bloodc, 1.6f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.life < projectile.damage - (target.defense * 1.1))
            {
                for (int i = 0; i < Main.rand.Next(3, 5); i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ProjectileType<BloodyBallGravity>(), 12, 10f, Main.myPlayer, 0, 10);
                    }
                }
            }
        }
    }
}