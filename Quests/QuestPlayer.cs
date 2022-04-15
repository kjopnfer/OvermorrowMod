using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests
{
    public class QuestPlayer : ModPlayer
    {
        private readonly List<BaseQuest> activeQuests = new List<BaseQuest>();
        public HashSet<string> CompletedQuests { get; } = new HashSet<string>();

        public IEnumerable<BaseQuest> CurrentQuests => activeQuests;

        public bool IsDoingQuest(string questId)
        {
            return activeQuests.Any(q => q.QuestId == questId);
        }

        public void AddQuest(BaseQuest quest)
        {
            activeQuests.Add(quest);
        }

        public void RemoveQuest(BaseQuest quest)
        {
            activeQuests.Remove(quest);
        }

        public BaseQuest QuestByNpc(int npcId)
        {
            return activeQuests.FirstOrDefault(q => npcId == q.QuestGiver);
        }

        public void CompleteQuest(string questId)
        {
            var quest = activeQuests.FirstOrDefault(q => q.QuestId == questId);
            // Should not happen!
            if (quest == null) throw new ArgumentException($"Player is not doing {questId}");
            quest.CompleteQuest(player, true);
            activeQuests.Remove(quest);
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                ["CompletedQuests"] = CompletedQuests.ToList(),
                ["CurrentQuests"] = CurrentQuests.Select(q => q.QuestName).ToList()
            };
        }

        public override void Load(TagCompound tag)
        {
            CompletedQuests.Clear();
            activeQuests.Clear();

            var completedQuests = tag.GetList<string>("CompletedQuests");
            foreach (var quest in completedQuests) CompletedQuests.Add(quest);

            var currentQuests = tag.GetList<string>("CurrentQuests");
            foreach (var questId in currentQuests)
            {
                if (Quests.QuestList.TryGetValue(questId, out var quest))
                {
                    activeQuests.Add(quest);
                }
            }
        }
    }
}
