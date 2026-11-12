using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class Milk : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Rare;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.DefenseFlat, 4),
        };
    }
}
