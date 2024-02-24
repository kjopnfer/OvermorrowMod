using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using System.Xml;
using System;
using OvermorrowMod.Core;
using ReLogic.Utilities;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Text;

namespace OvermorrowMod.Common.Cutscenes
{
    public class PopupState
    {
        // if these are const then the fucking uistate cant read them
        public readonly float DIALOGUE_DELAY = 30;
        public readonly float OPEN_TIME = 15;
        public readonly float CLOSE_TIME = 10;

        public bool CanOpen { get; private set; } = true;
        public bool CanClose { get; private set; } = false;
        public bool CanBeRemoved { get; private set; } = false;

        public int OpenTimer { get; private set; }
        public int CloseTimer { get; private set; }

        public Popup Popup { get; private set; }

        public PopupState(Popup popup)
        {
            Popup = popup;
        }

        public void ReplacePopup(Popup popup)
        {
            if (SoundEngine.TryGetActiveSound(Popup.drawSound, out var result)) result.Stop();

            Popup = popup;
        }

        public void Update()
        {
            if (CanOpen)
            {
                // This should only run once per state instance.
                // Allows Popups of a state to override each other but not repeat the same opening animation if they do.

                if (OpenTimer == 0)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/PopupShow")
                    {
                        Volume = 1.25f,
                        PitchVariance = 1.1f,
                        MaxInstances = 2,
                    }, Main.LocalPlayer.Center);
                }

                if (OpenTimer < OPEN_TIME) OpenTimer++;
                if (OpenTimer == OPEN_TIME)
                {
                    if (Popup.DelayTimer < DIALOGUE_DELAY) Popup.DelayTimer++;
                    if (Popup.DelayTimer == DIALOGUE_DELAY) CanOpen = false;
                }
            }
            else
            {
                if (Popup.DrawTimer < Popup.GetDrawTime()) // Draws the text for the specified time
                {
                    if (Popup.DrawTimer == 0)
                    {
                        if (!SoundEngine.TryGetActiveSound(Popup.drawSound, out var result))
                        {
                            Popup.drawSound = SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/DialogueDraw")
                            {
                                Volume = 1.25f,
                                PitchVariance = 1.1f,
                                MaxInstances = 3,
                            }, Main.LocalPlayer.Center);
                        }
                    }

                    Popup.DrawTimer++;
                }
                else if (Popup.DisplayTimer < Popup.GetDisplayTime()) // Holds the text for the specified time
                {
                    if (SoundEngine.TryGetActiveSound(Popup.drawSound, out var result)) result.Stop();
                    Popup.DisplayTimer++;
                }
                else if (Popup.GetNodeIteration() < Popup.GetListLength() - 1) // If there are any nodes left, reset timers and go to next
                {
                    Popup.GetNextNode();
                }

                //Main.NewText(Popup.DisplayTimer + " / " + Popup.GetDisplayTime());
                if (Popup.ShouldClose() && Popup.DisplayTimer >= Popup.GetDisplayTime()) // If there are no nodes left and it has finished displaying
                {
                    CanClose = true;
                }
            }

            if (CanClose)
            {

                if (CloseTimer < CLOSE_TIME) CloseTimer++;
                if (CloseTimer == CLOSE_TIME)
                {
                    //Main.NewText("flag removal");
                    CanBeRemoved = true;
                }
            }
        }

        public string GetPopupText()
        {
            int progress = (int)MathHelper.Lerp(0, Popup.GetText().Length, Popup.DrawTimer / (float)Popup.GetDrawTime());
            var text = Popup.GetText().Substring(0, progress);

            // If for some reason there are no colors specified don't parse the brackets
            if (Popup.GetColorHex() != null)
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
                        builder.Append("[c/" + Popup.GetColorHex() + ":");
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
                var hexTag = "[c/" + Popup.GetColorHex() + ":]";
                if (builder.ToString().Contains(hexTag))
                {
                    builder.Replace(hexTag, "[c/" + Popup.GetColorHex() + ": ]");
                }

                text = builder.ToString();
            }

            return text;
        }

        public Texture2D GetPopupFace()
        {
            return Popup.GetPortrait();
        }
    }

    public class Popup
    {
        public int DrawTimer;
        public int DisplayTimer;
        public int DelayTimer;

        public SlotId drawSound;

        private XmlNodeList nodeList;

        private int nodeIterator = 0;
        private string nodeID;
        private XmlDocument xmlDoc;

        public Popup(XmlDocument xmlDoc, string nodeID)
        {
            // The list of all the Text elements for a given node
            this.xmlDoc = xmlDoc;


            //this.nodeList = xmlDoc.GetElementsByTagName("Text");
            this.nodeID = nodeID;
            this.nodeList = GetAllNodeText(nodeID);
        }

        private XmlNode FindNode(string id)
        {
            var cringe = xmlDoc.GetElementsByTagName("Popup");
            foreach (XmlNode node in cringe)
            {
                if (node.Attributes["id"].Value == id) return node;
            }

            return null;
        }

        private XmlNodeList GetAllNodeText(string id)
        {
            // There is no node ID specified, return the Popup and it's text
            if (id == null)
            {
                var nodes = xmlDoc.GetElementsByTagName("Popup");
                return nodes.Item(0).ChildNodes;
            }

            XmlNode node = FindNode(id);
            return node.ChildNodes;
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

        public int GetNodeIteration() => nodeIterator;
        public int GetListLength() => nodeList.Count;
        public void GetNextNode()
        {
            nodeIterator++;
            ResetTimers();

            if (SoundEngine.TryGetActiveSound(drawSound, out var result)) result.Stop();
        }

        public void ResetTimers()
        {
            DrawTimer = 0;
            DisplayTimer = 0;
            DelayTimer = 0;

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

            return "FFFFFF";
        }
    }
}
