using Microsoft.Xna.Framework;
using OvermorrowMod.Core.Globals;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Loot
{
    public static class LootRoller
    {
        public static int[] RollOffers(LootPool pool, RarityModifier modifier, ItemKind allowedKinds, int count, Player player, ItemKind favoredKinds = ItemKind.None)
        {
            if (pool == null || count <= 0) return Array.Empty<int>();

            ItemKind chosenKind = PickOneKind(allowedKinds, favoredKinds);
            if (chosenKind == ItemKind.None) return Array.Empty<int>();

            var candidates = new List<(int Type, LootMetadataEntry Meta)>();
            foreach (var pair in LootMetadata.EntriesInPool(pool.GetType()))
            {
                if (pair.Value.Kind != chosenKind) continue;
                candidates.Add((pair.Key, pair.Value));
            }
            if (candidates.Count == 0) return Array.Empty<int>();

            ItemType activeClasses = ComputeActiveClasses(player);
            RarityWeights effectiveWeights = pool.BaseWeights + modifier;

            var bagPlayer = player.GetModPlayer<LootPlayer>();
            var offered = new List<int>(count);

            for (int i = 0; i < count; i++)
            {
                Rarity rolledRarity = effectiveWeights.Sample(Main.rand);
                var bucket = SelectBucket(candidates, rolledRarity);
                if (bucket.Count == 0) break;

                int? pick = WeightedPickFromBucket(bucket, activeClasses, chosenKind, player, pool.GetType(), bagPlayer, offered);
                if (pick == null) continue;

                offered.Add(pick.Value);
                bagPlayer.RecordOffered(pool.GetType(), pick.Value);
            }

            ApplyRelevanceGuarantee(offered, candidates, activeClasses);
            ApplyHybridSeeding(offered, candidates, activeClasses, chosenKind);

            return offered.ToArray();
        }

        public static void GiveCompanionAmmo(IEntitySource source, Vector2 position, int itemId)
        {
            if (itemId <= 0) return;

            Item reward = new Item();
            reward.SetDefaults(itemId);

            int ammoType;
            int stack;
            if (reward.useAmmo == AmmoID.Arrow)
            {
                ammoType = ItemID.WoodenArrow;
                stack = 100;
            }
            else if (reward.useAmmo == AmmoID.Bullet)
            {
                ammoType = ItemID.MusketBall;
                stack = 100;
            }
            else if (reward.useAmmo == AmmoID.Rocket)
            {
                ammoType = ItemID.RocketI;
                stack = 50;
            }
            else
            {
                return;
            }

            Item.NewItem(source, position, ammoType, stack);
        }

        private static ItemKind PickOneKind(ItemKind allowedKinds, ItemKind favoredKinds)
        {
            var bits = new List<ItemKind>();
            double total = 0;
            foreach (ItemKind k in Enum.GetValues(typeof(ItemKind)))
            {
                if (k == ItemKind.None) continue;
                if ((allowedKinds & k) == 0) continue;
                bits.Add(k);
                total += KindWeight(k, favoredKinds);
            }
            if (bits.Count == 0) return ItemKind.None;

            double cursor = Main.rand.NextDouble() * total;
            foreach (var k in bits)
            {
                cursor -= KindWeight(k, favoredKinds);
                if (cursor <= 0) return k;
            }
            return bits[^1];
        }

        private static double KindWeight(ItemKind kind, ItemKind favoredKinds)
        {
            return (favoredKinds & kind) != 0 ? 1.4 : 1.0;
        }

        private static List<(int Type, LootMetadataEntry Meta)> SelectBucket(List<(int Type, LootMetadataEntry Meta)> candidates, Rarity rolledRarity)
        {
            for (int r = (int)rolledRarity; r >= 0; r--)
            {
                var bucket = new List<(int, LootMetadataEntry)>();
                foreach (var c in candidates)
                {
                    if ((int)c.Meta.Rarity == r) bucket.Add(c);
                }
                if (bucket.Count > 0) return bucket;
            }
            for (int r = (int)rolledRarity + 1; r <= (int)Rarity.Epic; r++)
            {
                var bucket = new List<(int, LootMetadataEntry)>();
                foreach (var c in candidates)
                {
                    if ((int)c.Meta.Rarity == r) bucket.Add(c);
                }
                if (bucket.Count > 0) return bucket;
            }
            return new List<(int, LootMetadataEntry)>();
        }

        private static int? WeightedPickFromBucket(List<(int Type, LootMetadataEntry Meta)> bucket, ItemType activeClasses, ItemKind chosenKind, Player player, Type poolType, LootPlayer bagPlayer, List<int> alreadyOffered)
        {
            var weights = new List<(int Type, double Weight)>();
            double total = 0;
            foreach (var c in bucket)
            {
                if (alreadyOffered.Contains(c.Type)) continue;
                double w = RelevanceWeight(c.Meta.Affinities, activeClasses);
                if (chosenKind == ItemKind.Armor) w *= ArmorSlotWeight(c.Meta.ArmorSlot, player);
                weights.Add((c.Type, w));
                total += w;
            }

            var nonBag = weights.FindAll(w => !bagPlayer.WasRecentlyOffered(poolType, w.Type));
            double nonBagTotal = 0;
            foreach (var w in nonBag) nonBagTotal += w.Weight;

            List<(int Type, double Weight)> finalWeights;
            double finalTotal;
            if (nonBag.Count > 0 && nonBagTotal > 0)
            {
                finalWeights = nonBag;
                finalTotal = nonBagTotal;
            }
            else if (total > 0)
            {
                finalWeights = weights;
                finalTotal = total;
            }
            else
            {
                return null;
            }

            double cursor = Main.rand.NextDouble() * finalTotal;
            foreach (var w in finalWeights)
            {
                cursor -= w.Weight;
                if (cursor <= 0) return w.Type;
            }
            return finalWeights[^1].Type;
        }

        private static ItemType ComputeActiveClasses(Player player)
        {
            ItemType acc = ItemType.None;

            void Accumulate(Item item)
            {
                if (item == null || item.IsAir) return;
                if (LootMetadata.TryGetAny(item.type, out var meta))
                {
                    if (meta.Affinities != ItemType.Generic) acc |= meta.Affinities;
                }
            }

            Accumulate(player.HeldItem);
            int armorMax = Math.Min(10, player.armor.Length);
            for (int i = 0; i < armorMax; i++) Accumulate(player.armor[i]);

            if (player.TryGetModPlayer<SubworldPlayer>(out var subPlayer))
            {
                Accumulate(subPlayer.loadoutWeapon);
                Accumulate(subPlayer.loadoutMisc);
            }

            return acc;
        }

        private static double RelevanceWeight(ItemType affinities, ItemType activeClasses)
        {
            if (affinities == ItemType.None) return 0.3;
            if (affinities == ItemType.Generic) return 0.8;
            if (activeClasses == ItemType.None) return 1.0;
            var overlap = affinities & activeClasses;
            if (overlap == ItemType.None) return 0.3;
            if ((affinities & ~activeClasses) == ItemType.None) return 1.0;
            return 1.3;
        }

        private static double ArmorSlotWeight(ArmorSlot armorSlot, Player player)
        {
            return armorSlot switch
            {
                ArmorSlot.Head => SlotIsFilled(player, 0) ? 0.3 : 1.0,
                ArmorSlot.Body => SlotIsFilled(player, 1) ? 0.3 : 1.0,
                ArmorSlot.Legs => SlotIsFilled(player, 2) ? 0.3 : 1.0,
                _ => 1.0,
            };
        }

        private static bool SlotIsFilled(Player player, int armorIndex)
        {
            if (armorIndex < 0 || armorIndex >= player.armor.Length) return false;
            var s = player.armor[armorIndex];
            return s != null && !s.IsAir;
        }

        private static void ApplyRelevanceGuarantee(List<int> offered, List<(int Type, LootMetadataEntry Meta)> candidates, ItemType activeClasses)
        {
            if (activeClasses == ItemType.None) return;
            if (offered.Count == 0) return;

            bool anyIntersect = false;
            foreach (int t in offered)
            {
                if (LootMetadata.TryGetAny(t, out var m) && (m.Affinities & activeClasses) != ItemType.None)
                {
                    anyIntersect = true;
                    break;
                }
            }
            if (anyIntersect) return;

            int? swap = null;
            foreach (var c in candidates)
            {
                if (offered.Contains(c.Type)) continue;
                if ((c.Meta.Affinities & activeClasses) != ItemType.None)
                {
                    swap = c.Type;
                    break;
                }
            }
            if (swap == null) return;

            int replaceIdx = Main.rand.Next(offered.Count);
            offered[replaceIdx] = swap.Value;
        }

        private static void ApplyHybridSeeding(List<int> offered, List<(int Type, LootMetadataEntry Meta)> candidates, ItemType activeClasses, ItemKind chosenKind)
        {
            if (activeClasses == ItemType.None) return;
            if (activeClasses.BitCount() != 1) return;
            if (chosenKind == ItemKind.Armor) return;
            if (offered.Count == 0) return;

            foreach (int t in offered)
            {
                if (LootMetadata.TryGetAny(t, out var m) && m.Affinities.BitCount() > 1 && (m.Affinities & activeClasses) != ItemType.None) return;
            }

            int? hybrid = null;
            foreach (var c in candidates)
            {
                if (offered.Contains(c.Type)) continue;
                if (c.Meta.Affinities.BitCount() > 1 && (c.Meta.Affinities & activeClasses) != ItemType.None)
                {
                    hybrid = c.Type;
                    break;
                }
            }
            if (hybrid == null) return;

            int replaceIdx = Main.rand.Next(offered.Count);
            offered[replaceIdx] = hybrid.Value;
        }
    }
}
