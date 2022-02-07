using System.Collections.Generic;
using Terraria;
using Terraria.ID;

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
            TutorialGuideQuest = 1,
            GuideHouseQuest = 2
        }

        private List<IQuestRequirement> QuestRequirements = new List<IQuestRequirement>();
        public bool IsCompleted = false;

        public virtual List<string> QuestDialogue => null;
        //public virtual List<ItemInfo> QuestRewards => new List<ItemInfo>();
        public virtual (int, int)[] QuestRewards => null;
        public virtual List<string> HintDialogue => null;

        public virtual void SetDefaults() { }
        public virtual string QuestName() => "";
        public virtual int QuestGiver() => -1;
        public virtual int QuestID() => -1;
        public virtual string GetDialogue(int index) => QuestDialogue[index];

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
            Main.NewText("bruh" + QuestRewards[0].Item);

            foreach (ItemInfo info in QuestRewards)
            {
                Main.NewText("gave " + info.Stack);
                player.QuickSpawnItem(info.Item, info.Stack);
            }
        }*/


        /*public void AddReward(int ItemType, int ItemStack)
        {
            QuestRewards.Add(new ItemInfo(ItemType, ItemStack));
        }*/

        public string GetHint(int index) => HintDialogue[index];
        public void AddRequirement(IQuestRequirement requirement)
        {
            QuestRequirements.Add(requirement);
        }

        public bool CheckCompleted(Player player, int ID)
        {
            bool result = true;

            switch (ID)
            {
                case (int)Quest.ID.GuideHouseQuest:
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].type == NPCID.Guide && Main.npc[i].homeless)
                        {
                            result = false;
                        }
                    }
                    break;
                default:
                    foreach (IQuestRequirement requirement in QuestRequirements)
                    {
                        if (!requirement.CheckCompleted(player))
                        {
                            result = false;
                            Main.NewText(requirement.GetRequirementText());
                            break;
                        }
                    }
                    break;
            }

            return result;
        }
    }

    public class ItemRequirement : IQuestRequirement
    {
        public int ItemType;
        public int ItemStack;
        public ItemRequirement(int Type, int Stack)
        {
            ItemType = Type;
            ItemStack = Stack;
        }

        public bool CheckCompleted(Player player)
        {
            bool found = false;
            int currentStack = 0;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.type == ItemType)
                {
                    found = true;
                    currentStack += item.stack;
                }

                if (found && currentStack >= ItemStack)
                {
                    return true;
                }
            }
            return false;
        }

        public string GetRequirementText()
        {
            return "#" + ItemStack + " " + Lang.GetItemNameValue(ItemType);
        }
    }
}