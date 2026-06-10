using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class PredatorsTalisman : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Epic;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.DamageFlat, 3),
            new CollectibleBonus(CollectibleStat.DamagePercent, 3),
            new CollectibleBonus(CollectibleStat.CritChance, 3),
            new CollectibleBonus(CollectibleStat.ArmorPenetration, 5),
        };
    }
}
