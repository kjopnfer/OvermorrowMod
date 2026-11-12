using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class SundropBerry : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Common;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.MaxLife, 20),
        };
    }
}
