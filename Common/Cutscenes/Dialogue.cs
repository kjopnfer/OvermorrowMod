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

        public Dialogue(Texture2D speakerBody, string displayText, int drawTime, string bracketColor, XmlDocument xmlDoc)
        {
            this.speakerBody = speakerBody;
            this.displayText = displayText;
            this.drawTime = drawTime;

            this.bracketColor = bracketColor;
            this.xmlDoc = xmlDoc;
        }

        public string GetText(string id)
        {
            XmlNode starting = FindNode(id);

            var children = starting.ChildNodes;
            foreach (XmlNode child in children)
            {
                if (child.Name == "Message") return child.InnerText;
            }

            return null;
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

        /*public void Find(XmlDocument doc, string id)
        {
            var cringe = doc.GetElementsByTagName("DialogueNode");
            XmlNode starting = null;
            foreach (XmlNode node in cringe)
            {
                //Console.Write(node.Value);
                //Console.Write(node.Attributes["id"].Value);
                if (node.Attributes["id"].Value == id)
                {
                    starting = node;
                }
            }
            var children = starting.ChildNodes;
            foreach (XmlNode child in children)
            {
                if (child.Name == "msg") Console.WriteLine(child.InnerText);
                if (child.Name == "options")
                {
                    foreach (XmlNode option in child.ChildNodes)
                    {
                        Console.WriteLine(option.InnerText);
                    }
                }
                //Console.WriteLine(child.Name);
                //Console.WriteLine(child.Name + " : " + child.InnerText);
            }
        }
        */
    }
}