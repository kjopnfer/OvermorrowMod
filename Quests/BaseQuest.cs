using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace OvermorrowMod.Quests
{
    public enum QuestRepeatability
    {
        Repeatable,
        OncePerPlayer,
        OncePerWorld
    }

    public abstract class BaseQuest
    {
        protected virtual IEnumerable<IQuestRequirement> Requirements { get; set; }
        protected virtual IEnumerable<IQuestReward> Rewards { get; set; }
        protected virtual List<string> QuestDialogue { get; } = new List<string>();
        protected virtual List<string> QuestHint { get; } = new List<string>();
        protected virtual List<string> QuestEndDialogue { get; } = new List<string>();
        public virtual int QuestGiver { get; }
        public string QuestId => GetType().FullName;
        public abstract string QuestName { get; }
        public virtual QuestRepeatability Repeatability => QuestRepeatability.OncePerPlayer;
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

        public void CompleteQuest(Player player, bool success)
        {
            var modPlayer = player.GetModPlayer<QuestPlayer>();
            if (Repeatability == QuestRepeatability.OncePerPlayer && modPlayer.CompletedQuests.Contains(QuestId)) success = false;
            if (Repeatability == QuestRepeatability.OncePerWorld && Quests.GlobalCompletedQuests.Contains(QuestId)) success = false;

            if (success)
            {
                GiveRewards(player);
                Main.NewText("COMPLETED QUEST: " + QuestName, Color.Yellow);
            }
            modPlayer.RemoveQuest(this);
            if (Repeatability == QuestRepeatability.OncePerPlayer)
            {
                modPlayer.CompletedQuests.Add(QuestId);
            }
            else if (Repeatability == QuestRepeatability.OncePerWorld)
            {
                // For per-world quests any duplicates must be terminated here.
                Quests.GlobalCompletedQuests.Add(QuestId);
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    var p = Main.player[i];
                    if (!p.active || p == player) continue;

                    var extModPlayer = p.GetModPlayer<QuestPlayer>();
                    foreach (var quest in extModPlayer.CurrentQuests)
                    {
                        if (quest.QuestId == QuestId)
                        {
                            quest.CompleteQuest(p, false);
                        }
                    }
                }
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
        /// Return true if the given player is allowed to accept this quest in general.
        /// Ignore whether the quest has already been completed.
        /// </summary>
        protected virtual bool IsValidFor(Player player)
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
            if (modPlayer.CurrentQuests.Any(q => q.QuestId == QuestId)) return false;
            if (Repeatability == QuestRepeatability.OncePerPlayer && modPlayer.CompletedQuests.Contains(QuestId)) return false;
            if (Repeatability == QuestRepeatability.OncePerWorld && Quests.GlobalCompletedQuests.Contains(QuestId)) return false;
            return true;
        }
    }
}
