using Terraria.ModLoader;

namespace OvermorrowMod.Core.MetaUpgrades
{
    public class UpgradesCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "upgrades";

        public override string Usage => "/upgrades <reset|max>";

        public override string Description => "Sets every meta upgrade to rank 0 or its max rank";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            bool? max = args.Length == 0 ? null : args[0].ToLowerInvariant() switch
            {
                "reset" => false,
                "max" => true,
                _ => null,
            };

            if (max == null)
            {
                caller.Reply(Usage);
                return;
            }

            var upgrades = caller.Player.GetModPlayer<MetaUpgradePlayer>();
            foreach (MetaUpgrade upgrade in MetaUpgrade.All)
                upgrades.SetRank(upgrade, max.Value ? upgrade.MaxRank : 0);

            caller.Reply(max.Value ? "Every upgrade maxed" : "Every upgrade reset");
        }
    }
}
