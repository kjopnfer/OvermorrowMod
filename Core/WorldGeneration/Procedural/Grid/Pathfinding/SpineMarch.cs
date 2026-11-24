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

    /// <summary>
    /// Marches top to bottom along a zigzag line: sideways runs across a band of columns, alternating direction, that
    /// drop steadily from the start door at the top of the first run to the end door at the end of the last.
    /// </summary>
    public sealed class SwitchbackMarch : SpineMarch
    {
        private const int MaxColumnShift = 6;
        private const int MaxRowShift = 1;

        private readonly int _bandLeft;
        private readonly int _bandRight;
        private readonly int _top;
        private readonly int _depth;
        private readonly int _runs;
        private readonly bool _startLeft;

        public SwitchbackMarch(int bandLeft, int bandRight, int top, int depth, int runs, bool startLeft)
        {
            _bandLeft = bandLeft;
            _bandRight = bandRight;
            _top = top;
            _depth = depth;
            _runs = runs;
            _startLeft = startLeft;
        }

        private int RunLength => _bandRight - _bandLeft;

        private int Length => _runs * RunLength;

        private bool RunsRight(int run) => _startLeft == (run % 2 == 0);

        public Point Start => PointAt(0);

        public Point End => PointAt(Length);

        /// <summary>
        /// The cell on the line at <paramref name="along"/> cells from the start door.
        /// </summary>
        private Point PointAt(int along)
        {
            int run = Math.Min(_runs - 1, along / RunLength);
            int offset = along - run * RunLength;
            int col = RunsRight(run) ? _bandLeft + offset : _bandRight - offset;
            return new Point(col, _top + (int)Math.Round((double)along * _depth / Length));
        }

        public override int Along(Point cell)
        {
            int run = Math.Clamp((cell.Y - _top) * _runs / _depth, 0, _runs - 1);
            int offset = RunsRight(run) ? cell.X - _bandLeft : _bandRight - cell.X;
            return run * RunLength + Math.Clamp(offset, 0, RunLength);
        }

        public override int Extent(GridRoom room) => room.CellWidth;

        public override bool TargetAllowed(DungeonGrid grid, GridRoom prototype, int target, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor)
        {
            if (allowAnchor == null) return true;
            int row = PointAt(target).Y;
            for (int col = edgeBorder; col < grid.Cols - edgeBorder; col++)
                if (allowAnchor(prototype, new Point(col, row))) return true;
            return false;
        }

        /// <summary>
        /// Anchors near the line's cell at <paramref name="target"/>, no higher than the previous room's bottom row and
        /// above the end door, nearest first.
        /// </summary>
        public override IEnumerable<Point> Candidates(DungeonGrid grid, GridRoom prototype, int index, int target, Point prevAnchor, GridRoom prevRoom, Point endDoor, bool last, HashSet<Point> blocked, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor)
        {
            int w = prototype.CellWidth;
            int h = prototype.CellHeight;
            var point = PointAt(target);
            int run = Math.Min(_runs - 1, target / RunLength);
            int targetCol = RunsRight(run) ? point.X : point.X - w + 1;
            int targetRow = point.Y + prototype.AnchorOffsetFromCursor.Y;
            int prevBottom = prevAnchor.Y + prevRoom.CellHeight - 1;
            var ranked = new List<(Point Pos, int Rank)>();
            for (int shift = -MaxColumnShift; shift <= MaxColumnShift; shift++)
            {
                int col = targetCol + shift;
                if (col < _bandLeft - 1 || col + w - 1 > _bandRight + 1) continue;
                if (grid.NearVoid(col, w)) continue;
                for (int dy = -MaxRowShift; dy <= MaxRowShift; dy++)
                {
                    int row = targetRow + dy;
                    if (row < prevBottom || row + h - 1 > endDoor.Y) continue;
                    if (row < edgeBorder || row + h - 1 >= grid.Rows - edgeBorder) continue;
                    var pos = new Point(col, row);
                    if (allowAnchor != null && !allowAnchor(prototype, pos)) continue;
                    if (!FootprintIsOpen(grid, prototype, pos, blocked)) continue;
                    ranked.Add((pos, Math.Abs(shift) + 2 * Math.Abs(dy)));
                }
            }
            return Best(ranked, prototype, grid);
        }
    }
}
