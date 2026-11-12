using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class PinotNoir : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Epic;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.DamagePercent, -5),
            new CollectibleBonus(CollectibleStat.DefenseFlat, 5),
            new CollectibleBonus(CollectibleStat.DefensePercent, 10),
        };
    }
}
