using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.UI;
using Terraria.GameInput;
using Terraria.GameContent.UI.Elements;
using ReLogic.Content;

namespace OvermorrowMod.Common.Cutscenes
{
    // Should be able to push dialogue that will constantly loop through each and pop them from the list
    // Dialogue should be able to be cleared and then a new one added if it is urgent
    public class Dialogue
    {
        public string displayText;
        public int drawTime;
        public int showTime;

        /// <summary>
        /// Used to store information about a dialogue object
        /// </summary>
        /// <param name="displayText">The text to be displayed</param>
        /// <param name="drawTime">The amount of time it takes to completely draw the text</param>
        /// <param name="showTime">How long the text remains on screen after fully drawing</param>
        public Dialogue(string displayText, int drawTime, int showTime)
        {
            this.displayText = displayText;
            this.drawTime = drawTime;
            this.showTime = showTime;
        }
    }

    public class DialoguePlayer : ModPlayer
    {
        public List<Dialogue> DialogueList = new List<Dialogue>();
        public bool ShowDialogue = false;

        public void AddDialogue(string displayText, int drawtime, int showTime)
        {
            ShowDialogue = true;
            DialogueList.Add(new Dialogue(displayText, drawtime, showTime));
        }
    }
}