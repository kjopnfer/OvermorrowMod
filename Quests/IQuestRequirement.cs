using Terraria;

namespace OvermorrowMod.Quests
{
    public interface IQuestRequirement
    {
        bool IsCompleted(Player player);
        string Description { get; }
        void ResetState(Player player);
        void GenerateOnTake(Player player)
        {
        }
    }
}
