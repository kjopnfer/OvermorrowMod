using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class BeefWellington : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Legendary;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.DamagePercent, 4),
            new CollectibleBonus(CollectibleStat.MaxLife, 30),
            new CollectibleBonus(CollectibleStat.DefenseFlat, 4),
            new CollectibleBonus(CollectibleStat.CritChance, 4),
            new CollectibleBonus(CollectibleStat.MoveSpeedPercent, 4),
        };
    }
}
