using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding
{
    /// <summary>
    /// One step along a planned path: which cell to put down, and where in
    /// the grid to put it.
    /// </summary>
    /// <remarks>
    /// <see cref="GridAStar.FindPath"/> returns a list of these. The caller
    /// walks the list and stamps each entry onto the grid in order.
    /// </remarks>
    public readonly struct PathStep
    {
        public GridRoom Cell { get; }
        public Point Anchor { get; }

        public PathStep(GridRoom cell, Point anchor)
        {
            Cell = cell;
            Anchor = anchor;
        }
    }
}
