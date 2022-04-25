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

        public ItemRequirement(int type, int stack, string id)
        {
            if (stack <= 0) throw new ArgumentException($"Invalid stack size: {stack}");
            if (type <= 0) throw new ArgumentException($"Invalid type: {type}");
            this.stack = stack;
            this.type = type;
            ID = id;
        }

        public string Description => $"#{stack} {Lang.GetItemNameValue(type)}";

        public bool IsCompleted(Player player)
        {
            int remaining = stack;
            foreach (var item in player.inventory)
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
