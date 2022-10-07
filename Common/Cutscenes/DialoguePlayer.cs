using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using System.Xml;

namespace OvermorrowMod.Common.Cutscenes
{
    public partial class DialoguePlayer : ModPlayer
    {
        private Queue<Popup> PopupQueue = new Queue<Popup>();

        public bool AddedDialogue = false;

        public void AddPopup(Texture2D speakerPortrait, string displayText, int drawTime, int showTime, Color bracketColor, bool openAnimation = true, bool closeAnimation = true)
        {
            PopupQueue.Enqueue(new Popup(speakerPortrait, displayText, drawTime, showTime, bracketColor.Hex3(), openAnimation, closeAnimation));
        }

        public void AddPopup(XmlDocument xmlDoc)
        {
            PopupQueue.Enqueue(new Popup(xmlDoc));
        }

        public Popup GetPopup() => PopupQueue.Peek();

        public void ClearPopup() => PopupQueue.Clear();

        public void RemovePopup() => PopupQueue.Dequeue();

        public int GetQueueLength() => PopupQueue.Count;
    }
}