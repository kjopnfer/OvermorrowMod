using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class OrRequirement : IQuestRequirement
    {
        private IQuestRequirement[] clauses;

        public OrRequirement(params IQuestRequirement[] clauses)
        {
            this.clauses = clauses;
        }

        public string Description => $"One of {string.Join(" or ", clauses.Select(c => c.Description))}";

        public bool IsCompleted(Player player)
        {
            return clauses.Any(c => c.IsCompleted(player));
        }
    }
}
