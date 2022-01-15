using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Quests
{

    /*public abstract class Quest
    {
        public struct Requirement
        {
            public int ItemID;
            public int Stack;
            public bool Satisfied;

            public Requirement(int ItemID, int Stack, bool Satisfied)
            {
                this.ItemID = ItemID;
                this.Stack = Stack;
                this.Satisfied = Satisfied;
            }

            public void SetSatisfied(bool Satisfied)
            {
                this.Satisfied = Satisfied;
            }

        }

        public enum ID
        {
            TutorialGuideQuest = 1
        }

        public bool IsCompleted = false;
        /// <summary>
        /// List of dialogue for the Quest to be read by the NPC
        /// </summary>
        public virtual List<string> QuestDialogue => null;

        /// <summary>
        /// The dialogue for the Quest when the player clicks the Quest button again after already accepting
        /// </summary>        
        public virtual List<string> HintDialogue => null;

        public virtual List<Requirement> QuestRequirements => null;

        /// <summary>
        /// The name of the Quest to be read onto UI and the Chat
        /// </summary>
        /// <returns></returns>
        public virtual string QuestName() => "";

        /// <summary>
        /// The NPC associated with the Quest
        /// </summary>
        /// <returns></returns>
        public virtual int QuestGiver() => -1;

        /// <summary>
        /// The unique ID of the Quest
        /// </summary>
        /// <returns></returns>
        public virtual int QuestID() => -1;

        /// <summary>
        /// Gets the dialogue for the quest giver NPC, stored by index to allow text on multiple chat pages
        /// </summary>
        /// <param name="index">The index of the dialogue for a page</param>
        /// <returns></returns>
        public virtual string GetDialogue(int index) => QuestDialogue[index];

        /// <summary>
        /// Gets the hint dialogue for the quest giver NPC, stored by index to allow text on multiple chat pages
        /// </summary>
        /// <param name="index">The index of the dialogue for a page</param>
        /// <returns></returns>
        public virtual string GetHint(int index) => HintDialogue[index];

        /// <summary>
        /// Get the Quest requirements and their stacks stored in an array.
        /// </summary>
        /// <returns>An array of tuples in order by Item ID and Stack</returns>
        //public virtual List<Requirement> QuestRequirements() => null;

        /// <summary>
        /// Get the rewards and their stacks stored in an array.
        /// </summary>
        /// <returns>An array of lists in order by Item ID and Stack</returns>
        public virtual (int, int)[] QuestRewards() => null;
    }*/

    public class Quest
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

        /// <summary>
        /// Contains the list of rewards passed via <c>AddReward()</c>
        /// </summary>
        private List<ItemInfo> Rewards = new List<ItemInfo>();

        /// <summary>
        /// Contains the list of requirements passed in via <c>AddRequirement()</c>
        /// </summary>
        private List<IQuestRequirement> Requirements = new List<IQuestRequirement>();
        public virtual void SetDefaults() { }

        /// <summary>
        /// Adds a reward to the Quest, stored in <c>Rewards</c>
        /// </summary>
        /// <param name="ItemType"></param>
        /// <param name="ItemStack"></param>
        public void AddReward(int ItemType, int ItemStack)
        {
            Rewards.Add(new ItemInfo(ItemType, ItemStack));
        }
        
        /// <summary>
        /// Adds a requirement to the Quest, stored in <c>Requirements</c>
        /// </summary>
        /// <param name="requirement"></param>
        public void AddRequirement(IQuestRequirement requirement)
        {
            Requirements.Add(requirement);
        }

        /// <summary>
        /// Quickspawns the rewards set by the given Quest for the player
        /// </summary>
        /// <param name="player"></param>
        public void GiveRewards(Player player)
        {
            foreach (ItemInfo info in Rewards)
            {
                player.QuickSpawnItem(info.Item, info.Stack);
            }
        }


        public virtual void GiveExtraRewards(Player player) { }

        /// <summary>
        /// Loops through the Quest requirements to see if they are fulfilled
        /// </summary>
        /// <param name="player"></param>
        /// <returns>True or false</returns>
        public bool CheckCompleted(Player player)
        {
            bool result = true;
            foreach (IQuestRequirement requirement in Requirements)
            {
                if (!requirement.CheckCompleted(player))
                {
                    result = false;
                    Main.NewText(requirement.GetRequirementText());
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// List of dialogue for the Quest to be read by the NPC
        /// </summary>
        public virtual List<string> QuestDialogue => null;

        /// <summary>
        /// The dialogue for the Quest when the player clicks the Quest button again after already accepting
        /// </summary>        
        public virtual List<string> HintDialogue => null;
        /// <summary>
        /// The name of the Quest to be read onto UI and the Chat
        /// </summary>
        /// <returns></returns>
        public virtual string QuestName() => "";

        /// <summary>
        /// The NPC associated with the Quest
        /// </summary>
        /// <returns></returns>
        public virtual int QuestGiver() => -1;

        /// <summary>
        /// The unique ID of the Quest
        /// </summary>
        /// <returns></returns>
        public virtual int QuestID() => -1;

        /// <summary>
        /// Gets the dialogue for the quest giver NPC, stored by index to allow text on multiple chat pages
        /// </summary>
        /// <param name="index">The index of the dialogue for a page</param>
        /// <returns></returns>
        public virtual string GetDialogue(int index) => QuestDialogue[index];

        /// <summary>
        /// Gets the hint dialogue for the quest giver NPC, stored by index to allow text on multiple chat pages
        /// </summary>
        /// <param name="index">The index of the dialogue for a page</param>
        /// <returns></returns>
        public virtual string GetHint(int index) => HintDialogue[index];
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