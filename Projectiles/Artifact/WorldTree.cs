using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Artifact
{
    public class WorldTree : ModProjectile
    {
        private bool isActive = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("World Tree");
            Main.projFrames[projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.tileCollide = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 18000; // 5 minutes

            drawOffsetX = -55;
            drawOriginOffsetY = -188;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.66f, 0f);

            Player player = Main.player[projectile.owner];
            if (player.dead || !player.active)
            {
                return;
            }

            // Get the ground beneath the projectile
            Vector2 projectilePos = new Vector2(projectile.position.X / 16, projectile.position.Y / 16);
            Tile tile = Framing.GetTileSafely((int)projectilePos.X, (int)projectilePos.Y);
            while (!tile.active() || tile.type == TileID.Trees)
            {
                projectilePos.Y += 1;
                tile = Framing.GetTileSafely((int)projectilePos.X, (int)projectilePos.Y);
            }

            projectile.position = projectilePos * 16;

            projectile.ai[0] += 1;
            if (projectile.ai[1] < 200) // The radius
            {
                projectile.ai[1] += 15;
            }
            else
            {
                isActive = true;
            }

            for (int i = 0; i < 36; i++)
            {
                Vector2 dustPos = (projectile.Center - new Vector2(0, 68)) + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.ai[0]));
                Dust dust = Main.dust[Terraria.Dust.NewDust(dustPos, 15, 15, 107, 0f, 0f, 0, default, 1f)];
                dust.noGravity = true;
            }

            if (isActive)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    float distance = Vector2.Distance((projectile.Center - new Vector2(0, 68)), Main.player[i].Center);
                    if (distance <= 200)
                    {
                        Main.player[i].AddBuff(ModContent.BuffType<TreeBuff>(), 60);
                    }
                }
            }

            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;

            Texture2D texture = mod.GetTexture("Projectiles/Artifact/WorldTree_Glow");
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f + 6,
                    projectile.position.Y - Main.screenPosition.Y + projectile.height - drawRectangle.Height * 0.5f - 32
                ),
                drawRectangle,
                Color.White,
                projectile.rotation,
                new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                projectile.scale,
                projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}
