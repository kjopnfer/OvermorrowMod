using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.UI.Chat;
using System;
using OvermorrowMod.Core.Interfaces;

namespace OvermorrowMod.Content.UI.SpeechBubble
{
    public class Text
    {
        public string text { get; }
        public int drawTime { get; }
        public int holdTime { get; }

        public Text(string text, int drawTime, int holdTime)
        {
            this.text = text;
            this.drawTime = drawTime;
            this.holdTime = holdTime;
        }
    }

    public class BaseSpeechBubble
    {
        public bool isFinished = false;
        private int drawTime = 0;
        private int holdTime = 0;
        private int fadeTime { get; } = 60;

        public Queue<Text> speechBubble = new Queue<Text>();
        public BaseSpeechBubble() { }

        public void Update()
        {
            if (speechBubble.Count == 0)
            {
                isFinished = true;
                return;
            }

            Text currentText = speechBubble.Peek();

            if (drawTime < currentText.drawTime)
            {

            }
            else
            {
                if (holdTime < currentText.holdTime)
                {

                }
            }
        }

        public string GetText()
        {
            return "";
        }
    }

    public class UISpeechBubbleState : UIState
    {
        private Dictionary<int, BaseSpeechBubble> SpeechInstances = new Dictionary<int, BaseSpeechBubble>();

        public void AddSpeechBubble(NPC npc, BaseSpeechBubble text)
        {
            if (!SpeechInstances.ContainsKey(npc.type))
            {
                SpeechInstances.Add(npc.type, text);
            }
        }

        public override void Update(GameTime gameTime)
        {
            List<int> RemovalIndices = new List<int>();

            foreach (KeyValuePair<int, BaseSpeechBubble> instance in SpeechInstances)
            {
                BaseSpeechBubble speech = instance.Value;

                // Queue for removal from Dictionary if NPC is not active
                if (!Main.npc[instance.Key].active) RemovalIndices.Add(instance.Key);

                // Update counters
                speech.Update();

                // Remove from Dictionary if dialogue has finished
                if (speech.isFinished)
                {
                    RemovalIndices.Add(instance.Key);
                }

                // Else move to next dialogue
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (KeyValuePair<int, BaseSpeechBubble> instance in SpeechInstances)
            {
                if (!Main.npc[instance.Key].active) continue;

                NPC npc = Main.npc[instance.Key];
                BaseSpeechBubble speech = instance.Value;

                // Draw current dialogue above the NPC
                TextSnippet[] snippets = ChatManager.ParseMessage(speech.GetText(), Color.White).ToArray();
                ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, npc.getRect().TopLeft(), Color.White, 0f, Vector2.Zero, Vector2.One * 0.9f, out _, 150);
            }

            base.Draw(spriteBatch);
        }
    }
}