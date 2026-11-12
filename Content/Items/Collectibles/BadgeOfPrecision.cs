using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class BadgeOfPrecision : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Rare;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.CritChance, 4),
        };
    }
}
