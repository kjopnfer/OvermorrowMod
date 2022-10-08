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

        public void AddPopup(XmlDocument xmlDoc)
        {
            PopupQueue.Enqueue(new Popup(xmlDoc));
        }

        public void ClearPopup() => PopupQueue.Clear();

        public Popup RemovePopup() => PopupQueue.Dequeue();

        public int GetQueueLength() => PopupQueue.Count;
    }
}