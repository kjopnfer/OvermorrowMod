using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    public enum SearchPhase
    {
        Spine,
        Branch,
        Aux,
        Fork
    }

    /// <summary>
    /// Pathfinding counters for one generation phase of one dungeon.
    /// </summary>
    public sealed class SearchStats
    {
        public int Calls;

        /// <summary>
        /// Searches skipped because the reachability check found no open route to the goal.
        /// </summary>
        public int Rejected;

        /// <summary>
        /// Searches where A* ran and found no path within its budget.
        /// </summary>
        public int Failures;

        /// <summary>
        /// Failures that stopped when the expansion budget ran out. The other failures tried every route.
        /// </summary>
        public int BudgetSpentFailures;

        /// <summary>
        /// Rejected searches that A* solved anyway. Only counted when reachability validation is on, and must stay 0.
        /// </summary>
        public int RejectedSolvable;

        public long Expansions;
        public long SuccessExpansions;
        public long Ticks;
    }

    public readonly struct SearchSample
    {
        public readonly SearchPhase Phase;
        public readonly int Distance;
        public readonly int Expansions;

        public SearchSample(SearchPhase phase, int distance, int expansions)
        {
            Phase = phase;
            Distance = distance;
            Expansions = expansions;
        }
    }

    /// <summary>
    /// What happened while planning one dungeon, read by the generation benchmark.
    /// </summary>
    public sealed class PlanStats
    {
        public readonly SearchStats[] Searches = { new(), new(), new(), new() };

        /// <summary>
        /// Distance and expansion count of every successful search.
        /// </summary>
        public readonly List<SearchSample> Successes = new();

        public long ValidPlacementTicks;
        public int SpineAttempts;
        public int SpineAttemptsWithoutRoute;
        public int SpineAttemptsMissingRequired;
        public int SpineAttemptsWithBranch;
        public bool Accepted;
        public bool BranchPlaced;
        public int AuxPlaced;
        public int AuxAttempts;
        public int Forks;
        public int ForkStubs;
        public int ForkFailures;
        public long ElapsedMs;
    }
}
