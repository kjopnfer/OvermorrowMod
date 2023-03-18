using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using System.Xml;
using System;
using OvermorrowMod.Core;
using ReLogic.Utilities;
using Terraria.UI.Chat;
using Terraria.GameContent;
using System.Text;
using Terraria.Audio;

namespace OvermorrowMod.Common.Cutscenes
{
    public class Popup
    {
        public int DrawTimer;
        public int DisplayTimer;
        public int DelayTimer;

        public SlotId drawSound;

        private XmlNodeList nodeList;

        private int nodeIterator = 0;

        public Popup(XmlDocument xmlDoc)
        {
            this.nodeList = xmlDoc.GetElementsByTagName("Text");
        }

        /// <summary>
        /// Parses and returns the current text for the dialogue node
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            XmlNode node = nodeList[nodeIterator];
            var text = node.InnerText;
            text = text.Replace("${name}", Main.LocalPlayer.name);

            return text;
        }

        public void ResetTimers()
        {
            DrawTimer = 0;
            DisplayTimer = 0;
            DelayTimer = 0;
 
            if (SoundEngine.TryGetActiveSound(drawSound, out var result)) result.Stop();
        }

        public Texture2D GetPortrait() => ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/" + nodeList[nodeIterator].Attributes["npcPortrait"].Value, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public int GetDrawTime() => Convert.ToInt32(nodeList[nodeIterator].Attributes["drawTime"].Value);

        public int GetDisplayTime() => Convert.ToInt32(nodeList[nodeIterator].Attributes["displayTime"].Value);

        public int GetNodeIteration() => nodeIterator;
        public int GetListLength() => nodeList.Count;
        public void GetNextNode()
        {
            nodeIterator++;
            ResetTimers();

            if (SoundEngine.TryGetActiveSound(drawSound, out var result)) result.Stop();
        }

        /// <summary>
        /// Checks whether the element should close if it is the last element in the list
        /// </summary>
        /// <returns></returns>
        public bool ShouldClose() => nodeIterator == nodeList.Count - 1;

        public string GetColorHex()
        {
            if (nodeList[nodeIterator].Attributes["color"] != null)
            {
                return nodeList[nodeIterator].Attributes["color"].Value;
            }

            return null;
        }


    }
    /*public class Popup
    {
        public int DrawTimer;
        public int HoldTimer;
        public int OpenTimer;
        public int CloseTimer;
        public int DelayTimer;

        public readonly float OPEN_TIME = 15;
        public readonly float CLOSE_TIME = 10;
        public readonly float MAXIMUM_LENGTH = 280;
        public readonly float DIALOGUE_DELAY = 30;

        //private int xPosition = 235;
        //private int yPosition = Main.screenHeight - 375;

        public SlotId drawSound;

        private XmlDocument xmlDoc;
        private XmlNodeList nodeList;

        private int nodeIterator = 0;

        public Popup(XmlDocument xmlDoc)
        {
            this.xmlDoc = xmlDoc;
            this.nodeList = xmlDoc.GetElementsByTagName("Text");
        }

        /// <summary>
        /// Parses and returns the current text for the dialogue node
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            XmlNode node = nodeList[nodeIterator];
            var text = node.InnerText;
            text = text.Replace("${name}", Main.LocalPlayer.name);

            return text;
        }

        public Texture2D GetPortrait() => ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/" + nodeList[nodeIterator].Attributes["npcPortrait"].Value, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public int GetDrawTime() => Convert.ToInt32(nodeList[nodeIterator].Attributes["drawTime"].Value);

        public int GetDisplayTime() => Convert.ToInt32(nodeList[nodeIterator].Attributes["displayTime"].Value);

        public string GetColorHex()
        {
            if (nodeList[nodeIterator].Attributes["color"] != null)
            {
                return nodeList[nodeIterator].Attributes["color"].Value;
            }

            return null;
        }

        public int GetNodeIteration() => nodeIterator;
        public int GetListLength() => nodeList.Count;
        public void GetNextNode() => nodeIterator++;

        /// <summary>
        /// Checks whether the element should open if it is the first element in the list
        /// </summary>
        /// <returns></returns>
        public bool ShouldOpen() => nodeIterator == 0;

        /// <summary>
        /// Checks whether the element should close if it is the last element in the list
        /// </summary>
        /// <returns></returns>
        public bool ShouldClose() => nodeIterator == nodeList.Count - 1;

        public void DrawPopup(SpriteBatch spriteBatch, Vector2 textPosition)
        {
            if (OpenTimer == 0 && ShouldOpen()) SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/PopupShow")
            {
                Volume = 1.25f,
                PitchVariance = 1.1f,
                MaxInstances = 2,
            }, Main.LocalPlayer.Center);

            Texture2D backDrop = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialogueBack").Value;
            if (!ShouldOpen()) OpenTimer = (int)OPEN_TIME;
            float drawProgress = ModUtils.EaseOutQuint(Utils.Clamp(OpenTimer++, 0, OPEN_TIME) / OPEN_TIME);

            spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(1 - drawProgress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            float xScale = MathHelper.Lerp(1.25f, 1, drawProgress);
            float yScale = MathHelper.Lerp(0, 1, drawProgress);
            if (HoldTimer >= GetDrawTime())
            {
                xScale = 1;
                yScale = MathHelper.Lerp(1, 0, CloseTimer / 15f);
            }

            //spriteBatch.Draw(backDrop, new Vector2(xPosition + 48, yPosition + 7), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);
            spriteBatch.Draw(backDrop, new Vector2(textPosition.X + 96 + 30, textPosition.Y + 36), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);

            float scale = MathHelper.Lerp(0.5f, 1f, drawProgress);
            float xOffset = MathHelper.Lerp(-155, 0, drawProgress);
            if (HoldTimer >= GetDisplayTime())
            {
                //spriteBatch.Draw(GetPortrait(), new Vector2(xPosition - 36 + xOffset, yPosition - 16), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);
                spriteBatch.Draw(GetPortrait(), new Vector2(textPosition.X + 62 + xOffset, textPosition.Y + 14), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);
            }
            else
            {
                //spriteBatch.Draw(GetPortrait(), new Vector2(xPosition - 36 + xOffset, yPosition - 16), null, Color.White, 0f, backDrop.Size() / 2, scale, SpriteEffects.None, 1f);
                spriteBatch.Draw(GetPortrait(), new Vector2(textPosition.X + 62 + xOffset, textPosition.Y + 14), null, Color.White, 0f, backDrop.Size() / 2, scale, SpriteEffects.None, 1f);
            }

            spriteBatch.Reload(SpriteSortMode.Deferred);
        }

        public void DrawText(SpriteBatch spriteBatch, Vector2 textPosition)
        {
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

            int progress = (int)MathHelper.Lerp(0, GetText().Length, DrawTimer / (float)GetDrawTime());
            var text = GetText().Substring(0, progress);

            // If for some reason there are no colors specified don't parse the brackets
            if (GetColorHex() != null)
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
                        builder.Append("[c/" + GetColorHex() + ":");
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
                var hexTag = "[c/" + GetColorHex() + ":]";
                if (builder.ToString().Contains(hexTag))
                {
                    builder.Replace(hexTag, "[c/" + GetColorHex() + ": ]");
                }

                text = builder.ToString();
            }

            TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();

            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, textPosition, Color.White, 0f, Vector2.Zero, Vector2.One * 0.8f, out var hoveredSnippet, MAXIMUM_LENGTH);
        }

        public void HoldText(SpriteBatch spriteBatch, Vector2 textPosition)
        {
            var text = GetText();

            if (GetColorHex() != null)
            {
                // Create a new string, adding in hex tags whenever an opening bracket is found
                var builder = new StringBuilder();
                builder.Append("    ");

                foreach (var character in text)
                {
                    // Insert the hex tag if an opening bracket is found
                    if (character == '[')
                    {
                        builder.Append("[c/" + GetColorHex() + ":");
                    }
                    else
                    {
                        builder.Append(character);
                    }
                }

                if (!builder.ToString().Contains(']') && builder.ToString().Contains('[')) builder.Append(']');

                text = builder.ToString();
            }

            TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, textPosition, Color.White, 0f, Vector2.Zero, Vector2.One * 0.8f, out var hoveredSnippet, MAXIMUM_LENGTH);
        }

        public void ResetTimers()
        {
            DrawTimer = 0;
            HoldTimer = 0;
            OpenTimer = 0;
            CloseTimer = 0;
            DelayTimer = 0;

            if (SoundEngine.TryGetActiveSound(drawSound, out var result)) result.Stop();
        }
    }*/
}
