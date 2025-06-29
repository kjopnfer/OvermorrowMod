using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
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
            text = title;
            timer = 0;
            visible = true;
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
                alpha = 1f - ((timer - (ShowDuration - FadeDuration)) / (float)FadeDuration);
            }

            // Draw semi-transparent background
            Rectangle bgRect = new Rectangle(0, 60, Main.screenWidth, 80);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, bgRect, Color.Black * 0.5f * alpha);

            // Draw title text centered
            Vector2 textSize = FontAssets.DeathText.Value.MeasureString(text);
            Vector2 position = new Vector2(Main.screenWidth / 2 - textSize.X / 2, 100 - textSize.Y / 2);
            spriteBatch.DrawString(FontAssets.DeathText.Value, text, position, Color.White * alpha);

            // Update timer
            if (!Main.gamePaused)
            {
                timer++;
                if (timer >= ShowDuration)
                {
                    visible = false;
                    timer = 0;
                }
            }
        }
    }
}