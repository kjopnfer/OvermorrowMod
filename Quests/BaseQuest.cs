using Microsoft.Xna.Framework;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.State;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests
{
    public enum QuestRepeatability
    {
        Repeatable,
        OncePerPlayer,
        OncePerWorld,
        OncePerWorldPerPlayer
    }

    public enum QuestType
    {
        Fetch,
        Housing,
        Kill,
        Travel
    }

    public abstract class BaseQuest
    {
        public virtual IEnumerable<IQuestRequirement> Requirements { get; set; }
        public virtual IEnumerable<IQuestReward> Rewards { get; set; }
        protected virtual List<string> QuestDialogue { get; } = new List<string>();
        protected virtual List<string> QuestHint { get; } = new List<string>();
        protected virtual List<string> QuestEndDialogue { get; } = new List<string>();
        public virtual int QuestGiver { get; }
        public string QuestID => GetType().FullName;
        public abstract string QuestName { get; }
        public virtual QuestType Type => QuestType.Fetch;
        public virtual QuestRepeatability Repeatability => QuestRepeatability.OncePerPlayer;
        public virtual int QuestDelay => 600;
        public virtual string QuestDescription => "";
        public virtual void SetDefaults()
        {
            Requirements = Enumerable.Empty<IQuestRequirement>();
            Rewards = Enumerable.Empty<IQuestReward>();
        }

        /// <summary>
        /// Quest priority for the given NPC. Quests with higher priority will always be chosen first.
        /// Can be a positive or a negative number.
        /// </summary>
        public virtual int Priority { get; } = 0;

        /// <summary>
        /// Get dialogue line.
        /// </summary>
        public virtual string GetDialogue(int index) => QuestDialogue[index];
        public int DialogueCount => QuestDialogue.Count;
        /// <summary>
        /// Get hint dialogue line.
        /// </summary>
        public virtual string GetHint(int index) => QuestHint[index];
        public int HintCount => QuestHint.Count;

        /// <summary>
        /// Get end dialogue line
        /// </summary>
        public virtual string GetEndDialogue(int index) => QuestEndDialogue[index];
        public int EndDialogueCount => QuestEndDialogue.Count;

        /// <summary>
        /// Give rewards of this quest to the given player.
        /// </summary>
        private void GiveRewards(Player player)
        {
            foreach (var reward in Rewards)
            {
                reward.Give(player);
            }
        }

        /// <summary>
        /// Resets the kill count of the NPC within the Dictionary after completion
        /// </summary>
        /// <param name="player"></param>
        private void ResetEffects(Player player)
        {
            foreach (IQuestRequirement requirement in Requirements)
            {
                requirement.ResetState(player);
            }
        }

        public void CompleteQuest(Player player, bool success)
        {
            var modPlayer = player.GetModPlayer<QuestPlayer>();
            if (Quests.State.HasCompletedQuest(modPlayer, this)) success = false;
            var state = Quests.State.GetActiveQuestState(modPlayer, this);
            if (state == null) success = false;

            if (success)
            {
                ResetEffects(player);
                GiveRewards(player);
                Main.NewText("COMPLETED QUEST: " + QuestName, Color.Yellow);
            }
            Quests.State.CompleteQuest(modPlayer, this);
        }

        /// <summary>
        /// Returns true if all quest goals are completed by the given player.
        /// </summary>
        public bool CheckRequirements(QuestPlayer player, BaseQuestState state)
        {
            foreach (var requirement in Requirements)
            {
                if (!requirement.IsCompleted(player, state)) return false;
            }
            return true;
        }

        /// <summary>
        /// Return true if the given player is allowed to accept this quest in general.
        /// Ignore whether the quest has already been completed.
        /// </summary>
        public virtual bool IsValidFor(Player player)
        {
            return true;
        }

        /// <summary>
        /// Return true if the npc can give the current quest to the given player.
        /// </summary>
        /// <param name="npcType"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsValidQuest(int npcType, Player player)
        {
            // Can this NPC give this quest?
            if (QuestGiver != npcType) return false;
            // Per quest check
            if (!IsValidFor(player)) return false;
            var modPlayer = player.GetModPlayer<QuestPlayer>();
            // Is the player currently doing this quest?
            if (Quests.State.IsDoingQuest(modPlayer, QuestID)) return false;
            if (Quests.State.HasCompletedQuest(modPlayer, this)) return false;
            return true;
        }

        public virtual BaseQuestState GetNewState()
        {
            return new BaseQuestState(this, GetActiveRequirements().Select(req => req.GetNewState()).Where(req => req != null).ToList());
        }

        private IEnumerable<IQuestRequirement> GetRequirements(IQuestRequirement source)
        {
            if (source is BaseCompositeRequirement composite)
            {
                foreach (var req in composite.Clauses)
                {
                    foreach (var req2 in GetRequirements(req))
                    {
                        yield return req2;
                    }
                }
            }
            else
            {
                yield return source;
            }
        }

        /// <summary>
        /// Returns all requirements except Composite requirements, instead unwrapping those.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IQuestRequirement> GetActiveRequirements()
        {
            foreach (var req in Requirements)
            {
                foreach (var req2 in GetRequirements(req))
                {
                    yield return req2;
                }
            }
        }

        /// <summary>
        /// Return all active requirements of the given type
        /// </summary>
        /// <typeparam name="T">Requirement type</typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetActiveRequirementsOfType<T>() where T : IQuestRequirement
        {
            return GetActiveRequirements().OfType<T>();
        }
    }
}
