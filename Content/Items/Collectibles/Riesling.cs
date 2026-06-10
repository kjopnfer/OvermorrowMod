using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class Riesling : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Common;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.CritChance, -3),
            new CollectibleBonus(CollectibleStat.Knockback, 1),
        };
    }
}
