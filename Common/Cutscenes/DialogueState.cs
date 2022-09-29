using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Core;
using System.Text;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace OvermorrowMod.Common.Cutscenes
{
    public class DialogueState : UIState
    {
        private int DrawTimer;
        private int HoldTimer;
        private int OpenTimer;
        private int CloseTimer;
        private int DelayTimer;

        const float OPEN_TIME = 15;
        const float CLOSE_TIME = 10;
        const float MAXIMUM_LENGTH = 280;
        const float DIALOGUE_DELAY = 30;

        private int xPosition = 200;
        private int yPosition = Main.screenHeight - 375/*169*/;

        // This determines whether the UI is shown or not
        public override void Draw(SpriteBatch spriteBatch)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
            if (player.GetQueueLength() <= 0) return;

            Vector2 textPosition = new Vector2(xPosition - 95, yPosition - 25);
            DrawPopup(spriteBatch, player);

            if (DrawTimer < player.GetDialogue().drawTime && OpenTimer >= OPEN_TIME)
            {
                if (DelayTimer++ < DIALOGUE_DELAY) return;
                if (!Main.gamePaused) DrawTimer++;

                DrawText(spriteBatch, player, textPosition);
            }
            else // Hold the dialogue for the amount of time specified
            {
                if (DrawTimer < player.GetDialogue().drawTime) return;

                if (HoldTimer <= player.GetDialogue().showTime)
                {
                    if (!Main.gamePaused) HoldTimer++;

                    HoldText(spriteBatch, player, textPosition);
                }
                else
                {
                    if (!Main.gamePaused) CloseTimer++;
                    if (!player.GetDialogue().closeAnimation) CloseTimer = (int)CLOSE_TIME;

                    // Remove the dialogue from the list and reset counters
                    if (CloseTimer == CLOSE_TIME)
                    {
                        player.DequeueDialogue();
                        ClearTimers();
                    }
                }
            }
        }

        #region Helper Methods
        private void ClearTimers()
        {
            DrawTimer = 0;
            HoldTimer = 0;
            OpenTimer = 0;
            CloseTimer = 0;
            DelayTimer = 0;
        }

        private void DrawPopup(SpriteBatch spriteBatch, DialoguePlayer player)
        {
            Texture2D backDrop = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialogueBack").Value;
            if (!player.GetDialogue().openAnimation) OpenTimer = (int)OPEN_TIME;
            float drawProgress = ModUtils.EaseOutQuint(Utils.Clamp(OpenTimer++, 0, OPEN_TIME) / OPEN_TIME);

            spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(1 - drawProgress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            float xScale = MathHelper.Lerp(1.25f, 1, drawProgress);
            float yScale = MathHelper.Lerp(0, 1, drawProgress);
            if (HoldTimer >= player.GetDialogue().showTime)
            {
                xScale = 1;
                yScale = MathHelper.Lerp(1, 0, CloseTimer / 15f);
            }

            spriteBatch.Draw(backDrop, new Vector2(xPosition + 48, yPosition + 7), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);

            float scale = MathHelper.Lerp(0.5f, 1f, drawProgress);
            float xOffset = MathHelper.Lerp(-155, 0, drawProgress);
            if (HoldTimer >= player.GetDialogue().showTime)
            {
                spriteBatch.Draw(player.GetDialogue().speakerPortrait, new Vector2(xPosition + xOffset, yPosition), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);
            }
            else
            {
                spriteBatch.Draw(player.GetDialogue().speakerPortrait, new Vector2(xPosition + xOffset, yPosition), null, Color.White, 0f, backDrop.Size() / 2, scale, SpriteEffects.None, 1f);
            }

            spriteBatch.Reload(SpriteSortMode.Deferred);
        }

        private void DrawText(SpriteBatch spriteBatch, DialoguePlayer player, Vector2 textPosition)
        {
            // We need to detect if any color coded text is present, if it is then skip forward by the progression
            int progress = (int)MathHelper.Lerp(0, player.GetDialogue().displayText.Length, DrawTimer / (float)player.GetDialogue().drawTime);
            var text = player.GetDialogue().displayText.Substring(0, progress);

            // If for some reason there are no colors specified don't parse the brackets
            if (player.GetDialogue().bracketColor != null)
            {
                // The number of opening brackets MUST be the same as the number of closing brackets
                int numOpen = 0;
                int numClose = 0;

                // Create a new string, adding in hex tags whenever an opening bracket is found
                var builder = new StringBuilder();
                builder.Append("    "); // Appends to the beginning of the string

                foreach (var character in text)
                {
                    if (character == '[') // Insert the hex tag if an opening bracket is found
                    {
                        builder.Append("[c/" + player.GetDialogue().bracketColor + ":");
                        numOpen++;
                    }
                    else
                    {
                        if (character == ']')
                        {
                            numClose++;
                        }

                        builder.Append(character);
                    }
                }

                if (numOpen != numClose)
                {
                    builder.Append(']');
                }

                // Final check for if the tag has two brackets but no characters inbetween
                var hexTag = "[c/" + player.GetDialogue().bracketColor + ":]";
                if (builder.ToString().Contains(hexTag))
                {
                    builder.Replace(hexTag, "[c/" + player.GetDialogue().bracketColor + ": ]");
                }

                text = builder.ToString();
            }

            int hoveredSnippet = 0;
            TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();

            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, textPosition, Color.White, 0f, Vector2.Zero, Vector2.One * 0.8f, out hoveredSnippet, MAXIMUM_LENGTH);
        }

        private void HoldText(SpriteBatch spriteBatch, DialoguePlayer player, Vector2 textPosition)
        {
            var text = player.GetDialogue().displayText;

            if (player.GetDialogue().bracketColor != null)
            {
                // Create a new string, adding in hex tags whenever an opening bracket is found
                var builder = new StringBuilder();
                builder.Append("    ");

                foreach (var character in text)
                {
                    // Insert the hex tag if an opening bracket is found
                    if (character == '[')
                    {
                        builder.Append("[c/" + player.GetDialogue().bracketColor + ":");
                    }
                    else
                    {
                        builder.Append(character);
                    }
                }

                if (!builder.ToString().Contains(']') && builder.ToString().Contains('[')) builder.Append(']');

                text = builder.ToString();
            }

            int hoveredSnippet = 0;
            TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, textPosition, Color.White, 0f, Vector2.Zero, Vector2.One * 0.8f, out hoveredSnippet, MAXIMUM_LENGTH);
        }
        #endregion
    }
}