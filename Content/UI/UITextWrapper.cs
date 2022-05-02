using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace OvermorrowMod.Content.UI
{
    internal class UITextWrapper : UIElement
    {
        public string text;
        //private TextSnippet[] textArray;
        private string[] textArray = null;
        private bool wrapText;
        private int maxWidth;
        private int maxLines;
        public UITextWrapper(string text, int maxWidth, int maxLines = 10, bool wrapText = true)
        {
            this.text = text;
            this.maxWidth = maxWidth;
            this.maxLines = maxLines;
            this.wrapText = wrapText;

            UpdateText();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateText();
        }

        public void UpdateText()
        {
            if (string.IsNullOrEmpty(text))
            {
                textArray = null;
                return;
            }

            textArray = Utils.WordwrapString(text, FontAssets.MouseText.Value, maxWidth, maxLines, out _);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (textArray == null) return;

            Vector2 pos = GetDimensions().ToRectangle().TopLeft();

            int offsetY = 0;
            foreach (var line in textArray)
            {
                if (string.IsNullOrEmpty(line)) continue;

                Utils.DrawBorderString(spriteBatch, line, pos + Vector2.UnitY * offsetY, Color.Red);
                offsetY += 15;
            }
        }
    }
}