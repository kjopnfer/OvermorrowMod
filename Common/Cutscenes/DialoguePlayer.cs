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

namespace OvermorrowMod.Common.Cutscenes
{
    // Should be able to push dialogue that will constantly loop through each and pop them from the list
    // Dialogue should be able to be cleared and then a new one added if it is urgent
    public class Dialogue
    {
        public string speakerName;
        public string displayText;
        public int drawTime;
        public int showTime;

        public Color speakerColor;

        /// <summary>
        /// Used to store information about a dialogue object
        /// </summary>
        /// <param name="speakerName">The name of the speaker</param>
        /// <param name="displayText">The text to be displayed</param>
        /// <param name="drawTime">The amount of time it takes to completely draw the text</param>
        /// <param name="showTime">How long the text remains on screen after fully drawing</param>
        public Dialogue(string speakerName, string displayText, int drawTime, int showTime, Color speakerColor)
        {
            this.speakerName = speakerName;
            this.displayText = displayText;
            this.drawTime = drawTime;
            this.showTime = showTime;
            this.speakerColor = speakerColor;
        }
    }

    public class DialoguePlayer : ModPlayer
    {
        public List<Dialogue> DialogueList = new List<Dialogue>();
        public bool ShowDialogue = false;

        public void AddDialogue(string speakerName, string displayText, int drawtime, int showTime, Color speakerColor)
        {
            ShowDialogue = true;

            // Process the display text
            const int MAX_CHARACTERS = 40;
            string[] words = displayText.Split(new string[] { " " }, StringSplitOptions.None);
            var stringBuilder = new StringBuilder();

            int characterCount = 0;
            foreach (string word in words)
            {
                // We add +1 to the words to account for the spaces inbetween words.
                if (characterCount + word.Length + 1 > MAX_CHARACTERS) // We've reached the maximum limit of characters so we enter a new line
                {
                    stringBuilder.Append("\n" + word + " ");
                    characterCount = 0;
                }
                else
                {
                    characterCount += word.Length + 1;
                    stringBuilder.Append(word + " ");
                }
            }

            DialogueList.Add(new Dialogue(speakerName, stringBuilder.ToString(), drawtime, showTime, speakerColor));
        }
    }
}