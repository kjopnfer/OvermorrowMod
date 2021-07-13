using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class BloodyBallGravity : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Ball");
        }
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 540;
            projectile.tileCollide = false;
            aiType = ProjectileID.PainterPaintball;
            projectile.alpha = 255;
            projectile.aiStyle = 1;
        }
        public override void AI()
        {
            if (projectile.ai[1] == 10)
            {
                projectile.friendly = true;
                projectile.hostile = false;
            }

            Lighting.AddLight(projectile.Center, 1f, 0f, 0f);
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
    }
}