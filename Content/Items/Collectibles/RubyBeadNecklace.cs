using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;
using System;
using Terraria;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class RubyBeadNecklace : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Rare;

        protected override CollectibleBonus[] Bonuses => Array.Empty<CollectibleBonus>();

        protected override CollectibleEffect[] DescribedEffects => new[]
        {
            CollectibleEffect.RestAreaHeal,
        };

        protected override void OnConsumed(Player player)
        {
            player.GetModPlayer<CollectiblePlayer>().SetEffect(CollectibleEffect.RestAreaHeal, 10f);
        }
    }
}
