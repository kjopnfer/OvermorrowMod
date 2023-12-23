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
using static Terraria.Main;

namespace OvermorrowMod.Content.Skies
{
    public partial class RavensfellSky : CustomSky
    {
        private bool isActive = false;

        private const float Scale = 2f;
        private const float ParallaxMultiplier = 0.4f;
        float starOpacity => SetStarOpacity();

        public override float GetCloudAlpha() => 0f;

        public override Color OnTileColor(Color inColor)
        {
            Main.NewText(Main.time + " / slot: " + timeSlot + " / color: " + Main.ColorOfTheSkies);

            Color defaultColor = base.OnTileColor(inColor);
            Color tileColor = Color.Lerp(GetStartAndEndTileColors(defaultColor).Item1, GetStartAndEndTileColors(defaultColor).Item2, timeProgress);
            if (!Main.dayTime) tileColor = GetStartAndEndTileColors(defaultColor).Item1;
            tileColor.A = inColor.A;

            return Color.Lerp(inColor, tileColor, 1f);
            //return base.OnTileColor(inColor);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
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

            float width = Main.screenWidth / 2f;
            float height = Main.screenHeight / 2f;
            Color textureColor = Color.Lerp(Main.ColorOfTheSkies, Color.White, 0.45f);
            Vector2 origin = new Vector2(0f, biomeHeight);

            // Horizon
            if (maxDepth >= 9f && minDepth < 9f)
            {
                DrawSky(spriteBatch, width, height, textureColor, origin);
                //DrawFarTexture(spriteBatch, width, height, textureColor, origin);
                DrawSun(spriteBatch);
            }

            // I have no idea what this value represents
            const float someConstantValue = 3.40282347E+38f;
            if (maxDepth >= someConstantValue && minDepth < someConstantValue)
            {
                DrawStars();
            }

            // Far
            if (maxDepth >= 8f && minDepth < 8f)
            {
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

                float farScale = 0.5f;
                Texture2D farTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Far", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                int x = (int)(Main.screenPosition.X * 0.45f * ParallaxMultiplier);
                x %= (int)(farTexture.Width * farScale);
                int y = (int)(Main.screenPosition.Y * 0.4f * ParallaxMultiplier);
                y -= 1160; // 900
                Vector2 position = farTexture.Size() / 2f * farScale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + farTexture.Width * k * farScale, height - y);
                    spriteBatch.Draw(farTexture, pos - position - new Vector2(350, 0), null, textureColor, 0f, origin, farScale, SpriteEffects.None, 0f);
                }

                Texture2D midTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Mid3", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                x = (int)(Main.screenPosition.X * 0.5f * ParallaxMultiplier);
                x %= (int)(midTexture.Width * farScale);
                y = (int)(Main.screenPosition.Y * 0.45f * ParallaxMultiplier);
                y -= 1260; // 900
                position = midTexture.Size() / 2f * farScale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + farTexture.Width * k * farScale, height - y);
                    spriteBatch.Draw(midTexture, pos - position - new Vector2(300, 0), null, textureColor, 0f, origin, farScale, SpriteEffects.None, 0f);
                }


                float offset = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);

                float midScale = 0.5f;
                Texture2D midTexture2 = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Mid2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                x = (int)(Main.screenPosition.X * 0.3f);
                x %= (int)(midTexture2.Width * midScale);
                y = (int)(Main.screenPosition.Y * 0.5f * ParallaxMultiplier);
                y -= 1420; // 1000
                position = midTexture2.Size() / 2f * midScale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + midTexture2.Width * k * midScale, height - y);
                    spriteBatch.Draw(midTexture2, pos - position - new Vector2(950, 45 * offset), null, textureColor, 0f, origin, midScale, SpriteEffects.None, 0f);
                }
            }


            // Middle
            if (maxDepth >= 7f && minDepth < 7f)
            {
                DrawMidTextures(spriteBatch, width, height, textureColor, origin);
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

        public override void Update(GameTime gameTime)
        {
            if (isActive) { }
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