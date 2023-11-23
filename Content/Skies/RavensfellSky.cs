using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Skies
{
    public class RavensfellSky : CustomSky
    {
        private bool isActive = false;

        private const float Scale = 2f;
        private const float ScreenParralaxMultiplier = 0.4f;


        public override void Update(GameTime gameTime)
        {
            if (isActive) { }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            // Morning
            //spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), Color.Black);
            //Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Cosmic").Value;
            //Rectangle rect = new(0, 0, Main.screenWidth, Main.screenHeight);
            //spriteBatch.Draw(texture, rect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);


            // Day

            // Sunset

            // Night

            #region Manual Background
            // Small worlds, default draw height.
            int biomeHeight = ((int)Main.worldSurface + (int)Main.worldSurface) / 2;
            //int biomeHeight = (World.AstralBiome.YStart + (int)Main.worldSurface) / 2;

            // Medium worlds.
            if (Main.maxTilesX >= 6400 && Main.maxTilesX < 8400)
                biomeHeight = ((int)Main.worldSurface + (int)Main.worldSurface) / 4;
            //biomeHeight = (World.AstralBiome.YStart + (int)Main.worldSurface) / 4;

            // Large worlds (and anything bigger).
            //if (Main.maxTilesX >= 8400)
            //    biomeHeight = (World.AstralBiome.YStart + (int)Main.worldSurface) / 140;*/
            Main.NewText(Main.time);
            float width = Main.screenWidth / 2f;
            float height = Main.screenHeight / 2f;
            Color textureColor = Color.White;
            Vector2 origin = new Vector2(0f, biomeHeight);
            if (maxDepth >= 9f && minDepth < 9f)
            {
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), Color.Black);
                Texture2D morning = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/morning").Value;
                Texture2D day = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/day").Value;
                Texture2D sunset = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/sunset").Value;
                Texture2D night = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/night").Value;

                int x = (int)(Main.screenPosition.X * 0.4f * ScreenParralaxMultiplier);
                x %= (int)(morning.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.4f * ScreenParralaxMultiplier);
                y -= 1380; // 1000
                Vector2 position = morning.Size() / 2f * Scale;

                // 54000
                float nightAlpha = MathHelper.Lerp(1f, 0, (float)Utils.Clamp(Main.time / 13500, 0, 1f));
                float morningAlpha = MathHelper.Lerp(1f, 0, (float)Utils.Clamp((Main.time - 13500) / 13500, 0, 1f));
                float dayAlpha = MathHelper.Lerp(1f, 0, (float)Utils.Clamp((Main.time - 13500 * 2) / 13500, 0, 1f));
                float sunsetAlpha = MathHelper.Lerp(1f, 0, (float)Utils.Clamp((Main.time - 13500 * 3) / 13500, 0, 1f));

                //spriteBatch.Reload(BlendState.Additive);
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + morning.Width * k * Scale, height - y);

                    if (Main.dayTime)
                    {
                        spriteBatch.Draw(night, pos - position, null, Color.White, 0f, origin, Scale * 2, SpriteEffects.None, 0f);
                        spriteBatch.Draw(sunset, pos - position, null, new Color(227, 167, 154, 150) * sunsetAlpha, 0f, origin, Scale * 2, SpriteEffects.None, 0f);
                        spriteBatch.Draw(day, pos - position, null, new Color(227, 167, 154, 150) * dayAlpha, 0f, origin, Scale * 2, SpriteEffects.None, 0f);
                        spriteBatch.Draw(morning, pos - position, null, new Color(227, 167, 154, 150) * morningAlpha, 0f, origin, Scale * 2, SpriteEffects.None, 0f);
                        spriteBatch.Draw(night, pos - position, null, Color.White * nightAlpha, 0f, origin, Scale * 2, SpriteEffects.None, 0f);
                    }
                    else
                        spriteBatch.Draw(night, pos - position, null, Color.White, 0f, origin, Scale * 2, SpriteEffects.None, 0f);
                }


                //spriteBatch.Reload(BlendState.AlphaBlend);
            }

            /*if (maxDepth >= 8f && minDepth < 8f)
            {
                Texture2D farTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/forest_test_close", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int x = (int)(Main.screenPosition.X * 0.5f * ScreenParralaxMultiplier);
                x %= (int)(farTexture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.45f * ScreenParralaxMultiplier);
                y -= 1520; // 1000
                Vector2 position = farTexture.Size() / 2f * Scale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + farTexture.Width * k * Scale, height - y);
                    spriteBatch.Draw(farTexture, pos - position, null, textureColor, 0f, origin, Scale, SpriteEffects.None, 0f);
                }
            }

            if (maxDepth >= 6f && minDepth < 6f)
            {
                Texture2D closeTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/forest_test_close", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int x = (int)(Main.screenPosition.X * 0.9f * ScreenParralaxMultiplier);
                x %= (int)(closeTexture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.55f * ScreenParralaxMultiplier);
                y -= 1880; // 1000
                Vector2 position = closeTexture.Size() / 2f * Scale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + closeTexture.Width * k * Scale, height - y);
                    spriteBatch.Draw(closeTexture, pos - position, null, textureColor, 0f, origin, Scale, SpriteEffects.None, 0f);
                }
            }*/

            #endregion
        }

        public override bool IsActive()
        {
            return isActive;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }
    }
}