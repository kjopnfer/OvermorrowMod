using OvermorrowMod.Core.Loot.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Loot
{
    public class LootRarityItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            if (item.ModItem == null) return;
            if (LootMetadata.TryGetAny(item.type, out var meta))
                item.rare = RarityType(meta.Rarity);
        }

        public static int RarityType(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Rare => ModContent.RarityType<RareRarity>(),
                Rarity.Epic => ModContent.RarityType<EpicRarity>(),
                Rarity.Legendary => ModContent.RarityType<LegendaryRarity>(),
                _ => ModContent.RarityType<CommonRarity>(),
            };
        }
    }
}
