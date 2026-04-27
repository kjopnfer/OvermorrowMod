using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding
{
    /// <summary>
    /// Plans the cheapest sequence of cells to place to get from a start
    /// position to a goal position, using a caller-provided cost function.
    /// </summary>
    /// <remarks>
    /// How it works at a high level:
    /// <list type="number">
    /// <item>Start at the start position. Look at every cell type that's
    /// legal to place next (the previous cell tells us via its Exits).</item>
    /// <item>For each option, compute its cost and remember it.</item>
    /// <item>Pick the cheapest option so far, move to that position, and
    /// repeat. Always picking the cheapest known option means we end up
    /// with the cheapest path overall.</item>
    /// <item>Stop when we reach the goal. Walk back through "what came from
    /// what" to reconstruct the actual sequence of placements.</item>
    /// </list>
    /// <para/>
    /// Multi-cell support: because some cells (like stairs) span multiple
    /// grid squares, the planner makes sure a candidate's full footprint
    /// doesn't collide with the grid, blocked zones, or earlier placements
    /// in this same path.
    /// <para/>
    /// Waypoints: if you pass them, the planner runs once per segment
    /// (start → waypoint1 → waypoint2 → goal). The "previous cell" carries
    /// across segments so the path stays connected.
    /// <para/>
    /// Goal arrival on the final segment: the goal cell must already exist
    /// in the grid (e.g. a pre-placed door), and its facing exit must accept
    /// the type of cell that arrived next to it. This guarantees the path
    /// physically connects to the goal rather than just landing nearby.
    /// </remarks>
    public static class GridAStar
    {
        /// <summary>
        /// Safety limit. If the planner explores more places than this without
        /// finding the goal it gives up.
        /// </summary>
        private const int MaxExpansionsPerSegment = 50000;

        /// <summary>
        /// Plans a path from <paramref name="start"/> to <paramref name="goal"/>,
        /// optionally routing through <paramref name="waypoints"/> in order.
        /// Returns the list of placements to stamp, or <c>null</c> if no
        /// valid path exists.
        /// </summary>
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

            // Build the full list of places we need to hit in order:
            // start, then each waypoint, then the goal.
            var stops = new List<Point> { start };
            if (waypoints != null) stops.AddRange(waypoints);
            stops.Add(goal);

            // Plan one segment at a time (start → first waypoint, then first
            // waypoint → second waypoint, ..., then last waypoint → goal).
            // After each segment finishes, the last cell placed becomes the
            // "previous cell" for the next segment so the path links cleanly.
            // The streak counter also carries across — otherwise a chain of
            // same-type cells split by a waypoint could exceed the streak
            // cap because each segment would start counting from 1.
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

                if (segmentSteps == null) return null; // any segment fails -> whole plan fails

                fullPath.AddRange(segmentSteps);

                if (segmentSteps.Count > 0)
                {
                    // Compute streak BEFORE updating prevForSegment — the
                    // streak walk needs the cell that was at segmentStart
                    // before this segment ran, which is the current value of
                    // prevForSegment. After computing, we update prevForSegment
                    // to this segment's last cell for the next iteration.
                    startStreak = ComputeFinalStreak(segmentSteps, startStreak, prevForSegment);
                    prevForSegment = segmentSteps[segmentSteps.Count - 1].Cell;
                }
                // If 0 cells placed (arrival-without-placement), prev and streak stay as-is.

                segmentStart = segmentGoal;
            }

            return fullPath;
        }

        /// <summary>
        /// Replays a segment's placements to determine the streak count of
        /// the very last cell. The streak represents how many of the same
        /// type were placed in a row ending at the segment's last cell — used
        /// to seed the next segment so streak rules stay continuous across
        /// waypoint boundaries.
        /// </summary>
        private static int ComputeFinalStreak(List<PathStep> steps, int startStreak, GridRoom startCell)
        {
            // Walk the placements, updating streak the same way A* does.
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

        // Plans a single segment (no waypoints inside this — the outer
        // FindPath splits multi-waypoint paths into segments and calls this
        // once per segment).
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
            // A "node" is one state in the search: a position on the grid,
            // what cell was last placed there, and how many of that same
            // type have been placed in a row to get here. Two paths reaching
            // the same position via different last cells (or different
            // streak counts) are different nodes because their next legal
            // moves differ.
            var startNode = new Node(start, startCell.GetType(), startStreak, 0);

            // Cheapest total cost found so far to reach each node.
            var bestKnownCost = new Dictionary<Node, double> { [startNode] = 0.0 };

            // For each node, what cell was placed and where, and which earlier node it came from.
            // Used to walk back and rebuild the path once we hit the goal.
            var cameFrom = new Dictionary<Node, EdgeRecord>();

            // Queue of nodes we still need to look at, ordered by "estimated total path cost" (cheapest first).
            var toExplore = new PriorityQueue<Node, double>();
            toExplore.Enqueue(startNode, Heuristic(start, goal));

            int expansions = 0;
            while (toExplore.Count > 0 && expansions++ < MaxExpansionsPerSegment)
            {
                // Pull out the most promising (cheapest estimated) node.
                var current = toExplore.Dequeue();

                // Did we reach the goal by placing a cell on it? (Happens for
                // empty waypoints / empty goals.)
                if (current.Position == goal)
                {
                    // For intermediate waypoints, also require the cell at
                    // this position to be one the next segment can continue
                    // from. If a shaft lands on a waypoint where the next
                    // segment needs to walk horizontally, the next segment
                    // can't proceed — keep searching for a path that ends
                    // the segment on a horizontal-friendly cell instead.
                    if (!isFinalSegment
                        && waypointAcceptableTypes != null
                        && !waypointAcceptableTypes.Contains(current.PrevType))
                    {
                        continue;
                    }
                    return ReconstructPath(current, cameFrom);
                }

                // Find the actual cell sitting at the current position so we
                // can read its allowed next moves. For the very first step
                // it's the startCell; otherwise it's whatever was placed to
                // get here.
                GridRoom prevCell = current.Equals(startNode)
                    ? startCell
                    : cameFrom[current].Cell;

                // Final-segment arrival at an already-occupied goal (e.g. the
                // pre-placed east DoorRoom). We don't try to place a cell on
                // top of it — instead, if any of prev's exits would step the
                // cursor exactly onto the goal AND the goal's facing exit
                // accepts our type, that's a valid arrival. Cost is zero.
                if (isFinalSegment)
                {
                    foreach (var exit in prevCell.Exits)
                    {
                        var landing = new Point(current.Position.X + exit.CursorDelta.X,
                                                current.Position.Y + exit.CursorDelta.Y);
                        if (landing == goal && TargetAcceptsArrivalFromExit(grid, goal, exit, current.PrevType))
                            return ReconstructPath(current, cameFrom);
                    }
                }

                // Try every legal next move from this cell.
                foreach (var exit in prevCell.Exits)
                {
                    var candNext = new Point(current.Position.X + exit.CursorDelta.X,
                                             current.Position.Y + exit.CursorDelta.Y);

                    foreach (var candidate in exit.AllowedNext)
                    {
                        // Where would the candidate cell's anchor (top-left
                        // of its footprint) actually land?
                        var anchor = new Point(
                            candNext.X + candidate.AnchorOffsetFromCursor.X,
                            candNext.Y + candidate.AnchorOffsetFromCursor.Y);

                        // Skip placements that don't physically fit or that
                        // violate the cell's own placement rules. The
                        // pending-lookup lambda exposes cells planned earlier
                        // in this same FindPath (current segment via cameFrom,
                        // prior segments via priorSegmentSteps) so structural
                        // checks see the in-progress path, not just the
                        // committed grid.
                        if (!FootprintIsAvailable(grid, candidate, anchor, blocked, current, cameFrom))
                            continue;
                        var capturedCurrent = current;
                        if (!candidate.IsValidPlacement(grid, anchor,
                                (x, y) => LookupPathCell(capturedCurrent, cameFrom, priorSegmentSteps, x, y)))
                            continue;

                        // Entry-side rule: the candidate must be physically
                        // open on the side the walker is entering from. The
                        // source's exit authorizes which TYPES are allowed
                        // through it, but a corridor approached from above
                        // (or a shaft approached from the side) would be
                        // sealed on the entry face and the walker would hit
                        // a wall. We model this by requiring the candidate
                        // to have an exit whose direction is the inverse of
                        // the source's cursor delta — that exit represents
                        // an open face on the entry side.
                        //
                        // Only enforced for cardinal source moves. Stairs
                        // step diagonally (delta like (2,1)) and don't model
                        // their entry as an exit; for those we trust the
                        // source's AllowedNext authorization. The check is
                        // also skipped if the candidate has no cardinal
                        // exits at all (e.g. stair candidates), since there
                        // is nothing meaningful to compare against.
                        bool isCardinalSource = (exit.CursorDelta.X == 0) ^ (exit.CursorDelta.Y == 0);
                        if (isCardinalSource)
                        {
                            var inverse = new Point(-exit.CursorDelta.X, -exit.CursorDelta.Y);
                            bool candHasCardinalExit = false;
                            bool candHasMatchingExit = false;
                            foreach (var ce in candidate.Exits)
                            {
                                bool ceCardinal = (ce.CursorDelta.X == 0) ^ (ce.CursorDelta.Y == 0);
                                if (ceCardinal) candHasCardinalExit = true;
                                if (ce.CursorDelta == inverse) { candHasMatchingExit = true; break; }
                            }
                            if (candHasCardinalExit && !candHasMatchingExit) continue;
                        }

                        // Streak rule: if the candidate is the same type as
                        // the last placed cell, increment the streak; otherwise
                        // it resets. If a configured limit is exceeded, this
                        // placement isn't allowed and we skip it.
                        var candType = candidate.GetType();
                        int newStreak = (candType == current.PrevType) ? current.Streak + 1 : 1;
                        if (streakLimits.TryGetValue(candType, out int maxStreak) && newStreak > maxStreak)
                            continue;

                        // Vertical-run rule: count consecutive moves whose
                        // exit goes straight up or down. Resets to 0 the
                        // moment the walker moves horizontally. Caps the
                        // visual depth of any shaft chain (including any
                        // bookshelf landings inside it) since those landings
                        // also contribute to the vertical run.
                        int newVerticalRun = (exit.CursorDelta.Y != 0 && exit.CursorDelta.X == 0)
                            ? current.VerticalRun + 1
                            : 0;
                        if (newVerticalRun > maxVerticalRun) continue;

                        double cost = edgeCost(anchor, candidate);
                        if (double.IsPositiveInfinity(cost)) continue;

                        var neighbor = new Node(candNext, candType, newStreak, newVerticalRun);
                        double newCost = bestKnownCost[current] + cost;

                        // If we've already found a cheaper way to this node,
                        // ignore this longer one.
                        if (bestKnownCost.TryGetValue(neighbor, out double existingCost) && newCost >= existingCost)
                            continue;

                        // Otherwise record this as the new best and queue it.
                        // The priority is "cost so far + estimate of cost to reach the goal from here"
                        bestKnownCost[neighbor] = newCost;
                        cameFrom[neighbor] = new EdgeRecord(current, candidate, anchor);
                        toExplore.Enqueue(neighbor, newCost + Heuristic(candNext, goal));
                    }
                }
            }

            return null; // exhausted everything without reaching the goal
        }

        /// <summary>
        /// Estimated remaining cost from one position to another. 
        /// Uses the straight-line "city block" distance (how many grid steps apart they are). 
        /// Helps as a hint to prefer paths that look like they're heading toward the goal.
        /// </summary>
        private static double Heuristic(Point from, Point to) =>
            Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);

        /// <summary>
        /// Returns true if the candidate cell can fit at the given anchor:
        /// Every grid square must be in-bounds, empty, not in the
        /// blocked zone, and not already used by an earlier placement in this same path. 
        /// The last check matters for multi-cell pieces like 2x2 stairs 
        /// where a smaller earlier piece could occupy a square the new piece's footprint wants.
        /// </summary>
        private static bool FootprintIsAvailable(
            DungeonGrid grid, GridRoom candidate, Point anchor,
            HashSet<Point> blocked, Node current, Dictionary<Node, EdgeRecord> cameFrom)
        {
            // Check each square the candidate would cover.
            for (int sc = 0; sc < candidate.CellWidth; sc++)
            {
                for (int sr = 0; sr < candidate.CellHeight; sr++)
                {
                    int x = anchor.X + sc;
                    int y = anchor.Y + sr;
                    var slot = grid.GetSlot(x, y);
                    
                    if (slot == null) 
                        return false;            // off the grid
                    
                    if (!slot.IsEmpty) 
                        return false;           // something else is there

                    if (blocked.Contains(new Point(x, y))) 
                        return false; // forbidden zone
                }
            }

            // Walk back through the path so far and ensure no earlier
            // placement's footprint overlaps this candidate's footprint.
            var node = current;
            while (cameFrom.TryGetValue(node, out var rec))
            {
                if (FootprintsOverlap(rec.Cell, rec.Anchor, candidate, anchor))
                    return false;
                node = rec.Parent;
            }
            return true;
        }

        /// <summary>
        /// Returns true if two cells' footprints share any grid square.
        /// Standard rectangle-overlap test.
        /// </summary>
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
        /// Resolves what cell (if any) is planned at the given position, by
        /// walking the active node's cameFrom chain (cells planned earlier
        /// in the current segment) and the priorSegmentSteps list (cells
        /// planned in earlier segments of the same FindPath). Returns null
        /// if no cell is planned there. The caller falls back to the
        /// committed grid when this returns null.
        /// </summary>
        private static GridRoom LookupPathCell(Node node, Dictionary<Node, EdgeRecord> cameFrom,
                                               List<PathStep> priorSegmentSteps, int x, int y)
        {
            // Walk the chain of cells placed in the current segment so far.
            var n = node;
            while (cameFrom.TryGetValue(n, out var rec))
            {
                if (x >= rec.Anchor.X && x < rec.Anchor.X + rec.Cell.CellWidth
                 && y >= rec.Anchor.Y && y < rec.Anchor.Y + rec.Cell.CellHeight)
                {
                    return rec.Cell;
                }
                n = rec.Parent;
            }
            // Then check cells from prior segments of the same FindPath call.
            if (priorSegmentSteps != null)
            {
                for (int i = 0; i < priorSegmentSteps.Count; i++)
                {
                    var step = priorSegmentSteps[i];
                    if (x >= step.Anchor.X && x < step.Anchor.X + step.Cell.CellWidth
                     && y >= step.Anchor.Y && y < step.Anchor.Y + step.Cell.CellHeight)
                    {
                        return step.Cell;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if the planner can step from the current position straight
        /// onto an already-occupied goal cell using the given exit. Used when
        /// the goal already has a cell on it (e.g. a pre-placed door) — we
        /// don't try to place a new cell on top, we just verify the
        /// connection is physically valid: the goal's side facing us must
        /// accept the type of cell we're arriving with.
        /// </summary>
        private static bool TargetAcceptsArrivalFromExit(DungeonGrid grid, Point goal, CellExit walkerExit, Type walkerType)
        {
            var slot = grid.GetSlot(goal.X, goal.Y);
            if (slot == null || slot.IsEmpty) return false;

            // The goal's side facing us is the opposite of how we'd be moving.
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

        /// <summary>
        /// Walks back from the goal node through the "where did this come
        /// from" chain to rebuild the actual sequence of placements. The
        /// chain starts at the goal and ends at the start, so we reverse
        /// the result before returning.
        /// </summary>
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

        /// <summary>
        /// One state in the A* search: a grid position, the C# type of the
        /// cell last placed there, and how many of that same type were
        /// placed in a row to get here. Two paths arriving at the same
        /// position with different last-cell types or different streak
        /// counts are distinct states because their next moves differ.
        /// </summary>
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

        /// <summary>
        /// One step in the path the planner discovered:
        ///   - <see cref="Cell"/>: which cell was placed.
        ///   - <see cref="Anchor"/>: the top-left grid square of that cell's
        ///     footprint (where <c>grid.Place</c> needs to start stamping).
        ///     For 1x1 cells this is the same as the cursor position. For
        ///     multi-cell pieces like a 2x2 ascending stair, the anchor sits
        ///     one row above the cursor — the cursor enters the stair at
        ///     its bottom-left while the footprint extends up-and-right.
        ///   - <see cref="Parent"/>: which earlier state this came from.
        /// Linking these backward from the goal rebuilds the full path.
        /// </summary>
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
