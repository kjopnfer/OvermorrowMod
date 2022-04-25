using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class OrRequirement : IQuestRequirement
    {
        public IQuestRequirement[] clauses;

        public string ID { get; }

        public OrRequirement(string id, params IQuestRequirement[] clauses)
        {
            this.clauses = clauses;
            ID = id;
        }

        public string Description => $"One of {string.Join(" or ", clauses.Select(c => c.Description))}";

        public bool IsCompleted(Player player)
        {
            return clauses.Any(c => c.IsCompleted(player));
        }

        public void ResetState(Player player)
        {
            foreach (var req in clauses)
            {
                req.ResetState(player);
            }
        }
    }
}
