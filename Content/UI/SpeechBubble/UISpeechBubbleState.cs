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
        public int fadeTime { get; private set; } = 60;

        public Queue<Text> speechBubble = new Queue<Text>();
        public BaseSpeechBubble() { }

        public void Add(Text text)
        {
            Main.NewText("added");
            speechBubble.Enqueue(text);
        }

        public void Update()
        {
            Main.NewText("update " + drawTime + " / " + holdTime);

            // The last text should not be removed, since we need to fade out the text
            if (speechBubble.Count == 1 && isFinished)
            {
                if (fadeTime > 0) fadeTime--;
                return;
            }

            Text currentText = speechBubble.Peek();
            if (drawTime < currentText.drawTime) drawTime++;
            else
            {
                if (holdTime < currentText.holdTime) holdTime++;
                else
                {
                    if (speechBubble.Count > 1)
                    {
                        speechBubble.Dequeue();
                        Main.NewText("dequeued");

                        drawTime = 0;
                        holdTime = 0;
                    }
                    else
                    {
                        isFinished = true;
                    }
                }
            }
        }

        public string GetText()
        {
            Text currentText = speechBubble.Peek();

            int progress = (int)MathHelper.Lerp(0, currentText.text.Length, drawTime / (float)currentText.drawTime);
            var text = currentText.text.Substring(0, progress);

            return text;
        }
    }

    public class UISpeechBubbleState : UIState
    {
        private Dictionary<int, BaseSpeechBubble> SpeechInstances = new Dictionary<int, BaseSpeechBubble>();

        public void AddSpeechBubble(NPC npc, BaseSpeechBubble text)
        {
            if (!SpeechInstances.ContainsKey(npc.whoAmI)) SpeechInstances.Add(npc.whoAmI, text);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.gamePaused) return;

            List<int> RemovalIndices = new List<int>();

            foreach (KeyValuePair<int, BaseSpeechBubble> instance in SpeechInstances)
            {
                BaseSpeechBubble speech = instance.Value;

                // Queue for removal from Dictionary if NPC is not active
                if (!Main.npc[instance.Key].active) RemovalIndices.Add(instance.Key);

                speech.Update();

                // Remove from Dictionary if dialogue has finished
                if (speech.isFinished && speech.fadeTime <= 0) RemovalIndices.Add(instance.Key);
            }

            foreach (int index in RemovalIndices) SpeechInstances.Remove(index);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (KeyValuePair<int, BaseSpeechBubble> instance in SpeechInstances)
            {
                if (!Main.npc[instance.Key].active) continue;

                NPC npc = Main.npc[instance.Key];
                BaseSpeechBubble speech = instance.Value;

                if (speech.speechBubble.Count == 0)
                {
                    Main.NewText("error somehow no text at all?");
                    continue;
                }

                float alpha = MathHelper.Lerp(0f, 1f, speech.fadeTime / 60f);

                // Draw current dialogue above the NPC
                TextSnippet[] snippets = ChatManager.ParseMessage(speech.GetText(), Color.White).ToArray();
                ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, npc.getRect().TopLeft() - new Vector2(npc.width + speech.GetText().Length * 1.25f, snippets.Length * 28) - Main.screenPosition, Color.White * alpha, 0f, Vector2.Zero, Vector2.One * 0.9f, out _, 150);
            }

            base.Draw(spriteBatch);
        }
    }
}