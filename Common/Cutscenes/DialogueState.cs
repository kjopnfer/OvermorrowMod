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

namespace OvermorrowMod.Common.Cutscenes
{
    public class DialogueState : UIState
    {
        private UIElement DialogueBox;
        private UIText Name;
        private UIText Dialogue;
        public UIImage BackDrop;
        public UIImage Portrait;

        private int DialogueTimer;
        private int SecondaryTimer;
        private int AnimationTimer;

        public override void OnInitialize()
        {
            DialogueBox = new UIElement();
            DialogueBox.Width.Set(360f, 0f);
            DialogueBox.Height.Set(130f, 0f);
            //DialogueBox.HAlign = .5f;
            DialogueBox.Left.Set(295, 0f);
            DialogueBox.Top.Set(Main.screenHeight - 375f, 0f);

            /*BackDrop = new UIImage(ModContent.Request<Texture2D>(AssetDirectory.UI + "DialogueBack2"));
            BackDrop.Left.Set(0, 0f);
            BackDrop.Top.Set(0, 0f);

            Name = new UIText("", 1f);
            Name.Top.Set(105, 0f);
            Name.Left.Set(0, 0f);*/

            Dialogue = new UIText("", 1f);
            Dialogue.Top.Set(15, 0f);
            Dialogue.Left.Set(140, 0f);

            /*Portrait = new UIImage(ModContent.Request<Texture2D>(AssetDirectory.Empty));
            Portrait.Left.Set(0, 0f);
            Portrait.Top.Set(0, 0f);

            DialogueBox.Append(BackDrop);
            DialogueBox.Append(Name);*/
            DialogueBox.Append(Dialogue);
            //DialogueBox.Append(Portrait);
            Append(DialogueBox);
        }

        // This determines whether the UI is shown or not
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.GetModPlayer<DialoguePlayer>().DialogueList.Count > 0)
            {
                //BackDrop.Draw(spriteBatch);
                //Name.Draw(spriteBatch);
                Dialogue.Draw(spriteBatch);
                //Portrait.Draw(spriteBatch);
            }

            int xPosition = 245;
            int yPosition = Main.screenHeight - 375/*169*/;

            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (player.DialogueList.Count > 0)
            {
                #region Popup Animation
                spriteBatch.Reload(SpriteSortMode.Immediate);

                Texture2D backDrop = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialogueBack3").Value;
                float OPEN_TIME = 12;
                //float progress = (float)(Math.Sin(DialogueTimer / 5f) / 2 + 0.5f);
                float drawProgress = ModUtils.EaseOutQuint(Utils.Clamp(AnimationTimer++, 0, OPEN_TIME) / OPEN_TIME);

                Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
                effect.Parameters["WhiteoutProgress"].SetValue(1 - drawProgress);
                effect.CurrentTechnique.Passes["Whiteout"].Apply();

                float xScale = MathHelper.Lerp(1.25f, 1, drawProgress);
                float yScale = MathHelper.Lerp(0, 1, drawProgress);
                spriteBatch.Draw(backDrop, new Vector2(xPosition, yPosition), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);

                float scale = MathHelper.Lerp(0.5f, 1f, drawProgress);
                float xOffset = MathHelper.Lerp(-155, 0, drawProgress);
                spriteBatch.Draw(player.DialogueList[0].speakerPortrait, new Vector2(xPosition + xOffset, yPosition), null, Color.White, 0f, backDrop.Size() / 2, scale, SpriteEffects.None, 1f);

                spriteBatch.Reload(SpriteSortMode.Deferred);
                #endregion

                if (DialogueTimer < player.DialogueList[0].drawTime && AnimationTimer > OPEN_TIME)
                {
                    DialogueTimer++;

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

                    spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(xPosition - 75, yPosition - 50), Color.White);

                    //Dialogue.SetText(text);
                }
                else // Hold the dialogue for the amount of time specified
                {
                    //Main.NewText("HOLD" + SecondaryTimer);
                    if (SecondaryTimer++ <= player.DialogueList[0].showTime)
                    {
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

                        //Dialogue.SetText(text);
                        spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(xPosition - 75, yPosition - 50), Color.White);

                        // Remove the dialogue from the list and reset counters
                        if (SecondaryTimer == player.DialogueList[0].showTime)
                        {
                            player.DialogueList.RemoveAt(0);
                            DialogueTimer = 0;
                            SecondaryTimer = 0;
                            AnimationTimer = 0;
                        }
                    }
                }

            }
        }

        // This handles the dialogue that the player has, if it detects that the player has new dialogue then it starts drawing it
        public override void Update(GameTime gameTime)
        {
            /*DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (player.DialogueList.Count > 0)
            {
                //Name.SetText(player.DialogueList[0].speakerName);
                //Name.TextColor = player.DialogueList[0].speakerColor;
                //Portrait.SetImage(player.DialogueList[0].speakerPortrait);

                // Draw out the entire dialogue or something
                if (DialogueTimer++ < player.DialogueList[0].drawTime)
                {
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

                    //Main.NewText(text);
                    Dialogue.SetText(text);
                }
                else // Hold the dialogue for the amount of time specified
                {
                    //Main.NewText("HOLD" + SecondaryTimer);
                    if (SecondaryTimer++ <= player.DialogueList[0].showTime)
                    {
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

                        Dialogue.SetText(text);

                        // Remove the dialogue from the list and reset counters
                        if (SecondaryTimer == player.DialogueList[0].showTime)
                        {
                            player.DialogueList.RemoveAt(0);
                            DialogueTimer = 0;
                            SecondaryTimer = 0;
                        }
                    }
                }
            }*/

            base.Update(gameTime);
        }
    }
}