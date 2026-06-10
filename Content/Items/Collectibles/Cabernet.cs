using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class Cabernet : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Common;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.DefenseFlat, -5),
            new CollectibleBonus(CollectibleStat.DamagePercent, 5),
        };
    }
}
