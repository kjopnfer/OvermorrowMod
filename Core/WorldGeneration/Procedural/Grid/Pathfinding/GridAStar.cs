using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding
{
    /// <summary>
    /// A* over grid cells with project-specific extensions:
    /// multi-cell footprints (stairs span multiple squares and must not collide
    /// with the grid, blocked zones, or earlier placements in the same path);
    /// waypoint segmentation (start → wp1 → ... → goal, with the previous cell
    /// and streak counter carrying across segment boundaries); and occupied-goal
    /// arrival, where the goal already holds a pre-placed cell (e.g. a door)
    /// and the path must physically dock against its facing exit.
    /// </summary>
    public static class GridAStar
    {
        private const int MaxExpansionsPerSegment = 3000;

        /// <summary>Plans a path from start to goal through optional waypoints. Returns null if none exists.</summary>
        public static List<PathStep> FindPath(
            DungeonGrid grid,
            Point start,
            Point goal,
            GridRoom startCell,
            EdgeCost edgeCost,
            IReadOnlyList<Point> waypoints = null,
            HashSet<Point> blocked = null,
            IReadOnlyDictionary<Type, int> streakLimits = null,
            IReadOnlyDictionary<Type, int> minStreakLimits = null,
            HashSet<Type> waypointAcceptableTypes = null,
            int maxVerticalRun = int.MaxValue,
            int shaftRowWindow = int.MaxValue)
        {
            if (edgeCost == null) throw new ArgumentNullException(nameof(edgeCost));
            if (startCell == null) throw new ArgumentNullException(nameof(startCell));

            blocked ??= new HashSet<Point>();
            streakLimits ??= new Dictionary<Type, int>();
            minStreakLimits ??= new Dictionary<Type, int>();

            // Stops: start, waypoints in order, goal.
            var stops = new List<Point> { start };
            if (waypoints != null) stops.AddRange(waypoints);
            stops.Add(goal);

            // Plan one segment at a time. Previous cell and streak carry across
            // boundaries so streak caps hold and the path stays connected.
            var fullPath = new List<PathStep>();
            GridRoom prevForSegment = startCell;
            Point segmentStart = start;
            int startStreak = 1;

            for (int i = 1; i < stops.Count; i++)
            {
                Point segmentGoal = stops[i];
                bool isFinalSegment = (i == stops.Count - 1);

                var segmentSteps = FindSegment(
                    grid, segmentStart, segmentGoal, prevForSegment, startStreak,
                    edgeCost, blocked, streakLimits, minStreakLimits, isFinalSegment,
                    waypointAcceptableTypes, maxVerticalRun, shaftRowWindow, fullPath);

                if (segmentSteps == null) return null;

                fullPath.AddRange(segmentSteps);

                if (segmentSteps.Count > 0)
                {
                    // Streak must be computed BEFORE prevForSegment is reassigned.
                    startStreak = ComputeFinalStreak(segmentSteps, startStreak, prevForSegment);
                    prevForSegment = segmentSteps[segmentSteps.Count - 1].Cell;
                }
                else
                {
                    // Empty segment: arrived at a pre-placed waypoint. Continue from inside it.
                    var goalSlot = grid.GetSlot(segmentGoal.X, segmentGoal.Y);
                    if (goalSlot != null && !goalSlot.IsEmpty)
                    {
                        prevForSegment = goalSlot.Room;
                        startStreak = 1;
                    }
                }

                segmentStart = segmentGoal;
            }

            return fullPath;
        }

        /// <summary>Replays a segment to find the trailing streak count, used to seed the next segment.</summary>
        private static int ComputeFinalStreak(List<PathStep> steps, int startStreak, GridRoom startCell)
        {
            int streak = startStreak;
            Type prevType = startCell.GetType();
            foreach (var step in steps)
            {
                Type curType = step.Cell.GetType();
                streak = (curType == prevType) ? streak + 1 : 1;
                prevType = curType;
            }
            return streak;
        }

        // Plans a single segment between two stops.
        private static List<PathStep> FindSegment(
            DungeonGrid grid,
            Point start,
            Point goal,
            GridRoom startCell,
            int startStreak,
            EdgeCost edgeCost,
            HashSet<Point> blocked,
            IReadOnlyDictionary<Type, int> streakLimits,
            IReadOnlyDictionary<Type, int> minStreakLimits,
            bool isFinalSegment,
            HashSet<Type> waypointAcceptableTypes,
            int maxVerticalRun,
            int shaftRowWindow,
            List<PathStep> priorSegmentSteps)
        {
            var startNode = new Node(start, startCell.GetType(), startStreak, 0);
            var bestKnownCost = new Dictionary<Node, double> { [startNode] = 0.0 };
            var cameFrom = new Dictionary<Node, EdgeRecord>();
            var toExplore = new PriorityQueue<Node, double>();
            toExplore.Enqueue(startNode, Heuristic(start, goal));

            int expansions = 0;
            while (toExplore.Count > 0 && expansions++ < MaxExpansionsPerSegment)
            {
                var current = toExplore.Dequeue();

                // Goal reached by placing a cell on it (empty goal/waypoint).
                if (current.Position == goal)
                {
                    // Intermediate waypoints must end on a type the next segment can continue from.
                    if (!isFinalSegment
                        && waypointAcceptableTypes != null
                        && !waypointAcceptableTypes.Contains(current.PrevType))
                    {
                        continue;
                    }
                    // Min-streak: cannot end the path on a type that has not
                    // satisfied its minimum chain length.
                    if (minStreakLimits.TryGetValue(current.PrevType, out int minS)
                        && current.Streak < minS)
                        continue;
                    return ReconstructPath(current, cameFrom);
                }

                GridRoom prevCell = current.Equals(startNode)
                    ? startCell
                    : cameFrom[current].Cell;

                // Arrival at an already-occupied goal (e.g. pre-placed door or required room).
                // Valid if any of prev's exits steps onto the goal and the goal accepts the arriving type.
                {
                    foreach (var exit in prevCell.Exits)
                    {
                        var landing = new Point(current.Position.X + exit.CursorDelta.X,
                                                current.Position.Y + exit.CursorDelta.Y);
                        if (landing != goal) continue;
                        if (!TargetAcceptsArrivalFromExit(grid, goal, exit, current.PrevType)) continue;

                        // Min-streak applies when arriving at an occupied goal too.
                        if (minStreakLimits.TryGetValue(current.PrevType, out int minA)
                            && current.Streak < minA)
                            continue;

                        if (!isFinalSegment && waypointAcceptableTypes != null)
                        {
                            var goalSlot = grid.GetSlot(goal.X, goal.Y);
                            var goalType = goalSlot?.Room?.GetType();
                            if (goalType == null || !waypointAcceptableTypes.Contains(goalType))
                                continue;
                        }

                        return ReconstructPath(current, cameFrom);
                    }
                }

                // Hoisted once per dequeue: every prior chain-walk lookup
                // (FootprintIsAvailable, AntiFusionOk, IsValidPlacement,
                // OpenSidesMatch, PathHasShaftInColumn) was O(N) in the
                // current path length. Building a flat dict here makes them
                // all O(1) for the rest of this expansion.
                var pathOccupied = BuildPathOccupied(current, cameFrom, priorSegmentSteps, out var pathShafts);
                Func<int, int, GridRoom> pathCellLookup = (x, y) =>
                    pathOccupied.TryGetValue(new Point(x, y), out var p) ? p.cell : null;
                Func<int, int, (GridRoom cell, Point anchor)?> pathCellWithAnchorLookup = (x, y) =>
                    pathOccupied.TryGetValue(new Point(x, y), out var p) ? ((GridRoom, Point)?)(p.cell, p.anchor) : null;

                foreach (var exit in prevCell.Exits)
                {
                    var candNext = new Point(current.Position.X + exit.CursorDelta.X,
                                             current.Position.Y + exit.CursorDelta.Y);

                    foreach (var candidate in exit.AllowedNext)
                    {
                        var anchor = new Point(
                            candNext.X + candidate.AnchorOffsetFromCursor.X,
                            candNext.Y + candidate.AnchorOffsetFromCursor.Y);

                        if (!FootprintIsAvailable(grid, candidate, anchor, blocked, pathOccupied))
                            continue;
                        if (!candidate.IsValidPlacement(grid, anchor, pathCellLookup))
                            continue;

                        // Anti-fusion: a candidate's footprint must not border
                        // a foreign cell (one that's neither in this A* call's
                        // path nor part of the segment's start/goal subgoal).
                        // Without this, two A* calls placing cells side-by-side
                        // produce a connected hallway via shared open sides,
                        // which is the mechanism behind every accidental loop
                        // we've seen.
                        if (!AntiFusionOk(grid, candidate, anchor, start, startCell, goal, pathOccupied))
                            continue;

                        // Cardinal entries: candidate must expose an exit on the entry face.
                        // Compare signs, not deltas, so a 1-wide cell can dock against a 2-wide source.
                        // Skipped for diagonal (stair) sources and for candidates with no cardinal exits.
                        bool isCardinalSource = (exit.CursorDelta.X == 0) ^ (exit.CursorDelta.Y == 0);
                        if (isCardinalSource)
                        {
                            int srcSignX = System.Math.Sign(exit.CursorDelta.X);
                            int srcSignY = System.Math.Sign(exit.CursorDelta.Y);
                            bool candHasCardinalExit = false;
                            bool candHasMatchingExit = false;
                            foreach (var ce in candidate.Exits)
                            {
                                bool ceCardinal = (ce.CursorDelta.X == 0) ^ (ce.CursorDelta.Y == 0);
                                if (ceCardinal) candHasCardinalExit = true;
                                int ceSignX = System.Math.Sign(ce.CursorDelta.X);
                                int ceSignY = System.Math.Sign(ce.CursorDelta.Y);
                                if (ceCardinal && ceSignX == -srcSignX && ceSignY == -srcSignY)
                                { candHasMatchingExit = true; break; }
                            }
                            if (candHasCardinalExit && !candHasMatchingExit) continue;
                        }

                        // Shared borders must agree: both open or both closed. Catches intra- and cross-path collisions.
                        if (!OpenSidesMatch(candidate, anchor, grid, pathCellWithAnchorLookup))
                            continue;

                        // Path-aware shaft adjacency: the cost function rejects
                        // shafts adjacent to shafts in the committed grid, but a
                        // single A* call's placements aren't committed until
                        // FindPath returns. pathShafts mirrors the same info for
                        // shafts placed earlier in this run. shaftRowWindow limits
                        // the veto to nearby rows (int.MaxValue = whole column).
                        if (candidate is ShaftCell && PathShaftWithin(pathShafts, anchor, shaftRowWindow))
                            continue;

                        // Streak: same type increments, different resets, exceed-limit skips.
                        var candType = candidate.GetType();
                        int newStreak = (candType == current.PrevType) ? current.Streak + 1 : 1;
                        if (streakLimits.TryGetValue(candType, out int maxStreak) && newStreak > maxStreak)
                            continue;

                        // Grid-aware horizontal chain check: counts already-committed
                        // same-type cells extending left and right of the anchor's row.
                        // Catches the case where two separate paths each respect the
                        // path-level streak but stitch their placements into one long
                        // visual chain. Bounded walks with early exit so this stays
                        // cheap (~10 array reads per candidate worst case).
                        if (streakLimits.TryGetValue(candType, out int hMax))
                        {
                            int chain = 1;
                            int x = anchor.X - 1;
                            while (x >= 0 && chain <= hMax)
                            {
                                var s = grid.GetSlot(x, anchor.Y);
                                if (s == null || s.IsEmpty) break;
                                if (s.Room.GetType() != candType) break;
                                chain++;
                                x--;
                            }
                            x = anchor.X + candidate.CellWidth;
                            while (x < grid.Cols && chain <= hMax)
                            {
                                var s = grid.GetSlot(x, anchor.Y);
                                if (s == null || s.IsEmpty) break;
                                if (s.Room.GetType() != candType) break;
                                chain++;
                                x++;
                            }
                            if (chain > hMax) continue;
                        }

                        // Min-streak: leaving a type before its minimum is satisfied is rejected.
                        if (candType != current.PrevType
                            && minStreakLimits.TryGetValue(current.PrevType, out int minPrev)
                            && current.Streak < minPrev)
                            continue;

                        // Vertical run: counts consecutive vertical moves; horizontal resets to 0.
                        int newVerticalRun = (exit.CursorDelta.Y != 0 && exit.CursorDelta.X == 0)
                            ? current.VerticalRun + 1
                            : 0;
                        if (newVerticalRun > maxVerticalRun) continue;

                        double cost = edgeCost(anchor, candidate);
                        if (double.IsPositiveInfinity(cost)) continue;

                        var neighbor = new Node(candNext, candType, newStreak, newVerticalRun);
                        double newCost = bestKnownCost[current] + cost;

                        if (bestKnownCost.TryGetValue(neighbor, out double existingCost) && newCost >= existingCost)
                            continue;

                        bestKnownCost[neighbor] = newCost;
                        cameFrom[neighbor] = new EdgeRecord(current, candidate, anchor);
                        toExplore.Enqueue(neighbor, newCost + Heuristic(candNext, goal));
                    }
                }
            }

            return null;
        }

        /// <summary>Manhattan distance heuristic.</summary>
        private static double Heuristic(Point from, Point to) =>
            Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);

        /// <summary>
        /// True if the candidate fits: every square in-bounds, empty, not blocked,
        /// and not overlapping any earlier placement in this path (matters for multi-cell pieces).
        /// </summary>
        private static bool FootprintIsAvailable(
            DungeonGrid grid, GridRoom candidate, Point anchor,
            HashSet<Point> blocked,
            Dictionary<Point, (GridRoom cell, Point anchor)> pathOccupied)
        {
            for (int sc = 0; sc < candidate.CellWidth; sc++)
            {
                for (int sr = 0; sr < candidate.CellHeight; sr++)
                {
                    int x = anchor.X + sc;
                    int y = anchor.Y + sr;
                    var slot = grid.GetSlot(x, y);

                    if (slot == null) return false;
                    if (!slot.IsEmpty) return false;
                    var pt = new Point(x, y);
                    if (blocked.Contains(pt)) return false;
                    if (pathOccupied.ContainsKey(pt)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Builds a per-sub-cell dictionary of the points occupied by the current
        /// segment's in-progress path plus any prior-segment steps. Replaces
        /// O(N) cameFrom chain walks inside the inner loop with O(1) lookups.
        /// Also emits the anchors of ShaftCells placed in this path
        /// (used by the path-aware shaft adjacency check).
        /// </summary>
        private static Dictionary<Point, (GridRoom cell, Point anchor)> BuildPathOccupied(
            Node current, Dictionary<Node, EdgeRecord> cameFrom,
            List<PathStep> priorSegmentSteps, out List<Point> pathShafts)
        {
            var dict = new Dictionary<Point, (GridRoom, Point)>();
            pathShafts = new List<Point>();

            var n = current;
            while (cameFrom.TryGetValue(n, out var rec))
            {
                for (int sc = 0; sc < rec.Cell.CellWidth; sc++)
                    for (int sr = 0; sr < rec.Cell.CellHeight; sr++)
                        dict[new Point(rec.Anchor.X + sc, rec.Anchor.Y + sr)] = (rec.Cell, rec.Anchor);
                if (rec.Cell is ShaftCell) pathShafts.Add(rec.Anchor);
                n = rec.Parent;
            }
            if (priorSegmentSteps != null)
            {
                for (int i = 0; i < priorSegmentSteps.Count; i++)
                {
                    var step = priorSegmentSteps[i];
                    for (int sc = 0; sc < step.Cell.CellWidth; sc++)
                        for (int sr = 0; sr < step.Cell.CellHeight; sr++)
                            dict[new Point(step.Anchor.X + sc, step.Anchor.Y + sr)] = (step.Cell, step.Anchor);
                    if (step.Cell is ShaftCell) pathShafts.Add(step.Anchor);
                }
            }
            return dict;
        }

        /// <summary>
        /// True if a shaft already placed in this path sits in an adjacent column
        /// (col +/- 1) within <paramref name="rowWindow"/> rows of <paramref name="anchor"/>.
        /// With rowWindow = int.MaxValue this is column-global (any row).
        /// </summary>
        private static bool PathShaftWithin(List<Point> pathShafts, Point anchor, int rowWindow)
        {
            foreach (var s in pathShafts)
                if (Math.Abs(s.X - anchor.X) <= 1 && Math.Abs(s.Y - anchor.Y) <= rowWindow)
                    return true;
            return false;
        }

        /// <summary>
        /// True if every external border of the candidate's footprint agrees with its neighbor:
        /// both open or both closed. Empty neighbors and internal sub-cell edges are ignored.
        /// </summary>
        private static bool OpenSidesMatch(GridRoom candidate, Point anchor,
                                           DungeonGrid grid,
                                           Func<int, int, (GridRoom cell, Point anchor)?> pendingAnchorLookup)
        {
            var dirs = new (Direction side, int dx, int dy, Direction opposite)[]
            {
                (Direction.Top,    0, -1, Direction.Bottom),
                (Direction.Bottom, 0,  1, Direction.Top),
                (Direction.Left,  -1, 0, Direction.Right),
                (Direction.Right,  1, 0, Direction.Left),
            };

            for (int sc = 0; sc < candidate.CellWidth; sc++)
            {
                for (int sr = 0; sr < candidate.CellHeight; sr++)
                {
                    foreach (var d in dirs)
                    {
                        // Internal seams between sub-cells of the same candidate.
                        int neighborSubCol = sc + d.dx;
                        int neighborSubRow = sr + d.dy;
                        if (neighborSubCol >= 0 && neighborSubCol < candidate.CellWidth
                         && neighborSubRow >= 0 && neighborSubRow < candidate.CellHeight)
                            continue;

                        int gx = anchor.X + sc + d.dx;
                        int gy = anchor.Y + sr + d.dy;

                        // Pending placements first, then committed grid.
                        GridRoom neighbor = null;
                        int neighborSubX = 0, neighborSubY = 0;

                        if (pendingAnchorLookup != null)
                        {
                            var pending = pendingAnchorLookup(gx, gy);
                            if (pending.HasValue)
                            {
                                neighbor = pending.Value.cell;
                                neighborSubX = gx - pending.Value.anchor.X;
                                neighborSubY = gy - pending.Value.anchor.Y;
                            }
                        }
                        if (neighbor == null)
                        {
                            var slot = grid.GetSlot(gx, gy);
                            if (slot == null || slot.IsEmpty) continue;
                            neighbor = slot.Room;
                            neighborSubX = slot.SubCol;
                            neighborSubY = slot.SubRow;
                        }

                        bool ourSide = candidate.IsOpenSide(sc, sr, d.side);
                        bool theirSide = neighbor.IsOpenSide(neighborSubX, neighborSubY, d.opposite);

                        if (ourSide != theirSide) return false;

                        // Mutual AllowedNeighbors check on top of geometry.
                        // Asymmetric IsFeature exemption: when one side is a
                        // planner-placed feature (door, anchor combat, etc.)
                        // we skip the candidate's own whitelist so standard
                        // rooms don't have to list every feature type, but
                        // the feature's whitelist still gates what can dock
                        // against it.
                        if (ourSide)
                        {
                            if (!neighbor.IsFeature)
                            {
                                var weAccept = candidate.GetAcceptedNeighbors(sc, sr, d.side);
                                if (weAccept == null || !weAccept.Contains(neighbor.GetType()))
                                    return false;
                            }
                            if (!candidate.IsFeature)
                            {
                                var theyAccept = neighbor.GetAcceptedNeighbors(neighborSubX, neighborSubY, d.opposite);
                                if (theyAccept == null || !theyAccept.Contains(candidate.GetType()))
                                    return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Anti-fusion: rejects candidates whose footprint borders a foreign
        /// cell (a cell that's not part of this segment's path and not the
        /// segment's start or goal subgoal). Endpoint cells are exempt because
        /// docking against them is the whole point. Without this check, two
        /// A* segments placing cells side-by-side connect via shared open
        /// sides into accidental loops. Cardinal neighbors only — diagonals
        /// don't form connections through grid-cell borders.
        /// </summary>
        private static bool AntiFusionOk(
            DungeonGrid grid,
            GridRoom candidate, Point anchor,
            Point start, GridRoom startCell,
            Point goal,
            Dictionary<Point, (GridRoom cell, Point anchor)> pathOccupied)
        {
            int candW = candidate.CellWidth;
            int candH = candidate.CellHeight;

            // Footprint of the segment's start subgoal. If startCell is null,
            // treat as 1x1 (door / arbitrary point).
            int startW = startCell?.CellWidth ?? 1;
            int startH = startCell?.CellHeight ?? 1;

            // Footprint of the segment's goal subgoal. Look up the grid; if
            // the goal hasn't been pre-placed (rare), treat as 1x1.
            int goalW = 1, goalH = 1;
            var goalSlot = grid.GetSlot(goal.X, goal.Y);
            if (goalSlot != null && !goalSlot.IsEmpty)
            {
                goalW = goalSlot.Room.CellWidth;
                goalH = goalSlot.Room.CellHeight;
            }

            for (int sc = 0; sc < candW; sc++)
            {
                for (int sr = 0; sr < candH; sr++)
                {
                    int ix = anchor.X + sc;
                    int iy = anchor.Y + sr;

                    // Four cardinal neighbors of this footprint sub-cell.
                    var neighbors = new (int x, int y)[]
                    {
                        (ix + 1, iy), (ix - 1, iy), (ix, iy + 1), (ix, iy - 1),
                    };
                    foreach (var (nx, ny) in neighbors)
                    {
                        // Inside the candidate's own footprint? Same room, skip.
                        if (nx >= anchor.X && nx < anchor.X + candW
                         && ny >= anchor.Y && ny < anchor.Y + candH) continue;

                        // Inside the segment's start subgoal footprint? Allowed.
                        if (nx >= start.X && nx < start.X + startW
                         && ny >= start.Y && ny < start.Y + startH) continue;

                        // Inside the segment's goal subgoal footprint? Allowed.
                        if (nx >= goal.X && nx < goal.X + goalW
                         && ny >= goal.Y && ny < goal.Y + goalH) continue;

                        var nSlot = grid.GetSlot(nx, ny);
                        if (nSlot == null) continue;       // off-grid
                        if (nSlot.IsEmpty) continue;       // empty cell, no fusion possible

                        // In this segment's own in-progress path? Same A* run, OK.
                        if (pathOccupied.ContainsKey(new Point(nx, ny))) continue;

                        // Foreign occupied cell. Reject this candidate.
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>True if the occupied goal accepts the walker's arriving cell type on its facing exit.</summary>
        private static bool TargetAcceptsArrivalFromExit(DungeonGrid grid, Point goal, CellExit walkerExit, Type walkerType)
        {
            var slot = grid.GetSlot(goal.X, goal.Y);
            if (slot == null || slot.IsEmpty) return false;

            var oppositeDelta = new Point(-walkerExit.CursorDelta.X, -walkerExit.CursorDelta.Y);
            foreach (var targetExit in slot.Room.Exits)
            {
                if (targetExit.CursorDelta != oppositeDelta) continue;
                foreach (var opt in targetExit.AllowedNext)
                {
                    if (opt.GetType() == walkerType) return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>Walks the cameFrom chain back from goal to start, then reverses to placement order.</summary>
        private static List<PathStep> ReconstructPath(Node end, Dictionary<Node, EdgeRecord> cameFrom)
        {
            var steps = new List<PathStep>();
            var node = end;
            while (cameFrom.TryGetValue(node, out var rec))
            {
                steps.Add(new PathStep(rec.Cell, rec.Anchor));
                node = rec.Parent;
            }
            steps.Reverse();

            return steps;
        }

        /// <summary>One A* search state: position, last cell type, streak, vertical run.</summary>
        private readonly struct Node : IEquatable<Node>
        {
            public Point Position { get; }
            public Type PrevType { get; }
            public int Streak { get; }
            public int VerticalRun { get; }

            public Node(Point pos, Type prevType, int streak, int verticalRun)
            {
                Position = pos;
                PrevType = prevType;
                Streak = streak;
                VerticalRun = verticalRun;
            }

            public bool Equals(Node other) =>
                Position == other.Position
                && PrevType == other.PrevType
                && Streak == other.Streak
                && VerticalRun == other.VerticalRun;
            public override bool Equals(object obj) => obj is Node n && Equals(n);
            public override int GetHashCode() => HashCode.Combine(Position.X, Position.Y, PrevType, Streak, VerticalRun);
        }

        /// <summary>cameFrom record: parent node, placed cell, and footprint anchor.</summary>
        private readonly struct EdgeRecord
        {
            public Node Parent { get; }
            public GridRoom Cell { get; }
            public Point Anchor { get; }

            public EdgeRecord(Node parent, GridRoom cell, Point anchor)
            {
                Parent = parent;
                Cell = cell;
                Anchor = anchor;
            }
        }
    }
}
