using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Skies
{
    public class RavensfellSky : CustomSky
    {
        private bool isActive = false;

        private const float Scale = 2f;
        private const float ScreenParralaxMultiplier = 0.4f;
        float starOpacity = 1f;

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
            //Main.NewText(Main.time);
            float nightAlpha = MathHelper.Lerp(1f, 0, (float)Utils.Clamp(Main.time / 13500, 0, 1f));
            float morningAlpha = MathHelper.Lerp(1f, 0, (float)Utils.Clamp((Main.time - 13500) / 13500, 0, 1f));
            float dayAlpha = MathHelper.Lerp(1f, 0, (float)Utils.Clamp((Main.time - 13500 * 2) / 13500, 0, 1f));
            float sunsetAlpha = MathHelper.Lerp(1f, 0, (float)Utils.Clamp((Main.time - 13500 * 3) / 13500, 0, 1f));

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
                        spriteBatch.Draw(night, pos - position, null, new Color(158, 158, 158), 0f, origin, Scale * 2, SpriteEffects.None, 0f);
                }


                //spriteBatch.Reload(BlendState.AlphaBlend);
            }

            if (maxDepth >= 8f && minDepth < 8f)
            {
                Texture2D cloudsNight = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_night", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Texture2D cloudsSunset = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_sunset", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Texture2D cloudsDay = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_day", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Texture2D cloudsMorning = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_morning", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                int x = (int)(Main.screenPosition.X * 0.5f * ScreenParralaxMultiplier);
                x %= (int)(cloudsNight.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.45f * ScreenParralaxMultiplier);
                y -= 1820; // 1000
                Vector2 position = cloudsNight.Size() / 2f * Scale;
                Main.NewText(nightAlpha + " to " + (nightAlpha * 0.5f));

                spriteBatch.Reload(SpriteSortMode.Immediate);

                Effect effect = OvermorrowModFile.Instance.ImageLerp.Value;
                effect.Parameters["progress"].SetValue(nightAlpha);
                effect.Parameters["tex"].SetValue(cloudsDay);
                effect.CurrentTechnique.Passes["ImageLerp"].Apply();

                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + cloudsNight.Width * k * Scale, height - y);
                    spriteBatch.Draw(cloudsNight, pos - position, null, textureColor, 0f, origin, Scale, SpriteEffects.None, 0f); 

                    /*if (Main.dayTime)
                    {
                        spriteBatch.Draw(cloudsNight, pos - position, null, textureColor * 0.5f, 0f, origin, Scale, SpriteEffects.None, 0f);
                        spriteBatch.Draw(cloudsSunset, pos - position, null, textureColor * sunsetAlpha * 0.5f, 0f, origin, Scale, SpriteEffects.None, 0f);
                        spriteBatch.Draw(cloudsDay, pos - position, null, textureColor * dayAlpha * 0.5f, 0f, origin, Scale, SpriteEffects.None, 0f);
                        spriteBatch.Draw(cloudsMorning, pos - position, null, textureColor * morningAlpha * 0.5f, 0f, origin, Scale, SpriteEffects.None, 0f);
                        spriteBatch.Draw(cloudsNight, pos - position, null, textureColor * nightAlpha * 0.5f, 0f, origin, Scale, SpriteEffects.None, 0f);
                    }
                    else
                        spriteBatch.Draw(cloudsNight, pos - position, null, textureColor * 0.5f, 0f, origin, Scale, SpriteEffects.None, 0f);*/
                }

                spriteBatch.Reload(SpriteSortMode.Deferred);
            }

            /*if (maxDepth >= 6f && minDepth < 6f)
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

            #region Stars
            // I have no idea what this value represents
            const float someConstantValue = 3.40282347E+38f;
            if (maxDepth >= someConstantValue && minDepth < someConstantValue)
            {
                if (Main.netMode != NetmodeID.Server)
                {

                    int bgTop = (int)((-Main.screenPosition.Y) / (Main.worldSurface * 16.0 - 600.0) * 200.0);
                    float colorMult = 0.952f * starOpacity;
                    Color astralcyan = new Color(100, 183, 255);
                    Color purple = new Color(201, 148, 255);
                    Color yellow = new Color(255, 146, 73);
                    float width1 = Main.screenWidth / 500f;
                    float height1 = Main.screenHeight / 600f;
                    float width2 = Main.screenWidth / 600f;
                    float height2 = Main.screenHeight / 800f;
                    float width3 = Main.screenWidth / 200f;
                    float height3 = Main.screenHeight / 900f;
                    float width4 = Main.screenWidth / 1000f;
                    float height4 = Main.screenHeight / 200f;

                    spriteBatch.Reload(BlendState.Additive);
                    for (int i = 0; i < Main.star.Length; i++)
                    {

                        Star star = Main.star[i];
                        if (star == null) continue;

                        //Texture2D t2D = TextureAssets.Star[star.type].Value;
                        Texture2D starTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;

                        // Big Stars
                        Vector2 starOrigin = new Vector2(starTexture.Width * 0.5f, starTexture.Height * 0.5f);
                        float posX = star.position.X * width1;
                        float posY = star.position.Y * height1;
                        Vector2 position = new Vector2(posX + starOrigin.X, posY + starOrigin.Y + bgTop);
                        spriteBatch.Draw(starTexture, position, new Rectangle(0, 0, starTexture.Width, starTexture.Height), Color.White * star.twinkle * 0.25f, star.rotation, starOrigin, (star.scale) / 4f, SpriteEffects.None, 0f);


                        starTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Circle").Value;
                        starOrigin = new Vector2(starTexture.Width * 0.2f, starTexture.Height * 0.2f);
                        posX = star.position.X * width2;
                        posY = star.position.Y * height2;
                        position = new Vector2(posX + starOrigin.X, posY + starOrigin.Y + bgTop);
                        spriteBatch.Draw(starTexture, position, new Rectangle(0, 0, starTexture.Width, starTexture.Height), Color.White * star.twinkle * colorMult, star.rotation, starOrigin, (star.scale) / 4f, SpriteEffects.None, 0f);

                        // Small stars
                        starOrigin = new Vector2(starTexture.Width * 0.8f, starTexture.Height * 0.8f);
                        posX = star.position.X * width3;
                        posY = star.position.Y * height3;
                        position = new Vector2(posX + starOrigin.X, posY + starOrigin.Y + bgTop);
                        spriteBatch.Draw(starTexture, position, new Rectangle(0, 0, starTexture.Width, starTexture.Height), Color.White * star.twinkle * colorMult, star.rotation, starOrigin, star.scale / 5f, SpriteEffects.None, 0f);

                        // Small stars
                        starOrigin = new Vector2(starTexture.Width * 0.5f, starTexture.Height * 0.5f);
                        posX = star.position.X * width4;
                        posY = star.position.Y * height4;
                        position = new Vector2(posX + starOrigin.X, posY + starOrigin.Y + bgTop);
                        spriteBatch.Draw(starTexture, position, new Rectangle(0, 0, starTexture.Width, starTexture.Height), Color.White * star.twinkle * colorMult, star.rotation, starOrigin, star.scale / 5f, SpriteEffects.None, 0f / 2f);
                    }

                    spriteBatch.Reload(BlendState.AlphaBlend);
                }
            }
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