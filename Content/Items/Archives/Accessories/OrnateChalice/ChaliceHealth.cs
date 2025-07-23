using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Interfaces;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class ChaliceHealth : ModProjectile, IOutlineEntity
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;

        public bool ShouldDrawOutline => true;
        public Color OutlineColor => Color.Teal;
        public bool UseFillColor => true;
        public Texture2D FillTexture => ModContent.Request<Texture2D>(AssetDirectory.MapBackgrounds + "GrandArchives").Value;
        public Color? FillColor => Color.Black;
        public Action<SpriteBatch, GraphicsDevice, int, int, Entity> CustomDrawFunction => DrawStaticBackgroundFill;

        public override void SetStaticDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.timeLeft = ModUtils.SecondsToTicks(5);
            Projectile.tileCollide = false;
        }

        private float baseScale;
        public override void OnSpawn(IEntitySource source)
        {
            //baseScale = Main.rand.NextFloat(f, 2f);
            baseScale = 12f;
            Projectile.scale = baseScale;
        }

        public override void AI()
        {
            Projectile.rotation -= 0.02f;
            //float pulsateSpeed = 0.08f;
            //float pulsateAmount = 0.3f;

            //float pulsate = 1f + (float)Math.Sin(Projectile.timeLeft * pulsateSpeed) * pulsateAmount;
            //Projectile.scale = baseScale * pulsate;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_06").Value;
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() / 2f, 0.05f, SpriteEffects.None, 0);

            return false;
        }

        private void DrawStaticBackgroundFill(SpriteBatch spriteBatch, GraphicsDevice gD, int screenWidth, int screenHeight, Entity entity)
        {
            Texture2D backgroundTexture = ModContent.Request<Texture2D>(AssetDirectory.MapBackgrounds + "GrandArchives").Value;

            if (backgroundTexture == null) return;

            // Calculate how many tiles we need to cover the screen
            int tilesX = (screenWidth / backgroundTexture.Width) + 2;
            int tilesY = (screenHeight / backgroundTexture.Height) + 2;

            // Use screen position to offset the background pattern
            Vector2 offset = new Vector2(
                Main.screenPosition.X % backgroundTexture.Width,
                Main.screenPosition.Y % backgroundTexture.Height
            );

            for (int x = -1; x < tilesX; x++)
            {
                for (int y = -1; y < tilesY; y++)
                {
                    Vector2 position = new Vector2(x * backgroundTexture.Width, y * backgroundTexture.Height) - offset;
                    spriteBatch.Draw(backgroundTexture, position, Color.White);
                }
            }

            int xFrame = 0;
            float animationSpeed = 12f;
            int totalFrames = 9;
            int yFrame = (int)((Main.GameUpdateCount / 6) % totalFrames);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "ArchiveRat").Value;

            // Random drift calculation
            float time = Main.GameUpdateCount * 0.02f + entity.whoAmI * 0.5f; // Use entity's whoAmI for unique offset
            Vector2 driftOffset = new Vector2(
                (float)Math.Sin(time) * 120f,                    // Horizontal drift with 8 pixel amplitude
                (float)Math.Cos(time * 0.7f) * 120f              // Vertical drift with 6 pixel amplitude, different frequency
            );

            // Add some random jitter every 60 ticks
            if (Main.GameUpdateCount % 60 == 0)
            {
                Vector2 randomJitter = Main.rand.NextVector2Circular(3f, 3f);
                driftOffset += randomJitter;
            }

            var frame = new Rectangle(xFrame * (texture.Width / 10), yFrame * (texture.Height / 9), texture.Width / 10, texture.Height / 9);
            spriteBatch.Draw(texture, entity.Center - Main.screenPosition + driftOffset, frame, Color.White, 0f, frame.Size() / 2, 1f, SpriteEffects.None, 0);
        }
    }
}