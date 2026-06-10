using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;
using Terraria;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class Rose : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Epic;

        protected override CollectibleBonus[] Bonuses => new[]
        {
            new CollectibleBonus(CollectibleStat.Luck, -1),
        };

        protected override CollectibleEffect[] DescribedEffects => new[]
        {
            CollectibleEffect.DecreaseSourceScaling,
        };

        protected override void OnConsumed(Player player)
        {
            player.GetModPlayer<CollectiblePlayer>().SetEffect(CollectibleEffect.DecreaseSourceScaling, 1f);
        }
    }
}
