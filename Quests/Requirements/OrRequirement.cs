using OvermorrowMod.Quests.State;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class OrRequirement : BaseCompositeRequirement
    {
        public OrRequirement(string id, params IQuestRequirement[] clauses) : base(clauses, id) { }

        public override string Description => $"{string.Join(" or ", Clauses.Select(c => c.Description))}";

        public override bool IsCompleted(QuestPlayer player, BaseQuestState state)
        {
            return Clauses.Any(c => c.IsCompleted(player, state));
        }
    }
}
