using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace OvermorrowMod.Quests
{
    public abstract class BaseQuest
    {
        protected virtual IEnumerable<IQuestRequirement> Requirements { get; set; }
        protected virtual IEnumerable<IQuestReward> Rewards { get; set; }
        protected virtual List<string> QuestDialogue { get; } = new List<string>();
        protected virtual List<string> QuestHint { get; } = new List<string>();
        public virtual void SetDefaults()
        {
            Requirements = Enumerable.Empty<IQuestRequirement>();
            Rewards = Enumerable.Empty<IQuestReward>();
        }

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
        /// Give rewards of this quest to the given player.
        /// </summary>
        public void GiveRewards(Player player)
        {
            foreach (var reward in Rewards)
            {
                reward.Give(player);
            }
        }

        /// <summary>
        /// Returns true if all quest goals are completed by the given player.
        /// </summary>
        public bool CheckRequirements(Player player)
        {
            foreach (var requirement in Requirements)
            {
                if (!requirement.IsCompleted(player)) return false;
            }
            return true;
        }

        /// <summary>
        /// Return true if the given player is allowed to accept this quest.
        /// </summary>
        public virtual bool IsValidFor(Player player)
        {
            return true;
        }
    }
}
