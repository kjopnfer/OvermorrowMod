using OvermorrowMod.Common.Netcode;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public class QuestCommand : ModCommand
    {
        public override string Command => "quests";
        public override CommandType Type => CommandType.Chat | CommandType.Console;
        public override string Description => "Command for managing overmorrow quests";
        public override string Usage => $"/{Command} list [active/finished/unfinished] OR /{Command} reset OR /{Command} complete [QuestName]";

        private void LogActiveQuests(CommandCaller caller)
        {
            if (caller.Player == null) return;
            Main.NewText($"Active quests: {string.Join(", ", caller.Player.GetModPlayer<QuestPlayer>().CurrentQuests.Select(q => q.QuestName))}");
        }

        private void LogFinishedQuests(CommandCaller caller)
        {
            var globalQuestNames = Quests.GlobalCompletedQuests.Select(qid => Quests.QuestList[qid].QuestName);
            Main.NewText($"Globally completed quests: {string.Join(", ", globalQuestNames)}");
            if (caller.Player != null)
            {
                var modPlayer = caller.Player.GetModPlayer<QuestPlayer>();
                var localQuestNames = modPlayer.CompletedQuests.Select(qid => Quests.QuestList[qid].QuestName);
                Main.NewText($"Player completed quests: {string.Join(", ", localQuestNames)}");

                var worldLocalQuestNames = Quests.PerPlayerCompletedQuests[modPlayer.PlayerUUID].Select(qid => Quests.QuestList[qid].QuestName);
                Main.NewText($"Per player per world completed quests: {string.Join(", ", worldLocalQuestNames)}");
            }
        }

        private void LogUnfinishedQuests(CommandCaller caller)
        {
            IEnumerable<string> finishedQuestIds = Quests.GlobalCompletedQuests;
            if (caller.Player != null)
            {
                var modPlayer = caller.Player.GetModPlayer<QuestPlayer>();
                finishedQuestIds = finishedQuestIds
                    .Concat(modPlayer.CompletedQuests)
                    .Concat(Quests.PerPlayerCompletedQuests[modPlayer.PlayerUUID]);
            }
            var finishedQuestSet = new HashSet<string>(finishedQuestIds);
            var remainingQuests = Quests.QuestList.Values.Where(q => !finishedQuestSet.Contains(q.QuestID));
            var unfinishedQuestNames = remainingQuests.Select(q => q.QuestName);

            Main.NewText($"Remaining unfinished or repeatable quests: {string.Join(", ", unfinishedQuestNames)}");
        }

        private void List(CommandCaller caller, string[] args)
        {
            if (args.Length == 1 || args[1] == "active")
            {
                LogActiveQuests(caller);
            }
            if (args.Length == 1 || args[1] == "finished")
            {
                LogFinishedQuests(caller);
            }
            if (args.Length == 1 || args[1] == "unfinished")
            {
                LogUnfinishedQuests(caller);
            }
        }

        private void Reset(CommandCaller caller, string[] args)
        {
            Main.NewText("Cleared all quests for all players!");
            Quests.ClearAllCompletedQuests();
            NetworkMessageHandler.Quests.ResetQuest(-1, -1);
        }

        private void Complete(CommandCaller caller, string[] args)
        {
            if (caller.Player == null) return;
            if (args.Length == 1)
            {
                Main.NewText("Must specify which quest to complete");
                return;
            }
            BaseQuest quest = null;
            if (!Quests.QuestList.TryGetValue(args[1], out quest))
            {
                quest = Quests.QuestList.Values.FirstOrDefault(q => q.QuestName == args[1]);
            }

            if (quest == null)
            {
                Main.NewText($"No quest named {args[1]} found");
            }
            else
            {
                quest.CompleteQuest(caller.Player, true);
            }
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0)
            {
                Main.NewText("At least one argument is required: [list/reset/complete]");
                return;
            }

            if (args[0] == "list") List(caller, args);
            else if (args[0] == "reset") Reset(caller, args);
            else if (args[0] == "complete") Complete(caller, args);
            else
            {
                Main.NewText($"Unknown command: {args[0]}");
            }
        }
    }
}
