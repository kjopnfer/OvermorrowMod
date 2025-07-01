using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Core.UI
{
    public class TitleCard : UIState
    {
        private string text;
        private int timer;
        private readonly int ShowDuration = ModUtils.SecondsToTicks(5);
        private readonly int FadeDuration = ModUtils.SecondsToTicks(1);

        public static bool visible = false;

        public void ShowTitle(string title)
        {
            if (!Main.dedServ)
            {
                text = title;
                timer = 0;
                visible = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            // Calculate fade alpha
            float alpha = 1f;
            if (timer < FadeDuration)
            {
                // Fade in
                alpha = timer / (float)FadeDuration;
            }
            else if (timer > ShowDuration - FadeDuration)
            {
                // Fade out
                //alpha = 1f - ((timer - (ShowDuration - FadeDuration)) / (float)FadeDuration);
                alpha = EasingUtils.EaseInCubic(1f - ((timer - (ShowDuration - FadeDuration)) / (float)FadeDuration));
            }

            Vector2 textSize = FontAssets.DeathText.Value.MeasureString(text);

            // Draw semi-transparent background
            //Rectangle bgRect = new Rectangle((int)(Main.screenWidth / 2f) - Main.screenWidth / 2, 60, Main.screenWidth / 2, 80);
            //spriteBatch.Draw(TextureAssets.MagicPixel.Value, bgRect, Color.Black * 0.5f * alpha);
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            int stripHeight = 100;
            int stripY = 60;
            int stripWidth = 10;
            int maxDistance = (int)(textSize.X * 1.1f);
            Vector2 center = new Vector2(Main.screenWidth / 2f, stripY);

            for (int x = 0; x < maxDistance; x += stripWidth)
            {
                //float backgroundProgress = x / (float)maxDistance;
                float backgroundProgress = EasingUtils.EaseInCubic(x / (float)maxDistance);

                float segmentAlpha = (1f - backgroundProgress) * 1f * alpha;

                spriteBatch.Draw(pixel, new Rectangle((int)(center.X - x - stripWidth), stripY, stripWidth, stripHeight), Color.Black * segmentAlpha);
                spriteBatch.Draw(pixel, new Rectangle((int)(center.X + x), stripY, stripWidth, stripHeight), Color.Black * segmentAlpha);
            }

            // Draw title text centered
            float textAlpha = MathHelper.Clamp((timer - 20) / (float)ModUtils.SecondsToTicks(1), 0f, 1f);
            if (timer > ShowDuration - FadeDuration)
            {
                textAlpha = alpha;
            }

            Vector2 position = new Vector2(Main.screenWidth / 2 - textSize.X / 2, 120 - textSize.Y / 2);
            spriteBatch.DrawString(FontAssets.DeathText.Value, text, position, Color.White * textAlpha);

            Texture2D bannerLeftSegment = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardLeftSegment").Value;
            Texture2D bannerRightSegment = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardRightSegment").Value;

            float progress = MathHelper.Clamp(timer / (float)ModUtils.SecondsToTicks(1), 0, 1f);
            
            int bannerWidth = 20;
            if (timer > 20)
            {
                float delayedProgress = EasingUtils.EaseOutQuart(MathHelper.Clamp((timer - 20) / (float)ModUtils.SecondsToTicks(1), 0, 1f));
                //float delayedProgress = EasingUtils.EaseInOutCirc(MathHelper.Clamp((timer - 20) / (float)ModUtils.SecondsToTicks(1), 0, 1f));

                //float delayedProgress = MathHelper.Clamp((timer - 20) / (float)ModUtils.SecondsToTicks(1), 0, 1f);
                //bannerWidth = (int)MathHelper.SmoothStep(20, textSize.X, MathHelper.Clamp(delayedProgress, 0, 1f));
                bannerWidth = (int)MathHelper.Lerp(20, textSize.X, MathHelper.Clamp(delayedProgress, 0, 1f));

            }

            if (timer > ShowDuration - FadeDuration)
            {
                progress = EasingUtils.EaseInQuad(MathHelper.Clamp(1f - ((timer - (ShowDuration - FadeDuration)) / (float)FadeDuration), 0, 1f));
                bannerWidth = (int)MathHelper.Lerp(20, textSize.X, MathHelper.Clamp(progress, 0, 1f));
            }

            for (int i = 0; i < bannerWidth; i++)
            {
                spriteBatch.Draw(bannerLeftSegment, new Vector2(Main.screenWidth / 2 - i, 60), null, Color.White * alpha, 0f, bannerLeftSegment.Size() / 2f, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(bannerRightSegment, new Vector2(Main.screenWidth / 2 + i, 60), null, Color.White * alpha, 0f, bannerRightSegment.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < bannerWidth; i++)
            {
                spriteBatch.Draw(bannerLeftSegment, new Vector2(Main.screenWidth / 2 - i, 160), null, Color.White * alpha, 0f, bannerLeftSegment.Size() / 2f, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(bannerRightSegment, new Vector2(Main.screenWidth / 2 + i, 160), null, Color.White * alpha, 0f, bannerRightSegment.Size() / 2f, 1f, SpriteEffects.None, 0f);

            }

            Texture2D bannerLeftArrow = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardLeftArrow").Value;
            spriteBatch.Draw(bannerLeftArrow, new Vector2((Main.screenWidth / 2) - (bannerLeftArrow.Width / 2) - bannerWidth, 160), null, Color.White * alpha, 0f, bannerLeftArrow.Size() / 2f, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(bannerLeftArrow, new Vector2((Main.screenWidth / 2) - (bannerLeftArrow.Width / 2) - bannerWidth, 60), null, Color.White * alpha, 0f, bannerLeftArrow.Size() / 2f, 1f, SpriteEffects.None, 0f);

            Texture2D bannerRightArrow = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardRightArrow").Value;
            spriteBatch.Draw(bannerRightArrow, new Vector2((Main.screenWidth / 2) + (bannerRightArrow.Width / 2) + bannerWidth, 160), null, Color.White * alpha, 0f, bannerRightArrow.Size() / 2f, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(bannerRightArrow, new Vector2((Main.screenWidth / 2) + (bannerRightArrow.Width / 2) + bannerWidth, 60), null, Color.White * alpha, 0f, bannerRightArrow.Size() / 2f, 1f, SpriteEffects.None, 0f);

            Texture2D bannerMiddle = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardMiddle").Value;
            spriteBatch.Draw(bannerMiddle, new Vector2(Main.screenWidth / 2 /*- (bannerMiddle.Width / 2)*/, 160), null, Color.White * alpha, 0f, bannerMiddle.Size() / 2f, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(bannerMiddle, new Vector2(Main.screenWidth / 2 /*- (bannerMiddle.Width / 2)*/, 60), null, Color.White * alpha, 0f, bannerMiddle.Size() / 2f, 1f, SpriteEffects.None, 0f);

            // Update timer
            if (!Main.gamePaused)
            {
                timer++;
                if (timer >= ShowDuration)
                {
                    //visible = false;
                    timer = 0;
                }
            }
        }
    }
}