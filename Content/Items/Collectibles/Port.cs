using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;
using Terraria;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class Port : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Rare;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.DamagePercent, 3),
            new CollectibleBonus(CollectibleStat.MaxLife, 20),
            new CollectibleBonus(CollectibleStat.DefenseFlat, 3),
            new CollectibleBonus(CollectibleStat.CritChance, 3),
            new CollectibleBonus(CollectibleStat.MoveSpeedPercent, 3),
        };

        protected override CollectibleEffect[] DescribedEffects => new[]
        {
            CollectibleEffect.ShopMarkup,
        };

        protected override void OnConsumed(Player player)
        {
            player.GetModPlayer<CollectiblePlayer>().SetEffect(CollectibleEffect.ShopMarkup, 0.20f);
        }
    }
}
