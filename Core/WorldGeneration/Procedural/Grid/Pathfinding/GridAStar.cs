using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding
{
    /// <summary>
    /// A* over grid cells with project-specific extensions:
    /// multi-cell footprints (stairs span multiple squares and must not collide
    /// with the grid, blocked zones, or earlier placements in the same path)
    /// and occupied-goal arrival, where the goal already holds a pre-placed cell
    /// (e.g. a door) and the path must physically dock against its facing exit.
    /// One instance serves every search on one grid and reuses its buffers between searches.
    /// </summary>
    public sealed class GridAStar
    {
        public const int DefaultMaxExpansions = 3000;

        private static readonly (Direction side, int dx, int dy, Direction opposite)[] Sides =
        {
            (Direction.Top, 0, -1, Direction.Bottom),
            (Direction.Bottom, 0, 1, Direction.Top),
            (Direction.Left, -1, 0, Direction.Right),
            (Direction.Right, 1, 0, Direction.Left),
        };

        private static readonly (int dx, int dy)[] Neighbors8 = { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (1, -1), (-1, 1), (-1, -1) };

        private readonly DungeonGrid _grid;
        private readonly bool[] _blocked;
        private readonly IReadOnlyDictionary<Type, int> _streakLimits;
        private readonly IReadOnlyDictionary<Type, int> _minStreakLimits;
        private readonly int _maxVerticalRun;
        private readonly bool _validateReachability;
        private readonly PlanStats _stats;
        private readonly PendingPlacements _pending;
        private readonly List<Point> _pathShafts = new();

        // A node's parent, best cost and placed cell are overwritten whenever a cheaper route to it is found,
        // so walking parents always follows the current best chain.
        private readonly Dictionary<Node, int> _nodeIds = new();
        private readonly List<Node> _nodes = new();
        private readonly List<double> _bestCost = new();
        private readonly List<int> _parent = new();
        private readonly List<GridRoom> _placedCell = new();
        private readonly List<Point> _placedAnchor = new();
        private readonly PriorityQueue<int, double> _toExplore = new();

        private readonly int[] _floodMark;
        private readonly int[] _floodQueue;
        private readonly int[] _passableMark;
        private readonly bool[] _passable;
        private int _floodId;

        private Point _start;
        private Point _goal;
        private GridRoom _startCell;
        private int _startWidth;
        private int _startHeight;
        private int _goalWidth;
        private int _goalHeight;
        private bool _goalOccupied;

        /// <summary>
        /// The generation phase the next searches are counted under.
        /// </summary>
        public SearchPhase Phase { get; set; }

        public GridAStar(DungeonGrid grid, HashSet<Point> blocked, IReadOnlyDictionary<Type, int> streakLimits, IReadOnlyDictionary<Type, int> minStreakLimits, int maxVerticalRun, bool validateReachability, PlanStats stats)
        {
            _grid = grid;
            _streakLimits = streakLimits;
            _minStreakLimits = minStreakLimits;
            _maxVerticalRun = maxVerticalRun;
            _validateReachability = validateReachability;
            _stats = stats;
            _pending = new PendingPlacements(grid.Cols, grid.Rows);
            _floodMark = new int[grid.Cols * grid.Rows];
            _floodQueue = new int[grid.Cols * grid.Rows];
            _passableMark = new int[grid.Cols * grid.Rows];
            _passable = new bool[grid.Cols * grid.Rows];

            _blocked = new bool[grid.Cols * grid.Rows];
            foreach (var p in blocked)
                _blocked[Index(p.X, p.Y)] = true;
        }

        /// <summary>
        /// Plans a path from start to goal.
        /// </summary>
        /// <returns>The placed steps in order, or null if the goal is unreachable or the expansion budget runs out.</returns>
        public List<PathStep> FindPath(Point start, Point goal, GridRoom startCell, EdgeCost edgeCost, int maxExpansions = DefaultMaxExpansions)
        {
            long began = Stopwatch.GetTimestamp();
            var phase = _stats.Searches[(int)Phase];
            phase.Calls++;

            _start = start;
            _goal = goal;
            _startCell = startCell;
            _startWidth = startCell.CellWidth;
            _startHeight = startCell.CellHeight;
            var goalSlot = _grid.GetSlot(goal.X, goal.Y);
            _goalOccupied = goalSlot != null && !goalSlot.IsEmpty;
            _goalWidth = _goalOccupied ? goalSlot.Room.CellWidth : 1;
            _goalHeight = _goalOccupied ? goalSlot.Room.CellHeight : 1;

            List<PathStep> path;
            if (!GoalReachable())
            {
                phase.Rejected++;
                path = null;
                if (_validateReachability)
                {
                    path = Search(edgeCost, maxExpansions, out _, out _);
                    if (path != null) phase.RejectedSolvable++;
                }
            }
            else
            {
                path = Search(edgeCost, maxExpansions, out int expansions, out bool budgetSpent);
                phase.Expansions += expansions;
                if (path != null)
                {
                    phase.SuccessExpansions += expansions;
                    _stats.Successes.Add(new SearchSample(Phase, Math.Abs(start.X - goal.X) + Math.Abs(start.Y - goal.Y), expansions));
                }
                else
                {
                    phase.Failures++;
                    if (budgetSpent) phase.BudgetSpentFailures++;
                }
            }
            phase.Ticks += Stopwatch.GetTimestamp() - began;
            return path;
        }

        private List<PathStep> Search(EdgeCost edgeCost, int maxExpansions, out int expansions, out bool budgetSpent)
        {
            _nodeIds.Clear();
            _nodes.Clear();
            _bestCost.Clear();
            _parent.Clear();
            _placedCell.Clear();
            _placedAnchor.Clear();
            _toExplore.Clear();

            int startId = AddNode(new Node(_start, _startCell.GetType(), 1, 0));
            _bestCost[startId] = 0.0;
            _toExplore.Enqueue(startId, Heuristic(_start, _goal));

            expansions = 0;
            budgetSpent = false;
            int budget = 0;
            while (_toExplore.Count > 0 && budget++ < maxExpansions)
            {
                expansions++;
                int currentId = _toExplore.Dequeue();
                var current = _nodes[currentId];

                // Goal reached by placing a cell on it (empty goal).
                if (current.Position == _goal)
                {
                    // Cannot end the path on a type that has not satisfied its minimum chain length.
                    if (_minStreakLimits.TryGetValue(current.PrevType, out int minS) && current.Streak < minS)
                        continue;
                    return ReconstructPath(currentId);
                }

                GridRoom prevCell = currentId == startId ? _startCell : _placedCell[currentId];

                // Arrival at an already-occupied goal (e.g. pre-placed door or required room).
                // Valid if any of prev's exits steps onto the goal and the goal accepts the arriving type.
                foreach (var exit in prevCell.Exits)
                {
                    var landing = new Point(current.Position.X + exit.CursorDelta.X, current.Position.Y + exit.CursorDelta.Y);
                    if (landing != _goal) continue;
                    if (!TargetAcceptsArrivalFromExit(exit, current.PrevType)) continue;
                    if (_minStreakLimits.TryGetValue(current.PrevType, out int minA) && current.Streak < minA) continue;
                    return ReconstructPath(currentId);
                }

                StampPath(currentId);
                double currentCost = _bestCost[currentId];
                bool prevStreakUnmet = _minStreakLimits.TryGetValue(current.PrevType, out int minPrev) && current.Streak < minPrev;

                // Every check below is a pure filter, ordered cheapest first.
                foreach (var exit in prevCell.Exits)
                {
                    // Vertical run: counts consecutive vertical moves; horizontal resets to 0.
                    int newVerticalRun = (exit.CursorDelta.Y != 0 && exit.CursorDelta.X == 0) ? current.VerticalRun + 1 : 0;
                    if (newVerticalRun > _maxVerticalRun) continue;

                    var candNext = new Point(current.Position.X + exit.CursorDelta.X, current.Position.Y + exit.CursorDelta.Y);
                    bool isCardinalSource = (exit.CursorDelta.X == 0) ^ (exit.CursorDelta.Y == 0);

                    foreach (var candidate in exit.AllowedNext)
                    {
                        // Streak: same type increments, different resets, exceed-limit skips.
                        // Leaving a type before its minimum streak is satisfied is rejected.
                        var candType = candidate.GetType();
                        bool sameType = candType == current.PrevType;
                        int newStreak = sameType ? current.Streak + 1 : 1;
                        bool hasStreakLimit = _streakLimits.TryGetValue(candType, out int maxStreak);
                        if (hasStreakLimit && newStreak > maxStreak) continue;
                        if (!sameType && prevStreakUnmet) continue;

                        var anchor = new Point(candNext.X + candidate.AnchorOffsetFromCursor.X, candNext.Y + candidate.AnchorOffsetFromCursor.Y);
                        if (!FootprintIsAvailable(candidate, anchor)) continue;

                        // Committed same-type cells extending left and right of the anchor's row count toward
                        // the streak too, so two paths can't stitch their placements into one long chain.
                        if (hasStreakLimit && HorizontalChainLength(candType, anchor, candidate.CellWidth, maxStreak) > maxStreak) continue;

                        // The cost function rejects shafts beside committed shafts; this covers shafts placed
                        // earlier on the same path, which are not committed until the search returns.
                        if (candidate.Type == RoomType.VerticalConnector && PathShaftInAdjacentColumn(anchor)) continue;

                        if (isCardinalSource && !HasMatchingCardinalExit(candidate, exit.CursorDelta)) continue;

                        if (!AntiFusionOk(candidate, anchor)) continue;
                        if (!OpenSidesMatch(candidate, anchor)) continue;

                        long validStart = Stopwatch.GetTimestamp();
                        bool valid = candidate.IsValidPlacement(_grid, anchor, _pending);
                        _stats.ValidPlacementTicks += Stopwatch.GetTimestamp() - validStart;
                        if (!valid) continue;

                        double cost = edgeCost(anchor, candidate);
                        if (double.IsPositiveInfinity(cost)) continue;

                        var neighbor = new Node(candNext, candType, newStreak, newVerticalRun);
                        double newCost = currentCost + cost;

                        if (_nodeIds.TryGetValue(neighbor, out int neighborId))
                        {
                            if (newCost >= _bestCost[neighborId]) continue;
                        }
                        else
                        {
                            neighborId = AddNode(neighbor);
                        }

                        _bestCost[neighborId] = newCost;
                        _parent[neighborId] = currentId;
                        _placedCell[neighborId] = candidate;
                        _placedAnchor[neighborId] = anchor;
                        _toExplore.Enqueue(neighborId, newCost + Heuristic(candNext, _goal));
                    }
                }
            }

            budgetSpent = budget > maxExpansions;
            return null;
        }

        private int AddNode(Node node)
        {
            int id = _nodes.Count;
            _nodeIds[node] = id;
            _nodes.Add(node);
            _bestCost.Add(double.PositiveInfinity);
            _parent.Add(-1);
            _placedCell.Add(null);
            _placedAnchor.Add(default);
            return id;
        }

        private static double Heuristic(Point from, Point to) => Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);

        private int Index(int col, int row) => col * _grid.Rows + row;

        /// <summary>
        /// Loads the path that leads to node <paramref name="id"/> into the pending placements and collects its shafts.
        /// </summary>
        private void StampPath(int id)
        {
            _pending.StartPath();
            _pathShafts.Clear();
            for (int n = id; _parent[n] >= 0; n = _parent[n])
            {
                var cell = _placedCell[n];
                _pending.Add(cell, _placedAnchor[n]);
                if (cell.Type == RoomType.VerticalConnector) _pathShafts.Add(_placedAnchor[n]);
            }
        }

        /// <summary>
        /// True if every square of the footprint is in-bounds, empty, not blocked, and not already on this path.
        /// </summary>
        private bool FootprintIsAvailable(GridRoom candidate, Point anchor)
        {
            for (int sc = 0; sc < candidate.CellWidth; sc++)
            {
                for (int sr = 0; sr < candidate.CellHeight; sr++)
                {
                    int x = anchor.X + sc;
                    int y = anchor.Y + sr;
                    var slot = _grid.GetSlot(x, y);
                    if (slot == null || !slot.IsEmpty) return false;
                    if (_blocked[Index(x, y)]) return false;
                    if (_pending.Contains(x, y)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// True if the candidate exposes a cardinal exit on the face it is entered from, or has no cardinal exits.
        /// Compares signs, not deltas, so a 1-wide cell can dock against a 2-wide source.
        /// </summary>
        private static bool HasMatchingCardinalExit(GridRoom candidate, Point sourceDelta)
        {
            int srcSignX = Math.Sign(sourceDelta.X);
            int srcSignY = Math.Sign(sourceDelta.Y);
            bool hasCardinalExit = false;
            foreach (var ce in candidate.Exits)
            {
                bool ceCardinal = (ce.CursorDelta.X == 0) ^ (ce.CursorDelta.Y == 0);
                if (!ceCardinal) continue;
                hasCardinalExit = true;
                if (Math.Sign(ce.CursorDelta.X) == -srcSignX && Math.Sign(ce.CursorDelta.Y) == -srcSignY) return true;
            }
            return !hasCardinalExit;
        }

        private bool PathShaftInAdjacentColumn(Point anchor)
        {
            foreach (var s in _pathShafts)
                if (Math.Abs(s.X - anchor.X) <= 1) return true;
            return false;
        }

        /// <summary>
        /// Length of the run of committed <paramref name="type"/> cells on the anchor's row that the candidate would join,
        /// counting the candidate as one. Stops counting once it exceeds <paramref name="limit"/>.
        /// </summary>
        private int HorizontalChainLength(Type type, Point anchor, int width, int limit)
        {
            int chain = 1;
            for (int x = anchor.X - 1; x >= 0 && chain <= limit; x--)
            {
                var s = _grid.GetSlot(x, anchor.Y);
                if (s == null || s.IsEmpty || s.Room.GetType() != type) break;
                chain++;
            }
            for (int x = anchor.X + width; x < _grid.Cols && chain <= limit; x++)
            {
                var s = _grid.GetSlot(x, anchor.Y);
                if (s == null || s.IsEmpty || s.Room.GetType() != type) break;
                chain++;
            }
            return chain;
        }

        /// <summary>
        /// True if every external border of the candidate's footprint agrees with its neighbor, on this path or
        /// committed: both open or both closed. Empty neighbors and internal sub-cell edges are ignored.
        /// </summary>
        private bool OpenSidesMatch(GridRoom candidate, Point anchor)
        {
            for (int sc = 0; sc < candidate.CellWidth; sc++)
            {
                for (int sr = 0; sr < candidate.CellHeight; sr++)
                {
                    foreach (var d in Sides)
                    {
                        // Internal seams between sub-cells of the same candidate.
                        int neighborSubCol = sc + d.dx;
                        int neighborSubRow = sr + d.dy;
                        if (neighborSubCol >= 0 && neighborSubCol < candidate.CellWidth && neighborSubRow >= 0 && neighborSubRow < candidate.CellHeight)
                            continue;

                        int gx = anchor.X + sc + d.dx;
                        int gy = anchor.Y + sr + d.dy;

                        // Pending placements first, then committed grid.
                        GridRoom neighbor;
                        int neighborSubX;
                        int neighborSubY;
                        if (_pending.TryGet(gx, gy, out var pendingCell, out var pendingAnchor))
                        {
                            neighbor = pendingCell;
                            neighborSubX = gx - pendingAnchor.X;
                            neighborSubY = gy - pendingAnchor.Y;
                        }
                        else
                        {
                            var slot = _grid.GetSlot(gx, gy);
                            if (slot == null || slot.IsEmpty) continue;
                            neighbor = slot.Room;
                            neighborSubX = slot.SubCol;
                            neighborSubY = slot.SubRow;
                        }

                        bool ourSide = candidate.IsOpenSide(sc, sr, d.side);
                        bool theirSide = neighbor.IsOpenSide(neighborSubX, neighborSubY, d.opposite);
                        if (ourSide != theirSide) return false;
                        if (!ourSide) continue;

                        // Mutual AllowedNeighbors check on top of geometry. When one side is a planner-placed
                        // feature, the other side's whitelist is skipped so standard rooms don't have to list
                        // every feature type, but the feature's own whitelist still gates what can dock against it.
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
            return true;
        }

        /// <summary>
        /// Rejects candidates whose footprint borders a foreign cell: an occupied cell that is not on this path
        /// and not inside the start or goal footprint. Two paths placed side by side would join through shared
        /// open sides into a loop. Cardinal neighbors only, since diagonals don't connect through cell borders.
        /// </summary>
        private bool AntiFusionOk(GridRoom candidate, Point anchor)
        {
            int candW = candidate.CellWidth;
            int candH = candidate.CellHeight;

            for (int sc = 0; sc < candW; sc++)
            {
                for (int sr = 0; sr < candH; sr++)
                {
                    int ix = anchor.X + sc;
                    int iy = anchor.Y + sr;

                    foreach (var d in Sides)
                    {
                        int nx = ix + d.dx;
                        int ny = iy + d.dy;

                        if (nx >= anchor.X && nx < anchor.X + candW && ny >= anchor.Y && ny < anchor.Y + candH) continue;
                        if (InEndpointFootprint(nx, ny)) continue;

                        var nSlot = _grid.GetSlot(nx, ny);
                        if (nSlot == null || nSlot.IsEmpty) continue;
                        if (_pending.Contains(nx, ny)) continue;

                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// True inside the start footprint (the start cell's size at the start point) or the goal footprint
        /// (the occupying room's size at the goal point, or 1x1 for an empty goal).
        /// </summary>
        private bool InEndpointFootprint(int x, int y)
        {
            if (x >= _start.X && x < _start.X + _startWidth && y >= _start.Y && y < _start.Y + _startHeight) return true;
            return x >= _goal.X && x < _goal.X + _goalWidth && y >= _goal.Y && y < _goal.Y + _goalHeight;
        }

        /// <summary>
        /// Flood fill over the cells any path could occupy: in-bounds, empty, not blocked, and with no occupied
        /// cardinal neighbor outside the start and goal footprints. Every cell of every footprint A* can place
        /// passes this filter, each footprint contains its cursor, and consecutive footprints touch, so a false
        /// result means A* cannot reach the goal.
        /// </summary>
        private bool GoalReachable()
        {
            if (_start == _goal) return true;

            _floodId++;
            int head = 0;
            int tail = 0;

            foreach (var exit in _startCell.Exits)
            {
                int x = _start.X + exit.CursorDelta.X;
                int y = _start.Y + exit.CursorDelta.Y;
                if (_goalOccupied && x == _goal.X && y == _goal.Y) return true;
                if (!Passable(x, y)) continue;
                int i = Index(x, y);
                if (_floodMark[i] == _floodId) continue;
                _floodMark[i] = _floodId;
                _floodQueue[tail++] = i;
            }

            while (head < tail)
            {
                int i = _floodQueue[head++];
                int x = i / _grid.Rows;
                int y = i % _grid.Rows;
                if (_goalOccupied ? Math.Max(Math.Abs(x - _goal.X), Math.Abs(y - _goal.Y)) <= 1 : x == _goal.X && y == _goal.Y) return true;

                foreach (var (dx, dy) in Neighbors8)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    if (!Passable(nx, ny)) continue;
                    int ni = Index(nx, ny);
                    if (_floodMark[ni] == _floodId) continue;
                    _floodMark[ni] = _floodId;
                    _floodQueue[tail++] = ni;
                }
            }
            return false;
        }

        private bool Passable(int x, int y)
        {
            if (x < 0 || x >= _grid.Cols || y < 0 || y >= _grid.Rows) return false;
            int i = Index(x, y);
            if (_passableMark[i] == _floodId) return _passable[i];
            _passableMark[i] = _floodId;
            _passable[i] = ComputePassable(x, y, i);
            return _passable[i];
        }

        private bool ComputePassable(int x, int y, int index)
        {
            if (!_grid.GetSlot(x, y).IsEmpty || _blocked[index]) return false;
            foreach (var d in Sides)
            {
                int nx = x + d.dx;
                int ny = y + d.dy;
                var n = _grid.GetSlot(nx, ny);
                if (n == null || n.IsEmpty) continue;
                if (InEndpointFootprint(nx, ny)) continue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// True if the occupied goal accepts the walker's arriving cell type on its facing exit.
        /// </summary>
        private bool TargetAcceptsArrivalFromExit(CellExit walkerExit, Type walkerType)
        {
            if (!_goalOccupied) return false;

            var oppositeDelta = new Point(-walkerExit.CursorDelta.X, -walkerExit.CursorDelta.Y);
            foreach (var targetExit in _grid.GetSlot(_goal.X, _goal.Y).Room.Exits)
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
        /// Walks the parent chain back from the end node, then reverses to placement order.
        /// </summary>
        private List<PathStep> ReconstructPath(int endId)
        {
            var steps = new List<PathStep>();
            for (int n = endId; _parent[n] >= 0; n = _parent[n])
                steps.Add(new PathStep(_placedCell[n], _placedAnchor[n]));
            steps.Reverse();
            return steps;
        }

        /// <summary>
        /// One A* search state: position, last cell type, streak, vertical run.
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

            public bool Equals(Node other) => Position == other.Position && PrevType == other.PrevType && Streak == other.Streak && VerticalRun == other.VerticalRun;
            public override bool Equals(object obj) => obj is Node n && Equals(n);
            public override int GetHashCode() => HashCode.Combine(Position.X, Position.Y, PrevType, Streak, VerticalRun);
        }
    }
}
