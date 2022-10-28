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
        private Dialogue CurrentDialogue;

        public bool AddedDialogue = false;

        public void SetDialogue(Texture2D speakerBody, string displayText, int drawTime, XmlDocument xmlDoc)
        {
            CurrentDialogue = new Dialogue(speakerBody, displayText, drawTime, new Color(52, 201, 235).Hex3(), xmlDoc);
        }

        public void SetDialogue(Dialogue dialogue) => CurrentDialogue = dialogue;

        public void ClearDialogue() => CurrentDialogue = null;

        public Dialogue GetDialogue() => CurrentDialogue;

        public void AddPopup(XmlDocument xmlDoc)
        {
            PopupQueue.Enqueue(new Popup(xmlDoc));
        }

        public void ClearPopup() => PopupQueue.Clear();

        public Popup RemovePopup() => PopupQueue.Dequeue();

        public int GetQueueLength() => PopupQueue.Count;
    }
}