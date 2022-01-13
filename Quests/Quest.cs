using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Quests
{
    public abstract class Quest
    {
        public virtual List<string> QuestDialogue => null;
        public virtual string QuestName() => "";
        public virtual int QuestGiver() => -1;
        public virtual string GetDialogue(int index) => QuestDialogue[index];
        public virtual bool QuestRequirements() => false;
        public virtual void GiveReward() { }
    }
}