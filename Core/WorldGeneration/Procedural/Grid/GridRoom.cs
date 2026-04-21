using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    public enum Direction
    {
        Top,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// Base class for all grid-based room pieces. Each GridRoom occupies one or more
    /// cells in the dungeon grid (e.g. 1x1 for a bookshelf, 2x2 for stairs).
    ///
    /// Each edge declares which GridRoom types it accepts as neighbors.
    /// The grid checks mutual compatibility: A accepts B on that edge AND B accepts A.
    /// </summary>
    public abstract class GridRoom
    {
        public abstract int CellWidth { get; }
        public abstract int CellHeight { get; }

        /// <summary>
        /// Returns the set of GridRoom types accepted on the given edge of a sub-cell.
        /// For 1x1 rooms, subCol and subRow are always 0.
        /// For multi-cell rooms (e.g. 2x2 stairs), they identify which sub-cell.
        /// Return null or empty to reject all neighbors on that edge.
        /// </summary>
        public abstract HashSet<Type> GetAcceptedNeighbors(int subCol, int subRow, Direction side);

        /// <summary>
        /// Returns true if the given edge is internal to a multi-cell piece.
        /// Internal edges only match other internal edges of the same piece instance.
        /// For 1x1 rooms this always returns false.
        /// </summary>
        public virtual bool IsInternalEdge(int subCol, int subRow, Direction side)
        {
            return false;
        }

        /// <summary>
        /// Build this piece at the given world-space top-left corner.
        /// For multi-cell pieces, this is the top-left of the entire block
        /// (including internal padding between sub-cells).
        /// </summary>
        public abstract void Build(Point origin, int fillTileType, int liningTileType);

        // ─── Walker interface ────────────────────────────────────────────────
        // Cells expose a list of directional exits. Each exit says:
        //   (1) how the cursor moves if the walker takes this exit, and
        //   (2) what cell types are legal to place on the other side.
        // Bidirectional cells (shafts, corridors) have two opposite exits,
        // neutral w.r.t. direction of travel — the walker's backtrack-prevention
        // picks which one is "forward" based on where it came from.

        /// <summary>
        /// Offset from the walker's cursor position to this cell's anchor
        /// (top-left of the footprint). Default: anchor == cursor.
        /// Ascending stair overrides: cursor enters at the stair's bottom-left
        /// sub-cell (0, 1), so anchor sits one row above.
        /// </summary>
        public virtual Point AnchorOffsetFromCursor => Point.Zero;

        /// <summary>
        /// Directional exits from this cell. The walker picks one per step.
        /// Each exit contains a cursor delta and the cell types valid through it.
        /// </summary>
        public virtual CellExit[] Exits => Array.Empty<CellExit>();
    }

    /// <summary>
    /// A single exit from a cell: cursor delta + cell types legal through it.
    /// </summary>
    public readonly struct CellExit
    {
        public Point CursorDelta { get; }
        public GridRoom[] AllowedNext { get; }

        public CellExit(Point cursorDelta, GridRoom[] allowedNext)
        {
            CursorDelta = cursorDelta;
            AllowedNext = allowedNext;
        }
    }
}
