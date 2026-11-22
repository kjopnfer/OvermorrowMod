using Terraria.ModLoader;

namespace OvermorrowMod.Core.Loot
{
    public class RerollCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "rerolls";

        public override string Usage => "/rerolls [amount]";

        public override string Description => "Adds rerolls, 1 by default";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            int amount = args.Length > 0 && int.TryParse(args[0], out int parsed) ? parsed : 1;
            var rerolls = caller.Player.GetModPlayer<RerollPlayer>();
            rerolls.Rerolls += amount;
            caller.Reply($"Rerolls: {rerolls.Rerolls}");
        }
    }
}
