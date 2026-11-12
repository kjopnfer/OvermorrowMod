using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class WavebreakBadge : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Legendary;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.Knockback, 3),
            new CollectibleBonus(CollectibleStat.DamagePercent, 10),
        };
    }
}
