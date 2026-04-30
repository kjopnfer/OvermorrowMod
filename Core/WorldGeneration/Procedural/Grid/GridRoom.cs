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
        // Bidirectional cells (shafts, corridors) have two opposite exits and
        // are neutral with respect to direction of travel. The walker's
        // backtrack-prevention picks which one is "forward" based on where it
        // came from.

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

        /// <summary>
        /// Structural validity check against neighbors that aren't in the
        /// walker's forward direction. Default accepts any placement; cells
        /// with strict neighbor rules (shafts need bookshelves above/below,
        /// corridors/stairs can't sit next to shafts vertically) override this.
        /// The walker calls it after <c>FitsFootprint</c>; only a candidate
        /// that both fits and is structurally valid gets placed.
        /// <para/>
        /// <paramref name="pendingLookup"/> lets the planner expose
        /// in-progress placements that aren't yet committed to the grid
        /// (for example, other cells in the same A* path). Returning a
        /// non-null cell means "treat this position as if that cell were
        /// placed". When null (or returns null), the check falls back to
        /// the committed grid only.
        /// </summary>
        public virtual bool IsValidPlacement(DungeonGrid grid, Microsoft.Xna.Framework.Point anchor,
                                              System.Func<int, int, GridRoom> pendingLookup = null) => true;

        /// <summary>
        /// Helper for IsValidPlacement overrides: resolves what cell (if any)
        /// is at a given grid position, considering in-progress placements
        /// first and the committed grid second.
        /// </summary>
        protected static GridRoom GetEffectiveRoomAt(DungeonGrid grid,
                                                    System.Func<int, int, GridRoom> pendingLookup,
                                                    int x, int y)
        {
            if (pendingLookup != null)
            {
                var pending = pendingLookup(x, y);
                if (pending != null) return pending;
            }
            var slot = grid.GetSlot(x, y);
            if (slot == null || slot.IsEmpty) return null;
            return slot.Room;
        }

        /// <summary>
        /// Returns whether the given side of the given sub-cell is physically
        /// open (a doorway in the rendered tiles) or closed (a wall).
        /// <para/>
        /// This is independent of pathfinding direction: it describes the
        /// cell's geometry, not where the walker is going. Two adjacent
        /// cells must agree on the shared side: both open (a passable
        /// connection) or both closed (back-to-back walls). A mismatch
        /// produces a "wall facing open side" disconnect.
        /// <para/>
        /// Default: every cardinal side is closed. Each cell type overrides
        /// this to expose its actual openings.
        /// </summary>
        public virtual bool IsOpenSide(int subCol, int subRow, Direction side) => false;

        /// <summary>
        /// Whether this cell is acceptable to leave standing next to an
        /// empty neighbor. True for rooms that finish their own edges
        /// visually (bookshelves, doors, future standalone rooms). False
        /// for connector pieces (corridors, shafts, stairs) whose open
        /// sides must connect to another cell or the dungeon reads as
        /// having broken hallways.
        /// </summary>
        public virtual bool AllowsEmptyNeighbors => true;
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
