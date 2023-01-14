using OvermorrowMod.Quests;
using OvermorrowMod.Quests.State;
using Terraria;

namespace OvermorrowMod.Core.Interfaces
{
    public interface IQuestRequirement
    {
        string ID { get; }
        /// <summary>
        /// Try to complete a requirement, and return true if it has already been completed,
        /// or if it succeeded.
        /// </summary>
        bool TryCompleteRequirement(QuestPlayer player, BaseQuestState state);
        /// <summary>
        /// Return true from this method if this requirement can be handed in to an NPC now,
        /// for example handing in items, or manually checking completion.
        /// 
        /// Note that if this returns false, TryCompleteRequirement must not have any side effects!
        /// </summary>
        bool CanHandInRequirement(QuestPlayer player, BaseQuestState state);

        /// <summary>
        /// Check without side effects if this quest has been completed.
        /// </summary>
        bool IsCompleted(QuestPlayer player, BaseQuestState state);

        string Description { get; }
        void ResetState(Player player);
        BaseRequirementState GetNewState();
    }
}
