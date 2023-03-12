using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class ItemRequirement : BaseQuestRequirement<CompletableRequirementState>
    {
        public readonly int type;
        public readonly int stack;
        public readonly string description;

        private readonly bool consumeItems;

        public ItemRequirement(string id, int type, int stack, string description, bool consumeItems) : base(id)
        {
            if (stack <= 0) throw new ArgumentException($"Invalid stack size: {stack}");
            if (type <= 0) throw new ArgumentException($"Invalid type: {type}");
            this.stack = stack;
            this.type = type;
            this.description = description;
            this.consumeItems = consumeItems;
        }

        public override string Description => $"#{stack} {Lang.GetItemNameValue(type)}";

        protected override bool IsRequirementCompletable(QuestPlayer player, BaseQuestState state, CompletableRequirementState reqState)
        {
            int remaining = stack;
            foreach (var item in player.Player.inventory.Where(i => i.type == type && i.stack > 0))
            {
                remaining -= item.stack;
                if (remaining <= 0) return true;
            }
            return false;
        }

        public int GetCurrentStack(QuestPlayer player)
        {
            int stack = 0;
            foreach (var item in player.Player.inventory.Where(i => i.type == type && i.stack > 0))
            {
                stack += item.stack;
            }

            return stack;
        }

        protected override void CompleteRequirement(QuestPlayer player, BaseQuestState state, CompletableRequirementState reqState)
        {
            if (consumeItems)
            {
                int remaining = stack;
                foreach (var item in player.Player.inventory.Where(i => i.type == type && i.stack > 0))
                {
                    int toTake = Math.Min(item.stack, remaining);
                    item.stack -= toTake;
                    if (item.stack == 0) item.TurnToAir();
                    remaining -= toTake;
                    if (remaining == 0) break;
                }
            }
        }
    }
}
