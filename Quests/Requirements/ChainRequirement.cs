using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvermorrowMod.Quests.Requirements
{
    public class ChainRequirement : BaseCompositeRequirement
    {
        public ChainRequirement(IQuestRequirement[] clauses, string id) : base(clauses, id)
        {
        }

        public override string Description => $"Do in order: {string.Join(", ", this.Clauses.Select(c => c.Description))}";

        private IQuestRequirement GetActiveClause(QuestPlayer player, BaseQuestState state)
        {
            return Clauses.FirstOrDefault(req => !req.IsCompleted(player, state));
        }

        public override bool CanHandInRequirement(QuestPlayer player, BaseQuestState state)
        {
            return GetActiveClause(player, state)?.CanHandInRequirement(player, state) ?? false;
        }

        public override IEnumerable<IQuestRequirement> GetActiveClauses(QuestPlayer player, BaseQuestState state)
        {
            var clause = GetActiveClause(player, state);
            if (clause == null) return Enumerable.Empty<IQuestRequirement>();
            return new[] { clause };
        }

        public override bool IsCompleted(QuestPlayer player, BaseQuestState state)
        {
            return Clauses.All(req => req.IsCompleted(player, state));
        }

        public override bool TryCompleteRequirement(QuestPlayer player, BaseQuestState state)
        {
            var active = GetActiveClause(player, state);
            if (active != null)
            {
                active.TryCompleteRequirement(player, state);
            }
            return IsCompleted(player, state);
        }
    }
}
