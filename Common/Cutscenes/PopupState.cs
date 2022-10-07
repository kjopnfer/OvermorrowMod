using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Core;
using System.Text;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Audio;
using ReLogic.Utilities;

namespace OvermorrowMod.Common.Cutscenes
{
    public class PopupState : UIState
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

        private SlotId drawSound;

        public override void Draw(SpriteBatch spriteBatch)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
            if (player.GetQueueLength() <= 0) return;

            Vector2 textPosition = new Vector2(xPosition - 95, yPosition - 25);
            DrawPopup(spriteBatch, player);

            Popup currentPopup = player.GetPopup();

            if (DrawTimer < currentPopup.GetDrawTime() && OpenTimer >= OPEN_TIME)
            {
                if (DelayTimer++ < DIALOGUE_DELAY) return;
                if (!Main.gamePaused) DrawTimer++;

                DrawText(spriteBatch, player, textPosition);
            }
            else // Hold the dialogue for the amount of time specified
            {
                if (DrawTimer < currentPopup.GetDrawTime()) return;

                if (SoundEngine.TryGetActiveSound(drawSound, out var result)) result.Stop();

                if (HoldTimer <= currentPopup.GetDisplayTime())
                {
                    if (!Main.gamePaused) HoldTimer++;

                    HoldText(spriteBatch, player, textPosition);
                }
                else
                {
                    if (!Main.gamePaused) CloseTimer++;
                    if (!currentPopup.ShouldClose()) CloseTimer = (int)CLOSE_TIME;

                    // Remove the dialogue from the list and reset counters
                    if (CloseTimer == CLOSE_TIME)
                    {
                        if (currentPopup.GetNodeIteration() < currentPopup.GetListLength() - 1)
                        {
                            currentPopup.GetNextNode();
                        }
                        else
                        {
                            player.RemovePopup();
                        }

                        ResetTimers();
                    }
                }
            }
        }

        #region Helper Methods
        private void ResetTimers()
        {
            DrawTimer = 0;
            HoldTimer = 0;
            OpenTimer = 0;
            CloseTimer = 0;
            DelayTimer = 0;

            if (SoundEngine.TryGetActiveSound(drawSound, out var result)) result.Stop();
        }

        private void DrawPopup(SpriteBatch spriteBatch, DialoguePlayer player)
        {
            Popup currentPopup = player.GetPopup();

            if (OpenTimer == 0 && currentPopup.ShouldOpen()) SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/PopupShow")
            {
                Volume = 1.25f,
                PitchVariance = 1.1f,
                MaxInstances = 2,
            }, Main.LocalPlayer.Center);

            Texture2D backDrop = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialogueBack").Value;
            if (!currentPopup.ShouldOpen()) OpenTimer = (int)OPEN_TIME;
            float drawProgress = ModUtils.EaseOutQuint(Utils.Clamp(OpenTimer++, 0, OPEN_TIME) / OPEN_TIME);

            spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(1 - drawProgress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            float xScale = MathHelper.Lerp(1.25f, 1, drawProgress);
            float yScale = MathHelper.Lerp(0, 1, drawProgress);
            if (HoldTimer >= currentPopup.GetDrawTime())
            {
                xScale = 1;
                yScale = MathHelper.Lerp(1, 0, CloseTimer / 15f);
            }

            spriteBatch.Draw(backDrop, new Vector2(xPosition + 48, yPosition + 7), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);

            float scale = MathHelper.Lerp(0.5f, 1f, drawProgress);
            float xOffset = MathHelper.Lerp(-155, 0, drawProgress);
            if (HoldTimer >= currentPopup.GetDisplayTime())
            {
                spriteBatch.Draw(currentPopup.GetPortrait(), new Vector2(xPosition - 36 + xOffset, yPosition - 16), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);
            }
            else
            {
                spriteBatch.Draw(currentPopup.GetPortrait(), new Vector2(xPosition - 36 + xOffset, yPosition - 16), null, Color.White, 0f, backDrop.Size() / 2, scale, SpriteEffects.None, 1f);
            }

            spriteBatch.Reload(SpriteSortMode.Deferred);
        }

        private void DrawText(SpriteBatch spriteBatch, DialoguePlayer player, Vector2 textPosition)
        {
            Popup currentPopup = player.GetPopup();

            if (!SoundEngine.TryGetActiveSound(drawSound, out var result))
            {
                drawSound = SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/DialogueDraw")
                {
                    Volume = 1.25f,
                    PitchVariance = 1.1f,
                    MaxInstances = 1,
                    //SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
                }, Main.LocalPlayer.Center);
            }

            // We need to detect if any color coded text is present, if it is then skip forward by the progression
            int progress = (int)MathHelper.Lerp(0, currentPopup.GetText().Length, DrawTimer / (float)currentPopup.GetDrawTime());
            var text = currentPopup.GetText().Substring(0, progress);

            // If for some reason there are no colors specified don't parse the brackets
            if (currentPopup.GetColorHex() != null)
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
                        builder.Append("[c/" + currentPopup.GetColorHex() + ":");
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
                var hexTag = "[c/" + currentPopup.GetColorHex() + ":]";
                if (builder.ToString().Contains(hexTag))
                {
                    builder.Replace(hexTag, "[c/" + currentPopup.GetColorHex() + ": ]");
                }

                text = builder.ToString();
            }

            int hoveredSnippet = 0;
            TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();

            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, textPosition, Color.White, 0f, Vector2.Zero, Vector2.One * 0.8f, out hoveredSnippet, MAXIMUM_LENGTH);
        }

        private void HoldText(SpriteBatch spriteBatch, DialoguePlayer player, Vector2 textPosition)
        {
            Popup currentPopup = player.GetPopup();
            var text = currentPopup.GetText();

            if (currentPopup.GetColorHex() != null)
            {
                // Create a new string, adding in hex tags whenever an opening bracket is found
                var builder = new StringBuilder();
                builder.Append("    ");

                foreach (var character in text)
                {
                    // Insert the hex tag if an opening bracket is found
                    if (character == '[')
                    {
                        builder.Append("[c/" + currentPopup.GetColorHex() + ":");
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