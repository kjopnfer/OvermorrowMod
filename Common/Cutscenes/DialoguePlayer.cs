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
        public Texture2D speakerPortrait;

        public string speakerName;
        public string displayText;
        public int drawTime;
        public int showTime;

        public Color speakerColor;
        public string bracketColor;

        /// <summary>
        /// Used to store information about a dialogue object
        /// </summary>
        /// <param name="speakerName">The name of the speaker</param>
        /// <param name="displayText">The text to be displayed</param>
        /// <param name="drawTime">The amount of time it takes to completely draw the text</param>
        /// <param name="showTime">How long the text remains on screen after fully drawing</param>
        /// <param name="speakerColor">The color of the speaker's name</param>
        /// <param name="bracketColor">The hex color of the text when enclosed in brackets</param>
        public Dialogue(Texture2D speakerPortrait, string speakerName, string displayText, int drawTime, int showTime, Color speakerColor, string bracketColor)
        {
            this.speakerPortrait = speakerPortrait;
            this.speakerName = speakerName;
            this.displayText = displayText;
            this.drawTime = drawTime;
            this.showTime = showTime;
            this.speakerColor = speakerColor;
            this.bracketColor = bracketColor;
        }
    }

    public class DialoguePlayer : ModPlayer
    {
        public List<Dialogue> DialogueList = new List<Dialogue>();
        public bool ShowDialogue = false;

        public void AddDialogue(Texture2D speakerPortrait, string speakerName, string displayText, int drawtime, int showTime, Color speakerColor, string bracketColor = null)
        {
            ShowDialogue = true;

            // Process the display text
            const int MAX_CHARACTERS = 28;
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

            // Post-processing tag parser that is probably not super efficient, but serves to support multi-line hex tags
            // THE CODE ASSUMES THE USER HAS USED THE TAGS CORRECTLY

            /// An example case:
            /// Lorem ipsum dolor sit amet, [consectetur adipiscing elit,
            /// sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
            /// Ut enim ad minim veniam, quis nostrud exercitation ullamco
            /// laboris nisi ut aliquip] ex ea commodo consequat.

            // The following code will parse each line that has already been split by new-line tags
            // When it finds an opening tag, it will add a closing bracket. Vice versa, if it finds a closing tag.
            // If within two tags that span multiple lines, it will flag itself as inbetween tags and enclose itself.

            string[] lineSplits = stringBuilder.ToString().Split(new string[] { "\n" }, StringSplitOptions.None);
            stringBuilder = new StringBuilder();
            bool inBracket = false;
            foreach (string line in lineSplits)
            {
                if (line.Contains('[') && !line.Contains(']'))
                {
                    Main.NewText("added ]");
                    stringBuilder.Append(line + "]\n");
                    inBracket = true;
                }

                if (inBracket && !line.Contains('[') && !line.Contains(']'))
                {
                    Main.NewText("added [ and ]");

                    stringBuilder.Append('[' + line + "]\n");
                }
                else
                {
                    if (!inBracket)
                    {
                        Main.NewText("added nothing");

                        stringBuilder.Append(line + "\n");
                    }
                }

                if (!line.Contains('[') && line.Contains(']') && inBracket)
                {
                    Main.NewText("added [");

                    stringBuilder.Append('[' + line + "\n");
                    inBracket = false;
                }
            }

            DialogueList.Add(new Dialogue(speakerPortrait, speakerName, stringBuilder.ToString(), drawtime, showTime, speakerColor, bracketColor));
        }

    }
}