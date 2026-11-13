using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    public class GridSlot
    {
        public GridRoom Room;
        public int SubCol;
        public int SubRow;
        public int GroupId;

        public bool IsEmpty => Room == null;
    }

    /// <summary>
    /// A 2D grid of cells with fixed spacing. Each slot can hold a GridRoom reference.
    /// The grid handles world-space coordinate conversion, placement validation via
    /// mutual edge compatibility, and tracks which slots are occupied.
    /// Write slots only through this class's methods; they keep the placement indexes in sync.
    /// </summary>
    public class DungeonGrid
    {
        public const int CellTileWidth = 18;
        public const int CellTileHeight = 26;
        public const int HorizontalPadding = 8;
        public const int VerticalPadding = 4;
        public const int HorizontalSpacing = CellTileWidth + HorizontalPadding;
        public const int VerticalSpacing = CellTileHeight + VerticalPadding;

        public int Cols { get; }
        public int Rows { get; }

        /// <summary>
        /// World-tile position the grid maps from. Set during planning to a local
        /// placeholder and rebased to the dungeon's final position at render time.
        /// </summary>
        public Point Origin { get; set; }

        private readonly GridSlot[,] _slots;
        private int _nextGroupId = 1;
        private readonly Dictionary<Type, List<Point>> _anchorsByType = new();
        private readonly int[] _placementsByRole = new int[Enum.GetValues(typeof(RoomType)).Length];
        private readonly int[] _shaftsPerColumn;
        private readonly List<Point> _doorAnchors = new();

        public DungeonGrid(int cols, int rows, Point origin)
        {
            Cols = cols;
            Rows = rows;
            Origin = origin;
            _slots = new GridSlot[cols, rows];
            _shaftsPerColumn = new int[cols];

            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows; r++)
                    _slots[c, r] = new GridSlot();
        }

        public int NextGroupId() => _nextGroupId++;

        /// <summary>
        /// Anchor cells of every placed room whose <see cref="GridRoom.Type"/> is <see cref="RoomType.Door"/>.
        /// </summary>
        public IReadOnlyList<Point> DoorAnchors => _doorAnchors;

        /// <summary>
        /// Number of rooms of <paramref name="roomType"/> or a subclass placed on the grid.
        /// </summary>
        public int CountPlacements(Type roomType)
        {
            int count = 0;
            foreach (var kv in _anchorsByType)
                if (roomType.IsAssignableFrom(kv.Key)) count += kv.Value.Count;
            return count;
        }

        /// <summary>
        /// Anchor cells of every placed room of <paramref name="roomType"/> or a subclass.
        /// </summary>
        public IEnumerable<Point> AnchorsOf(Type roomType)
        {
            foreach (var kv in _anchorsByType)
            {
                if (!roomType.IsAssignableFrom(kv.Key)) continue;
                foreach (var anchor in kv.Value)
                    yield return anchor;
            }
        }

        public int CountPlacements(RoomType role) => _placementsByRole[(int)role];

        public bool ColumnHasShaft(int col) => col >= 0 && col < Cols && _shaftsPerColumn[col] > 0;

        public GridSlot GetSlot(int col, int row)
        {
            if (col < 0 || col >= Cols || row < 0 || row >= Rows)
                return null;
            return _slots[col, row];
        }

        /// <summary>
        /// Converts a grid cell position to a world-space tile position (top-left of the cell).
        /// </summary>
        public Point GridToWorld(int col, int row)
        {
            return new Point(
                Origin.X + col * HorizontalSpacing,
                Origin.Y + row * VerticalSpacing
            );
        }

        /// <summary>
        /// Checks whether a GridRoom can be placed at the given grid position.
        /// Verifies all required slots are empty and edge compatibility with existing neighbors.
        /// </summary>
        public bool CanPlace(GridRoom room, int col, int row)
        {
            for (int sc = 0; sc < room.CellWidth; sc++)
            {
                for (int sr = 0; sr < room.CellHeight; sr++)
                {
                    int gc = col + sc;
                    int gr = row + sr;

                    if (gc < 0 || gc >= Cols || gr < 0 || gr >= Rows)
                        return false;

                    if (!_slots[gc, gr].IsEmpty)
                        return false;

                    if (!CheckEdgeCompatibility(room, sc, sr, gc, gr))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Places a GridRoom at the given grid position, filling all sub-cell slots and replacing whatever they held.
        /// </summary>
        public void Place(GridRoom room, int col, int row, int groupId)
        {
            for (int sc = 0; sc < room.CellWidth; sc++)
            {
                for (int sr = 0; sr < room.CellHeight; sr++)
                {
                    var slot = _slots[col + sc, row + sr];
                    if (!slot.IsEmpty) UpdateIndexes(slot, col + sc, row + sr, -1);
                    slot.Room = room;
                    slot.SubCol = sc;
                    slot.SubRow = sr;
                    slot.GroupId = groupId;
                    UpdateIndexes(slot, col + sc, row + sr, 1);
                }
            }
        }

        public void ClearSlot(int col, int row)
        {
            var slot = _slots[col, row];
            if (!slot.IsEmpty) UpdateIndexes(slot, col, row, -1);
            slot.Room = null;
            slot.SubCol = 0;
            slot.SubRow = 0;
            slot.GroupId = 0;
        }

        /// <summary>
        /// Clears every in-bounds slot of <paramref name="room"/>'s footprint at the given anchor.
        /// </summary>
        public void Clear(GridRoom room, int col, int row)
        {
            for (int sc = 0; sc < room.CellWidth; sc++)
            {
                for (int sr = 0; sr < room.CellHeight; sr++)
                {
                    int c = col + sc;
                    int r = row + sr;
                    if (c < 0 || c >= Cols || r < 0 || r >= Rows) continue;
                    ClearSlot(c, r);
                }
            }
        }

        public void ClearAll()
        {
            for (int c = 0; c < Cols; c++)
                for (int r = 0; r < Rows; r++)
                    ClearSlot(c, r);
        }

        public (GridRoom room, int subCol, int subRow, int groupId)[,] CaptureSlots()
        {
            var snapshot = new (GridRoom, int, int, int)[Cols, Rows];
            for (int c = 0; c < Cols; c++)
            {
                for (int r = 0; r < Rows; r++)
                {
                    var s = _slots[c, r];
                    snapshot[c, r] = (s.Room, s.SubCol, s.SubRow, s.GroupId);
                }
            }
            return snapshot;
        }

        public void RestoreSlots((GridRoom room, int subCol, int subRow, int groupId)[,] snapshot)
        {
            ClearAll();
            for (int c = 0; c < Cols; c++)
            {
                for (int r = 0; r < Rows; r++)
                {
                    var (room, subCol, subRow, groupId) = snapshot[c, r];
                    if (room == null) continue;
                    var slot = _slots[c, r];
                    slot.Room = room;
                    slot.SubCol = subCol;
                    slot.SubRow = subRow;
                    slot.GroupId = groupId;
                    UpdateIndexes(slot, c, r, 1);
                }
            }
        }

        /// <summary>
        /// Shafts are counted per slot; every other index counts a room once, at its anchor slot.
        /// </summary>
        private void UpdateIndexes(GridSlot slot, int col, int row, int delta)
        {
            var room = slot.Room;
            if (room.Type == RoomType.VerticalConnector) _shaftsPerColumn[col] += delta;
            if (slot.SubCol != 0 || slot.SubRow != 0) return;

            var anchor = new Point(col, row);
            var type = room.GetType();
            if (!_anchorsByType.TryGetValue(type, out var anchors))
                _anchorsByType[type] = anchors = new List<Point>();
            if (delta > 0) anchors.Add(anchor);
            else anchors.Remove(anchor);

            _placementsByRole[(int)room.Type] += delta;
            if (room.Type != RoomType.Door) return;
            if (delta > 0) _doorAnchors.Add(anchor);
            else _doorAnchors.Remove(anchor);
        }

        private bool CheckEdgeCompatibility(GridRoom room, int subCol, int subRow, int gridCol, int gridRow)
        {
            // Check all four directions against existing neighbors
            if (!CheckDirection(room, subCol, subRow, gridCol, gridRow, Direction.Left, -1, 0))
                return false;
            if (!CheckDirection(room, subCol, subRow, gridCol, gridRow, Direction.Right, 1, 0))
                return false;
            if (!CheckDirection(room, subCol, subRow, gridCol, gridRow, Direction.Top, 0, -1))
                return false;
            if (!CheckDirection(room, subCol, subRow, gridCol, gridRow, Direction.Bottom, 0, 1))
                return false;

            return true;
        }

        private bool CheckDirection(GridRoom room, int subCol, int subRow, int gridCol, int gridRow,
            Direction side, int dc, int dr)
        {
            int neighborCol = gridCol + dc;
            int neighborRow = gridRow + dr;
            var neighborSlot = GetSlot(neighborCol, neighborRow);

            // No neighbor (out of bounds or empty) is always valid
            if (neighborSlot == null || neighborSlot.IsEmpty)
                return true;

            var neighborRoom = neighborSlot.Room;
            Direction opposite = GetOpposite(side);

            bool thisInternal = room.IsInternalEdge(subCol, subRow, side);
            bool neighborInternal = neighborRoom.IsInternalEdge(neighborSlot.SubCol, neighborSlot.SubRow, opposite);

            // Internal edges only match other internal edges of the same piece
            if (thisInternal || neighborInternal)
                return false;

            // Mutual compatibility: both sides must accept each other
            var thisAccepts = room.GetAcceptedNeighbors(subCol, subRow, side);
            var neighborAccepts = neighborRoom.GetAcceptedNeighbors(neighborSlot.SubCol, neighborSlot.SubRow, opposite);

            bool thisAcceptsNeighbor = thisAccepts != null && thisAccepts.Contains(neighborRoom.GetType());
            bool neighborAcceptsThis = neighborAccepts != null && neighborAccepts.Contains(room.GetType());

            return thisAcceptsNeighbor && neighborAcceptsThis;
        }

        private static Direction GetOpposite(Direction dir)
        {
            return dir switch
            {
                Direction.Top => Direction.Bottom,
                Direction.Bottom => Direction.Top,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => dir
            };
        }
    }
}
