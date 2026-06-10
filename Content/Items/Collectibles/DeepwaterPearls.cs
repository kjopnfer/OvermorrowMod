using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;
using System;
using Terraria;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class DeepwaterPearls : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Epic;

        protected override CollectibleBonus[] Bonuses => Array.Empty<CollectibleBonus>();

        protected override CollectibleEffect[] DescribedEffects => new[]
        {
            CollectibleEffect.ShopHealDiscount,
        };

        protected override void OnConsumed(Player player)
        {
            player.GetModPlayer<CollectiblePlayer>().SetEffect(CollectibleEffect.ShopHealDiscount, 0.20f);
        }
    }
}
