using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class WaterBullet : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Bullet");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 2;
            projectile.timeLeft = 400;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            projectile.ranged = true;
        }

        public override void AI()
        {
            int num949 = Main.rand.Next(5, 10);
            for (int num948 = 0; num948 < num949; num948++)
            {
                int num947 = Dust.NewDust(projectile.Center, 0, 0, DustID.Water_Snow, 0f, 0f, 100, default(Color), 0.5f);
                Dust dust24 = Main.dust[num947];
                dust24.velocity *= 1.6f;
                Dust expr_9DCA_cp_0 = Main.dust[num947];
                expr_9DCA_cp_0.velocity.Y = expr_9DCA_cp_0.velocity.Y - 1f;
                dust24 = Main.dust[num947];
                dust24.position -= Vector2.One * 4f;
                Main.dust[num947].position = Vector2.Lerp(Main.dust[num947].position, projectile.Center, 0.5f);
                Main.dust[num947].noGravity = true;
            }

            int num434 = Dust.NewDust(projectile.Center, 0, 0, DustID.Water_Snow, 0f, 0f, 100);
            Main.dust[num434].noLight = true;
            Main.dust[num434].noGravity = true;
            Main.dust[num434].velocity = projectile.velocity;
            Main.dust[num434].position -= Vector2.One * 4f;
            Main.dust[num434].scale = 0.8f;
            if (++projectile.frameCounter >= 9)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 5)
                {
                    projectile.frame = 0;
                }
            }

            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;

                Vector2 value30 = Vector2.Normalize(projectile.velocity);
                int num406 = Main.rand.Next(5, 10);
                for (int num405 = 0; num405 < num406; num405++)
                {
                    int num404 = Dust.NewDust(projectile.Center, 0, 0, DustID.Water_Snow, 0f, 0f, 100);
                    Dust expr_3DD4_cp_0 = Main.dust[num404];
                    expr_3DD4_cp_0.velocity.Y = expr_3DD4_cp_0.velocity.Y - 1f;
                    Main.dust[num404].velocity += value30 * 2f;
                    Main.dust[num404].position -= Vector2.One * 4f;
                    Main.dust[num404].noGravity = true;
                }
            }
        }
    

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Makes dust projectiled on tile
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);

            return true;
        }
    }
}