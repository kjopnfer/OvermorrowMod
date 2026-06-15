using System.Collections.Generic;
using OvermorrowMod.Core.Loot;
using Terraria;

namespace OvermorrowMod.Core.Shop
{
    public static class ArchiveShopPool
    {
        public const int DefaultStockSize = 4;

        public static List<(int type, int price)> Roll(Player player, int count = DefaultStockSize)
        {
            var result = new List<(int, int)>();

            LootPool pool = LootPoolRegistry.GetActive();
            if (pool == null) return result;

            var used = new HashSet<int>();
            int attempts = 0;
            int maxAttempts = count * 6;

            while (result.Count < count && attempts < maxAttempts)
            {
                attempts++;

                int id = LootRoller.RollOne(pool, RandomKind(), default, player, used);
                if (id == 0) continue;

                used.Add(id);
                result.Add((id, PriceFor(pool, id)));
            }

            return result;
        }

        private static ItemKind RandomKind()
        {
            return Main.rand.Next(3) switch
            {
                0 => ItemKind.Weapon,
                1 => ItemKind.Accessory,
                _ => ItemKind.Armor,
            };
        }

        private static int PriceFor(LootPool pool, int itemType)
        {
            Rarity rarity = Rarity.Common;
            if (LootMetadata.TryGet(pool.GetType(), itemType, out var meta))
                rarity = meta.Rarity;

            return rarity switch
            {
                Rarity.Rare => Item.buyPrice(gold: 1, silver: 50),
                Rarity.Epic => Item.buyPrice(gold: 4),
                Rarity.Legendary => Item.buyPrice(gold: 10),
                _ => Item.buyPrice(silver: 75),
            };
        }
    }
}
