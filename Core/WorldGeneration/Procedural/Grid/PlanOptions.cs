namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    public sealed class PlanOptions
    {
        /// <summary>
        /// Log each planning step and write a dump file per dungeon.
        /// </summary>
        public bool Diagnostics { get; init; }

        /// <summary>
        /// Run A* even for searches the reachability check rejects, to verify the check. Slower; for benchmarking.
        /// </summary>
        public bool ValidateReachability { get; init; }

        /// <summary>
        /// Plan the dungeons one after another on the calling thread. The plans are identical in both modes.
        /// </summary>
        public bool Sequential { get; init; }
    }
}
