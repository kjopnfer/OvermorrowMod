using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class OrRequirement : BaseCompositeRequirement
    {
        public OrRequirement(string id, params IQuestRequirement[] clauses) : base(clauses, id) { }

        public override string Description => $"One of {string.Join(" or ", Clauses.Select(c => c.Description))}";

        public override bool CanHandInRequirement(QuestPlayer player, BaseQuestState state)
        {
            return Clauses.Any(c => c.CanHandInRequirement(player, state));
        }

        public override IEnumerable<IQuestRequirement> GetActiveClauses(QuestPlayer player, BaseQuestState state)
        {
            // For an OR requirement, all clauses are active at any point in time.
            return Clauses;
        }

        public override bool IsCompleted(QuestPlayer player, BaseQuestState state)
        {
            return Clauses.Any(c => c.IsCompleted(player, state));
        }

        public override bool TryCompleteRequirement(QuestPlayer player, BaseQuestState state)
        {
            // This is fail-fast, so only one requirement will actually be completed, which is important.
            return Clauses.Any(c => c.TryCompleteRequirement(player, state));
        }
    }
}
