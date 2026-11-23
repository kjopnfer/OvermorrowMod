using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding
{
    /// <summary>
    /// Plans the spine by marching from the start door to the end door: the required rooms get evenly spaced columns
    /// between the doors in a random order, each is placed near the elevation curve at its column, and an A* leg links
    /// it to the previous room.
    /// </summary>
    public static class SpinePlanner
    {
        public struct PlanResult
        {
            /// <summary>
            /// The legs' steps in spine order, already committed to the grid. Required rooms are not included.
            /// </summary>
            public List<PathStep> Steps;

            /// <summary>
            /// Anchors of the required rooms, already committed to the grid.
            /// </summary>
            public List<Point> RequiredAnchors;

            public bool Success;
        }

        private const int CandidatesPerRoom = 10;
        private const int MaxColumnShift = 8;
        private const int MinLegColumns = 1;
        private const int OrderShuffles = 8;
        private const int OrderTries = 3;
        private const int LegMinExpansions = 400;
        private const int LegExpansionsPerCell = 60;

        /// <summary>
        /// Lays out the spine. Doors must already be on the grid at <paramref name="startDoorPos"/> and
        /// <paramref name="endDoorPos"/>, with the start door west of the end door. On failure every placement made by
        /// this call is rolled back, leaving only the doors.
        /// </summary>
        /// <param name="allowAnchor">
        /// Optional filter on a required room's candidate anchors, given the room's prototype.
        /// </param>
        /// <param name="forbidden">
        /// Moves between rooms no leg may make.
        /// </param>
        /// <param name="skipUnlinkable">
        /// Leave out a required room with no spot a leg reaches instead of failing.
        /// </param>
        public static PlanResult TryPlanSpine(DungeonGrid grid, GridAStar search, Point startDoorPos, Point endDoorPos, GridRoom startDoorRoom, IReadOnlyList<Func<GridRoom>> requiredRoomFactories, double[] elevation, int gridRows, EdgeCost costFn, HashSet<Point> blocked, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor, MoveMask forbidden, bool skipUnlinkable, Random rand, Action<string> log)
        {
            var rooms = requiredRoomFactories.Select(factory => (Factory: factory, Prototype: Feature(factory))).ToList();
            for (int order = 0; order < OrderTries; order++)
            {
                List<int> targets = null;
                for (int i = 0; i < OrderShuffles; i++)
                {
                    Shuffle(rooms, rand);
                    targets = TargetColumns(rooms.Select(r => r.Prototype.CellWidth).ToList(), startDoorPos.X, endDoorPos.X);
                    if (Enumerable.Range(0, rooms.Count).All(r => ColumnAllowed(rooms[r].Prototype, targets[r], gridRows, edgeBorder, allowAnchor))) break;
                }

                var result = TryMarch(grid, search, startDoorPos, endDoorPos, startDoorRoom, rooms, targets, elevation, gridRows, costFn, blocked, edgeBorder, allowAnchor, forbidden, skipUnlinkable && order == OrderTries - 1, log);
                if (result.HasValue) return result.Value;
            }
            return new PlanResult { Success = false };
        }

        /// <summary>
        /// Places <paramref name="rooms"/> in order at their <paramref name="targets"/> columns, linking each to the one
        /// before, then links the last to the end door.
        /// </summary>
        /// <returns>
        /// The spine, or null with the grid left as it was.
        /// </returns>
        private static PlanResult? TryMarch(DungeonGrid grid, GridAStar search, Point startDoorPos, Point endDoorPos, GridRoom startDoorRoom, List<(Func<GridRoom> Factory, GridRoom Prototype)> rooms, List<int> targets, double[] elevation, int gridRows, EdgeCost costFn, HashSet<Point> blocked, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor, MoveMask forbidden, bool skipUnlinkable, Action<string> log)
        {
            var steps = new List<PathStep>();
            var anchors = new List<Point>();
            var placedRooms = new List<(GridRoom Room, Point Anchor)>();
            Point prevAnchor = startDoorPos;
            GridRoom prevRoom = startDoorRoom;
            int prevRight = startDoorPos.X;

            for (int r = 0; r < rooms.Count; r++)
            {
                var (factory, prototype) = rooms[r];
                bool linked = false;
                foreach (var pos in Candidates(grid, prototype, targets[r], new Point(prevRight, prevAnchor.Y), r == rooms.Count - 1 ? endDoorPos : null, endDoorPos.X, elevation, gridRows, blocked, edgeBorder, allowAnchor))
                {
                    var room = Feature(factory);
                    grid.Place(room, pos.X, pos.Y, grid.NextGroupId());
                    var leg = search.FindPath(prevAnchor, pos, prevRoom, costFn, LegBudget(prevAnchor, pos), forbidden);
                    if (leg == null)
                    {
                        grid.Clear(room, pos.X, pos.Y);
                        continue;
                    }

                    Commit(grid, leg, steps);
                    placedRooms.Add((room, pos));
                    anchors.Add(pos);
                    prevAnchor = pos;
                    prevRoom = room;
                    prevRight = pos.X + room.CellWidth - 1;
                    linked = true;
                    break;
                }

                if (!linked)
                {
                    log?.Invoke($"SpinePlanner: no linked spot for {prototype.GetType().Name} near column {targets[r]}.");
                    if (skipUnlinkable) continue;
                    Rollback(grid, steps, placedRooms);
                    return null;
                }
            }

            var lastLeg = search.FindPath(prevAnchor, endDoorPos, prevRoom, costFn, LegBudget(prevAnchor, endDoorPos), forbidden);
            if (lastLeg == null)
            {
                log?.Invoke("SpinePlanner: no leg to the end door.");
                Rollback(grid, steps, placedRooms);
                return null;
            }
            Commit(grid, lastLeg, steps);

            return new PlanResult { Steps = steps, RequiredAnchors = anchors, Success = true };
        
        }

        private static int LegBudget(Point from, Point to) => Math.Max(LegMinExpansions, LegExpansionsPerCell * (Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y)));

        private static GridRoom Feature(Func<GridRoom> factory)
        {
            var room = factory();
            room.IsFeature = true;
            return room;
        }

        /// <summary>
        /// Anchor columns that spread rooms of the given widths evenly between the two door columns, in order.
        /// </summary>
        private static List<int> TargetColumns(List<int> widths, int startCol, int endCol)
        {
            double gap = (double)(endCol - startCol - 1 - widths.Sum()) / (widths.Count + 1);
            var targets = new List<int>(widths.Count);
            int widthBefore = 0;
            for (int i = 0; i < widths.Count; i++)
            {
                targets.Add(startCol + 1 + (int)Math.Round(gap * (i + 1)) + widthBefore);
                widthBefore += widths[i];
            }
            return targets;
        }

        private static bool ColumnAllowed(GridRoom prototype, int col, int gridRows, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor)
        {
            if (allowAnchor == null) return true;
            for (int row = edgeBorder; row < gridRows - edgeBorder; row++)
                if (allowAnchor(prototype, new Point(col, row))) return true;
            return false;
        }

        /// <summary>
        /// Open anchors for <paramref name="prototype"/> near <paramref name="targetCol"/> and the elevation curve,
        /// between <paramref name="prevEnd"/> (the previous room's right column and row) and <paramref name="endCol"/>,
        /// nearest first. A room never sits more rows from the previous room, or from <paramref name="nextDoor"/>, than
        /// there are columns between them to climb in.
        /// </summary>
        private static IEnumerable<Point> Candidates(DungeonGrid grid, GridRoom prototype, int targetCol, Point prevEnd, Point? nextDoor, int endCol, double[] elevation, int gridRows, HashSet<Point> blocked, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor)
        {
            int w = prototype.CellWidth;
            int h = prototype.CellHeight;
            int band = h + 1;
            var ranked = new List<(Point Pos, int Rank)>();
            for (int shift = -MaxColumnShift; shift <= MaxColumnShift; shift++)
            {
                int col = targetCol + shift;
                int columnsBefore = col - prevEnd.X - 1;
                int columnsAfter = endCol - (col + w);
                if (columnsBefore < MinLegColumns || columnsAfter < MinLegColumns) continue;
                if (grid.NearVoid(col, w)) continue;

                int centerRow = (int)Math.Round(elevation[col]) + prototype.AnchorOffsetFromCursor.Y;
                for (int dy = -band; dy <= band; dy++)
                {
                    int row = centerRow + dy;
                    if (row < edgeBorder || row + h - 1 >= gridRows - edgeBorder) continue;
                    if (Math.Abs(row - prevEnd.Y) > columnsBefore) continue;
                    if (nextDoor.HasValue && Math.Abs(row - nextDoor.Value.Y) > columnsAfter) continue;
                    var pos = new Point(col, row);
                    if (allowAnchor != null && !allowAnchor(prototype, pos)) continue;
                    if (!FootprintIsOpen(grid, prototype, pos, blocked)) continue;
                    ranked.Add((pos, 2 * Math.Abs(shift) + Math.Abs(dy)));
                }
            }

            return ranked.OrderBy(c => c.Rank).Select(c => c.Pos).Where(pos => prototype.IsValidPlacement(grid, pos)).Take(CandidatesPerRoom);
        }

        private static bool FootprintIsOpen(DungeonGrid grid, GridRoom prototype, Point anchor, HashSet<Point> blocked)
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

        private static void Commit(DungeonGrid grid, List<PathStep> leg, List<PathStep> steps)
        {
            foreach (var step in leg)
                grid.Place(step.Cell, step.Anchor.X, step.Anchor.Y, grid.NextGroupId());
            steps.AddRange(leg);
        }

        private static void Rollback(DungeonGrid grid, List<PathStep> steps, List<(GridRoom Room, Point Anchor)> placedRooms)
        {
            foreach (var step in steps)
                grid.Clear(step.Cell, step.Anchor.X, step.Anchor.Y);
            foreach (var (room, anchor) in placedRooms)
                grid.Clear(room, anchor.X, anchor.Y);
        }

        private static void Shuffle<T>(List<T> list, Random rand)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
