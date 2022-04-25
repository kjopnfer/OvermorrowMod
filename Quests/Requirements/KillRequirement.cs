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

        public bool IsCompleted(Player player)
        {
            int remaining = amount;
            var KilledList = player.GetModPlayer<QuestPlayer>().KilledNPCs;

            foreach (int ID in type)
            {
                if (KilledList.ContainsKey(ID)) remaining -= KilledList[ID];
            }

            if (remaining <= 0) return true;

            return false;
        }

        public void ResetState(Player player)
        {
            var KilledList = player.GetModPlayer<QuestPlayer>().KilledNPCs;
            foreach (int ID in type)
            {
                if (KilledList.ContainsKey(ID)) KilledList[ID] = 0;
            }
        }

        public BaseRequirementState GetNewState()
        {
            return new KillRequirementState(this);
        }
    }
}
