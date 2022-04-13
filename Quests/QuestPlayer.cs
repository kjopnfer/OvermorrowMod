using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public class ActiveQuest
    {
        public BaseQuest Quest { get; }
        public int NpcId { get; }
        public ActiveQuest(BaseQuest quest, int npcId)
        {
            Quest = quest;
            NpcId = npcId;
        }
    }
    public class QuestPlayer : ModPlayer
    {
        private List<ActiveQuest> activeQuests = new List<ActiveQuest>();

        public bool IsDoingQuest(int questId)
        {
            return activeQuests.Any(q => q.Quest.QuestId == questId);
        }

        public void AddQuest(int npcId, BaseQuest quest)
        {
            activeQuests.Add(new ActiveQuest(quest, npcId));
        }

        public BaseQuest QuestByNpc(int npcId)
        {
            return activeQuests.FirstOrDefault(q => npcId == q.NpcId)?.Quest;
        }

        public void CompleteQuest(int questId)
        {
            var quest = activeQuests.FirstOrDefault(q => q.Quest.QuestId == questId);
            // Should not happen!
            if (quest == null) throw new ArgumentException($"Player is not doing {questId}");
            quest.Quest.GiveRewards(player);
            activeQuests.Remove(quest);
        }
    }
}
