using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class HousingRequirement : BaseQuestRequirement<CompletableRequirementState>
    {
        private readonly int npcType;

        public HousingRequirement(string id, int npcType) : base(id)
        {
            if (npcType <= 0) throw new ArgumentException($"Invalid NPC Type: {npcType}");
            this.npcType = npcType;
        }

        public override string Description => $"{Lang.GetNPCNameValue(npcType)} must have a home";

        protected override bool IsRequirementCompletable(QuestPlayer player, BaseQuestState state, CompletableRequirementState reqState)
        {
            // This requirement is checked on hand-in, so we need to return true from here
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].type == npcType)
                {
                    return Main.npc[i].active && !Main.npc[i].homeless;
                }
            }
            return false;
        }

        protected override void CompleteRequirement(QuestPlayer player, BaseQuestState state, CompletableRequirementState reqState)
        {
            // This method does nothing, because no active action is taken to complete the requirement.
        }
    }
}
