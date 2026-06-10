using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class FourLeafClover : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Epic;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.Luck, 1),
        };
    }
}
