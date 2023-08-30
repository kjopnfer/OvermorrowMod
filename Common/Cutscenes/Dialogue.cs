using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Common.Cutscenes
{
    public class Dialogue
    {
        public Texture2D speakerBody;

        public string displayText;
        public int drawTime;

        public string bracketColor;
        public XmlDocument xmlDoc;

        private XmlNodeList textList;

        private int textIterator = 0;

        public Dialogue(Texture2D speakerBody, string displayText, int drawTime, string bracketColor, XmlDocument xmlDoc)
        {
            this.speakerBody = speakerBody;
            this.displayText = displayText;
            this.drawTime = drawTime;

            this.bracketColor = bracketColor;
            this.xmlDoc = xmlDoc;

            UpdateList("start");
        }

        /// <summary>
        /// Gets the number of Text nodes within the current DialogueNode
        /// </summary>
        public int GetTextListLength() => textList.Count;

        /// <summary>
        /// Gets the current Text node index within the current DialogueNode
        /// </summary>
        public int GetTextIteration() => textIterator;

        /// <summary>
        /// Gets the next Text node within the current DialogueNode
        /// </summary>
        public void IncrementText() => textIterator++;

        /*public string GetText(string id)
        {
            XmlNode starting = FindNode(id);

            var children = starting.ChildNodes;
            foreach (XmlNode child in children)
            {
                if (child.Name == "Message") return child.InnerText;
            }

            return null;
        }*/

        /// <summary>
        /// Grabs a list of texts based on the node ID
        /// </summary>
        /// <param name="id"></param>
        public void UpdateList(string id)
        {
            textIterator = 0;
            XmlNode starting = FindNode(id);
            var children = starting.ChildNodes;

            foreach (XmlNode child in children)
            {
                if (child.Name == "Dialogue")
                {
                    textList = child.SelectNodes(".//Text");
                }
            }
        }

        /// <summary>
        /// Gets the current Text node's actual text.
        /// </summary>
        public string GetText()
        {
            XmlNode node = textList[textIterator];
            if (node.Attributes["time"] != null)
            {
                string value = node.Attributes["time"].Value;
                drawTime = int.Parse(value);
            }

            if (node.Attributes["color"] != null)
            {
                string value = node.Attributes["color"].Value;
                bracketColor = value;
            }

            return node.InnerText;
        }

        public List<OptionButton> GetOptions(string id)
        {
            List<OptionButton> optionButtons = new List<OptionButton>();

            XmlNode starting = FindNode(id);
            var children = starting.ChildNodes;
            foreach (XmlNode child in children)
            {
                if (child.Name == "Options")
                {
                    foreach (XmlNode option in child.ChildNodes)
                    {
                        if (option.Attributes == null) continue;

                        // options will always have a linkID since the options cannot exit the text
                        // click on item button, gives item and then advance text

                        // click on marker button, puts marker and then advance text

                        // click on button, advance the text

                        // click on button, open shop, exit dialogue

                        var icon = option.Attributes["icon"] == null ? "chat" : option.Attributes["icon"].Value;

                        if (option.Attributes["action"] != null)
                        {
                            switch (option.Attributes["action"].Value)
                            {
                                case "item":
                                    if (option.Attributes["item-type"].Value != null)
                                    {
                                        /*var itemID = option.Attributes["item-type"].Value;
                                        var isNumber = int.TryParse(itemID, out _);

                                        int itemStack = option.Attributes["item-stack"] != null ? int.Parse(option.Attributes["item-stack"].Value) : 1;

                                        if (isNumber) // The string could be parsed as a number, therefore it's vanilla.
                                        {
                                            optionButtons.Add(new OptionButton(option.InnerText, option.Attributes["link"].Value, int.Parse(itemID), itemStack));
                                        }
                                        else // Otherwise, it's modded so don't parse.
                                        {
                                            optionButtons.Add(new OptionButton(option.InnerText, option.Attributes["link"].Value, itemID, itemStack));
                                        }*/
                                    }
                                    break;
                                case "marker":
                                    if (option.Attributes["marker-type"].Value != "null")
                                    {

                                    }
                                    break;
                                default:
                                    optionButtons.Add(new OptionButton(icon, option.InnerText, option.Attributes["link"].Value, option.Attributes["action"].Value));
                                    break;
                            }
                        }
                        else if (option.Attributes["link"] != null)
                        {
                            //Main.NewText(icon + " / " + option.Attributes["link"].Value);
                            optionButtons.Add(new OptionButton(icon, option.InnerText, option.Attributes["link"].Value, "none"));
                        }
                    }
                }
            }

            return optionButtons.Count > 0 ? optionButtons : null;
        }

        private XmlNode FindNode(string id)
        {
            var cringe = xmlDoc.GetElementsByTagName("DialogueNode");
            foreach (XmlNode node in cringe)
            {
                // This is might cause problems somewhere down the line, idk, I forgot what Main.LocalPlayer does
                if (node.Attributes["flag"] != null)
                {
                    Main.LocalPlayer.GetModPlayer<DialoguePlayer>().DialogueFlags.Add(node.Attributes["flag"].Value);
                }

                if (node.Attributes["id"].Value == id) return node;
            }

            return null;
        }
    }
}