using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.DripplerBoss
{
    public class BloodyBallGravity : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Ball");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 540;
            Projectile.tileCollide = false;
            AIType = ProjectileID.PainterPaintball;
            Projectile.alpha = 255;
            Projectile.aiStyle = 1;
        }
        public override void AI()
        {
            if (Projectile.ai[1] == 10)
            {
                Projectile.friendly = true;
                Projectile.hostile = false;
            }

            Lighting.AddLight(Projectile.Center, 1f, 0f, 0f);
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
    }
}