using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Dialogue
{
    public class DialogueWindow
    {
        // Choices can be optional, if none specified: they will close the dialogue window
        // Dialogue nodes should have an onClose method or something to assign quests or do other actions
        protected virtual DialogueNode[] Dialogue { get; set; }

        public DialogueWindow() { }

        private Text[] GetDialogue(string id) => Dialogue.Where(node => node.id == id).First().texts;
        public Text[] GetTexts(string id) => GetDialogue(id);
        public Text GetText(string id, int textIterator) => GetTexts(id)[textIterator];

        public DialogueChoice[] GetOptions(string id) => Dialogue.Where(node => node.id == id).First().choice;


        /*new DialogueNode(
              "start",
              new Text[]
              {
                  new Text("text", drawTime, holdTime),
                  new Text("text", drawTime, holdTime),
              }
          ),
          new DialogueNode(
              "second",
              new Text[]
              {
                  new Text("text", drawTime, holdTime),
                  new Text("text", drawTime, holdTime),
              }
          ),
          new DialogueNode(
              "third",
              new Text[]
              {
                  new Text("text", drawTime, holdTime),
                  new Text("text", drawTime, holdTime),
              }
          ),
        */
        // Needs to be able to take a Texture to represent the NPC

        // Text must specify the draw time
        // Needs to display icons for the dialogue options


        // Needs to be able to support "actions" to:
        // Assign quests
        // Spawn NPCs
        // etc. all done dynamically to allow the user to specify the end action
    }

    public class DialogueChoice
    {
        public string text { get; }

        /// <summary>
        /// The ID of the DialogueNode the choice should direct to
        /// </summary>
        public string link { get; }

        public Texture2D texture { get; }

        public DialogueChoice(Texture2D texture, string text, string link)
        {
            this.texture = texture;
            this.text = text;
            this.link = link;
        }

        public DialogueChoice(string text, string link = null)
        {
            this.texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_ChatIcon").Value;
            this.text = text;
            this.link = link;
        }
    }

    public class DialogueNode
    {
        public string id { get; }
        public Text[] texts { get; }
        public DialogueChoice[] choice { get; }

        public DialogueNode(string id, Text[] texts, DialogueChoice[] choice = null)
        {
            this.id = id;
            this.texts = texts;
            this.choice = choice;
        }
    }
}