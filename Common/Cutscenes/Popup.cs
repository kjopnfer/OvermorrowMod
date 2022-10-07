using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using System.Xml;
using System;
using OvermorrowMod.Core;

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

        private XmlDocument xmlDoc;
        private XmlNodeList nodeList;

        private int nodeIterator = 0;

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

        public Popup(XmlDocument xmlDoc)
        {
            this.xmlDoc = xmlDoc;
            this.nodeList = xmlDoc.GetElementsByTagName("Text");
        }

        public string GetText()
        {
            XmlNode node = nodeList[nodeIterator];
            return node.InnerText;
        }
        
        public Texture2D GetPortrait() => ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/" + nodeList[nodeIterator].Attributes["npcPortrait"].Value, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public int GetDrawTime() => Convert.ToInt32(nodeList[nodeIterator].Attributes["drawTime"].Value);

        public int GetDisplayTime() => Convert.ToInt32(nodeList[nodeIterator].Attributes["displayTime"].Value);

        public string GetColorHex()
        {
            int r = Convert.ToInt32(nodeList[nodeIterator].Attributes["bracketColorR"].Value);
            int g = Convert.ToInt32(nodeList[nodeIterator].Attributes["bracketColorG"].Value);
            int b = Convert.ToInt32(nodeList[nodeIterator].Attributes["bracketColorB"].Value);

            return new Color(r, g, b).Hex3();
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
    }
}