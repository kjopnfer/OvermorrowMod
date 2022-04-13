using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class HousingRequirement : IQuestRequirement
    {
        private readonly int npcType;

        public HousingRequirement(int npcType)
        {
            if (npcType <= 0) throw new ArgumentException($"Invalid NPC Type: {npcType}");
            this.npcType = npcType;
        }

        public string Description => $"{Lang.GetNPCNameValue(npcType)} must have a home";

        public bool IsCompleted(Player player)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].type == npcType)
                {
                    return Main.npc[i].active && !Main.npc[i].homeless;
                }
            }
            return false;
        }
    }
}
