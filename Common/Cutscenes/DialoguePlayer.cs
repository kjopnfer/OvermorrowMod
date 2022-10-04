using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using System.Xml;

namespace OvermorrowMod.Common.Cutscenes
{
    public class Popup
    {
        public Texture2D speakerPortrait;

        public string displayText;
        public int drawTime;
        public int showTime;
        public string bracketColor;

        private bool openAnimation;
        private bool closeAnimation;

        /// <summary>
        /// Used to store information about a dialogue object
        /// </summary>
        /// <param name="displayText">The text to be displayed</param>
        /// <param name="drawTime">The amount of time it takes to completely draw the text</param>
        /// <param name="showTime">How long the text remains on screen after fully drawing</param>
        /// <param name="bracketColor">The hex color of the text when enclosed in brackets</param>
        public Popup(Texture2D speakerPortrait, string displayText, int drawTime, int showTime, string bracketColor, bool openAnimation, bool closeAnimation)
        {
            this.speakerPortrait = speakerPortrait;
            this.displayText = displayText;
            this.drawTime = drawTime;
            this.showTime = showTime;
            this.bracketColor = bracketColor;

            this.openAnimation = openAnimation;
            this.closeAnimation = closeAnimation;
        }

        public bool ShouldOpen() => openAnimation;
        public bool ShouldClose() => closeAnimation;
    }

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
    }

    public class DialoguePlayer : ModPlayer
    {
        private Queue<Popup> PopupQueue = new Queue<Popup>();
        private Dialogue CurrentDialogue = null;

        public bool AddedDialogue = false;

        public bool pickupWood = false;
        public bool distanceGuide = false;
        public bool guideGreeting = false;

        public void SetDialogue(Texture2D speakerBody, string displayText, int drawTime, XmlDocument xmlDoc)
        {
            CurrentDialogue = new Dialogue(speakerBody, displayText, drawTime, new Color(52, 201, 235).Hex3(), xmlDoc);
        }

        public void SetDialogue(Dialogue dialogue) => CurrentDialogue = dialogue;

        public void ClearDialogue() => CurrentDialogue = null;

        public Dialogue GetDialogue() => CurrentDialogue;

        public void AddPopup(Texture2D speakerPortrait, string displayText, int drawTime, int showTime, Color bracketColor, bool openAnimation = true, bool closeAnimation = true)
        {
            PopupQueue.Enqueue(new Popup(speakerPortrait, displayText, drawTime, showTime, bracketColor.Hex3(), openAnimation, closeAnimation));
        }

        public Popup GetPopup() => PopupQueue.Peek();

        public void ClearPopup() => PopupQueue.Clear();

        public void RemovePopup() => PopupQueue.Dequeue();

        public int GetQueueLength() => PopupQueue.Count;
    }
}