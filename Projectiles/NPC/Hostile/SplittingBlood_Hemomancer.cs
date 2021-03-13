using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class SplittingBlood_Hemomancer : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Splitting Blood Ball");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 80;
            projectile.alpha = 255;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 1f, 0f, 0f);
            projectile.ai[0]++;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 5, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 2.5f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }
        }

        public override void Kill(int timeLeft)
        {
            // I'm lazy
            int projectiles = Main.rand.Next(4, 7);
            projectile.netUpdate = true;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < projectiles; i++)
                {
                    Projectile.NewProjectile(projectile.Center, new Vector2(7).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<BloodyBall>(), projectile.damage / 2, 2, Main.myPlayer);
                }
            }
        }
    }
}