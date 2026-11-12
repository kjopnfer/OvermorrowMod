using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OvermorrowMod.Core.WorldGeneration.TestSubworld
{
    /// <summary>
    /// Plans <see cref="TestSubworld"/> layouts without painting and reports planning time, search counters,
    /// outcomes and a fingerprint per layout. Layout i uses seed baseSeed + i, so for a fixed base seed equal
    /// fingerprints mean a change did not alter any layout. With reachability validation on, every search the
    /// reachability check rejects still runs A*, and the report counts the ones A* solved (must be 0).
    /// </summary>
    public static class GenBench
    {
        public const int DefaultCount = 5;
        public const int MaxCount = 50;

        public sealed class Result
        {
            public string Report;
            public long AverageMs;
            public long WorstMs;
        }

        public static Result Run(int count, int baseSeed, PlanOptions options)
        {
            var pass = TestSubworld.CreateGenPass();
            var plans = new List<DungeonPlan>();
            var layoutLines = new StringBuilder();
            long totalMs = 0;
            long worstMs = 0;
            int gen0 = GC.CollectionCount(0);
            int gen1 = GC.CollectionCount(1);
            int gen2 = GC.CollectionCount(2);
            long allocated = GC.GetTotalAllocatedBytes();

            for (int i = 0; i < count; i++)
            {
                int seed = baseSeed + i;
                var rand = new Random(seed);
                var stopwatch = Stopwatch.StartNew();
                var layoutPlans = pass.CreateLayout(rand).Plan(rand, options);
                long ms = stopwatch.ElapsedMilliseconds;
                totalMs += ms;
                worstMs = Math.Max(worstMs, ms);

                var fingerprints = layoutPlans.Select(p => Fingerprint(p.Grid)).ToArray();
                layoutLines.AppendLine($"layout {i} seed {seed}: {ms} ms, fingerprint {Fnv(string.Join("|", fingerprints)):x16}");
                for (int d = 0; d < layoutPlans.Length; d++)
                    layoutLines.AppendLine($"  dungeon {d}: {layoutPlans[d].Stats.ElapsedMs} ms, fingerprint {fingerprints[d]}");
                plans.AddRange(layoutPlans);
            }

            var sb = new StringBuilder();
            long averageMs = totalMs / count;
            sb.AppendLine($"genbench {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            string validation = options.ValidateReachability ? "on" : "off";
            string planning = options.Sequential ? "sequential" : "parallel";
            sb.AppendLine($"base seed {baseSeed}, {count} layouts, {plans.Count} dungeons, {planning} planning, reachability validation {validation}");
            sb.AppendLine($"average {averageMs} ms, worst {worstMs} ms");
            sb.AppendLine();
            sb.Append(layoutLines);
            sb.AppendLine();
            AppendSearches(sb, plans);
            sb.AppendLine();
            AppendOutcomes(sb, plans);
            sb.AppendLine();
            AppendRoomCounts(sb, plans);
            sb.AppendLine();
            sb.AppendLine($"GC collections: gen0 {GC.CollectionCount(0) - gen0}, gen1 {GC.CollectionCount(1) - gen1}, gen2 {GC.CollectionCount(2) - gen2}");
            sb.AppendLine($"allocated: {(GC.GetTotalAllocatedBytes() - allocated) / (1024 * 1024)} MB");

            return new Result { Report = sb.ToString(), AverageMs = averageMs, WorstMs = worstMs };
        }

        private static void AppendSearches(StringBuilder sb, List<DungeonPlan> plans)
        {
            sb.AppendLine("searches     calls  rejected   failed  budget spent  rejected+solvable  expansions  exp/success   time ms");
            foreach (SearchPhase phase in Enum.GetValues(typeof(SearchPhase)))
            {
                var total = new SearchStats();
                foreach (var plan in plans)
                {
                    var s = plan.Stats.Searches[(int)phase];
                    total.Calls += s.Calls;
                    total.Rejected += s.Rejected;
                    total.Failures += s.Failures;
                    total.BudgetSpentFailures += s.BudgetSpentFailures;
                    total.RejectedSolvable += s.RejectedSolvable;
                    total.Expansions += s.Expansions;
                    total.SuccessExpansions += s.SuccessExpansions;
                    total.Ticks += s.Ticks;
                }
                int successes = total.Calls - total.Failures - total.Rejected;
                long perSuccess = successes > 0 ? total.SuccessExpansions / successes : 0;
                sb.AppendLine($"{phase,-8} {total.Calls,9} {total.Rejected,9} {total.Failures,8} {total.BudgetSpentFailures,13} {total.RejectedSolvable,18} {total.Expansions,11} {perSuccess,12} {TicksToMs(total.Ticks),9}");
            }
            sb.AppendLine($"IsValidPlacement inside searches: {TicksToMs(plans.Sum(p => p.Stats.ValidPlacementTicks))} ms");
            sb.AppendLine();
            sb.AppendLine("successful searches: expansions per cell of distance | expansions | distance");
            sb.AppendLine("phase     count    p50    p90    p99    max |    p50    p90    p99    max |  p50  p90  max");
            foreach (SearchPhase phase in Enum.GetValues(typeof(SearchPhase)))
            {
                var samples = plans.SelectMany(p => p.Stats.Successes).Where(s => s.Phase == phase).ToList();
                if (samples.Count == 0) continue;
                var ratios = samples.Select(s => (double)s.Expansions / Math.Max(1, s.Distance)).OrderBy(r => r).ToList();
                var expansions = samples.Select(s => (double)s.Expansions).OrderBy(e => e).ToList();
                var distances = samples.Select(s => (double)s.Distance).OrderBy(d => d).ToList();
                sb.AppendLine($"{phase,-8} {samples.Count,6} {Percentile(ratios, 0.5),6:0.0} {Percentile(ratios, 0.9),6:0.0} {Percentile(ratios, 0.99),6:0.0} {ratios[^1],6:0.0} | {Percentile(expansions, 0.5),6:0} {Percentile(expansions, 0.9),6:0} {Percentile(expansions, 0.99),6:0} {expansions[^1],6:0} | {Percentile(distances, 0.5),4:0} {Percentile(distances, 0.9),4:0} {distances[^1],4:0}");
            }
        }

        private static void AppendOutcomes(StringBuilder sb, List<DungeonPlan> plans)
        {
            int n = plans.Count;
            int attempts = plans.Sum(p => p.Stats.SpineAttempts);
            sb.AppendLine("outcomes");
            sb.AppendLine($"spine attempts: {attempts} ({(double)attempts / n:0.0} per dungeon)");
            sb.AppendLine($"  no route: {plans.Sum(p => p.Stats.SpineAttemptsWithoutRoute)}");
            sb.AppendLine($"  missing a required room: {Share(plans.Sum(p => p.Stats.SpineAttemptsMissingRequired), attempts)}");
            sb.AppendLine($"  combat branch placed: {Share(plans.Sum(p => p.Stats.SpineAttemptsWithBranch), attempts)}");
            sb.AppendLine($"accepted: {Share(plans.Count(p => p.Stats.Accepted), n)}, the rest restored from the best attempt");
            sb.AppendLine($"combat branch in final layout: {Share(plans.Count(p => p.Stats.BranchPlaced), n)}");
            sb.AppendLine($"aux branches placed: {Share(plans.Sum(p => p.Stats.AuxPlaced), plans.Sum(p => p.Stats.AuxAttempts))}");
            sb.AppendLine($"forks: {plans.Sum(p => p.Stats.Forks)}, short stubs: {plans.Sum(p => p.Stats.ForkStubs)}, unplaced: {plans.Sum(p => p.Stats.ForkFailures)}");
        }

        private static void AppendRoomCounts(StringBuilder sb, List<DungeonPlan> plans)
        {
            var totals = new SortedDictionary<string, int>();
            var maxima = new Dictionary<string, int>();
            foreach (var plan in plans)
            {
                var counts = new Dictionary<string, int>();
                var grid = plan.Grid;
                for (int c = 0; c < grid.Cols; c++)
                {
                    for (int r = 0; r < grid.Rows; r++)
                    {
                        var slot = grid.GetSlot(c, r);
                        if (slot.IsEmpty || slot.SubCol != 0 || slot.SubRow != 0) continue;
                        string name = slot.Room.GetType().Name;
                        counts[name] = counts.GetValueOrDefault(name) + 1;
                    }
                }
                foreach (var kv in counts)
                {
                    totals[kv.Key] = totals.GetValueOrDefault(kv.Key) + kv.Value;
                    maxima[kv.Key] = Math.Max(maxima.GetValueOrDefault(kv.Key), kv.Value);
                }
            }
            sb.AppendLine("rooms per dungeon      average    max");
            foreach (var kv in totals)
                sb.AppendLine($"  {kv.Key,-18} {(double)kv.Value / plans.Count,8:0.00} {maxima[kv.Key],6}");
        }

        /// <summary>
        /// Hash over every placed room's anchor cell and type, in grid order.
        /// </summary>
        private static string Fingerprint(DungeonGrid grid)
        {
            var sb = new StringBuilder();
            for (int c = 0; c < grid.Cols; c++)
            {
                for (int r = 0; r < grid.Rows; r++)
                {
                    var slot = grid.GetSlot(c, r);
                    if (slot.IsEmpty || slot.SubCol != 0 || slot.SubRow != 0) continue;
                    sb.Append(c).Append(',').Append(r).Append(',').Append(slot.Room.GetType().Name).Append(';');
                }
            }
            return Fnv(sb.ToString()).ToString("x16");
        }

        private static ulong Fnv(string s)
        {
            ulong hash = 14695981039346656037UL;
            foreach (char ch in s)
            {
                hash ^= ch;
                hash *= 1099511628211UL;
            }
            return hash;
        }

        private static double Percentile(List<double> sorted, double p) => sorted[Math.Min(sorted.Count - 1, (int)(p * sorted.Count))];

        private static string Share(int part, int whole) => whole == 0 ? $"{part}" : $"{part} of {whole} ({100.0 * part / whole:0}%)";

        private static long TicksToMs(long ticks) => ticks * 1000 / Stopwatch.Frequency;
    }
}
