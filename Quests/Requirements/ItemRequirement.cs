using OvermorrowMod.Quests.State;
using System;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class ItemRequirement : IQuestRequirement
    {
        private readonly int type;
        private readonly int stack;

        public string ID { get; }

        public ItemRequirement(int type, int stack)
        {
            if (stack <= 0) throw new ArgumentException($"Invalid stack size: {stack}");
            if (type <= 0) throw new ArgumentException($"Invalid type: {type}");
            this.stack = stack;
            this.type = type;
            ID = null;
        }

        public string Description => $"{GetItemCount(Main.LocalPlayer)}/{stack} {Lang.GetItemNameValue(type)}";

        public int GetItemCount(Player player)
        {
            int count = 0;
            foreach (var item in player.inventory)
            {
                if (item.type == type && item.stack > 0) count += item.stack;
            }

            return count;
        }

        public bool IsCompleted(QuestPlayer player, BaseQuestState state)
        {
            int remaining = stack;
            foreach (var item in player.Player.inventory)
            {
                if (item.type == type && item.stack > 0) remaining -= item.stack;
                if (remaining <= 0) return true;
            }
            return false;
        }

        public void ResetState(Player player) { }

        public BaseRequirementState GetNewState()
        {
            return null;
        }
    }
}
