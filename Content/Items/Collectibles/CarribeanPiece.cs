using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;
using System;
using Terraria;

namespace OvermorrowMod.Content.Items.Collectibles
{
    public class CarribeanPiece : CollectibleItem
    {
        public override Rarity Rarity => Rarity.Legendary;

        protected override CollectibleBonus[] Bonuses => Array.Empty<CollectibleBonus>();

        protected override CollectibleEffect[] DescribedEffects => new[]
        {
            CollectibleEffect.ExtraCoinsOnHit,
        };

        protected override void OnConsumed(Player player)
        {
            player.GetModPlayer<CollectiblePlayer>().SetEffect(CollectibleEffect.ExtraCoinsOnHit, 0.05f);
        }
    }
}
