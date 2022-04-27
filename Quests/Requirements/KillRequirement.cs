using OvermorrowMod.Quests.State;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class KillRequirement : IQuestRequirement
    {
        public List<int> NPCTypes { get; }
        public int TargetNumber { get; }

        public string ID { get; }

        public KillRequirement(List<int> type, int amount, string id)
        {
            if (amount <= 0) throw new ArgumentException($"Invalid amount: {amount}");
            NPCTypes = type;
            TargetNumber = amount;
            ID = id;
        }

        // I don't know how to show each of the required types, lol
        public string Description => $"#{TargetNumber} from any of {string.Join(", ", NPCTypes.Select(typ => Lang.GetNPCNameValue(typ)))}";

        public bool IsCompleted(QuestPlayer player, BaseQuestState state)
        {
            var reqState = state.GetRequirementState(this) as KillRequirementState;

            int remaining = TargetNumber;
            foreach (int id in NPCTypes)
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
