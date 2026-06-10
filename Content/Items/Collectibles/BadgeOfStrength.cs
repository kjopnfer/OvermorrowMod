using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class BadgeOfStrength : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Rare;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.DamagePercent, 4),
        };
    }
}
