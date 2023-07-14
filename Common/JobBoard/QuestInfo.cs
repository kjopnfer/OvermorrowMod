using OvermorrowMod.Quests;
using System.Collections.Generic;

namespace OvermorrowMod.Common.JobBoard
{
    public struct QuestTakerInfo
    {
        public string UUID { get; }
        public string Name { get; }
        public List<BaseQuest> Quests { get; } = new List<BaseQuest>();
        public QuestTakerInfo(string UUID, string Name)
        {
            this.UUID = UUID;
            this.Name = Name;
        }
    }

    public struct QuestInfo
    {
        public bool Taken = false;
        public BaseQuest Quest;

        public QuestInfo(BaseQuest Quest)
        {
            this.Quest = Quest;
        }
    }

    public enum QuestStatus
    {
        Unclaimed = 0,
        InProgress = 1,
        Completed = 2
    }
}