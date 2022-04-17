using OvermorrowMod.Common.Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests
{
    public class QuestPlayer : ModPlayer
    {
        public string PlayerUUID { get; private set; } = null;

        private readonly List<BaseQuest> activeQuests = new List<BaseQuest>();
        public HashSet<string> CompletedQuests { get; } = new HashSet<string>();

        public IEnumerable<BaseQuest> CurrentQuests => activeQuests.Concat(Quests.PerPlayerActiveQuests[PlayerUUID]);

        public HashSet<string> LocalCompletedQuests { get; } = new HashSet<string>();

        public bool IsDoingQuest(string questId)
        {
            return CurrentQuests.Any(q => q.QuestId == questId);
        }

        public void AddQuest(BaseQuest quest)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer == Player)
                NetworkMessageHandler.Quests.TakeQuest(-1, -1, quest.QuestId);
            if (quest.Repeatability == QuestRepeatability.OncePerWorldPerPlayer || quest.Repeatability == QuestRepeatability.OncePerWorld)
            {
                Quests.PerPlayerActiveQuests[PlayerUUID].Add(quest);
            }
            else
            {
                activeQuests.Add(quest);
            }
        }

        public void RemoveQuest(BaseQuest quest)
        {
            activeQuests.Remove(quest);
            Quests.PerPlayerActiveQuests[PlayerUUID].Remove(quest);
        }

        public BaseQuest QuestByNpc(int npcId)
        {
            return CurrentQuests.FirstOrDefault(q => npcId == q.QuestGiver);
        }

        public void CompleteQuest(string questId)
        {
            var quest = CurrentQuests.FirstOrDefault(q => q.QuestId == questId);
            // Should not happen!
            if (quest == null) throw new ArgumentException($"Player is not doing {questId}");
            // Send message to server if the quest is being completed for the current player
            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer == Player)
                NetworkMessageHandler.Quests.CompleteQuest(-1, -1, questId);

            quest.CompleteQuest(Player, true);
            RemoveQuest(quest);
        }
        public override void SaveData(TagCompound tag)
        {
            tag["CompletedQuests"] = CompletedQuests.ToList();
            tag["CurrentQuests"] = activeQuests.Select(q => q.QuestId).ToList();
            tag["PlayerUUID"] = PlayerUUID;
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            CompletedQuests.Clear();
            activeQuests.Clear();

            var completedQuests = tag.GetList<string>("CompletedQuests");
            foreach (var quest in completedQuests)
            {
                if (!Quests.QuestList.TryGetValue(quest, out var qInst) || qInst.Repeatability != QuestRepeatability.OncePerPlayer)
                    continue;
                CompletedQuests.Add(quest);
            }

            var currentQuests = tag.GetList<string>("CurrentQuests");
            foreach (var questId in currentQuests)
            {
                if (Quests.QuestList.TryGetValue(questId, out var quest)
                    && (quest.Repeatability == QuestRepeatability.Repeatable || quest.Repeatability == QuestRepeatability.OncePerPlayer))
                {
                    activeQuests.Add(quest);
                }
            }

            PlayerUUID = tag.GetString("PlayerUUID");
            if (PlayerUUID == null) PlayerUUID = Guid.NewGuid().ToString();

            if (!Quests.PerPlayerCompletedQuests.ContainsKey(PlayerUUID))
            {
                Quests.PerPlayerCompletedQuests[PlayerUUID] = new HashSet<string>();
            }
            if (!Quests.PerPlayerActiveQuests.ContainsKey(PlayerUUID))
            {
                Quests.PerPlayerActiveQuests[PlayerUUID] = new List<BaseQuest>();
            }
        }
    }
}
