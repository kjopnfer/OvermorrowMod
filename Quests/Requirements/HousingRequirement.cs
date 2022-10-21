using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class HousingRequirement : IQuestRequirement
    {
        private readonly int npcType;
        public string ID { get; }

        public HousingRequirement(int npcType)
        {
            if (npcType <= 0) throw new ArgumentException($"Invalid NPC Type: {npcType}");
            this.npcType = npcType;
            ID = null;
        }

        public string Description => $"{Lang.GetNPCNameValue(npcType)} must have a home";

        public bool IsCompleted(QuestPlayer player, BaseQuestState state)
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

        public void ResetState(Player player) { }

        public BaseRequirementState GetNewState()
        {
            return null;
        }
    }
}
