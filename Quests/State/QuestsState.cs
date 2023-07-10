using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests.State
{
    public class QuestsState
    {
        private Dictionary<string, Dictionary<string, BaseQuestState>> StatesByPlayer { get; } =
            new Dictionary<string, Dictionary<string, BaseQuestState>>();
        private HashSet<string> CompletedWorldQuests { get; } = new HashSet<string>();

        public IEnumerable<BaseQuestState> GetActiveQuests(QuestPlayer player)
        {
            return StatesByPlayer.GetValueOrDefault(player.PlayerUUID)?.Values?.Where(state => !state.Completed)?.ToList()
                ?? Enumerable.Empty<BaseQuestState>();
        }

        public void RemoveQuest(QuestPlayer player, BaseQuest quest)
        {
            StatesByPlayer[player.PlayerUUID].Remove(quest.QuestID);
        }

        public void RemoveQuest(string playerID, BaseQuest quest)
        {
            StatesByPlayer[playerID].Remove(quest.QuestID);
        }

        public void CompleteQuest(QuestPlayer player, BaseQuest quest)
        {
            // Get the player's quest states from the dictionary
            if (!StatesByPlayer.TryGetValue(player.PlayerUUID, out var playerQuests))
            {
                // Initialize empty dictionary if the player's quests states are not found
                StatesByPlayer[player.PlayerUUID] = playerQuests = new Dictionary<string, BaseQuestState>();
            }

            // Get the state of the quest given the quest's ID and set completion to true
            if (!playerQuests.TryGetValue(quest.QuestID, out var state))
            {
                // Initialize the quest with a new state if not found
                playerQuests[quest.QuestID] = state = quest.GetNewState();
            }
            state.Completed = true;

            //if (quest.Repeatability != QuestRepeatability.Repeatable) state.Completed = true;

            if (quest.Repeatability == QuestRepeatability.OncePerWorld)
            {
                CompletedWorldQuests.Add(quest.QuestID);
                StatesByPlayer[player.PlayerUUID].Remove(quest.QuestID);

                // For per-world quests any duplicates must be terminated here.
                foreach (var q in GetPerPlayerQuestsForWorld().Values.SelectMany(q => q).Where(q => q.Quest.QuestID == quest.QuestID).ToList())
                {
                    q.Quest.CompleteQuest(player.Player, false);
                }
            }
        }

        public void AddQuest(QuestPlayer player, BaseQuest quest)
        {
            if (!StatesByPlayer.TryGetValue(player.PlayerUUID, out var playerQuests))
            {
                StatesByPlayer[player.PlayerUUID] = playerQuests = new Dictionary<string, BaseQuestState>();
            }
            playerQuests[quest.QuestID] = quest.GetNewState();
        }

        public BaseQuestState GetActiveQuestState(QuestPlayer player, BaseQuest quest)
        {
            return StatesByPlayer.GetValueOrDefault(player.PlayerUUID)?.GetValueOrDefault(quest.QuestID);
        }

        public bool HasCompletedQuest(QuestPlayer player, BaseQuest quest)
        {
            if (quest.Repeatability == QuestRepeatability.OncePerWorld)
            {
                return CompletedWorldQuests.Contains(quest.QuestID);
            }
            else
            {
                return StatesByPlayer.TryGetValue(player.PlayerUUID, out var quests)
                    && quests.TryGetValue(quest.QuestID, out var questState)
                    && questState.Completed;
            }
        }

        /// <summary>
        /// A variant of IsDoingQuest but without checking if the state is completed.
        /// Created because I don't know why state completion is there when I just want to
        /// know if the player is doing the quest in the first place and not having already finished it.
        /// </summary>
        public bool CheckDoingQuest(QuestPlayer player, string questID)
        {
            return StatesByPlayer.TryGetValue(player.PlayerUUID, out var quests)
               && quests.TryGetValue(questID, out var questState);
        }

        public bool IsDoingQuest(QuestPlayer player, string questID)
        {
            return StatesByPlayer.TryGetValue(player.PlayerUUID, out var quests)
                && quests.TryGetValue(questID, out var questState)
                && questState.Completed;
        }

        public IEnumerable<string> GetWorldQuestsToSave()
        {
            return CompletedWorldQuests;
        }

        public Dictionary<string, IEnumerable<BaseQuestState>> GetPerPlayerQuestsForWorld()
        {
            return StatesByPlayer.ToDictionary(kvp => kvp.Key,
                kvp => (IEnumerable<BaseQuestState>)kvp.Value.Values
                    .Where(q =>
                        q.Quest.Repeatability == QuestRepeatability.OncePerWorldPerPlayer
                        || q.Quest.Repeatability == QuestRepeatability.OncePerWorld).ToList());
        }

        public IEnumerable<BaseQuestState> GetPerPlayerQuests(QuestPlayer player)
        {
            return StatesByPlayer.GetValueOrDefault(player.PlayerUUID)?.Values ?? Enumerable.Empty<BaseQuestState>();
        }

        public void Reset()
        {
            CompletedWorldQuests.Clear();
            foreach (var kvp in StatesByPlayer)
            {
                kvp.Value.Clear();
            }
        }

        public void LoadWorld(Dictionary<string, IEnumerable<TagCompound>> questsByPlayer, IEnumerable<string> completedWorldQuests)
        {
            foreach (var kvp in questsByPlayer)
            {
                if (!StatesByPlayer.TryGetValue(kvp.Key, out var playerQuests))
                {
                    StatesByPlayer[kvp.Key] = playerQuests = new Dictionary<string, BaseQuestState>();
                }
                foreach (var tag in kvp.Value)
                {
                    var questID = tag.GetString("questID");
                    if (Quests.QuestList.TryGetValue(questID, out var quest))
                    {
                        if (quest.Repeatability == QuestRepeatability.Repeatable || quest.Repeatability == QuestRepeatability.OncePerPlayer) continue;

                        var state = quest.GetNewState();
                        state.Load(tag);
                        playerQuests[questID] = state;
                    }
                }
            }

            foreach (var id in completedWorldQuests)
            {
                if (Quests.QuestList.TryGetValue(id, out var quest) && quest.Repeatability == QuestRepeatability.OncePerWorld)
                {
                    CompletedWorldQuests.Add(id);
                }
            }
        }

        public void LoadPlayer(QuestPlayer player, IEnumerable<TagCompound> playerQuests)
        {
            if (!StatesByPlayer.TryGetValue(player.PlayerUUID, out var playerStates))
            {
                StatesByPlayer[player.PlayerUUID] = playerStates = new Dictionary<string, BaseQuestState>();
            }

            foreach (var questTag in playerQuests)
            {
                var questID = questTag.GetString("questID");
                if (Quests.QuestList.TryGetValue(questID, out var quest))
                {
                    if (quest.Repeatability == QuestRepeatability.OncePerWorld || quest.Repeatability == QuestRepeatability.OncePerWorldPerPlayer) continue;

                    var state = quest.GetNewState();
                    state.Load(questTag);
                    playerStates[questID] = state;
                }
            }
        }

        public IEnumerable<(BaseQuestState, T)> GetActiveRequirementsOfType<T>(QuestPlayer player) where T : BaseRequirementState
        {
            return GetActiveQuests(player).SelectMany(q =>
                q.Quest.GetActiveRequirements(player, q)
                .Select(req => q.GetRequirementState(req))
                .OfType<T>()
                .Select(reqState => (q, reqState)));
        }
    }
}
