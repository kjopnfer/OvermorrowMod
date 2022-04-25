using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests.State
{
    public class QuestsState
    {
        private Dictionary<string, Dictionary<string, BaseQuestState>> StatesByPlayer { get; } =
            new Dictionary<string, Dictionary<string, BaseQuestState>>();
        private HashSet<string> CompletedWorldQuests { get; } = new HashSet<string>();
        private Dictionary<string, HashSet<string>> PerPlayerCompletedQuests { get; } = new Dictionary<string, HashSet<string>>();

        public IEnumerable<BaseQuestState> GetActiveQuests(QuestPlayer player)
        {
            return StatesByPlayer.GetValueOrDefault(player.PlayerUUID)?.Values.Where(state => !state.Completed).ToList();
        }

        public void CompleteQuest(QuestPlayer player, BaseQuest quest)
        {
            StatesByPlayer[player.PlayerUUID][quest.QuestID].Completed = true;
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
            else
            {
                if (!PerPlayerCompletedQuests.TryGetValue(player.PlayerUUID, out var compQuests))
                {
                    PerPlayerCompletedQuests[player.PlayerUUID] = compQuests = new HashSet<string>();
                }
                compQuests.Add(quest.QuestID);
            }
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
                return PerPlayerCompletedQuests.TryGetValue(player.PlayerUUID, out var compQuests) && compQuests.Contains(quest.QuestID);
            }
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
            foreach (var kvp in PerPlayerCompletedQuests) kvp.Value.Clear();
            foreach (var kvp in StatesByPlayer)
            {
                foreach (var state in kvp.Value) state.Value.Reset();
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
                if (!PerPlayerCompletedQuests.TryGetValue(kvp.Key, out var perPlayerCompleted))
                {
                    PerPlayerCompletedQuests[kvp.Key] = perPlayerCompleted = new HashSet<string>();
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
                        if (state.Completed)
                        {
                            perPlayerCompleted.Add(questID);
                        }
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
    }
}
