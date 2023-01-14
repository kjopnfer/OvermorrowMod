using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public abstract class BaseQuestRequirement<TState> : IQuestRequirement where TState : CompletableRequirementState
    {
        public string ID { get; }

        public BaseQuestRequirement(string id)
        {
            ID = id;
        }

        /// <summary>
        /// Return true if the requirement can be completed, i.e. CompleteRequirement can be executed.
        /// This method is also called to check what text to render in the chat with NPCs. It should never have
        /// side effects.
        /// </summary>
        protected abstract bool IsRequirementCompletable(QuestPlayer player, BaseQuestState state, TState reqState);

        /// <summary>
        /// Perform the necessary actions to complete this requirement.
        /// Does not need to set reqState.IsCompleted. Should never be allowed to fail if IsRequirementCompletable
        /// return true, will never be called unless it is true.
        /// </summary>
        protected abstract void CompleteRequirement(QuestPlayer player, BaseQuestState state, TState reqState);

        public virtual bool TryCompleteRequirement(QuestPlayer player, BaseQuestState state)
        {
            var reqState = state.GetRequirementState(this) as TState;

            if (reqState == null) throw new InvalidOperationException("Missing requirement state, this is a bug");

            if (reqState.IsCompleted) return true;
            if (IsRequirementCompletable(player, state, reqState))
            {
                CompleteRequirement(player, state, reqState);
                reqState.IsCompleted = true;
            }
            return reqState.IsCompleted;
        }
        /// <summary>
        /// Return true from this method if this requirement can be handed in to an NPC now,
        /// for example handing in items, or manually checking completion.
        /// 
        /// Note that if this returns false, TryCompleteRequirement must not have any side effects!
        /// </summary>
        public bool CanHandInRequirement(QuestPlayer player, BaseQuestState state)
        {
            var reqState = state.GetRequirementState(this) as TState;

            if (reqState.IsCompleted) return false;

            return IsRequirementCompletable(player, state, reqState);
        }

        public abstract string Description { get; }

        public virtual void ResetState(Player player)
        {
        }

        public virtual BaseRequirementState GetNewState()
        {
            return Activator.CreateInstance(typeof(TState), new[] { this }) as TState;
        }

        public bool IsCompleted(QuestPlayer player, BaseQuestState state)
        {
            var reqState = state.GetRequirementState(this) as TState;
            return reqState.IsCompleted;
        }
    }
}
