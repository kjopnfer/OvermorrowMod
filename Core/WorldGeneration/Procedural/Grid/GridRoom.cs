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
        /// Total tile width of this room's footprint, including any internal
        /// horizontal padding between sub-cells. A 1x1 room is one cell wide;
        /// a 2x1 room is two cells plus one internal seam.
        /// </summary>
        public int FootprintWidth =>
            DungeonGrid.CellTileWidth * CellWidth
            + DungeonGrid.HorizontalPadding * (CellWidth - 1);

        /// <summary>
        /// Total tile height of this room's footprint, including any internal
        /// vertical padding between sub-cells.
        /// </summary>
        public int FootprintHeight =>
            DungeonGrid.CellTileHeight * CellHeight
            + DungeonGrid.VerticalPadding * (CellHeight - 1);

        /// <summary>
        /// Padding rendering priority. Higher values are painted later by
        /// <see cref="PaddingBuilder.BuildAll"/>, so a higher-priority room
        /// always wins shared strips against a lower-priority neighbor.
        /// Default 0; rooms whose padding must override neighbor styling
        /// (e.g. CombatRoom's corridor-style entry passage) raise this.
        /// </summary>
        public virtual int PaddingPriority => 0;

        /// <summary>
        /// Renders the padding strip on one outward side of this room.
        /// PaddingBuilder calls this once per side per placed room.
        /// <para/>
        /// Contract: use <see cref="OvermorrowMod.Common.Utilities.WorldGenUtils.ReplaceTile"/>
        /// rather than PlaceTile for tile writes, so that openings cleared by
        /// Build cannot be accidentally sealed. SetWall and ClearWall are free
        /// to use since walls do not block movement.
        /// <para/>
        /// Default: do nothing, leaving the strip as the initial stone fill.
        /// </summary>
        /// <param name="ctx">Describes the strip to paint and which side it is on.</param>
        public virtual void BuildPadding(PaddingContext ctx) { }

        /// <summary>
        /// Per-side list of cell types that may neighbor this room. The base
        /// class derives both GetAcceptedNeighbors and the default Exits from
        /// this single source so a room only declares its allowed neighbors
        /// once.
        /// <para/>
        /// Rooms with sub-cell-specific rules (e.g. stairs, where only one
        /// sub-cell accepts a given side) should override
        /// GetAcceptedNeighbors directly and ignore this method.
        /// <para/>
        /// Default returns an empty array, meaning "no accepted neighbors on
        /// any side." Override per-side via a switch on Direction.
        /// </summary>
        protected virtual GridRoom[] AllowedNeighbors(Direction side) => Array.Empty<GridRoom>();

        /// <summary>
        /// Returns the set of GridRoom types accepted on the given edge of a sub-cell.
        /// For 1x1 rooms, subCol and subRow are always 0.
        /// For multi-cell rooms (e.g. 2x2 stairs), they identify which sub-cell.
        /// Return null or empty to reject all neighbors on that edge.
        /// <para/>
        /// Default impl skips internal edges (so multi-cell pieces don't try
        /// to match against their own seam) and otherwise reads from
        /// AllowedNeighbors. Sub-cell-aware rooms override this directly.
        /// </summary>
        public virtual HashSet<Type> GetAcceptedNeighbors(int subCol, int subRow, Direction side)
        {
            if (IsInternalEdge(subCol, subRow, side)) return null;
            var neighbors = AllowedNeighbors(side);
            if (neighbors == null || neighbors.Length == 0) return null;
            var set = new HashSet<Type>();
            foreach (var n in neighbors) set.Add(n.GetType());
            return set;
        }

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

        /// <summary>
        /// Decoration pass run after Build, BuildPadding, DecorateShafts, and
        /// ApplySideCaps. Use it to drop furniture and props that depend on
        /// neighbor context (e.g. sconces in a bookshelf's side padding only
        /// when a shaft sits above or below). Default: no-op.
        /// </summary>
        public virtual void PlaceFurniture(FurnitureContext ctx) { }

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
        /// <para/>
        /// Default impl builds one cardinal exit per side that has any entries
        /// in AllowedNeighbors. Cursor deltas are computed from CellWidth and
        /// CellHeight so a 1-cell-wide room steps (+1, 0) on the right and a
        /// 2-wide room steps (+2, 0). Rooms with non-cardinal exits (e.g.
        /// the diagonal stair exit) override this directly.
        /// </summary>
        public virtual CellExit[] Exits
        {
            get
            {
                var list = new List<CellExit>(4);
                AddCardinalExit(list, Direction.Right,  new Point(CellWidth, 0));
                AddCardinalExit(list, Direction.Left,   new Point(-1, 0));
                AddCardinalExit(list, Direction.Bottom, new Point(0, CellHeight));
                AddCardinalExit(list, Direction.Top,    new Point(0, -1));
                return list.ToArray();
            }
        }

        private void AddCardinalExit(List<CellExit> list, Direction side, Point delta)
        {
            var n = AllowedNeighbors(side);
            if (n != null && n.Length > 0) list.Add(new CellExit(delta, n));
        }

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
    /// One outward side of a placed room, passed to <see cref="GridRoom.BuildPadding"/>
    /// so the room can paint its own padding strip.
    /// <para/>
    /// PaddingBuilder calls BuildPadding once per side per room (Top, Bottom,
    /// Left, Right). Multi-cell rooms see one strip per side that already
    /// covers the internal-seam corners, so a 2x1 room's Top is 44 wide
    /// rather than two disjoint 18-wide strips.
    /// <para/>
    /// The context also exposes the dungeon grid and this room's grid
    /// position so a room can look up its neighbors when its visual depends
    /// on what is across the strip (for example, corridors only render their
    /// passage pattern when their neighbor is also a corridor).
    /// </summary>
    public readonly struct PaddingContext
    {
        /// <summary>
        /// Which outward side of the room this strip sits on. The strip lies
        /// just outside the room's footprint on that side.
        /// </summary>
        public readonly Direction Side;

        /// <summary>
        /// World-space X of the strip's top-left corner.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// World-space Y of the strip's top-left corner.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// Strip width in tiles. For Top and Bottom this equals the room's
        /// full footprint width (including any internal horizontal seam).
        /// For Left and Right this equals <see cref="DungeonGrid.HorizontalPadding"/>.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Strip height in tiles. For Left and Right this equals the room's
        /// full footprint height (including any internal vertical seam).
        /// For Top and Bottom this equals <see cref="DungeonGrid.VerticalPadding"/>.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// The dungeon's default solid fill tile (typically stone). Padding
        /// renderers rarely need this since the canvas-wide initial fill
        /// already stamps every gap with this tile, but it is provided for
        /// the occasional renderer that wants to explicitly reset a region.
        /// </summary>
        public readonly int FillTileType;

        /// <summary>
        /// The dungeon grid this room lives in. Use together with
        /// <see cref="Col"/> and <see cref="Row"/> to look up neighbors when
        /// the room's painting depends on what is on the other side of the
        /// strip.
        /// </summary>
        public readonly DungeonGrid Grid;

        /// <summary>
        /// Grid column of this room's anchor (its top-left sub-cell). For a
        /// 1x1 room this is the cell's column; for a 2x1 room at columns
        /// (C, C+1), this is C.
        /// </summary>
        public readonly int Col;

        /// <summary>
        /// Grid row of this room's anchor (its top-left sub-cell). For a 1x1
        /// room this is the cell's row; for a 2x2 room at rows (R, R+1),
        /// this is R.
        /// </summary>
        public readonly int Row;

        public PaddingContext(Direction side, int x, int y, int width, int height,
                              int fillTileType, DungeonGrid grid, int col, int row)
        {
            Side = side;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            FillTileType = fillTileType;
            Grid = grid;
            Col = col;
            Row = row;
        }
    }

    /// <summary>
    /// Context passed to <see cref="GridRoom.PlaceFurniture"/>. Exposes the
    /// room's world origin, its grid position, and the dungeon grid so
    /// neighbor-aware placements can branch on what sits above, below, or
    /// beside the room.
    /// </summary>
    public readonly struct FurnitureContext
    {
        public readonly Point Origin;
        public readonly DungeonGrid Grid;
        public readonly int Col;
        public readonly int Row;
        public readonly int FillTileType;
        public readonly int LiningTileType;

        public FurnitureContext(Point origin, DungeonGrid grid, int col, int row,
                                int fillTileType, int liningTileType)
        {
            Origin = origin;
            Grid = grid;
            Col = col;
            Row = row;
            FillTileType = fillTileType;
            LiningTileType = liningTileType;
        }
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
