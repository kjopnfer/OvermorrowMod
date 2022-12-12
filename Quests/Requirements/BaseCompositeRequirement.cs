using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public abstract class BaseCompositeRequirement : IQuestRequirement
    {
        public abstract IEnumerable<IQuestRequirement> GetActiveClauses(QuestPlayer player, BaseQuestState state);

        public virtual IEnumerable<IQuestRequirement> AllClauses => Clauses;

        protected IQuestRequirement[] Clauses { get; }

        public string ID { get; }

        public BaseCompositeRequirement(IQuestRequirement[] clauses, string id)
        {
            ID = id;
            Clauses = clauses;
        }

        public abstract string Description { get; }

        public virtual BaseRequirementState GetNewState()
        {
            return null;
        }

        public abstract bool TryCompleteRequirement(QuestPlayer player, BaseQuestState state);

        public void ResetState(Player player)
        {
            foreach (var req in Clauses)
            {
                req.ResetState(player);
            }
        }

        public abstract bool CanHandInRequirement(QuestPlayer player, BaseQuestState state);

        public abstract bool IsCompleted(QuestPlayer player, BaseQuestState state);
    }
}
