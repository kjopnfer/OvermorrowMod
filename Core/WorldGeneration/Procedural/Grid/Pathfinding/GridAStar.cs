using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding
{
    /// <summary>
    /// Directions of movement between rooms, combinable.
    /// </summary>
    [Flags]
    public enum MoveMask
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
    }

    public static class MoveMasks
    {
        /// <summary>
        /// The moves along each nonzero component of <paramref name="direction"/>.
        /// </summary>
        public static MoveMask Toward(Point direction) => (direction.X < 0 ? MoveMask.Left : direction.X > 0 ? MoveMask.Right : MoveMask.None) | (direction.Y < 0 ? MoveMask.Up : direction.Y > 0 ? MoveMask.Down : MoveMask.None);

        /// <summary>
        /// True if <paramref name="delta"/> has a component in a direction of <paramref name="mask"/>.
        /// </summary>
        public static bool Includes(this MoveMask mask, Point delta) => (mask & Toward(delta)) != 0;
    }

    /// <summary>
    /// A* over grid cells with project-specific extensions:
    /// multi-cell footprints (stairs span multiple squares and must not collide
    /// with the grid, blocked zones, or earlier placements in the same path)
    /// and room-to-room routing: the path leaves the start room through one of its exits
    /// and ends against any opening of the placed goal room that accepts the arriving cell.
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

        private readonly List<Point> _goalApproaches = new();

        private Point _start;
        private Point _startAnchor;
        private GridRoom _startCell;
        private Point _goalAnchor;
        private GridRoom _goalRoom;

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
        /// Plans a path between two rooms already placed on the grid.
        /// </summary>
        /// <param name="startAnchor">Top-left cell of the room the path leaves from.</param>
        /// <param name="goalCell">Any cell of the room the path arrives at.</param>
        /// <param name="startCell">The room at <paramref name="startAnchor"/>.</param>
        /// <param name="forbidden">Moves between rooms the path may not make.</param>
        /// <returns>The placed steps in order, or null if the goal is unreachable or the expansion budget runs out.</returns>
        public List<PathStep> FindPath(Point startAnchor, Point goalCell, GridRoom startCell, EdgeCost edgeCost, int maxExpansions = DefaultMaxExpansions, MoveMask forbidden = MoveMask.None)
        {
            long began = Stopwatch.GetTimestamp();
            var phase = _stats.Searches[(int)Phase];
            phase.Calls++;

            _startCell = startCell;
            _startAnchor = startAnchor;
            _start = new Point(startAnchor.X - startCell.AnchorOffsetFromCursor.X, startAnchor.Y - startCell.AnchorOffsetFromCursor.Y);
            var goalSlot = _grid.GetSlot(goalCell.X, goalCell.Y);
            _goalRoom = goalSlot.Room;
            _goalAnchor = new Point(goalCell.X - goalSlot.SubCol, goalCell.Y - goalSlot.SubRow);
            _goalApproaches.Clear();
            foreach (var opening in _goalRoom.Openings)
                _goalApproaches.Add(new Point(_goalAnchor.X + opening.OutsideCell.X, _goalAnchor.Y + opening.OutsideCell.Y));

            List<PathStep> path;
            if (!GoalReachable())
            {
                phase.Rejected++;
                path = null;
                if (_validateReachability)
                {
                    path = Search(edgeCost, maxExpansions, forbidden, out _, out _);
                    if (path != null) phase.RejectedSolvable++;
                }
            }
            else
            {
                path = Search(edgeCost, maxExpansions, forbidden, out int expansions, out bool budgetSpent);
                phase.Expansions += expansions;
                if (path != null)
                {
                    phase.SuccessExpansions += expansions;
                    _stats.Successes.Add(new SearchSample(Phase, (int)Heuristic(_start), expansions));
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

        private List<PathStep> Search(EdgeCost edgeCost, int maxExpansions, MoveMask forbidden, out int expansions, out bool budgetSpent)
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
            _toExplore.Enqueue(startId, Heuristic(_start));

            expansions = 0;
            budgetSpent = false;
            bool filterMoves = forbidden != MoveMask.None;
            int budget = 0;
            while (_toExplore.Count > 0 && budget++ < maxExpansions)
            {
                expansions++;
                int currentId = _toExplore.Dequeue();
                var current = _nodes[currentId];
                GridRoom prevCell = currentId == startId ? _startCell : _placedCell[currentId];

                // A path cannot end on a type that has not satisfied its minimum chain length.
                foreach (var exit in prevCell.Exits)
                {
                    if (filterMoves && forbidden.Includes(exit.CursorDelta)) continue;
                    if (!GoalAcceptsArrival(current.Position, exit, current.PrevType)) continue;
                    if (_minStreakLimits.TryGetValue(current.PrevType, out int minA) && current.Streak < minA) continue;
                    return ReconstructPath(currentId);
                }

                StampPath(currentId);
                double currentCost = _bestCost[currentId];
                bool prevStreakUnmet = _minStreakLimits.TryGetValue(current.PrevType, out int minPrev) && current.Streak < minPrev;

                // Every check below is a pure filter, ordered cheapest first.
                foreach (var exit in prevCell.Exits)
                {
                    if (filterMoves && forbidden.Includes(exit.CursorDelta)) continue;

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
                        if (!FitsVoidColumns(candidate, anchor)) continue;

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
                        _toExplore.Enqueue(neighborId, newCost + Heuristic(candNext));
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

        /// <summary>
        /// Manhattan distance from <paramref name="from"/> to the nearest cell of the goal footprint.
        /// </summary>
        private double Heuristic(Point from)
        {
            int right = _goalAnchor.X + _goalRoom.CellWidth - 1;
            int bottom = _goalAnchor.Y + _goalRoom.CellHeight - 1;
            int dx = from.X < _goalAnchor.X ? _goalAnchor.X - from.X : from.X > right ? from.X - right : 0;
            int dy = from.Y < _goalAnchor.Y ? _goalAnchor.Y - from.Y : from.Y > bottom ? from.Y - bottom : 0;
            return dx + dy;
        }

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
        /// True if the footprint is entirely on void strip columns for a bridge, or entirely off them for anything else.
        /// </summary>
        private bool FitsVoidColumns(GridRoom candidate, Point anchor)
        {
            bool bridge = candidate.Type == RoomType.Bridge;
            for (int sc = 0; sc < candidate.CellWidth; sc++)
                if (_grid.IsVoid(anchor.X + sc) != bridge) return false;
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

                        bool ourSide = candidate.OpensToward(sc, sr, d.side, neighbor);
                        bool theirSide = neighbor.OpensToward(neighborSubX, neighborSubY, d.opposite, candidate);
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
        /// Rejects candidates that border a foreign cell (an occupied cell that is not on this path and not inside
        /// the start or goal footprint) where either side of the shared border is open. Two paths joined through an
        /// open border would form a loop. Borders closed on both sides stay walls, so separate paths may touch there.
        /// Cardinal neighbors only, since diagonals don't connect through cell borders.
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

                        if (candidate.OpensToward(sc, sr, d.side, nSlot.Room)) return false;
                        if (nSlot.Room.OpensToward(nSlot.SubCol, nSlot.SubRow, d.opposite, candidate)) return false;
                    }
                }
            }
            return true;
        }

        private bool InEndpointFootprint(int x, int y) => InFootprint(_startCell, _startAnchor, x, y) || InFootprint(_goalRoom, _goalAnchor, x, y);

        private static bool InFootprint(GridRoom room, Point anchor, int x, int y) => x >= anchor.X && x < anchor.X + room.CellWidth && y >= anchor.Y && y < anchor.Y + room.CellHeight;

        /// <summary>
        /// Flood fill over the cells any path could occupy: in-bounds, empty, not blocked, and with no occupied
        /// cardinal neighbor outside the start and goal footprints whose side facing the cell is open. Every cell of every footprint A* can place
        /// passes this filter, each footprint contains its cursor, consecutive footprints touch, and the last
        /// footprint covers the cell just outside the goal opening it arrives through, so a false result means
        /// A* cannot reach the goal.
        /// </summary>
        private bool GoalReachable()
        {
            _floodId++;
            int head = 0;
            int tail = 0;

            foreach (var exit in _startCell.Exits)
            {
                int x = _start.X + exit.CursorDelta.X;
                int y = _start.Y + exit.CursorDelta.Y;
                if (InFootprint(_goalRoom, _goalAnchor, x, y)) return true;
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
                if (_goalApproaches.Contains(new Point(x, y))) return true;

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
                if (n.Room.OpensToward(n.SubCol, n.SubRow, d.opposite, null)) return false;
            }
            return true;
        }

        /// <summary>
        /// True if <paramref name="exit"/>, taken from <paramref name="cursor"/>, lands on a goal sub-cell whose side facing
        /// the walker is open and accepts <paramref name="walkerType"/>.
        /// </summary>
        private bool GoalAcceptsArrival(Point cursor, CellExit exit, Type walkerType)
        {
            int subCol = cursor.X + exit.CursorDelta.X - _goalAnchor.X;
            int subRow = cursor.Y + exit.CursorDelta.Y - _goalAnchor.Y;
            if (subCol < 0 || subCol >= _goalRoom.CellWidth || subRow < 0 || subRow >= _goalRoom.CellHeight) return false;
            var facing = exit.Side.Opposite();
            if (!_goalRoom.IsOpenSide(subCol, subRow, facing)) return false;
            var accepted = _goalRoom.GetAcceptedNeighbors(subCol, subRow, facing);
            return accepted != null && accepted.Contains(walkerType);
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
