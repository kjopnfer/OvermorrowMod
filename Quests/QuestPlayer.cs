using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public class QuestPlayer : ModPlayer
    {
        private readonly List<BaseQuest> activeQuests = new List<BaseQuest>();
        public HashSet<int> CompletedQuests { get; } = new HashSet<int>();

        public IEnumerable<BaseQuest> CurrentQuests => activeQuests;

        public bool IsDoingQuest(int questId)
        {
            return activeQuests.Any(q => q.QuestId == questId);
        }

        public void AddQuest(BaseQuest quest)
        {
            activeQuests.Add(quest);
        }

        public BaseQuest QuestByNpc(int npcId)
        {
            return activeQuests.FirstOrDefault(q => npcId == q.QuestGiver);
        }

        public void CompleteQuest(int questId)
        {
            var quest = activeQuests.FirstOrDefault(q => q.QuestId == questId);
            // Should not happen!
            if (quest == null) throw new ArgumentException($"Player is not doing {questId}");
            quest.CompleteQuest(player, true);
            activeQuests.Remove(quest);
        }
    }
}
