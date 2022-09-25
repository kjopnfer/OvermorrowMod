using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.UI;
using Terraria.GameInput;
using Terraria.GameContent.UI.Elements;
using ReLogic.Content;
using OvermorrowMod.Core;
using System.Text;
using Terraria.GameContent;
using System;
using ReLogic.Graphics;
using Terraria.UI.Chat;

namespace OvermorrowMod.Common.Cutscenes
{
    public class DialogueState : UIState
    {
        private int DialogueTimer;
        private int SecondaryTimer;
        private int OpenTimer;
        private int CloseTimer;

        // This determines whether the UI is shown or not
        public override void Draw(SpriteBatch spriteBatch)
        {
            float OPEN_TIME = 15;
            float CLOSE_TIME = 10;
            float MAXIMUM_LENGTH = 265;

            int xPosition = 200;
            int yPosition = Main.screenHeight - 375/*169*/;

            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (player.DialogueList.Count > 0)
            {
                #region Popup Animation
                Texture2D backDrop = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialogueBack3").Value;
                if (!player.DialogueList[0].openAnimation) OpenTimer = (int)OPEN_TIME;
                float drawProgress = ModUtils.EaseOutQuint(Utils.Clamp(OpenTimer++, 0, OPEN_TIME) / OPEN_TIME);
                
                spriteBatch.Reload(SpriteSortMode.Immediate);

                Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
                effect.Parameters["WhiteoutProgress"].SetValue(1 - drawProgress);
                effect.CurrentTechnique.Passes["Whiteout"].Apply();

                float xScale = MathHelper.Lerp(1.25f, 1, drawProgress);
                float yScale = MathHelper.Lerp(0, 1, drawProgress);
                if (SecondaryTimer >= player.DialogueList[0].showTime)
                {
                    xScale = 1;
                    yScale = MathHelper.Lerp(1, 0, CloseTimer / 15f);
                }

                spriteBatch.Draw(backDrop, new Vector2(xPosition, yPosition), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);

                float scale = MathHelper.Lerp(0.5f, 1f, drawProgress);
                float xOffset = MathHelper.Lerp(-155, 0, drawProgress);
                if (SecondaryTimer >= player.DialogueList[0].showTime)
                {
                    spriteBatch.Draw(player.DialogueList[0].speakerPortrait, new Vector2(xPosition + xOffset, yPosition), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);
                }
                else
                {
                    spriteBatch.Draw(player.DialogueList[0].speakerPortrait, new Vector2(xPosition + xOffset, yPosition), null, Color.White, 0f, backDrop.Size() / 2, scale, SpriteEffects.None, 1f);
                }

                spriteBatch.Reload(SpriteSortMode.Deferred);
                #endregion

                if (DialogueTimer < player.DialogueList[0].drawTime && OpenTimer >= OPEN_TIME)
                {
                    if (!Main.gamePaused) DialogueTimer++;

                    // We need to detect if any color coded text is present, if it is then skip forward by the progression
                    int progress = (int)MathHelper.Lerp(0, player.DialogueList[0].displayText.Length, DialogueTimer / (float)player.DialogueList[0].drawTime);
                    var text = player.DialogueList[0].displayText.Substring(0, progress);

                    if (player.DialogueList[0].bracketColor != null)
                    {
                        // The number of opening brackets MUST be the same as the number of closing brackets
                        int numOpen = 0;
                        int numClose = 0;

                        // Create a new string, adding in hex tags whenever an opening bracket is found
                        var builder = new StringBuilder();
                        foreach (var character in text)
                        {
                            if (character == '[') // Insert the hex tag if an opening bracket is found
                            {
                                builder.Append("[c/" + player.DialogueList[0].bracketColor + ":");
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
                        var hexTag = "[c/" + player.DialogueList[0].bracketColor + ":]";
                        if (builder.ToString().Contains(hexTag))
                        {
                            builder.Replace(hexTag, "[c/" + player.DialogueList[0].bracketColor + ": ]");
                        }

                        text = builder.ToString();
                    }

                    int hoveredSnippet = 0;
                    Vector2 position = new Vector2(xPosition - 75, yPosition - 50);
                    TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();

                    ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, position, Color.White, 0f, Vector2.Zero, Vector2.One, out hoveredSnippet, MAXIMUM_LENGTH);
                }
                else // Hold the dialogue for the amount of time specified
                {
                    if (DialogueTimer < player.DialogueList[0].drawTime) return;

                    if (SecondaryTimer <= player.DialogueList[0].showTime)
                    {
                        if (!Main.gamePaused) SecondaryTimer++;

                        var text = player.DialogueList[0].displayText;

                        if (player.DialogueList[0].bracketColor != null)
                        {
                            // Create a new string, adding in hex tags whenever an opening bracket is found
                            var builder = new StringBuilder();
                            foreach (var character in text)
                            {
                                // Insert the hex tag if an opening bracket is found
                                if (character == '[')
                                {
                                    builder.Append("[c/" + player.DialogueList[0].bracketColor + ":");
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
                        Vector2 position = new Vector2(xPosition - 75, yPosition - 50);
                        TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
                        ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, position, Color.White, 0f, Vector2.Zero, Vector2.One, out hoveredSnippet, MAXIMUM_LENGTH);
                    }
                    else
                    {
                        if (!Main.gamePaused) CloseTimer++;

                        if (!player.DialogueList[0].closeAnimation) CloseTimer = (int)CLOSE_TIME;

                        // Remove the dialogue from the list and reset counters
                        if (CloseTimer == CLOSE_TIME)
                        {
                            player.DialogueList.RemoveAt(0);
                            DialogueTimer = 0;
                            SecondaryTimer = 0;
                            OpenTimer = 0;
                            CloseTimer = 0;
                        }
                    }
                }
            }
        }
    }
}