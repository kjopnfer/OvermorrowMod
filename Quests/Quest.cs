using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Quests
{
    public abstract class Quest
    {
        public struct ItemInfo
        {
            public int Stack;
            public int Item;

            public ItemInfo(int Type, int Stack = 1)
            {
                Item = Type;
                this.Stack = Stack;
            }
        }
        public enum ID
        {
            TutorialGuideQuest = 1
        }

        public bool IsCompleted = false;
        public virtual List<string> QuestDialogue => null;
        //public virtual List<ItemInfo> QuestRewards => null;
        public virtual (int, int)[] QuestRewards => null;

        public virtual void SetDefaults() { }
        public virtual string QuestName() => "";
        public virtual int QuestGiver() => -1;
        public virtual int QuestID() => -1;
        public virtual string GetDialogue(int index) => QuestDialogue[index];
        public virtual bool QuestRequirements() => false;

        /// <summary>
        /// Adds a reward to the Quest, stored in <c>Rewards</c>
        /// </summary>
        /// <param name="ItemType"></param>
        /// <param name="ItemStack"></param>
        /*public void AddReward(int ItemType, int ItemStack)
        {
            QuestRewards.Add(new ItemInfo(ItemType, ItemStack));
        }*/

        /// <summary>
        /// Quickspawns the rewards set by the given Quest for the player
        /// </summary>
        /// <param name="player"></param>
        public void GiveRewards(Player player)
        {
            foreach ((int, int) Reward in QuestRewards)
            {
                player.QuickSpawnItem(Reward.Item1, Reward.Item2);
            }
        }
        /*public void GiveRewards(Player player)
        {
            foreach (ItemInfo info in QuestRewards)
            {
                player.QuickSpawnItem(info.Item, info.Stack);
            }
        }*/
    }
}