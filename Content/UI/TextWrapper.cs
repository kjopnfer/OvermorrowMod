using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Content.UI
{
    internal class TextWrapper : UIElement
    {
        public string text;
        //private TextSnippet[] textArray;
        private string[] textArray;
        private bool wrapText;
        private int maxWidth;
        private int maxLines;
        public TextWrapper(string text, int maxWidth, int maxLines = 10, bool wrapText = true)
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

            string[] lines = wrapText ? ModUtils.WrapText(FontAssets.MouseText.Value, text, GetDimensions().Width, 1f).Split('\n') : text.Split('\n');

            //textArray = ChatManager.ParseMessage(text, Color.Red).ToArray();
            //ChatManager.ConvertNormalSnippets(textArray);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (textArray == null) return;

            //var style = GetDimensions();
            //Vector2 tl = style.Position();
            //Vector2 pos = tl - Vector2.UnitX;
            //
            //float maxWidth = wrapText ? style.Width : -1f;

            Vector2 pos = GetDimensions().ToRectangle().TopLeft();

            int offsetY = 0;
            foreach (var line in textArray)
            {
                Utils.DrawBorderString(spriteBatch, line, pos + Vector2.UnitY * offsetY, Color.Red);
                offsetY += 15;
            }

            //DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, textArray, pos, Color.Red, 0f, Vector2.Zero, Vector2.One, maxWidth);
        }

        /*public static Vector2 DrawColorCodedString(SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth, bool ignoreColors = false)
		{
			Vector2 vector2;
			int num = -1;
			Vector2 vector21 = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			Vector2 x = position;
			Vector2 vector22 = x;
			float single = font.MeasureString(" ").X;
			Color visibleColor = baseColor;
			float single1 = 0f;
			for (int i = 0; i < (int)snippets.Length; i++)
			{
				TextSnippet textSnippet = snippets[i];
				textSnippet.Update();
				if (!ignoreColors)
				{
					visibleColor = textSnippet.GetVisibleColor();
				}
				float scale = textSnippet.Scale;
				if (!textSnippet.UniqueDraw(false, out vector2, spriteBatch, x, visibleColor, scale))
				{
					string[] strArrays = textSnippet.Text.Split(new char[] { '\n' });
					string[] strArrays1 = strArrays;
					for (int j = 0; j < (int)strArrays1.Length; j++)
					{
						string[] strArrays2 = strArrays1[j].Split(new char[] { ' ' });
						for (int k = 0; k < (int)strArrays2.Length; k++)
						{
							if (k != 0)
							{
								ref float singlePointer = ref x.X;
								singlePointer = singlePointer + single * baseScale.X * scale;
							}
							if (maxWidth > 0f && x.X - position.X + font.MeasureString(strArrays2[k]).X * baseScale.X * scale > maxWidth)
							{
								x.X = position.X;
								ref float y = ref x.Y;
								y = y + (float)font.LineSpacing * single1 * baseScale.Y;
								vector22.Y = Math.Max(vector22.Y, x.Y);
								single1 = 0f;
							}
							if (single1 < scale)
							{
								single1 = scale;
							}
							DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, font, strArrays2[k], x, visibleColor, rotation, origin, (baseScale * textSnippet.Scale) * scale, SpriteEffects.None, 0f);
							Vector2 vector23 = font.MeasureString(strArrays2[k]);
							if (vector21.Between(x, x + vector23))
							{
								num = i;
							}
							ref float x1 = ref x.X;
							x1 = x1 + vector23.X * baseScale.X * scale;
							vector22.X = Math.Max(vector22.X, x.X);
						}
						if ((int)strArrays.Length > 1 && j < strArrays1.Length - 1)
						{
							ref float lineSpacing = ref x.Y;
							lineSpacing = lineSpacing + (float)font.LineSpacing * single1 * baseScale.Y;
							x.X = position.X;
							vector22.Y = Math.Max(vector22.Y, x.Y);
							single1 = 0f;
						}
					}
				}
				else
				{
					if (vector21.Between(x, x + vector2))
					{
						num = i;
					}
					ref float singlePointer1 = ref x.X;
					singlePointer1 = singlePointer1 + vector2.X * baseScale.X * scale;
					vector22.X = Math.Max(vector22.X, x.X);
				}
			}

			return vector22;
		}*/
    }
}