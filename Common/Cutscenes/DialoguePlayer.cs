using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using System.Xml;
using Terraria.ModLoader.IO;
using System.Linq;
using Terraria.GameInput;
using OvermorrowMod.Core;
using OvermorrowMod.Quests;
using System.Text;
using Terraria.ID;
using Terraria.Audio;

namespace OvermorrowMod.Common.Cutscenes
{
    public partial class DialoguePlayer : ModPlayer
    {
        /// <summary>
        /// The Popup Queue represents global instances and are not tied to NPCs.
        /// <para>This is used for Popup based conversations and events, where multiple characters are chained together.</para>
        /// </summary>
        private Queue<Popup> PopupQueue = new Queue<Popup>();
        private Dialogue CurrentDialogue;

        //public Dictionary<int, Popup> NPCPopups = new Dictionary<int, Popup>();
        public Dictionary<int, PopupState> PopupStates = new Dictionary<int, PopupState>();

        // Used to store any flags triggered by the player when speaking to NPCs
        public HashSet<string> DialogueFlags = new HashSet<string>();

        public bool AddedDialogue = false;
        public bool LockPlayer = false;

        public override void SaveData(TagCompound tag)
        {
            tag["DialogueFlags"] = DialogueFlags.ToList();
            //tag["kittFirst"] = kittFirst;
        }

        public override void LoadData(TagCompound tag)
        {
            DialogueFlags = tag.Get<List<string>>("DialogueFlags").ToHashSet();
            //kittFirst = tag.Get<bool>("kittFirst");
        }


        public void AddNPCPopup(int id, XmlDocument xmlDoc)
        {
            if (PopupStates.ContainsKey(id))
                PopupStates[id].ReplacePopup(new Popup(xmlDoc));
            else
                PopupStates.Add(id, new PopupState(new Popup(xmlDoc)));
            //PopupStates[id] = new PopupState(new Popup(xmlDoc));
        }

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

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!LockPlayer) return;

            Player player = Main.LocalPlayer;

            if (!player.mount.Active || !player.mount.Cart)
            {
                if (triggersSet.Grapple)
                {
                    triggersSet.Grapple = false;
                    player.controlHook = false;
                }

                if (triggersSet.Up)
                {
                    triggersSet.Up = false;
                    player.controlUp = false;
                }

                if (triggersSet.Down)
                {
                    triggersSet.Down = false;
                    player.controlDown = false;
                }

                if (triggersSet.Left)
                {
                    triggersSet.Left = false;
                    player.controlLeft = false;
                }

                if (triggersSet.Right)
                {
                    triggersSet.Right = false;
                    player.controlRight = false;
                }

                if (triggersSet.Jump)
                {
                    triggersSet.Jump = false;
                    player.controlJump = false;
                }
            }
        }

        private void UpdatePopupStates()
        {
            foreach (var popupState in PopupStates.Values)
            {
                popupState.Update();
            }
        }

        private void RemovePopupStates()
        {
            List<int> removedIndices = new List<int>();

            foreach (KeyValuePair<int, PopupState> popupState in PopupStates)
            {
                if (popupState.Value.CanBeRemoved) removedIndices.Add(popupState.Key);
            }

            foreach (int index in removedIndices)
            {
                PopupStates.Remove(index);
            }
        }

        public override void PreUpdate()
        {
            UpdatePopupStates();
            RemovePopupStates();

            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            DialoguePlayer dialoguePlayer = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();


            if (/*!dialoguePlayer.guideGreeting*/ greetCounter % 480 == 0)
            {
                //XmlDocument doc = ModUtils.GetXML(AssetDirectory.Popup + "GuideGreeting.xml");
                XmlDocument doc = ModUtils.GetXML(AssetDirectory.Popup + "GuideCampAxe.xml");

                dialoguePlayer.AddNPCPopup(NPCID.Guide, doc);
                //dialoguePlayer.AddPopup(doc);
                dialoguePlayer.guideGreeting = true;
            }
            else
            {
                if (greetCounter >= 420)
                {
                    unlockedGuideCampfire = true;
                }
            }

            greetCounter++;

            /*if (questPlayer.FindActiveQuest("GuideCampfire"))
            {
                if (guideCampfireCounter++ == 30)
                {
                    XmlDocument doc = ModUtils.GetXML(AssetDirectory.Popup + "GuideCampAxe.xml");
                    dialoguePlayer.AddPopup(doc);
                }
            }*/

            base.PreUpdate();
        }
    }

    public class PopupState
    {
        // if these are const then the fucking uistate cant read them
        public readonly float DIALOGUE_DELAY = 30;
        public readonly float OPEN_TIME = 15;
        public readonly float CLOSE_TIME = 10;

        public bool CanOpen { get; private set; } = true;
        public bool CanClose { get; private set; } = false;
        public bool CanBeRemoved { get; private set; } = false;

        public int OpenTimer { get; private set; }
        public int CloseTimer { get; private set; }

        public Popup Popup { get; private set; }

        public PopupState(Popup popup)
        {
            Popup = popup;
        }

        public void ReplacePopup(Popup popup)
        {
            Popup = popup;
        }

        public void Update()
        {        
            if (CanOpen)
            {
                // This should only run once per state instance.
                // Allows Popups of a state to override each other but not repeat the same opening animation if they do.

                if (OpenTimer == 0)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/PopupShow")
                    {
                        Volume = 1.25f,
                        PitchVariance = 1.1f,
                        MaxInstances = 2,
                    }, Main.LocalPlayer.Center);
                }

                if (OpenTimer < OPEN_TIME) OpenTimer++;
                if (OpenTimer == OPEN_TIME)
                {
                    if (Popup.DelayTimer < DIALOGUE_DELAY) Popup.DelayTimer++;
                    if (Popup.DelayTimer == DIALOGUE_DELAY) CanOpen = false;
                }
            }
            else
            {
                //Main.NewText(Popup.DrawTimer + " / " + Popup.GetDrawTime());

                if (Popup.DrawTimer < Popup.GetDrawTime()) // Draws the text for the specified time
                {
                    if (Popup.DrawTimer == 0)
                    {
                        if (!SoundEngine.TryGetActiveSound(Popup.drawSound, out var result))
                        {
                            Popup.drawSound = SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/DialogueDraw")
                            {
                                Volume = 1.25f,
                                PitchVariance = 1.1f,
                                MaxInstances = 2,
                            }, Main.LocalPlayer.Center);
                        }
                    }

                    Popup.DrawTimer++;
                }
                else if (Popup.DisplayTimer < Popup.GetDisplayTime()) // Holds the text for the specified time
                {
                    if (SoundEngine.TryGetActiveSound(Popup.drawSound, out var result)) result.Stop();
                    Popup.DisplayTimer++;
                }
                else if (Popup.GetNodeIteration() < Popup.GetListLength() - 1) // If there are any nodes left, reset timers and go to next
                {
                    Popup.GetNextNode();
                }

                //Main.NewText(Popup.DisplayTimer + " / " + Popup.GetDisplayTime());
                if (Popup.ShouldClose() && Popup.DisplayTimer >= Popup.GetDisplayTime()) // If there are no nodes left and it has finished displaying
                {
                    Main.NewText("can close");
                    CanClose = true;
                }
            }

            if (CanClose)
            {

                if (CloseTimer < CLOSE_TIME) CloseTimer++;
                if (CloseTimer == CLOSE_TIME)
                {
                    Main.NewText("flag removal");
                    CanBeRemoved = true;
                }
            }
        }

        public string GetPopupText()
        {
            int progress = (int)MathHelper.Lerp(0, Popup.GetText().Length, Popup.DrawTimer / (float)Popup.GetDrawTime());
            var text = Popup.GetText().Substring(0, progress);

            // If for some reason there are no colors specified don't parse the brackets
            if (Popup.GetColorHex() != null)
            {
                // The number of opening brackets MUST be the same as the number of closing brackets
                int numOpen = 0;
                int numClose = 0;

                // Create a new string, adding in hex tags whenever an opening bracket is found
                var builder = new StringBuilder();
                builder.Append("    "); // Appends to the beginning of the string

                foreach (var character in text)
                {
                    if (character == '[') // Insert the hex tag if an opening bracket is found
                    {
                        builder.Append("[c/" + Popup.GetColorHex() + ":");
                        numOpen++;
                    }
                    else
                    {
                        if (character == ']')
                        {
                            numClose++;
                        }

                        builder.Append(character);
                    }
                }

                if (numOpen != numClose)
                {
                    builder.Append(']');
                }

                // Final check for if the tag has two brackets but no characters inbetween
                var hexTag = "[c/" + Popup.GetColorHex() + ":]";
                if (builder.ToString().Contains(hexTag))
                {
                    builder.Replace(hexTag, "[c/" + Popup.GetColorHex() + ": ]");
                }

                text = builder.ToString();
            }

            return text;
        }

        public Texture2D GetPopupFace()
        {
            return Popup.GetPortrait();
        }
    }
}