using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Core.Globals;
using System;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.MetaUpgrades
{
    public class StonesCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "stones";

        public override string Usage => "/stones <green|blue|orange|chaos> [amount]";

        public override string Description => "Adds Monkey Stones to the wallet, 10 by default";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0 || !Enum.TryParse(args[0], true, out MonkeyStoneTier tier))
            {
                caller.Reply(Usage);
                return;
            }

            int amount = args.Length > 1 && int.TryParse(args[1], out int parsed) ? parsed : 10;
            var wallet = caller.Player.GetModPlayer<MonkeyStonePlayer>();
            wallet.Add(tier, amount);
            caller.Reply($"{tier}: {wallet.Get(tier)}");
        }
    }
}
