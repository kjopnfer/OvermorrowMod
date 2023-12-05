using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System;
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
            
            // Horizon
            if (maxDepth >= 9f && minDepth < 9f)
            {
                float horizonScale = 1f;
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), Color.Black);
                Texture2D morning = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/morning").Value;
                Texture2D day = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Morning").Value;
                Texture2D sunset = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/sunset").Value;
                Texture2D night = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/night").Value;

                int x = (int)(Main.screenPosition.X * 0.4f * ScreenParralaxMultiplier);
                x %= (int)(day.Width * horizonScale);
                int y = (int)(Main.screenPosition.Y * 0.4f * ScreenParralaxMultiplier);
                y -= 1200; // 1000
                Vector2 position = day.Size() / 2f * horizonScale;

                // 54000

                //spriteBatch.Reload(BlendState.Additive);
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + day.Width * k * horizonScale, height - y);

                    if (Main.dayTime)
                    {
                        //spriteBatch.Draw(night, pos - position, null, Color.White, 0f, origin, horizonScale * 2, SpriteEffects.None, 0f);
                        //spriteBatch.Draw(sunset, pos - position, null, new Color(227, 167, 154, 150) * sunsetAlpha, 0f, origin, horizonScale * 2, SpriteEffects.None, 0f);
                        spriteBatch.Draw(day, pos - position, null, new Color(227, 167, 154, 150) * dayAlpha, 0f, origin, horizonScale, SpriteEffects.None, 0f);
                        //spriteBatch.Draw(morning, pos - position, null, new Color(227, 167, 154, 150) * morningAlpha, 0f, origin, horizonScale * 2, SpriteEffects.None, 0f);
                        //spriteBatch.Draw(night, pos - position, null, Color.White * nightAlpha, 0f, origin, horizonScale * 2, SpriteEffects.None, 0f);
                    }
                    else
                        spriteBatch.Draw(night, pos - position, null, new Color(158, 158, 158), 0f, origin, horizonScale * 2, SpriteEffects.None, 0f);
                }

                DrawFarTexture(spriteBatch, width, height, textureColor, origin);

                //spriteBatch.Reload(BlendState.AlphaBlend);
            }


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


            // Far
            if (maxDepth >= 8f && minDepth < 8f)
            {
                float farScale = 0.5f;
                Texture2D farTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Far", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int x = (int)(Main.screenPosition.X * 0.5f * ScreenParralaxMultiplier);
                x %= (int)(farTexture.Width * farScale);
                int y = (int)(Main.screenPosition.Y * 0.45f * ScreenParralaxMultiplier);
                y -= 1260; // 900
                Vector2 position = farTexture.Size() / 2f * farScale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + farTexture.Width * k * farScale, height - y);
                    spriteBatch.Draw(farTexture, pos - position, null, textureColor, 0f, origin, farScale, SpriteEffects.None, 0f);
                }

                /*Texture2D cloudsNight = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_night", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                int x = (int)(Main.screenPosition.X * 0.5f * ScreenParralaxMultiplier);
                x %= (int)(cloudsNight.Width * Scale);

                int y = (int)(Main.screenPosition.Y * 0.45f * ScreenParralaxMultiplier);
                y -= 1820; // 1000

                spriteBatch.Reload(SpriteSortMode.Immediate);
                Texture2D startTexture = cloudsNight;

                Effect effect = OvermorrowModFile.Instance.ImageLerp.Value;
                if (Main.dayTime)
                {
                    int skyIteration = (int)Math.Floor(Main.time / 13500);
                    var textures = GetCloudStartAndEndTextures(skyIteration);
                    startTexture = textures.Item1;

                    float progress = MathHelper.Lerp(0f, 1f, (float)((Main.time % 13500) / 13500f));

                    effect.Parameters["progress"].SetValue(1 - progress); // Don't know why this is reversed
                    effect.Parameters["tex"].SetValue(textures.Item2);
                }
                
                effect.CurrentTechnique.Passes["ImageLerp"].Apply();

                Vector2 position = cloudsNight.Size() / 2f * Scale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + cloudsNight.Width * k * Scale, height - y);
                    if (Main.dayTime) spriteBatch.Draw(startTexture, pos - position, null, textureColor * 0.5f, 0f, origin, Scale, SpriteEffects.None, 0f);
                    else spriteBatch.Draw(cloudsNight, pos - position, null, textureColor * 0.5f, 0f, origin, Scale, SpriteEffects.None, 0f); 
                }

                spriteBatch.Reload(SpriteSortMode.Deferred);*/
            }


            // Middle
            if (maxDepth >= 7f && minDepth < 7f)
            {
                float midScale = 0.5f;
                Texture2D midTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Mid", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int x = (int)(Main.screenPosition.X * 0.8f * ScreenParralaxMultiplier);
                x %= (int)(midTexture.Width * midScale);
                int y = (int)(Main.screenPosition.Y * 0.5f * ScreenParralaxMultiplier);
                y -= 1420; // 1000
                Vector2 position = midTexture.Size() / 2f * midScale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + midTexture.Width * k * midScale, height - y);
                    spriteBatch.Draw(midTexture, pos - position, null, Color.White, 0f, origin, midScale, SpriteEffects.None, 0f);
                }
            }

            // Close
            /*if (maxDepth >= 6f && minDepth < 6f)
            {
                float closeScale = 0.5f;
                Texture2D closeTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Close", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int x = (int)(Main.screenPosition.X * 0.9f * ScreenParralaxMultiplier);
                x %= (int)(closeTexture.Width * 1.5f);
                int y = (int)(Main.screenPosition.Y * 0.55f * ScreenParralaxMultiplier);
                y -= 1600; // 1000
                Vector2 position = closeTexture.Size() / 2f * closeScale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + closeTexture.Width * k * closeScale, height - y);
                    spriteBatch.Draw(closeTexture, pos - position, null, textureColor, 0f, origin, closeScale, SpriteEffects.None, 0f);
                }
            }
            
            // Front
            if (maxDepth >= 5f && minDepth < 5f)
            {
                float frontScale = 1.25f;
                Texture2D frontTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Front", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int x = (int)(Main.screenPosition.X * 1.1f * ScreenParralaxMultiplier);
                x %= (int)(frontTexture.Width * frontScale);
                int y = (int)(Main.screenPosition.Y * 0.6f * ScreenParralaxMultiplier);
                y -= 1800; // 1000
                Vector2 position = frontTexture.Size() / 2f * frontScale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + frontTexture.Width * k * frontScale, height - y);
                    spriteBatch.Draw(frontTexture, pos - position, null, textureColor, 0f, origin, frontScale, SpriteEffects.None, 0f);
                }
            }*/
            #endregion

          }

        private (Texture2D, Texture2D) GetCloudStartAndEndTextures(int id)
        {
            return id switch
            {
                0 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_night").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_morning").Value),
                1 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_morning").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_day").Value),
                2 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_day").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_sunset").Value),
                3 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_sunset").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_night").Value),
                _ => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_night").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_night").Value),
            };
        }

        private void DrawFarTexture(SpriteBatch spriteBatch, float width, float height, Color textureColor, Vector2 origin)
        {
            float farScale = 0.5f;
            Texture2D farTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Clouds", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            int x = (int)(Main.screenPosition.X * 0.4f * ScreenParralaxMultiplier);
            x %= (int)(farTexture.Width * farScale);
            int y = (int)(Main.screenPosition.Y * 0.4f * ScreenParralaxMultiplier);
            y -= 700; // 1000

            Vector2 position = farTexture.Size() / 2f * farScale;
            for (int k = -1; k <= 1; k++)
            {
                var pos = new Vector2(width - x + farTexture.Width * k * farScale, height - y);
                spriteBatch.Draw(farTexture, pos - position, null, textureColor, 0f, origin, farScale, SpriteEffects.None, 0f);
            }
        }

        public override float GetCloudAlpha() => 0f;

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