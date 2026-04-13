using Microsoft.Xna.Framework;

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
        public Point Origin { get; }

        private readonly GridSlot[,] _slots;
        private int _nextGroupId = 1;

        public DungeonGrid(int cols, int rows, Point origin)
        {
            Cols = cols;
            Rows = rows;
            Origin = origin;
            _slots = new GridSlot[cols, rows];

            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows; r++)
                    _slots[c, r] = new GridSlot();
        }

        public int NextGroupId() => _nextGroupId++;

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
        /// Places a GridRoom at the given grid position, filling all sub-cell slots.
        /// </summary>
        public void Place(GridRoom room, int col, int row, int groupId)
        {
            for (int sc = 0; sc < room.CellWidth; sc++)
            {
                for (int sr = 0; sr < room.CellHeight; sr++)
                {
                    var slot = _slots[col + sc, row + sr];
                    slot.Room = room;
                    slot.SubCol = sc;
                    slot.SubRow = sr;
                    slot.GroupId = groupId;
                }
            }
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
