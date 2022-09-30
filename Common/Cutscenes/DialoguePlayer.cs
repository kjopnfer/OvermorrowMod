using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace OvermorrowMod.Common.Cutscenes
{
    public class Popup
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
    }

    public class Dialogue
    {
        public Texture2D speakerBody;

        public string displayText;
        public int drawTime;

        public string bracketColor;
        public List<OptionButton> options;

        public Dialogue(Texture2D speakerBody, string displayText, int drawTime, string bracketColor, List<OptionButton> options = null)
        {
            this.speakerBody = speakerBody;
            this.displayText = displayText;
            this.drawTime = drawTime;

            this.bracketColor = bracketColor;
            this.options = options;
        }

        public void AddOptions(OptionButton option)
        {
            if (options.Count < 4) options.Add(option);
        }
    }


    public class DialoguePlayer : ModPlayer
    {
        private Queue<Popup> PopupQueue = new Queue<Popup>();
        private Dialogue CurrentDialogue;

        public bool AddedDialogue = false;

        public bool pickupWood = false;
        public bool distanceGuide = false;
        public bool guideGreeting = false;

        public void SetDialogue(Texture2D speakerBody, string displayText, int drawTime, List<OptionButton> options)
        {
            CurrentDialogue = new Dialogue(speakerBody, displayText, drawTime, Color.White.Hex3(), options);
        }

        public void SetDialogue(Texture2D speakerBody, string displayText, int drawTime, Color color)
        {
            CurrentDialogue = new Dialogue(speakerBody, displayText, drawTime, color.Hex3());
        }

        public void SetDialogue(Dialogue dialogue)
        {
            CurrentDialogue = dialogue;
        }

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