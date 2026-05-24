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
    /// Walks the dungeon path in context-aware batches, spending a threat budget per batch.
    /// Each batch's intensity depends on the running context, and elites claim their cell exclusively.
    /// </summary>
    public static class EncounterSelector
    {
        private const int FirstBatchSize = 2;
        private const float FirstBatchIntensityMin = 0.3f;
        private const float FirstBatchIntensityMax = 0.5f;

        private const float EarlyProgressThreshold = 0.25f;
        private const float EarlyIntensityCap = 1.0f;

        private const float LowIntensityMax = 0.8f;
        private const float MidIntensityMax = 1.2f;

        private const float AfterHighMin = 0.5f;
        private const float AfterHighMax = 0.8f;
        private const float AfterMidMin = 0.4f;
        private const float AfterMidMax = 1.3f;
        private const float AfterLowMin = 0.4f;
        private const float AfterLowMax = 1.6f;
        private const float TwoLowsInARowMin = 0.8f;

        private const float PerCellThreatCap = 3.0f;
        private const float AverageCommonThreat = 1.3f;

        private const float EliteIntensityThreshold = 1.2f;
        private const int EliteMinBatchDistance = 2;

        private class Context
        {
            public float PrevIntensity = -1f;
            public float PrevPrevIntensity = -1f;
            public int BatchesSinceElite = EliteMinBatchDistance;
            public int TotalCells;
            public int CellsConsumed;
            public float Progress => (float)CellsConsumed / Math.Max(1, TotalCells);
        }

        public static void Run(List<SpawnSlot> allSlots, IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool> dungeonBindings, Dictionary<Point, IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool>> cellLocalBindings, float baseDensity, float eliteChance, Random rand)
        {
            if (allSlots == null || allSlots.Count == 0) return;

            ResolvePools(allSlots, dungeonBindings, cellLocalBindings);
            var resolvedSlots = allSlots.Where(s => s.Pool != null).ToList();
            if (resolvedSlots.Count == 0) return;

            var slotsByCell = resolvedSlots.GroupBy(s => s.GridCoord).ToDictionary(g => g.Key, g => g.ToList());
            var cellOrder = slotsByCell.Keys.ToList();
            cellOrder.Sort((a, b) => a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y));

            var ctx = new Context { TotalCells = cellOrder.Count };
            int cursor = 0;
            while (cursor < cellOrder.Count)
            {
                int batchSize = ChooseBatchSize(ctx, cellOrder.Count - cursor);
                float intensity = ChooseIntensity(ctx, baseDensity, rand);
                var batchCells = cellOrder.GetRange(cursor, batchSize);

                DistributeBatch(batchCells, intensity, slotsByCell, eliteChance, ctx, rand);

                ctx.PrevPrevIntensity = ctx.PrevIntensity;
                ctx.PrevIntensity = intensity;
                ctx.CellsConsumed += batchSize;
                cursor += batchSize;
            }

            PlaceTileEntities(resolvedSlots);
        }

        private static int ChooseBatchSize(Context ctx, int remaining)
        {
            int desired;
            if (ctx.PrevIntensity < 0) desired = FirstBatchSize;
            else if (ctx.PrevIntensity >= MidIntensityMax) desired = 5;
            else if (ctx.PrevIntensity >= LowIntensityMax) desired = 4;
            else desired = 3;
            return Math.Min(desired, remaining);
        }

        private static float ChooseIntensity(Context ctx, float baseDensity, Random rand)
        {
            if (ctx.PrevIntensity < 0)
                return Lerp(FirstBatchIntensityMin, FirstBatchIntensityMax, rand) * baseDensity;

            float min;
            float max;
            if (ctx.PrevIntensity >= MidIntensityMax) { min = AfterHighMin; max = AfterHighMax; }
            else if (ctx.PrevIntensity >= LowIntensityMax) { min = AfterMidMin; max = AfterMidMax; }
            else { min = AfterLowMin; max = AfterLowMax; }

            if (ctx.PrevIntensity < LowIntensityMax && ctx.PrevPrevIntensity >= 0 && ctx.PrevPrevIntensity < LowIntensityMax)
                min = Math.Max(min, TwoLowsInARowMin);

            if (ctx.Progress < EarlyProgressThreshold)
                max = Math.Min(max, EarlyIntensityCap);

            if (min > max) min = max;
            return Lerp(min, max, rand) * baseDensity;
        }

        private static void DistributeBatch(List<Point> batchCells, float intensity, Dictionary<Point, List<SpawnSlot>> slotsByCell, float eliteChance, Context ctx, Random rand)
        {
            float budget = batchCells.Count * intensity;
            Point? eliteCell = null;

            bool eliteEligible = intensity >= EliteIntensityThreshold && ctx.BatchesSinceElite >= EliteMinBatchDistance;
            if (eliteEligible && rand.NextDouble() < eliteChance)
                eliteCell = TryPlaceElite(batchCells, ref budget, slotsByCell, rand);

            if (eliteCell.HasValue) ctx.BatchesSinceElite = 0;
            else ctx.BatchesSinceElite++;

            var commonCells = batchCells.Where(c => slotsByCell.ContainsKey(c) && c != eliteCell).ToList();
            if (commonCells.Count == 0 || budget <= 0) return;

            int targetActive = Math.Max(1, Math.Min(commonCells.Count, (int)Math.Round(budget / AverageCommonThreat)));
            ShuffleInPlace(commonCells, rand);
            var activeCells = commonCells.Take(targetActive).ToList();

            var cellThreat = activeCells.ToDictionary(c => c, _ => 0f);
            bool placed = true;
            while (budget > 0 && placed)
            {
                placed = false;
                ShuffleInPlace(activeCells, rand);
                foreach (var cell in activeCells)
                {
                    if (cellThreat[cell] >= PerCellThreatCap) continue;
                    var unresolvedSlots = slotsByCell[cell].Where(s => s.ResolvedNpcType < 0).ToList();
                    if (unresolvedSlots.Count == 0) continue;

                    int alliesInCell = slotsByCell[cell].Count(s => s.ResolvedNpcType >= 0);
                    var slot = unresolvedSlots[rand.Next(unresolvedSlots.Count)];
                    var commons = slot.Pool.Entries.Where(e => e.Tier == SpawnTier.Common && e.Threat <= budget && cellThreat[cell] + e.Threat <= PerCellThreatCap && e.MinAlliesInCell <= alliesInCell).ToList();
                    if (commons.Count == 0) continue;

                    var pick = commons[rand.Next(commons.Count)];
                    slot.ResolvedNpcType = pick.NpcType;
                    cellThreat[cell] += pick.Threat;
                    budget -= pick.Threat;
                    placed = true;
                    if (budget <= 0) break;
                }
            }
        }

        private static Point? TryPlaceElite(List<Point> batchCells, ref float budget, Dictionary<Point, List<SpawnSlot>> slotsByCell, Random rand)
        {
            float localBudget = budget;
            var candidates = batchCells.Where(c => slotsByCell.ContainsKey(c)).ToList();
            ShuffleInPlace(candidates, rand);
            foreach (var cell in candidates)
            {
                foreach (var slot in slotsByCell[cell])
                {
                    var elites = slot.Pool.Entries.Where(e => e.Tier == SpawnTier.Elite && e.Threat <= localBudget).ToList();
                    if (elites.Count == 0) continue;

                    var pick = elites[rand.Next(elites.Count)];
                    slot.ResolvedNpcType = pick.NpcType;
                    budget -= pick.Threat;
                    return cell;
                }
            }
            return null;
        }

        private static void ResolvePools(List<SpawnSlot> slots, IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool> dungeonBindings, Dictionary<Point, IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool>> cellLocalBindings)
        {
            foreach (var slot in slots)
            {
                SpawnPool pool = null;
                if (cellLocalBindings != null && cellLocalBindings.TryGetValue(slot.GridCoord, out var localMap) && localMap != null && localMap.TryGetValue(slot.Color, out var localPool))
                    pool = localPool;
                else if (dungeonBindings != null && dungeonBindings.TryGetValue(slot.Color, out var dungeonPool))
                    pool = dungeonPool;
                slot.Pool = pool;
            }
        }

        private static void PlaceTileEntities(List<SpawnSlot> resolvedSlots)
        {
            foreach (var slot in resolvedSlots)
            {
                if (slot.ResolvedNpcType < 0) continue;
                int teId = ModContent.GetInstance<NPCSpawnPoint>().Place(slot.WorldPos.X, slot.WorldPos.Y);
                if (teId == -1) continue;
                if (TileEntity.ByID.TryGetValue(teId, out TileEntity te) && te is NPCSpawnPoint sp)
                    sp.NPCType = slot.ResolvedNpcType;
            }
        }

        private static float Lerp(float min, float max, Random rand) => min + (float)rand.NextDouble() * (max - min);

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
