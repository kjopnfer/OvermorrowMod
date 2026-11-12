using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.WorldGeneration;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
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

        private bool timerMode;
        private bool timerStarted;
        private bool introSoundPlayed;

        private int seenBonusCount;
        private int bonusPopupTimer;
        private int bonusPopupSeconds;
        private readonly int BonusPopupDuration = ModUtils.SecondsToTicks(1.5f);
        private readonly int TitleHoldDuration = ModUtils.SecondsToTicks(2.5f);
        private const float TimerTextScale = 0.85f;
        private const float TimerShrinkScale = 0.8f;

        private const float TopRowY = 60f;
        private const float BaseBottomY = 160f;
        private const float ContentY = 120f;

        private int SwapStart => FadeDuration + TitleHoldDuration;
        private int SwapEnd => SwapStart + FadeDuration;

        public static bool visible = false;

        public void ShowTitle(string title, bool withTimer = false)
        {
            if (!Main.dedServ)
            {
                text = title;
                timer = 0;
                timerMode = withTimer;
                timerStarted = false;
                introSoundPlayed = false;
                visible = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            if (!introSoundPlayed)
            {
                SoundEngine.PlaySound(new SoundStyle("OvermorrowMod/Sounds/QuestAccept"));
                introSoundPlayed = true;
            }

            Vector2 textSize = FontAssets.DeathText.Value.MeasureString(text);

            float borderAlpha;
            float titleAlpha;
            float timerAlpha = 0f;
            int bannerWidth = 20;
            float assemblyScale = 1f;

            if (timerMode)
            {
                borderAlpha = timer < FadeDuration ? timer / (float)FadeDuration : 1f;

                if (timer < SwapStart)
                    titleAlpha = MathHelper.Clamp((timer - 20) / (float)FadeDuration, 0f, 1f);
                else if (timer < SwapEnd)
                    titleAlpha = EasingUtils.EaseInCubic(MathHelper.Clamp(1f - ((timer - SwapStart) / (float)FadeDuration), 0f, 1f));
                else
                    titleAlpha = 0f;

                timerAlpha = timer < SwapEnd ? 0f : MathHelper.Clamp((timer - SwapEnd) / (float)FadeDuration, 0f, 1f);

                if (timer > 20)
                {
                    float delayedProgress = EasingUtils.EaseOutQuart(MathHelper.Clamp((timer - 20) / (float)FadeDuration, 0, 1f));
                    bannerWidth = (int)MathHelper.Lerp(20, textSize.X, delayedProgress);
                }

                if (timer >= SwapStart)
                {
                    float shrinkProgress = EasingUtils.EaseInQuad(MathHelper.Clamp((timer - SwapStart) / (float)FadeDuration, 0, 1f));
                    assemblyScale = MathHelper.Lerp(1f, TimerShrinkScale, shrinkProgress);

                    float timerTargetWidth = FontAssets.DeathText.Value.MeasureString("0:00").X;
                    bannerWidth = (int)MathHelper.Lerp(textSize.X, timerTargetWidth, shrinkProgress);
                }

                bannerWidth = (int)(bannerWidth * assemblyScale);
            }
            else
            {
                float alpha = 1f;
                if (timer < FadeDuration)
                    alpha = timer / (float)FadeDuration;
                else if (timer > ShowDuration - FadeDuration)
                    alpha = EasingUtils.EaseInCubic(1f - ((timer - (ShowDuration - FadeDuration)) / (float)FadeDuration));

                borderAlpha = alpha;

                titleAlpha = MathHelper.Clamp((timer - 20) / (float)FadeDuration, 0f, 1f);
                if (timer > ShowDuration - FadeDuration)
                    titleAlpha = alpha;

                if (timer > 20)
                {
                    float delayedProgress = EasingUtils.EaseOutQuart(MathHelper.Clamp((timer - 20) / (float)FadeDuration, 0, 1f));
                    bannerWidth = (int)MathHelper.Lerp(20, textSize.X, delayedProgress);
                }

                if (timer > ShowDuration - FadeDuration)
                {
                    float shrink = EasingUtils.EaseInQuad(MathHelper.Clamp(1f - ((timer - (ShowDuration - FadeDuration)) / (float)FadeDuration), 0, 1f));
                    bannerWidth = (int)MathHelper.Lerp(20, textSize.X, shrink);
                }
            }

            float topRowY = TopRowY;
            float bottomRowY = TopRowY + (BaseBottomY - TopRowY) * assemblyScale;
            float contentCenterY = TopRowY + (ContentY - TopRowY) * assemblyScale;

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            int stripWidth = 10;
            int stripY = (int)topRowY;
            int stripHeight = (int)(bottomRowY - topRowY);
            int maxDistance = (int)((timerMode ? bannerWidth : textSize.X) * 1.1f);
            Vector2 center = new Vector2(Main.screenWidth / 2f, stripY);

            for (int x = 0; x < maxDistance; x += stripWidth)
            {
                float backgroundProgress = EasingUtils.EaseInCubic(x / (float)maxDistance);
                float segmentAlpha = (1f - backgroundProgress) * borderAlpha;

                spriteBatch.Draw(pixel, new Rectangle((int)(center.X - x - stripWidth), stripY, stripWidth, stripHeight), Color.Black * segmentAlpha);
                spriteBatch.Draw(pixel, new Rectangle((int)(center.X + x), stripY, stripWidth, stripHeight), Color.Black * segmentAlpha);
            }

            if (titleAlpha > 0f)
            {
                Vector2 position = new Vector2(Main.screenWidth / 2 - textSize.X * assemblyScale / 2, contentCenterY - textSize.Y * assemblyScale / 2);
                spriteBatch.DrawString(FontAssets.DeathText.Value, text, position, Color.White * titleAlpha, 0f, Vector2.Zero, assemblyScale, SpriteEffects.None, 0f);
            }

            if (timerMode && timerAlpha > 0f)
            {
                string timerText = SubworldTimer.Display();
                Vector2 timerSize = FontAssets.DeathText.Value.MeasureString(timerText);
                float timerScale = TimerTextScale * assemblyScale;
                Vector2 timerPosition = new Vector2(Main.screenWidth / 2 - timerSize.X * timerScale / 2, contentCenterY - timerSize.Y * timerScale / 2);

                Color timerColor = Color.White;
                if (SubworldTimer.RemainingTicks <= ModUtils.SecondsToTicks(60))
                {
                    float pulse = ((float)System.Math.Sin(timer * 0.08f) + 1f) / 2f;
                    timerColor = Color.Lerp(new Color(150, 20, 20), new Color(255, 70, 70), pulse);
                }

                spriteBatch.DrawString(FontAssets.DeathText.Value, timerText, timerPosition, timerColor * timerAlpha, 0f, Vector2.Zero, timerScale, SpriteEffects.None, 0f);

                if (SubworldTimer.BonusCount != seenBonusCount)
                {
                    seenBonusCount = SubworldTimer.BonusCount;
                    bonusPopupSeconds = SubworldTimer.LastBonusSeconds;
                    bonusPopupTimer = BonusPopupDuration;
                }

                if (bonusPopupTimer > 0)
                {
                    float life = bonusPopupTimer / (float)BonusPopupDuration;
                    float rise = (1f - life) * 40f;
                    float popupAlpha = EasingUtils.EaseOutQuart(MathHelper.Clamp(life, 0f, 1f));
                    string popupText = $"+{bonusPopupSeconds} seconds";
                    float popupScale = timerScale * 0.5f;
                    Vector2 popupPos = new Vector2(timerPosition.X + timerSize.X * timerScale + 12f, timerPosition.Y - rise);
                    spriteBatch.DrawString(FontAssets.DeathText.Value, popupText, popupPos, new Color(80, 220, 90) * popupAlpha * timerAlpha, 0f, Vector2.Zero, popupScale, SpriteEffects.None, 0f);
                }
            }

            Texture2D bannerLeftSegment = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardLeftSegment").Value;
            Texture2D bannerRightSegment = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardRightSegment").Value;

            for (int i = 0; i < bannerWidth; i++)
            {
                spriteBatch.Draw(bannerLeftSegment, new Vector2(Main.screenWidth / 2 - i, topRowY), null, Color.White * borderAlpha, 0f, bannerLeftSegment.Size() / 2f, assemblyScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(bannerRightSegment, new Vector2(Main.screenWidth / 2 + i, topRowY), null, Color.White * borderAlpha, 0f, bannerRightSegment.Size() / 2f, assemblyScale, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < bannerWidth; i++)
            {
                spriteBatch.Draw(bannerLeftSegment, new Vector2(Main.screenWidth / 2 - i, bottomRowY), null, Color.White * borderAlpha, 0f, bannerLeftSegment.Size() / 2f, assemblyScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(bannerRightSegment, new Vector2(Main.screenWidth / 2 + i, bottomRowY), null, Color.White * borderAlpha, 0f, bannerRightSegment.Size() / 2f, assemblyScale, SpriteEffects.None, 0f);
            }

            Texture2D bannerLeftArrow = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardLeftArrow").Value;
            float leftArrowOffset = (bannerLeftArrow.Width / 2f) * assemblyScale + bannerWidth;
            spriteBatch.Draw(bannerLeftArrow, new Vector2((Main.screenWidth / 2) - leftArrowOffset, bottomRowY), null, Color.White * borderAlpha, 0f, bannerLeftArrow.Size() / 2f, assemblyScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bannerLeftArrow, new Vector2((Main.screenWidth / 2) - leftArrowOffset, topRowY), null, Color.White * borderAlpha, 0f, bannerLeftArrow.Size() / 2f, assemblyScale, SpriteEffects.None, 0f);

            Texture2D bannerRightArrow = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardRightArrow").Value;
            float rightArrowOffset = (bannerRightArrow.Width / 2f) * assemblyScale + bannerWidth;
            spriteBatch.Draw(bannerRightArrow, new Vector2((Main.screenWidth / 2) + rightArrowOffset, bottomRowY), null, Color.White * borderAlpha, 0f, bannerRightArrow.Size() / 2f, assemblyScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bannerRightArrow, new Vector2((Main.screenWidth / 2) + rightArrowOffset, topRowY), null, Color.White * borderAlpha, 0f, bannerRightArrow.Size() / 2f, assemblyScale, SpriteEffects.None, 0f);

            Texture2D bannerMiddle = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardMiddle").Value;
            spriteBatch.Draw(bannerMiddle, new Vector2(Main.screenWidth / 2, bottomRowY), null, Color.White * borderAlpha, 0f, bannerMiddle.Size() / 2f, assemblyScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bannerMiddle, new Vector2(Main.screenWidth / 2, topRowY), null, Color.White * borderAlpha, 0f, bannerMiddle.Size() / 2f, assemblyScale, SpriteEffects.None, 0f);

            if (!Main.gamePaused)
            {
                timer++;
                if (bonusPopupTimer > 0) bonusPopupTimer--;

                if (timerMode)
                {
                    if (!timerStarted && timer >= SwapEnd)
                    {
                        SubworldTimer.Start();
                        timerStarted = true;
                    }
                }
                else if (timer >= ShowDuration)
                {
                    visible = false;
                    timer = 0;
                }
            }
        }
    }
}
