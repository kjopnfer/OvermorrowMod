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
using System;
using System.Text;
using ReLogic.Graphics;
using Terraria.GameContent;

namespace OvermorrowMod.Common.Cutscenes
{
    // Should be able to push dialogue that will constantly loop through each and pop them from the list
    // Dialogue should be able to be cleared and then a new one added if it is urgent
    public class Dialogue
    {
        public Texture2D speakerPortrait;

        public string displayText;
        public int drawTime;
        public int showTime;
        public string bracketColor;

        public bool openAnimation;
        public bool closeAnimation;

        /// <summary>
        /// Used to store information about a dialogue object
        /// </summary>
        /// <param name="displayText">The text to be displayed</param>
        /// <param name="drawTime">The amount of time it takes to completely draw the text</param>
        /// <param name="showTime">How long the text remains on screen after fully drawing</param>
        /// <param name="bracketColor">The hex color of the text when enclosed in brackets</param>
        public Dialogue(Texture2D speakerPortrait, string displayText, int drawTime, int showTime, string bracketColor, bool openAnimation, bool closeAnimation)
        {
            this.speakerPortrait = speakerPortrait;
            this.displayText = displayText;
            this.drawTime = drawTime;
            this.showTime = showTime;
            this.bracketColor = bracketColor;

            this.openAnimation = openAnimation;
            this.closeAnimation = closeAnimation;
        }
    }

    public class DialogueInstance
    {
        public List<Dialogue> DialogueList;

        public DialogueInstance()
        {

        }
    }

    public class DialoguePlayer : ModPlayer
    {
        public List<Dialogue> DialogueList = new List<Dialogue>();
        public bool ShowDialogue = false;

        public void AddDialogue(Texture2D speakerPortrait, string displayText, int drawtime, int showTime, string bracketColor = null, bool openAnimation = true, bool closeAnimation = true)
        {
            ShowDialogue = true;
            DialogueList.Add(new Dialogue(speakerPortrait, displayText, drawtime, showTime, bracketColor, openAnimation, closeAnimation));
        }
    }
}