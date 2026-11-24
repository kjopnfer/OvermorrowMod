using OvermorrowMod.Core.Globals;
using OvermorrowMod.Content.Items.Archives;
using System;
using System.Linq;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Runs
{
    public class RunRecordCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "runrecord";

        public override string Description => "Prints the current run's record";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var record = caller.Player.GetModPlayer<RunRecordPlayer>();
            var stones = caller.Player.GetModPlayer<MonkeyStonePlayer>();

            caller.Reply($"Time: {TimeSpan.FromSeconds(record.RunTicks / 60)}, rooms cleared: {record.RoomsCleared}");
            caller.Reply("Kills: " + string.Join(", ", Enum.GetValues<KillTier>().Select(tier => $"{tier} {record.KillsByTier[(int)tier]}")));
            caller.Reply("Items: " + string.Join(", ", Enum.GetValues<Loot.Rarity>().Select(rarity => $"{rarity} {record.ItemsByRarity[(int)rarity]}")));
            caller.Reply("Stones gained: " + string.Join(", ", Enum.GetValues<MonkeyStoneTier>().Select(tier => $"{tier} {stones.GetGainedThisRun(tier)}")));
        }
    }
}
