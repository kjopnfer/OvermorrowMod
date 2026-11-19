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

    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction side) => side switch
        {
            Direction.Top => Direction.Bottom,
            Direction.Bottom => Direction.Top,
            Direction.Left => Direction.Right,
            _ => Direction.Left,
        };

        /// <summary>
        /// The grid offset of the cell across this side.
        /// </summary>
        public static Point Step(this Direction side) => side switch
        {
            Direction.Top => new Point(0, -1),
            Direction.Bottom => new Point(0, 1),
            Direction.Left => new Point(-1, 0),
            _ => new Point(1, 0),
        };
    }

    public enum RoomType
    {
        None,
        Filler,
        Combat,
        Chest,
        Door,
        VerticalConnector,
        HorizontalConnector,
        DiagonalConnector,
        Start,
        Bridge,
        TreasureDoor
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
        /// Whether the planner sends a reward dead end and a loop out of this room's unused openings.
        /// The room gets at most one reward branch.
        /// </summary>
        public virtual bool IsJunction => false;

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

        public virtual bool IsSpawnRoom => false;

        /// <summary>
        /// Paints the padding strip on one outward side; called once per side per room.
        /// </summary>
        public virtual void BuildPadding(PaddingContext ctx) { }

        /// <summary>
        /// Per-side cell types that may neighbor this room. The base derives GetAcceptedNeighbors
        /// and the default Exits from this.
        /// </summary>
        protected virtual GridRoom[] AllowedNeighbors(Direction side) => Array.Empty<GridRoom>();

        private HashSet<Type>[] _acceptedBySide;
        private CellExit[] _exits;
        private RoomOpening[] _openings;

        private static readonly Direction[] OpeningSideOrder = { Direction.Right, Direction.Left, Direction.Bottom, Direction.Top };

        /// <summary>
        /// The room types accepted on the given sub-cell edge, or null to reject all.
        /// Sub-cell-aware rooms (e.g. stairs) override this directly. Callers must not modify the returned set.
        /// </summary>
        public virtual HashSet<Type> GetAcceptedNeighbors(int subCol, int subRow, Direction side)
        {
            if (IsInternalEdge(subCol, subRow, side)) return null;
            _acceptedBySide ??= BuildAcceptedBySide();
            return _acceptedBySide[(int)side];
        }

        private HashSet<Type>[] BuildAcceptedBySide()
        {
            var sets = new HashSet<Type>[4];
            foreach (Direction side in Enum.GetValues(typeof(Direction)))
            {
                var neighbors = AllowedNeighbors(side);
                if (neighbors == null || neighbors.Length == 0) continue;
                var set = new HashSet<Type>();
                foreach (var n in neighbors) set.Add(n.GetType());
                sets[(int)side] = set;
            }
            return sets;
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
        /// Only affects rooms the pathfinder places mid-path: it decides which sub-cell the walker steps into.
        /// Rooms placed directly are connected through their <see cref="Openings"/> whatever this value is.
        /// </summary>
        public virtual Point AnchorOffsetFromCursor => Point.Zero;

        /// <summary>
        /// Every open sub-cell edge on the outside of the footprint, ordered right, left, bottom, top,
        /// then by sub-cell along each side.
        /// </summary>
        public RoomOpening[] Openings => _openings ??= BuildOpenings();

        private RoomOpening[] BuildOpenings()
        {
            var list = new List<RoomOpening>();
            foreach (var side in OpeningSideOrder)
            {
                bool vertical = side is Direction.Left or Direction.Right;
                int count = vertical ? CellHeight : CellWidth;
                for (int i = 0; i < count; i++)
                {
                    int subCol = side == Direction.Right ? CellWidth - 1 : side == Direction.Left ? 0 : i;
                    int subRow = side == Direction.Bottom ? CellHeight - 1 : side == Direction.Top ? 0 : i;
                    if (IsOpenSide(subCol, subRow, side)) list.Add(new RoomOpening(subCol, subRow, side));
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Directional exits the walker can take from this cell, built once per instance by <see cref="CreateExits"/>.
        /// </summary>
        public CellExit[] Exits => _exits ??= CreateExits();

        /// <summary>
        /// Default: one exit per opening whose side has allowed neighbors, moving the cursor to the cell just outside it.
        /// </summary>
        protected virtual CellExit[] CreateExits()
        {
            var list = new List<CellExit>(Openings.Length);
            foreach (var opening in Openings)
            {
                var neighbors = AllowedNeighbors(opening.Side);
                if (neighbors == null || neighbors.Length == 0) continue;
                var outside = opening.OutsideCell;
                list.Add(new CellExit(new Point(outside.X + AnchorOffsetFromCursor.X, outside.Y + AnchorOffsetFromCursor.Y), opening.Side, neighbors));
            }
            return list.ToArray();
        }

        /// <summary>
        /// Structural validity check against neighbors outside the walker's forward direction.
        /// Default accepts any placement.
        /// </summary>
        /// <param name="pending">The searching path's uncommitted placements, or null outside a search.</param>
        public virtual bool IsValidPlacement(DungeonGrid grid, Point anchor, PendingPlacements pending = null) => true;

        /// <summary>
        /// Resolves the cell at a grid position, preferring the searching path's uncommitted placements.
        /// </summary>
        protected static GridRoom GetEffectiveRoomAt(DungeonGrid grid, PendingPlacements pending, int x, int y)
        {
            var pendingRoom = pending?.CellAt(x, y);
            if (pendingRoom != null) return pendingRoom;
            var slot = grid.GetSlot(x, y);
            if (slot == null || slot.IsEmpty) return null;
            return slot.Room;
        }

        /// <summary>
        /// True if any cell of a committed or pending room of <paramref name="roomType"/> (or a subclass) lies within
        /// Chebyshev distance below <paramref name="distance"/> of <paramref name="point"/>.
        /// </summary>
        protected static bool AnyCellWithin(DungeonGrid grid, PendingPlacements pending, Type roomType, Point point, int distance)
        {
            foreach (var anchor in grid.AnchorsOf(roomType))
                if (FootprintWithin(grid.GetSlot(anchor.X, anchor.Y).Room, anchor, point, distance)) return true;
            if (pending == null) return false;
            foreach (var step in pending.Steps)
                if (roomType.IsInstanceOfType(step.Cell) && FootprintWithin(step.Cell, step.Anchor, point, distance)) return true;
            return false;
        }

        private static bool FootprintWithin(GridRoom room, Point anchor, Point point, int distance)
        {
            int right = anchor.X + room.CellWidth - 1;
            int bottom = anchor.Y + room.CellHeight - 1;
            int dx = point.X < anchor.X ? anchor.X - point.X : point.X > right ? point.X - right : 0;
            int dy = point.Y < anchor.Y ? anchor.Y - point.Y : point.Y > bottom ? point.Y - bottom : 0;
            return Math.Max(dx, dy) < distance;
        }

        /// <summary>
        /// Whether the given sub-cell side is physically open (a doorway) rather than a wall.
        /// Adjacent cells must agree on the shared side. Default: all closed.
        /// </summary>
        public virtual bool IsOpenSide(int subCol, int subRow, Direction side) => false;

        /// <summary>
        /// Whether the given sub-cell side is a doorway when <paramref name="neighbor"/> sits against it.
        /// Default: <see cref="IsOpenSide"/>. Rooms whose padding closes a side for some neighbors override this.
        /// </summary>
        /// <param name="neighbor">The room across the side, or null for a room of no particular type.</param>
        public virtual bool OpensToward(int subCol, int subRow, Direction side, GridRoom neighbor) => IsOpenSide(subCol, subRow, side);

        /// <summary>
        /// True if the two cells share a doorway: each side of the border is open toward the other room.
        /// </summary>
        public static bool Connects(GridSlot from, Direction side, GridSlot to) => from.Room.OpensToward(from.SubCol, from.SubRow, side, to.Room) && to.Room.OpensToward(to.SubCol, to.SubRow, side.Opposite(), from.Room);

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
    /// Context passed to <see cref="GridRoom.Build"/>: world origin, the active dungeon palette, fill/lining tiles, and grid position.
    /// </summary>
    public readonly struct BuildContext
    {
        public readonly Point Origin;
        public readonly DungeonPalette Palette;
        public readonly int FillTileType;
        public readonly int LiningTileType;
        public readonly DungeonGrid Grid;
        public readonly int Col;
        public readonly int Row;

        public BuildContext(Point origin, DungeonPalette palette, int fillTileType, int liningTileType, DungeonGrid grid, int col, int row)
        {
            Origin = origin;
            Palette = palette;
            FillTileType = fillTileType;
            LiningTileType = liningTileType;
            Grid = grid;
            Col = col;
            Row = row;
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
    /// A single exit from a cell: cursor delta, the side of the cell it leaves through, and the cell types legal through it.
    /// </summary>
    public readonly struct CellExit
    {
        public Point CursorDelta { get; }
        public Direction Side { get; }
        public GridRoom[] AllowedNext { get; }

        public CellExit(Point cursorDelta, Direction side, GridRoom[] allowedNext)
        {
            CursorDelta = cursorDelta;
            Side = side;
            AllowedNext = allowedNext;
        }
    }

    /// <summary>
    /// An open edge on the outside of a room's footprint.
    /// </summary>
    public readonly struct RoomOpening
    {
        public int SubCol { get; }
        public int SubRow { get; }
        public Direction Side { get; }

        public RoomOpening(int subCol, int subRow, Direction side)
        {
            SubCol = subCol;
            SubRow = subRow;
            Side = side;
        }

        /// <summary>
        /// The cell just outside this opening, relative to the room's anchor.
        /// </summary>
        public Point OutsideCell => new Point(SubCol + Side.Step().X, SubRow + Side.Step().Y);
    }
}
