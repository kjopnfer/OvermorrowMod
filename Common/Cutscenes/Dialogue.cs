using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Collections.Generic;

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

        public int GetTextListLength() => textList.Count;

        public int GetTextIteration() => textIterator;

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

        public string GetText(string id)
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

                        if (option.Attributes["action"] != null)
                        {
                            if (option.Attributes["action"].Value == "exit")
                            {
                                optionButtons.Add(new OptionButton(option.InnerText, "none", "exit"));
                            }
                        }
                        else if (option.Attributes["link"] != null)
                            optionButtons.Add(new OptionButton(option.InnerText, option.Attributes["link"].Value));
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
                if (node.Attributes["id"].Value == id) return node;
            }

            return null;
        }
    }
}