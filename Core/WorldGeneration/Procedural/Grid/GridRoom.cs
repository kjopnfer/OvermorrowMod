using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Core.NPCs;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    public enum Direction
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum RoomType
    {
        None,
        Filler,
        Combat,
        Treasure,
        Door,
        VerticalConnector,
        HorizontalConnector,
        DiagonalConnector
    }

    /// <summary>
    /// Base class for all grid room pieces. Occupies one or more cells; each edge declares
    /// the room types it accepts as neighbors, matched mutually against the neighbor.
    /// </summary>
    public abstract class GridRoom
    {
        public abstract int CellWidth { get; }
        public abstract int CellHeight { get; }

        /// <summary>
        /// The structural function this room fills, used by the generator's layout checks.
        /// </summary>
        public virtual RoomType Type => RoomType.None;

        /// <summary>
        /// Whether the planner placed this room directly rather than A* placing it.
        /// </summary>
        public bool IsFeature { get; set; } = false;

        /// <summary>
        /// Total tile width of the footprint, including internal padding between sub-cells.
        /// </summary>
        public int FootprintWidth =>
            DungeonGrid.CellTileWidth * CellWidth
            + DungeonGrid.HorizontalPadding * (CellWidth - 1);

        /// <summary>
        /// Total tile height of the footprint, including internal padding between sub-cells.
        /// </summary>
        public int FootprintHeight =>
            DungeonGrid.CellTileHeight * CellHeight
            + DungeonGrid.VerticalPadding * (CellHeight - 1);

        /// <summary>
        /// Higher values paint their padding later, winning shared strips against lower-priority neighbors.
        /// </summary>
        public virtual int PaddingPriority => 0;

        /// <summary>
        /// Paints the padding strip on one outward side; called once per side per room.
        /// </summary>
        public virtual void BuildPadding(PaddingContext ctx) { }

        /// <summary>
        /// Per-side cell types that may neighbor this room. The base derives GetAcceptedNeighbors
        /// and the default Exits from this.
        /// </summary>
        protected virtual GridRoom[] AllowedNeighbors(Direction side) => Array.Empty<GridRoom>();

        /// <summary>
        /// The room types accepted on the given sub-cell edge, or null to reject all.
        /// Sub-cell-aware rooms (e.g. stairs) override this directly.
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
        /// Whether the given edge is internal to a multi-cell piece. Always false for 1x1 rooms.
        /// </summary>
        public virtual bool IsInternalEdge(int subCol, int subRow, Direction side)
        {
            return false;
        }

        /// <summary>
        /// Builds this piece at the world-space top-left corner carried by the context.
        /// </summary>
        public abstract void Build(BuildContext ctx);

        /// <summary>
        /// Decoration pass run after Build and padding. Drops neighbor-dependent props.
        /// </summary>
        public virtual void PlaceFurniture(FurnitureContext ctx) { }

        /// <summary>
        /// Overrides which SpawnPool a painted color resolves to inside this cell. Return null to use the dungeon bindings.
        /// </summary>
        public virtual IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool> GetSpawnBindings() => null;

        /// <summary>
        /// Extracts the spawn slots from this cell's aseprite layer.
        /// </summary>
        public virtual void PlaceSpawns(FurnitureContext ctx, List<SpawnSlot> slots) { }

        /// <summary>
        /// Helper for PlaceSpawns overrides: appends a SpawnSlot for each painted pixel in the Spawns layer.
        /// </summary>
        protected void HarvestSpawns(FurnitureContext ctx, List<SpawnSlot> slots, string asepritePath)
        {
            int paintX = ctx.Origin.X - DungeonGrid.HorizontalPadding;
            int paintY = ctx.Origin.Y - DungeonGrid.VerticalPadding;
            Point gridCoord = new Point(ctx.Col, ctx.Row);
            TexGen.HarvestAsepriteLayer(SheetLayer.Spawns, asepritePath, paintX, paintY, (x, y, color) =>
            {
                slots.Add(new SpawnSlot { WorldPos = new Point(x, y), Color = color, GridCoord = gridCoord });
            });
        }

        /// <summary>
        /// Offset from the walker's cursor to this cell's anchor (footprint top-left). Default: zero.
        /// </summary>
        public virtual Point AnchorOffsetFromCursor => Point.Zero;

        /// <summary>
        /// Directional exits the walker can take from this cell. Default: one cardinal exit per side
        /// that has allowed neighbors. Rooms with non-cardinal exits override this.
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
        /// Structural validity check against neighbors outside the walker's forward direction.
        /// Default accepts any placement. pendingLookup exposes uncommitted in-path placements.
        /// </summary>
        public virtual bool IsValidPlacement(DungeonGrid grid, Microsoft.Xna.Framework.Point anchor,
                                              System.Func<int, int, GridRoom> pendingLookup = null) => true;

        /// <summary>
        /// Resolves the cell at a grid position, preferring uncommitted placements via pendingLookup.
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
        /// Whether the given sub-cell side is physically open (a doorway) rather than a wall.
        /// Adjacent cells must agree on the shared side. Default: all closed.
        /// </summary>
        public virtual bool IsOpenSide(int subCol, int subRow, Direction side) => false;

        /// <summary>
        /// Whether this cell may sit next to an empty neighbor. False for connectors,
        /// whose open sides must connect to another cell.
        /// </summary>
        public virtual bool AllowsEmptyNeighbors => true;

        /// <summary>
        /// Whether this room's padding reads correctly facing an empty neighbor. False for connectors;
        /// the side-cap pass walls off the dead-end sides of rooms that return false.
        /// </summary>
        public virtual bool OwnsPadding => false;
    }

    /// <summary>
    /// Context passed to <see cref="GridRoom.Build"/>: world origin, the active dungeon palette, and fill/lining tiles.
    /// </summary>
    public readonly struct BuildContext
    {
        public readonly Point Origin;
        public readonly DungeonPalette Palette;
        public readonly int FillTileType;
        public readonly int LiningTileType;

        public BuildContext(Point origin, DungeonPalette palette, int fillTileType, int liningTileType)
        {
            Origin = origin;
            Palette = palette;
            FillTileType = fillTileType;
            LiningTileType = liningTileType;
        }
    }

    /// <summary>
    /// One outward side of a placed room, passed to <see cref="GridRoom.BuildPadding"/> so it can paint that strip.
    /// </summary>
    public readonly struct PaddingContext
    {
        public readonly Direction Side;
        public readonly int X;
        public readonly int Y;
        public readonly int Width;
        public readonly int Height;
        public readonly int FillTileType;
        public readonly DungeonGrid Grid;
        public readonly int Col;
        public readonly int Row;
        public readonly DungeonPalette Palette;

        public PaddingContext(Direction side, int x, int y, int width, int height, int fillTileType, DungeonGrid grid, int col, int row, DungeonPalette palette)
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
            Palette = palette;
        }
    }

    /// <summary>
    /// Context passed to <see cref="GridRoom.PlaceFurniture"/>: world origin, grid position, and grid for neighbor lookups.
    /// </summary>
    public readonly struct FurnitureContext
    {
        public readonly Point Origin;
        public readonly DungeonGrid Grid;
        public readonly int Col;
        public readonly int Row;
        public readonly int FillTileType;
        public readonly int LiningTileType;
        public readonly DungeonPalette Palette;

        public FurnitureContext(Point origin, DungeonGrid grid, int col, int row, int fillTileType, int liningTileType, DungeonPalette palette)
        {
            Origin = origin;
            Grid = grid;
            Col = col;
            Row = row;
            FillTileType = fillTileType;
            LiningTileType = liningTileType;
            Palette = palette;
        }
    }

    /// <summary>
    /// A single exit from a cell: cursor delta plus the cell types legal through it.
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
