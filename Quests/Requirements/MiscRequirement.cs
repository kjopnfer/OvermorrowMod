using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    /// <summary>
    /// <para>For any quests that have an ingame action that doesn't already fall under the currently existing requirements.</para>
    /// An example being lighting a campfire or solving a puzzle.
    /// </summary>
    public class MiscRequirement : BaseQuestRequirement<MiscRequirementState>
    {
        public readonly string description;

        public MiscRequirement(string id, string description) : base(id)
        {
            this.description = description;
        }

        public override string Description => $"{description}";

        protected override bool IsRequirementCompletable(QuestPlayer player, BaseQuestState state, MiscRequirementState reqState)
        {
            return false;
        }

        protected override void CompleteRequirement(QuestPlayer player, BaseQuestState state, MiscRequirementState reqState)
        {
        }
    }
}