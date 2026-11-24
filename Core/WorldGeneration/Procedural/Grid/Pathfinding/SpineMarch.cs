using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding
{
    /// <summary>
    /// The axis a spine marches along from its start door to its end door, and where each required room may sit.
    /// </summary>
    public abstract class SpineMarch
    {
        protected const int CandidatesPerRoom = 10;

        /// <summary>
        /// A cell's position along the march.
        /// </summary>
        public abstract int Along(Point cell);

        /// <summary>
        /// Cells a room covers along the march.
        /// </summary>
        public abstract int Extent(GridRoom room);

        /// <summary>
        /// True if some anchor at <paramref name="target"/> along the march passes <paramref name="allowAnchor"/>.
        /// </summary>
        public abstract bool TargetAllowed(DungeonGrid grid, GridRoom prototype, int target, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor);

        /// <summary>
        /// Open anchors for the required room at position <paramref name="index"/> in the march order, best first.
        /// </summary>
        /// <param name="target">
        /// The room's target position along the march.
        /// </param>
        /// <param name="last">
        /// The room is the last before the end door.
        /// </param>
        public abstract IEnumerable<Point> Candidates(DungeonGrid grid, GridRoom prototype, int index, int target, Point prevAnchor, GridRoom prevRoom, Point endDoor, bool last, HashSet<Point> blocked, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor);

        protected static bool FootprintIsOpen(DungeonGrid grid, GridRoom prototype, Point anchor, HashSet<Point> blocked)
        {
            for (int sc = 0; sc < prototype.CellWidth; sc++)
            {
                for (int sr = 0; sr < prototype.CellHeight; sr++)
                {
                    var cell = new Point(anchor.X + sc, anchor.Y + sr);
                    if (blocked.Contains(cell)) return false;
                    var slot = grid.GetSlot(cell.X, cell.Y);
                    if (slot == null || !slot.IsEmpty) return false;
                }
            }
            return true;
        }

        protected static IEnumerable<Point> Best(List<(Point Pos, int Rank)> ranked, GridRoom prototype, DungeonGrid grid) => ranked.OrderBy(c => c.Rank).Select(c => c.Pos).Where(pos => prototype.IsValidPlacement(grid, pos)).Take(CandidatesPerRoom);
    }

    /// <summary>
    /// Marches west to east, each required room near an elevation curve of rows by column.
    /// </summary>
    public sealed class CurveMarch : SpineMarch
    {
        private const int MaxColumnShift = 8;
        private const int MinLegColumns = 1;

        private readonly double[] _elevation;

        public CurveMarch(double[] elevation)
        {
            _elevation = elevation;
        }

        public override int Along(Point cell) => cell.X;

        public override int Extent(GridRoom room) => room.CellWidth;

        public override bool TargetAllowed(DungeonGrid grid, GridRoom prototype, int target, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor)
        {
            if (allowAnchor == null) return true;
            for (int row = edgeBorder; row < grid.Rows - edgeBorder; row++)
                if (allowAnchor(prototype, new Point(target, row))) return true;
            return false;
        }

        /// <summary>
        /// Anchors near the target column and the elevation curve, between the previous room's right column and the end
        /// door, nearest first. A room never sits more rows from the previous room, or from the end door when it is the
        /// last room, than there are columns between them to climb in.
        /// </summary>
        public override IEnumerable<Point> Candidates(DungeonGrid grid, GridRoom prototype, int index, int target, Point prevAnchor, GridRoom prevRoom, Point endDoor, bool last, HashSet<Point> blocked, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor)
        {
            int w = prototype.CellWidth;
            int h = prototype.CellHeight;
            int band = h + 1;
            int prevRight = prevAnchor.X + prevRoom.CellWidth - 1;
            var ranked = new List<(Point Pos, int Rank)>();
            for (int shift = -MaxColumnShift; shift <= MaxColumnShift; shift++)
            {
                int col = target + shift;
                int columnsBefore = col - prevRight - 1;
                int columnsAfter = endDoor.X - (col + w);
                if (columnsBefore < MinLegColumns || columnsAfter < MinLegColumns) continue;
                if (grid.NearVoid(col, w)) continue;

                int centerRow = (int)Math.Round(_elevation[col]) + prototype.AnchorOffsetFromCursor.Y;
                for (int dy = -band; dy <= band; dy++)
                {
                    int row = centerRow + dy;
                    if (row < edgeBorder || row + h - 1 >= grid.Rows - edgeBorder) continue;
                    if (Math.Abs(row - prevAnchor.Y) > columnsBefore) continue;
                    if (last && Math.Abs(row - endDoor.Y) > columnsAfter) continue;
                    var pos = new Point(col, row);
                    if (allowAnchor != null && !allowAnchor(prototype, pos)) continue;
                    if (!FootprintIsOpen(grid, prototype, pos, blocked)) continue;
                    ranked.Add((pos, 2 * Math.Abs(shift) + Math.Abs(dy)));
                }
            }
            return Best(ranked, prototype, grid);
        }
    }
}
