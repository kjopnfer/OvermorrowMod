using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class KillRequirement : IQuestRequirement
    {
        public readonly int type;
        public readonly int amount;

        public KillRequirement(int type, int amount)
        {
            if (amount <= 0) throw new ArgumentException($"Invalid amount: {amount}");
            this.type = type;
            this.amount = amount;
        }

        public string Description => $"#{amount} {Lang.GetNPCNameValue(type)}";

        public bool IsCompleted(Player player)
        {
            var KilledList = player.GetModPlayer<QuestPlayer>().KilledNPCs;

            if (KilledList.ContainsKey(type) && KilledList[type] >= amount)
            {
                return true;
            }

            return false;
        }
    }
}
