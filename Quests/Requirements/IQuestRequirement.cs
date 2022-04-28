using OvermorrowMod.Quests.State;
using Terraria;

namespace OvermorrowMod.Quests
{
    public interface IQuestRequirement
    {
        string ID { get; }
        bool IsCompleted(QuestPlayer player, BaseQuestState state);
        string Description { get; }
        void ResetState(Player player);
        BaseRequirementState GetNewState();
    }
}
