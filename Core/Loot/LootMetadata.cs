using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Loot
{
    public enum ArmorSlot { None, Head, Body, Legs }

    public readonly struct LootMetadataEntry
    {
        public readonly ItemType Affinities;
        public readonly Rarity Rarity;
        public readonly ItemKind Kind;
        public readonly ArmorSlot ArmorSlot;

        public LootMetadataEntry(ItemType affinities, Rarity rarity, ItemKind kind, ArmorSlot armorSlot)
        {
            Affinities = affinities;
            Rarity = rarity;
            Kind = kind;
            ArmorSlot = armorSlot;
        }
    }

    public static class LootMetadata
    {
        private static readonly Dictionary<Type, Dictionary<int, LootMetadataEntry>> byPool = new();

        public static void Set(Type poolType, ModItem item, ItemType affinities, Rarity rarity)
        {
            if (!byPool.TryGetValue(poolType, out var dict))
            {
                dict = new Dictionary<int, LootMetadataEntry>();
                byPool[poolType] = dict;
            }
            var kind = InferKind(item.Item);
            var armorSlot = InferArmorSlot(item.Item);
            dict[item.Type] = new LootMetadataEntry(affinities, rarity, kind, armorSlot);
        }

        public static void Set(Type poolType, int itemType, ItemType affinities, Rarity rarity)
        {
            if (!byPool.TryGetValue(poolType, out var dict))
            {
                dict = new Dictionary<int, LootMetadataEntry>();
                byPool[poolType] = dict;
            }
            var probe = new Item();
            probe.SetDefaults(itemType);
            var kind = InferKind(probe);
            var armorSlot = InferArmorSlot(probe);
            dict[itemType] = new LootMetadataEntry(affinities, rarity, kind, armorSlot);
        }

        public static bool TryGet(Type poolType, int itemType, out LootMetadataEntry entry)
        {
            if (byPool.TryGetValue(poolType, out var dict)) return dict.TryGetValue(itemType, out entry);
            entry = default;
            return false;
        }

        /// <summary>
        /// Looks up an item's metadata in any pool. Used when the caller does not have
        /// a specific pool context, e.g. inspecting a player's equipped item for affinity.
        /// </summary>
        public static bool TryGetAny(int itemType, out LootMetadataEntry entry)
        {
            foreach (var dict in byPool.Values)
            {
                if (dict.TryGetValue(itemType, out entry)) return true;
            }
            entry = default;
            return false;
        }

        public static IEnumerable<KeyValuePair<int, LootMetadataEntry>> EntriesInPool(Type poolType)
        {
            if (byPool.TryGetValue(poolType, out var dict)) return dict;
            return Array.Empty<KeyValuePair<int, LootMetadataEntry>>();
        }

        public static void Clear() => byPool.Clear();

        private static ItemKind InferKind(Item item)
        {
            if (item.consumable && item.damage <= 0) return ItemKind.Consumable;
            if (item.accessory) return ItemKind.Accessory;
            if (item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0) return ItemKind.Armor;
            if (item.damage > 0) return ItemKind.Weapon;
            return ItemKind.Consumable;
        }

        private static ArmorSlot InferArmorSlot(Item item)
        {
            if (item.headSlot >= 0) return ArmorSlot.Head;
            if (item.bodySlot >= 0) return ArmorSlot.Body;
            if (item.legSlot >= 0) return ArmorSlot.Legs;
            return ArmorSlot.None;
        }
    }
}
