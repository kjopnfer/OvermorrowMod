using Microsoft.Xna.Framework;
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
        private const int MaxExpansionsPerSegment = 50000;

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
            HashSet<Type> waypointAcceptableTypes = null,
            int maxVerticalRun = int.MaxValue)
        {
            if (edgeCost == null) throw new ArgumentNullException(nameof(edgeCost));
            if (startCell == null) throw new ArgumentNullException(nameof(startCell));

            blocked ??= new HashSet<Point>();
            streakLimits ??= new Dictionary<Type, int>();

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
                    edgeCost, blocked, streakLimits, isFinalSegment,
                    waypointAcceptableTypes, maxVerticalRun, fullPath);

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
            bool isFinalSegment,
            HashSet<Type> waypointAcceptableTypes,
            int maxVerticalRun,
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

                foreach (var exit in prevCell.Exits)
                {
                    var candNext = new Point(current.Position.X + exit.CursorDelta.X,
                                             current.Position.Y + exit.CursorDelta.Y);

                    foreach (var candidate in exit.AllowedNext)
                    {
                        var anchor = new Point(
                            candNext.X + candidate.AnchorOffsetFromCursor.X,
                            candNext.Y + candidate.AnchorOffsetFromCursor.Y);

                        // pendingLookup exposes in-progress path cells so structural checks see them too.
                        if (!FootprintIsAvailable(grid, candidate, anchor, blocked, current, cameFrom))
                            continue;
                        var capturedCurrent = current;
                        if (!candidate.IsValidPlacement(grid, anchor,
                                (x, y) => LookupPathCell(capturedCurrent, cameFrom, priorSegmentSteps, x, y)))
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
                        if (!OpenSidesMatch(candidate, anchor, grid,
                                (x, y) => LookupPathCellWithAnchor(capturedCurrent, cameFrom, priorSegmentSteps, x, y)))
                            continue;

                        // Streak: same type increments, different resets, exceed-limit skips.
                        var candType = candidate.GetType();
                        int newStreak = (candType == current.PrevType) ? current.Streak + 1 : 1;
                        if (streakLimits.TryGetValue(candType, out int maxStreak) && newStreak > maxStreak)
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
            HashSet<Point> blocked, Node current, Dictionary<Node, EdgeRecord> cameFrom)
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
                    if (blocked.Contains(new Point(x, y))) return false;
                }
            }

            // Walk the in-progress path for footprint overlap.
            var node = current;
            while (cameFrom.TryGetValue(node, out var rec))
            {
                if (FootprintsOverlap(rec.Cell, rec.Anchor, candidate, anchor))
                    return false;
                node = rec.Parent;
            }
            return true;
        }

        /// <summary>Rectangle-overlap test on two footprints.</summary>
        private static bool FootprintsOverlap(GridRoom a, Point aAnchor, GridRoom b, Point bAnchor)
        {
            int aRight = aAnchor.X + a.CellWidth;
            int aBottom = aAnchor.Y + a.CellHeight;
            int bRight = bAnchor.X + b.CellWidth;
            int bBottom = bAnchor.Y + b.CellHeight;

            return aAnchor.X < bRight && aRight > bAnchor.X
                && aAnchor.Y < bBottom && aBottom > bAnchor.Y;
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
                    }
                }
            }
            return true;
        }

        /// <summary>Returns the in-progress path cell at (x, y), or null. Caller falls back to the grid.</summary>
        private static GridRoom LookupPathCell(Node node, Dictionary<Node, EdgeRecord> cameFrom,
                                               List<PathStep> priorSegmentSteps, int x, int y)
        {
            var found = LookupPathCellWithAnchor(node, cameFrom, priorSegmentSteps, x, y);
            return found?.cell;
        }

        /// <summary>Like <see cref="LookupPathCell"/> but also returns the cell's anchor for sub-cell math.</summary>
        private static (GridRoom cell, Point anchor)? LookupPathCellWithAnchor(
            Node node, Dictionary<Node, EdgeRecord> cameFrom,
            List<PathStep> priorSegmentSteps, int x, int y)
        {
            var n = node;
            while (cameFrom.TryGetValue(n, out var rec))
            {
                if (x >= rec.Anchor.X && x < rec.Anchor.X + rec.Cell.CellWidth
                 && y >= rec.Anchor.Y && y < rec.Anchor.Y + rec.Cell.CellHeight)
                {
                    return (rec.Cell, rec.Anchor);
                }
                n = rec.Parent;
            }
            if (priorSegmentSteps != null)
            {
                for (int i = 0; i < priorSegmentSteps.Count; i++)
                {
                    var step = priorSegmentSteps[i];
                    if (x >= step.Anchor.X && x < step.Anchor.X + step.Cell.CellWidth
                     && y >= step.Anchor.Y && y < step.Anchor.Y + step.Cell.CellHeight)
                    {
                        return (step.Cell, step.Anchor);
                    }
                }
            }
            return null;
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
