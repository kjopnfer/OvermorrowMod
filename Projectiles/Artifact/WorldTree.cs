using System;
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
            drawOriginOffsetY = -168;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.66f, 0f);

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
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = mod.GetTexture("Projectiles/Artifact/WorldTree_Glowmask");
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f + 5f,
                    projectile.position.Y - Main.screenPosition.Y + projectile.height - texture.Height * 0.5f - 32f
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                projectile.rotation,
                texture.Size() * 0.5f,
                projectile.scale,
                SpriteEffects.None,
                0f
            );
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}
