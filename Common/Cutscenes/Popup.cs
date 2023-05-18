using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using System.Xml;
using System;
using OvermorrowMod.Core;
using ReLogic.Utilities;
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

            return null;
        }
    }
}
