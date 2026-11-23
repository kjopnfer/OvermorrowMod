using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    public enum SearchPhase
    {
        Spine,
        Branch,
        Aux,
        Fork,
        Tributary
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
        public readonly SearchStats[] Searches = { new(), new(), new(), new(), new() };

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
        public int TributaryTries;
        public int TributariesPlaced;

        /// <summary>
        /// Times a fork exit didn't fit and the dungeon was planned again with its fork exits on the spine.
        /// </summary>
        public int ExitForkFallbacks;

        /// <summary>
        /// The spawn room sits past the middle of the dungeon's occupied cells toward one of its doors.
        /// </summary>
        public bool SpawnTowardDoor;

        /// <summary>
        /// A fork exit's door sits short of the dungeon's middle row in the exit's direction.
        /// </summary>
        public bool ExitInBackHalf;

        /// <summary>
        /// Per door: the rooms with no route to it that never steps against the door's direction, out of all rooms.
        /// </summary>
        public readonly List<(LayoutDirection Door, int Missing, int Rooms)> MonotoneRoutes = new();
        public int TreasureDoors;
        public long ElapsedMs;
    }
}
