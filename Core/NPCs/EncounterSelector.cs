using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.RoomManager;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.NPCs
{
    /// <summary>
    /// Resolves harvested SpawnSlots into NPCSpawnPoint TileEntities. Runs at
    /// gen time after furniture. Elite pass first, then segment-partitioned
    /// common pass with a per-cell soft cap.
    /// </summary>
    public static class EncounterSelector
    {
        private const int SoftCapPerCell = 3;

        public static void Run(List<SpawnSlot> allSlots, IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool> dungeonBindings, Dictionary<Point, IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool>> cellLocalBindings, float baseDensity, float eliteChance, Random rand)
        {
            if (allSlots == null || allSlots.Count == 0) return;

            ResolvePools(allSlots, dungeonBindings, cellLocalBindings);

            var resolvedSlots = allSlots.Where(s => s.Pool != null).ToList();
            if (resolvedSlots.Count == 0) return;

            var slotsByCell = resolvedSlots.GroupBy(s => s.GridCoord).ToDictionary(g => g.Key, g => g.ToList());
            var cellCoords = slotsByCell.Keys.ToList();
            cellCoords.Sort((a, b) => a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y));
            int cellCount = cellCoords.Count;

            int totalEnemies = (int)Math.Round(baseDensity * cellCount);
            if (totalEnemies <= 0) return;

            int segmentCount = Math.Max(1, Math.Min(cellCount, (int)Math.Ceiling(cellCount / 2.5)));
            int[] segmentLengths = RandomIntegerPartition(cellCount, segmentCount, rand, minPerSegment: 1);
            int[] segmentCounts = RandomIntegerPartition(totalEnemies, segmentCount, rand, minPerSegment: 0);

            var cellToSegment = new Dictionary<Point, int>();
            int cellIdx = 0;
            for (int s = 0; s < segmentCount; s++)
                for (int i = 0; i < segmentLengths[s]; i++)
                {
                    if (cellIdx < cellCoords.Count) cellToSegment[cellCoords[cellIdx]] = s;
                    cellIdx++;
                }

            RunElitePass(resolvedSlots, eliteChance, cellToSegment, segmentCounts, rand);

            for (int s = 0; s < segmentCount; s++)
                ResolveSegmentCommons(s, segmentCounts[s], cellToSegment, slotsByCell, rand);

            PlaceTileEntities(resolvedSlots);
        }

        private static void ResolvePools(List<SpawnSlot> slots, IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool> dungeonBindings, Dictionary<Point, IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool>> cellLocalBindings)
        {
            foreach (var slot in slots)
            {
                SpawnPool pool = null;
                if (cellLocalBindings != null
                    && cellLocalBindings.TryGetValue(slot.GridCoord, out var localMap)
                    && localMap != null
                    && localMap.TryGetValue(slot.Color, out var localPool))
                {
                    pool = localPool;
                }
                else if (dungeonBindings != null && dungeonBindings.TryGetValue(slot.Color, out var dungeonPool))
                {
                    pool = dungeonPool;
                }
                slot.Pool = pool;
            }
        }

        private static void RunElitePass(List<SpawnSlot> resolvedSlots, float eliteChance, Dictionary<Point, int> cellToSegment, int[] segmentCounts, Random rand)
        {
            if (rand.NextDouble() >= eliteChance) return;

            var eliteOptions = resolvedSlots
                .Where(s => s.ResolvedNpcType < 0)
                .SelectMany(s => s.Pool.Entries.Where(e => e.Tier == SpawnTier.Elite).Select(e => (slot: s, entry: e)))
                .ToList();
            if (eliteOptions.Count == 0) return;

            var pick = eliteOptions[rand.Next(eliteOptions.Count)];
            pick.slot.ResolvedNpcType = pick.entry.NpcType;
            if (cellToSegment.TryGetValue(pick.slot.GridCoord, out int segIdx))
                segmentCounts[segIdx] = Math.Max(0, segmentCounts[segIdx] - 1);
        }

        private static void ResolveSegmentCommons(int segmentIndex, int quota, Dictionary<Point, int> cellToSegment, Dictionary<Point, List<SpawnSlot>> slotsByCell, Random rand)
        {
            if (quota <= 0) return;

            var segCells = cellToSegment
                .Where(kv => kv.Value == segmentIndex && slotsByCell.ContainsKey(kv.Key))
                .Select(kv => kv.Key)
                .ToList();
            if (segCells.Count == 0) return;

            var cellQuotas = segCells.ToDictionary(c => c, _ => 0);

            DistributeQuota(segCells, slotsByCell, cellQuotas, ref quota, capPerCell: SoftCapPerCell, rand);
            if (quota > 0) DistributeQuota(segCells, slotsByCell, cellQuotas, ref quota, capPerCell: int.MaxValue, rand);

            foreach (var c in segCells)
            {
                int q = cellQuotas[c];
                if (q == 0) continue;
                var available = slotsByCell[c].Where(s => s.ResolvedNpcType < 0).ToList();
                ShuffleInPlace(available, rand);

                int placed = 0;
                foreach (var slot in available)
                {
                    if (placed >= q) break;
                    var commons = slot.Pool.Entries.Where(e => e.Tier == SpawnTier.Common).ToList();
                    if (commons.Count == 0) continue;
                    var pick = commons[rand.Next(commons.Count)];
                    slot.ResolvedNpcType = pick.NpcType;
                    placed++;
                }
            }
        }

        private static void DistributeQuota(List<Point> segCells, Dictionary<Point, List<SpawnSlot>> slotsByCell, Dictionary<Point, int> cellQuotas, ref int quota, int capPerCell, Random rand)
        {
            while (quota > 0)
            {
                bool added = false;
                ShuffleInPlace(segCells, rand);
                foreach (var c in segCells)
                {
                    if (quota <= 0) break;
                    int current = cellQuotas[c];
                    int availableUnresolved = slotsByCell[c].Count(s => s.ResolvedNpcType < 0);
                    if (current < capPerCell && current < availableUnresolved)
                    {
                        cellQuotas[c] = current + 1;
                        quota--;
                        added = true;
                    }
                }
                if (!added) break;
            }
        }

        private static void PlaceTileEntities(List<SpawnSlot> resolvedSlots)
        {
            int teType = ModContent.GetInstance<NPCSpawnPoint>().Type;
            foreach (var slot in resolvedSlots)
            {
                if (slot.ResolvedNpcType < 0) continue;
                int teId = ModContent.GetInstance<NPCSpawnPoint>().Place(slot.WorldPos.X, slot.WorldPos.Y);
                if (teId == -1) continue;
                if (TileEntity.ByID.TryGetValue(teId, out TileEntity te) && te is NPCSpawnPoint sp)
                    sp.NPCType = slot.ResolvedNpcType;
            }
        }

        private static int[] RandomIntegerPartition(int total, int parts, Random rand, int minPerSegment)
        {
            if (parts <= 0) return Array.Empty<int>();
            if (parts == 1) return new[] { total };

            int adjustedTotal = total - minPerSegment * parts;
            if (adjustedTotal < 0)
            {
                var fallback = new int[parts];
                int per = total / parts;
                int remainder = total % parts;
                for (int i = 0; i < parts; i++) fallback[i] = per + (i < remainder ? 1 : 0);
                return fallback;
            }

            var dividers = new int[parts - 1];
            for (int i = 0; i < parts - 1; i++) dividers[i] = rand.Next(0, adjustedTotal + 1);
            Array.Sort(dividers);

            var counts = new int[parts];
            int prev = 0;
            for (int i = 0; i < parts - 1; i++)
            {
                counts[i] = dividers[i] - prev + minPerSegment;
                prev = dividers[i];
            }
            counts[parts - 1] = adjustedTotal - prev + minPerSegment;
            return counts;
        }

        private static void ShuffleInPlace<T>(List<T> list, Random rand)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
