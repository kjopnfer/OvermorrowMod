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
        // These are used to lerp between the textures/colors based on the time of day
        int timeSlot => (int)Math.Floor(Main.time / 13500);
        float timeProgress => MathHelper.Lerp(0f, 1f, (float)((Main.time % 13500) / 13500f));

        #region Color and Opacity
        private (Texture2D, Texture2D) GetCloudStartAndEndTextures()
        {
            return timeSlot switch
            {
                0 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_night").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_morning").Value),
                1 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_morning").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_day").Value),
                2 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_day").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_sunset").Value),
                3 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_sunset").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_night").Value),
                _ => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_night").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/clouds_night").Value),
            };
        }

        private (Texture2D, Texture2D) GetHorizonStartAndEndTextures()
        {
            return timeSlot switch
            {
                0 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Night").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Morning").Value),
                1 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Morning").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Day").Value),
                2 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Day").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Sunset").Value),
                3 => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Sunset").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Night").Value),
                _ => (ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Night").Value, ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Night").Value),
            };
        }

        /// <param name="timeSlot"></param>
        /// <param name="defaultColor">I don't know what vanilla uses.</param>
        private (Color, Color) GetStartAndEndTileColors(Color defaultColor)
        {
            Color morning = new Color(243, 168, 160);
            Color day = defaultColor;
            Color sunset = new Color(223, 93, 127);
            Color night = new Color(53, 81, 130);

            if (!Main.dayTime) return (night, night);

            return timeSlot switch
            {
                0 => (night, morning),
                1 => (morning, day),
                2 => (day, sunset),
                3 => (sunset, night),
                _ => (night, night),
            };
        }

        private (Color, Color) GetStartAndEndSunColors()
        {
            Color morning = new Color(255, 255, 255);
            Color midMorning = new Color(248, 187, 173);
            Color day = new Color(241, 118, 90);
            Color midSunset = new Color(248, 181, 143);
            Color sunset = new Color(255, 243, 196);

            return timeSlot switch
            {
                0 => (morning, midMorning),
                1 => (midMorning, day),
                2 => (day, midSunset),
                3 => (midSunset, sunset),
                _ => (sunset, sunset),
            };
        }

        private (Color, Color) GetStartAndEndHorizonColors()
        {
            Color morning = new Color(62, 154, 229);
            Color day = new Color(154, 168, 192);
            Color sunset = new Color(66, 79, 197);
            Color night = new Color(3, 19, 32);

            return timeSlot switch
            {
                0 => (night, morning),
                1 => (morning, day),
                2 => (day, sunset),
                3 => (sunset, night),
                _ => (night, night),
            };
        }

        private float SetStarOpacity()
        {
            if (Main.dayTime)
            {
                if (timeSlot == 1 || timeSlot == 2) return 0;

                if (timeSlot == 0) return MathHelper.Lerp(1f, 0, timeProgress);
                if (timeSlot == 3) return MathHelper.Lerp(0, 1f, timeProgress);
                if (timeSlot == 3) return MathHelper.Lerp(0, 1f, timeProgress);
            }

            return 1f;
        }
        #endregion

        #region Drawing
        private void DrawStars()
        {
            if (Main.netMode == NetmodeID.Server) return;

            int bgTop = (int)((-Main.screenPosition.Y) / (Main.worldSurface * 16.0 - 600.0) * 200.0);
            float colorMult = 0.952f * starOpacity;
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
                spriteBatch.Draw(starTexture, position, new Rectangle(0, 0, starTexture.Width, starTexture.Height), Color.White * star.twinkle * 0.25f * colorMult, star.rotation, starOrigin, (star.scale) / 4f, SpriteEffects.None, 0f);

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

        private void DrawFarClouds(SpriteBatch spriteBatch, float width, float height, Color textureColor, Vector2 origin)
        {
            float farScale = 0.5f;
            Texture2D farTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Clouds", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            int x = (int)(Main.screenPosition.X * 0.4f * ParallaxMultiplier);
            x %= (int)(farTexture.Width * farScale);
            int y = (int)(Main.screenPosition.Y * 0.4f * ParallaxMultiplier);
            y -= 700; // 1000

            Vector2 position = farTexture.Size() / 2f * farScale;
            for (int k = -1; k <= 1; k++)
            {
                var pos = new Vector2(width - x + farTexture.Width * k * farScale, height - y);
                spriteBatch.Draw(farTexture, pos - position, null, textureColor, 0f, origin, farScale, SpriteEffects.None, 0f);
            }
        }

        private void DrawSky(SpriteBatch spriteBatch, float width, float height, Color textureColor, Vector2 origin)
        {
            Color horizonColor = Color.Lerp(GetStartAndEndHorizonColors().Item1, GetStartAndEndHorizonColors().Item2, timeProgress);
            if (!Main.dayTime) horizonColor = GetStartAndEndHorizonColors().Item1;
            spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), horizonColor);

            float horizonScale = 1f;
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Ravensfell_Horizon_Night", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            int x = (int)(Main.screenPosition.X * 0.5f * ParallaxMultiplier);
            x %= (int)(texture.Width * horizonScale);

            int y = (int)(Main.screenPosition.Y * 0.45f * ParallaxMultiplier);
            y -= 650; // 1000

            spriteBatch.Reload(SpriteSortMode.Immediate);
            Texture2D startTexture = texture;

            Effect effect = OvermorrowModFile.Instance.ImageLerp.Value;
            if (Main.dayTime)
            {
                var textures = GetHorizonStartAndEndTextures();
                startTexture = textures.Item1;

                effect.Parameters["progress"].SetValue(1 - timeProgress); // Don't know why this is reversed
                effect.Parameters["tex"].SetValue(textures.Item2);
            }

            effect.CurrentTechnique.Passes["ImageLerp"].Apply();

            Vector2 position = texture.Size() / 2f * horizonScale;
            for (int k = -1; k <= 1; k++)
            {
                var pos = new Vector2(width - x + texture.Width * k * horizonScale, height - y);
                if (Main.dayTime) spriteBatch.Draw(startTexture, pos - position, null, Color.White, 0f, origin, horizonScale, SpriteEffects.None, 0f);
                else spriteBatch.Draw(texture, pos - position, null, Color.White * 0.5f, 0f, origin, horizonScale, SpriteEffects.None, 0f);
            }

            spriteBatch.Reload(SpriteSortMode.Deferred);
        }

        private void DrawSun(SpriteBatch spriteBatch)
        {
            // This is all just vanilla code
            int num13 = screenWidth;
            int num14 = screenHeight;
            Vector2 zero = Vector2.Zero;
            if (num13 < 800)
            {
                int num15 = 800 - num13;
                zero.X -= (float)num15 * 0.5f;
                num13 = 800;
            }
            if (num14 < 600)
            {
                int num16 = 600 - num14;
                zero.Y -= (float)num16 * 0.5f;
                num14 = 600;
            }

            SceneArea sceneArea = default(SceneArea);
            sceneArea.bgTopY = 0;
            sceneArea.totalWidth = num13;
            sceneArea.totalHeight = num14;
            sceneArea.SceneLocalScreenPositionOffset = zero;

            Texture2D sunTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Backgrounds/Sun", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num = moonType;

            Texture2D moonTexture = TextureAssets.Moon[num].Value;

            int num2 = sceneArea.bgTopY;
            int num3 = (int)(time / 54000.0 * (double)(sceneArea.totalWidth + (float)(sunTexture.Width * 2))) - sunTexture.Width;
            int num4 = 0;
            int num6 = (int)(time / 32400.0 * (double)(sceneArea.totalWidth + (float)(moonTexture.Width * 2))) - moonTexture.Width;
            int num7 = 0;
            float num8 = 1f;
            float num9 = (float)(time / 32400.0) * 2f - 7.3f;

            if (Main.dayTime)
            {
                double num10;
                if (Main.time < 27000.0)
                {
                    num10 = Math.Pow(1.0 - Main.time / 54000.0 * 2.0, 2.0);
                    num4 = (int)(num10 * 250.0 + 180.0);
                }
                else
                {
                    num10 = Math.Pow((Main.time / 54000.0 - 0.5) * 2.0, 2.0);
                    num4 = (int)(num10 * 250.0 + 180.0);
                }

                Vector2 position = new Vector2(num3, num4 + Main.sunModY) + sceneArea.SceneLocalScreenPositionOffset;
                Color sunColor = Color.Lerp(GetStartAndEndSunColors().Item1, GetStartAndEndSunColors().Item2, timeProgress);
                spriteBatch.Draw(sunTexture, position, null, sunColor, 0f, sunTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                double num11;
                if (time < 16200.0)
                {
                    num11 = Math.Pow(1.0 - time / 32400.0 * 2.0, 2.0);
                    num7 = (int)((double)num2 + num11 * 250.0 + 180.0);
                }
                else
                {
                    num11 = Math.Pow((time / 32400.0 - 0.5) * 2.0, 2.0);
                    num7 = (int)((double)num2 + num11 * 250.0 + 180.0);
                }
                num8 = (float)(1.2 - num11 * 0.4);

                Color moonColor = Color.White;
                Vector2 position2 = new Vector2(num6, num7 + moonModY) + sceneArea.SceneLocalScreenPositionOffset;
                if (WorldGen.drunkWorldGen)
                {
                    spriteBatch.Draw(TextureAssets.SmileyMoon.Value, position2, new Rectangle(0, 0, TextureAssets.SmileyMoon.Width(), TextureAssets.SmileyMoon.Height()), moonColor, num9 / 2f + (float)Math.PI, new Vector2(TextureAssets.SmileyMoon.Width() / 2, TextureAssets.SmileyMoon.Width() / 2), num8, SpriteEffects.None, 0f);
                }
                else if (pumpkinMoon)
                {
                    spriteBatch.Draw(TextureAssets.PumpkinMoon.Value, position2, new Rectangle(0, TextureAssets.PumpkinMoon.Width() * moonPhase, TextureAssets.PumpkinMoon.Width(), TextureAssets.PumpkinMoon.Width()), moonColor, num9, new Vector2(TextureAssets.PumpkinMoon.Width() / 2, TextureAssets.PumpkinMoon.Width() / 2), num8, SpriteEffects.None, 0f);
                }
                else if (snowMoon)
                {
                    spriteBatch.Draw(TextureAssets.SnowMoon.Value, position2, new Rectangle(0, TextureAssets.SnowMoon.Width() * moonPhase, TextureAssets.SnowMoon.Width(), TextureAssets.SnowMoon.Width()), moonColor, num9, new Vector2(TextureAssets.SnowMoon.Width() / 2, TextureAssets.SnowMoon.Width() / 2), num8, SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.Draw(TextureAssets.Moon[num].Value, position2, new Rectangle(0, TextureAssets.Moon[num].Width() * moonPhase, TextureAssets.Moon[num].Width(), TextureAssets.Moon[num].Width()), moonColor, num9, new Vector2(TextureAssets.Moon[num].Width() / 2, TextureAssets.Moon[num].Width() / 2), num8, SpriteEffects.None, 0f);
                }
            }
        }
        #endregion
    }
}