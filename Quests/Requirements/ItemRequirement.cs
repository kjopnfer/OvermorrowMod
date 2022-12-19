using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class ItemRequirement : BaseQuestRequirement<CompletableRequirementState>
    {
        private readonly int type;
        private readonly int stack;
        private readonly bool consumeItems;

        public ItemRequirement(string id, int type, int stack, bool consumeItems) : base(id)
        {
            if (stack <= 0) throw new ArgumentException($"Invalid stack size: {stack}");
            if (type <= 0) throw new ArgumentException($"Invalid type: {type}");
            this.stack = stack;
            this.type = type;
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
