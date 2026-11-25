using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Archives;
using Terraria.ID;
using Terraria.Localization;

namespace OvermorrowMod.Core.MetaUpgrades
{
    public enum UpgradeGroup { Supplies, Shops, Luck, Healing }

    public readonly record struct StoneCost(MonkeyStoneTier Tier, int Amount);

    public class MetaUpgrade
    {
        public string Name { get; }

        public UpgradeGroup Group { get; }

        public int IconItem { get; }

        /// <summary>
        /// The upgrade's value at each rank, starting with the base at rank 0.
        /// </summary>
        public int[] Values { get; }

        /// <summary>
        /// The cost of each rank, starting with rank 1.
        /// </summary>
        public StoneCost[][] Costs { get; }

        public int MaxRank => Costs.Length;

        public string DisplayName => Text("DisplayName");

        public string Description => Text("Description");

        public string ValueText(int rank) => Text("Rank" + rank);

        private MetaUpgrade(string name, UpgradeGroup group, int iconItem, int[] values, params StoneCost[][] costs)
        {
            Name = name;
            Group = group;
            IconItem = iconItem;
            Values = values;
            Costs = costs;
        }

        private string Text(string key) => Language.GetTextValue(LocalizationPath.MetaUpgrades + Name + "." + key);

        private static StoneCost Green(int amount) => new(MonkeyStoneTier.Green, amount);

        private static StoneCost Blue(int amount) => new(MonkeyStoneTier.Blue, amount);

        private static StoneCost Orange(int amount) => new(MonkeyStoneTier.Orange, amount);

        public static readonly MetaUpgrade StartingPouch = new(nameof(StartingPouch), UpgradeGroup.Supplies, ItemID.GoldCoin, [0, 5000, 10000, 20000, 30000], [Green(10)], [Green(20)], [Blue(4)], [Orange(2)]);

        public static readonly MetaUpgrade PackedPotion = new(nameof(PackedPotion), UpgradeGroup.Supplies, ItemID.LesserHealingPotion, [0, 1, 2, 2], [Green(15)], [Green(25)], [Blue(5)]);

        public static readonly MetaUpgrade ExtraInventory = new(nameof(ExtraInventory), UpgradeGroup.Supplies, ItemID.Chest, [5, 6, 7, 8, 9, 10], [Blue(4)], [Blue(6)], [Blue(8)], [Blue(11)], [Blue(15)]);

        public static readonly MetaUpgrade Haggler = new(nameof(Haggler), UpgradeGroup.Shops, ItemID.DiscountCard, [0, 5, 10, 15], [Green(20)], [Blue(5)], [Orange(3)]);

        public static readonly MetaUpgrade ExtraShelf = new(nameof(ExtraShelf), UpgradeGroup.Shops, ItemID.PeddlersSatchel, [4, 5], [Blue(6)]);

        public static readonly MetaUpgrade Restock = new(nameof(Restock), UpgradeGroup.Shops, ItemID.WoodenCrate, [0, 1], [Blue(5)]);

        public static readonly MetaUpgrade SpareReroll = new(nameof(SpareReroll), UpgradeGroup.Luck, ItemID.FallenStar, [1, 2, 3, 4], [Green(20)], [Blue(5)], [Orange(3)]);

        public static readonly MetaUpgrade Scavenger = new(nameof(Scavenger), UpgradeGroup.Luck, ItemID.LuckyCoin, [0, 10, 20, 30], [Green(15)], [Blue(4)], [Orange(2)]);

        public static readonly MetaUpgrade QuickRecovery = new(nameof(QuickRecovery), UpgradeGroup.Healing, ItemID.Stopwatch, [60, 55, 50, 45], [Green(15)], [Green(25)], [Blue(4)]);

        public static readonly MetaUpgrade HeartyMeal = new(nameof(HeartyMeal), UpgradeGroup.Healing, ItemID.BowlofSoup, [0, 1], [Blue(3), Green(15)]);

        public static readonly MetaUpgrade[] All = [StartingPouch, PackedPotion, ExtraInventory, Haggler, ExtraShelf, Restock, SpareReroll, Scavenger, QuickRecovery, HeartyMeal];
    }
}
