using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria;
using System.Xml;
using Terraria.ModLoader.IO;
using System.Linq;
using Terraria.GameInput;
using OvermorrowMod.Quests;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Common.Dialogue;

namespace OvermorrowMod.Common.Cutscenes
{
    public partial class DialoguePlayer : ModPlayer
    {
        private DialogueWindow CurrentWindow;

        /// <summary>
        /// This contains Popup states based on an ID, usually the NPC's ID.
        /// </summary>
        public Dictionary<int, PopupState> PopupStates = new Dictionary<int, PopupState>();

        /// <summary>
        /// This contains Popup states that are being sent to the PopupStates dictionary but is not ready for the Popup yet.
        /// <para>This occurs when a Popup is closing and we don't want a Popup to be added at that time.</para>
        /// </summary>
        private Dictionary<int, Popup> QueuedPopups = new Dictionary<int, Popup>();

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

        /// <summary>
        /// Adds a Popup instance into a Dictionary with the given ID (usually the NPC's ID).
        /// If nodeId is not specified, the first Popup found within the file is used.
        /// </summary>
        /// <param name="npcID"></param>
        /// <param name="xmlDoc"></param>
        /// <param name="nodeID"></param>
        public void AddNPCPopup(int npcID, XmlDocument xmlDoc, string nodeID = null)
        {
            if (PopupStates.ContainsKey(npcID))
            {
                if (!PopupStates[npcID].CanClose) // Replace the current Popup if it is not in a closed state
                {
                    PopupStates[npcID].ReplacePopup(new Popup(xmlDoc, nodeID));
                }
                else // Otherwise, if it is in a closing state then add it into a Queue that will be given to a new state when ready
                {
                    if (!QueuedPopups.ContainsKey(npcID)) QueuedPopups.Add(npcID, new Popup(xmlDoc, nodeID));
                    else QueuedPopups[npcID] = new Popup(xmlDoc, nodeID);
                }
            }
            else
                PopupStates.Add(npcID, new PopupState(new Popup(xmlDoc, nodeID)));
        }

        public bool CheckPopupAlreadyActive(int npcID) => PopupStates.ContainsKey(npcID);


        public void LoadDialogueWindow(DialogueWindow window)
        {
            CurrentWindow = window;
        }

        public void ClearWindow() => CurrentWindow = null;

        public DialogueWindow GetDialogueWindow() => CurrentWindow;

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

        private void UpdatePopupQueue()
        {
            List<int> DequeuedPopups = new List<int>();

            foreach (KeyValuePair<int, Popup> popup in QueuedPopups)
            {
                if (!PopupStates.ContainsKey(popup.Key))
                {
                    PopupStates.Add(popup.Key, new PopupState(popup.Value));
                    DequeuedPopups.Add(popup.Key);
                }
            }

            foreach (int popup in DequeuedPopups)
            {
                QueuedPopups.Remove(popup);
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
            UpdatePopupQueue();
            UpdatePopupStates();
            RemovePopupStates();
            GeneralUpdateDialogue();

            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            DialoguePlayer dialoguePlayer = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (!dialoguePlayer.guideGreeting && greetCounter == 480)
            {
                //XmlDocument doc = ModUtils.GetXML(AssetDirectory.Popup + "GuideGreeting.xml");
                /*XmlDocument doc = ModUtils.GetXML(AssetDirectory.Popup + "GuideCampAxe.xml");

                dialoguePlayer.AddNPCPopup(NPCID.Guide, doc);*/
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

            base.PreUpdate();
        }
    }

}