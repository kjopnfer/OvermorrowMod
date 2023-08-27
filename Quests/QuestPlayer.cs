using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Netcode;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.NPCs;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.State;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests
{
    public partial class QuestPlayer : ModPlayer
    {
        public string PlayerUUID { get; private set; } = Guid.NewGuid().ToString();

        public string SelectedLocation { get; set; } = null;

        public IEnumerable<BaseQuestState> CurrentQuests => Quests.State.GetActiveQuests(this);

        public bool IsDoingQuest(string questId)
        {
            return Quests.State.IsDoingQuest(this, questId);
        }

        public void AddQuest(BaseQuest quest)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer == Player)
                NetworkMessageHandler.Quests.TakeQuest(-1, -1, quest.QuestID);

            Quests.State.AddQuest(this, quest);
        }

        /// <summary>
        /// Checks whether the player has completed the given quest before
        /// </summary>
        /// <typeparam name="ModQuest"></typeparam>
        /// <returns></returns>
        public bool HasCompletedQuest<ModQuest>() where ModQuest : BaseQuest
        {
            var completedQuests = Quests.State.GetPerPlayerQuests(this).Where(q => q.Quest is ModQuest).Where(q => q.Completed);
            return completedQuests.Count() > 0;
        }

        public BaseQuestState QuestByNPC(int npcId)
        {
            return CurrentQuests.FirstOrDefault(q => npcId == q.Quest.QuestGiver);
        }

        public void CompleteQuest(string questId)
        {
            var quest = CurrentQuests.FirstOrDefault(q => q.Quest.QuestID == questId);
            // Should not happen!
            if (quest == null) throw new ArgumentException($"Player is not doing {questId}");
            // Send message to server if the quest is being completed for the current player
            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer == Player)
                NetworkMessageHandler.Quests.CompleteQuest(-1, -1, questId);

            quest.Quest.CompleteQuest(Player, true);
        }

        public void TickQuestRequirements(string questId)
        {
            var quest = CurrentQuests.FirstOrDefault(q => q.Quest.QuestID == questId);
            foreach (var req in quest.Quest.Requirements)
            {
                req.TryCompleteRequirement(this, quest);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["questStates"] = Quests.State.GetPerPlayerQuests(this).ToList();
            tag["PlayerUUID"] = PlayerUUID;
            tag["grabbedAxe"] = grabbedAxe;
            tag["showCampfireArrow"] = showCampfireArrow;

            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            grabbedAxe = tag.GetBool("grabbedAxe");
            showCampfireArrow = tag.GetBool("showCampfireArrow");

            PlayerUUID = tag.GetString("PlayerUUID");
            if (PlayerUUID == null) PlayerUUID = Guid.NewGuid().ToString();

            var questStates = tag.GetList<TagCompound>("questStates");
            Quests.State.LoadPlayer(this, questStates);
        }

        private int markerCounter = 0;
        public override void PreUpdate()
        {
            UpdateTravelMarkers();
            AutoCompleteRequirements();
        }

        /// <summary>
        /// Determines whether the player has an active Quest of the specified name.
        /// <para>Based off of the internal class name for the Quest.</para>
        /// </summary>
        public bool FindActiveQuest(string name)
        {
            foreach (var quest in CurrentQuests)
            {
                string questID = quest.Quest.QuestID.Split("OvermorrowMod.Quests.ModQuests.")[1];
                if (questID == name) return true;
            }

            return false;
        }
    }
}
