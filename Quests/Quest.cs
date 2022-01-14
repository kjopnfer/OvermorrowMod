using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Quests
{
    public abstract class Quest
    {
        public enum ID
        {
            TutorialGuideQuest = 1
        }

        public bool IsCompleted = false;
        public virtual List<string> QuestDialogue => null;
        public virtual string QuestName() => "";
        public virtual int QuestGiver() => -1;
        public virtual int QuestID() => -1;
        public virtual string GetDialogue(int index) => QuestDialogue[index];
        public virtual bool QuestRequirements() => false;
        public virtual void GiveReward() { }
    }
}