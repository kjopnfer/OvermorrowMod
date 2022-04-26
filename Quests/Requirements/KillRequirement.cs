using OvermorrowMod.Quests.State;
using System;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class KillRequirement : IQuestRequirement
    {
        public readonly List<int> type;
        public readonly int amount;

        public string ID { get; }

        public KillRequirement(List<int> type, int amount, string id)
        {
            if (amount <= 0) throw new ArgumentException($"Invalid amount: {amount}");
            this.type = type;
            this.amount = amount;
            ID = id;
        }

        // I don't know how to show each of the required types, lol
        public string Description => $"#{amount} {Lang.GetNPCNameValue(type[0])}";

        public bool IsCompleted(QuestPlayer player, BaseQuestState state)
        {
            var reqState = state.GetRequirementState(this) as KillRequirementState;

            int remaining = amount;
            foreach (int id in type)
            {
                if (reqState.NumKilled.TryGetValue(id, out int value)) remaining -= value;
            }

            return remaining <= 0;
        }

        public void ResetState(Player player)
        {
        }

        public BaseRequirementState GetNewState()
        {
            return new KillRequirementState(this);
        }
    }
}
