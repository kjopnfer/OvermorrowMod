using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding
{
    /// <summary>
    /// Plans the spine as a sequence of independent A* segments chained
    /// through pre-placed required rooms. Each segment is its own FindPath
    /// call between consecutive subgoals (door or required room). On
    /// failure the offending required room is relocated to its next
    /// candidate position and the affected segments are retried; other
    /// segments stay intact. Replaces the previous all-or-nothing
    /// waypointed FindPath which orphaned required rooms whenever any
    /// single segment clashed.
    /// </summary>
    public static class SpinePlanner
    {
        /// <summary>Result of a planning attempt.</summary>
        public struct PlanResult
        {
            /// <summary>All path steps in spine order. Already committed to the grid on success.</summary>
            public List<PathStep> Steps;

            /// <summary>Anchors of required rooms that ended up on the spine (already committed to the grid).</summary>
            public List<Point> RequiredAnchors;

            public bool Success;
        }

        /// <summary>
        /// Tries to lay out the spine. Doors must already be on the grid
        /// at <paramref name="startDoorPos"/> and <paramref name="endDoorPos"/>.
        /// On success, all spine cells (including the required rooms) are
        /// stamped onto the grid; the caller must NOT re-stamp them.
        /// On failure, every placement made by this call is rolled back,
        /// leaving only the doors.
        /// </summary>
        public static PlanResult TryPlanSpine(
            DungeonGrid grid,
            Point startDoorPos, Point endDoorPos,
            GridRoom startDoorRoom, GridRoom endDoorRoom,
            IReadOnlyList<Func<GridRoom>> requiredRoomFactories,
            double[] elevation, int gridCols, int gridRows,
            EdgeCost costFn, HashSet<Point> blocked,
            IReadOnlyDictionary<Type, int> streakLimits,
            IReadOnlyDictionary<Type, int> minStreakLimits,
            int maxVerticalRun, int edgeBorder, int minDoorDistance,
            int minSubgoalSpacing)
        {
            const int MaxCandidatesPerRoom = 8;
            const int MaxOuterAttempts = 30;

            var rooms = new List<RoomState>(requiredRoomFactories.Count);
            foreach (var factory in requiredRoomFactories)
            {
                // Required rooms are planner anchors. IsFeature lets standard
                // cells dock against them without listing every feature type
                // in their own AllowedNeighbors.
                var prototype = factory();
                prototype.IsFeature = true;
                var cands = GenerateCandidates(prototype, elevation, gridCols, gridRows,
                                               startDoorPos, endDoorPos, blocked, edgeBorder,
                                               minDoorDistance, MaxCandidatesPerRoom);
                rooms.Add(new RoomState
                {
                    Factory = factory,
                    Prototype = prototype,
                    Candidates = cands,
                    Idx = 0,
                    Skipped = cands.Count == 0,
                });
            }

            for (int outer = 0; outer < MaxOuterAttempts; outer++)
            {
                if (!PlaceRequiredRooms(grid, rooms, minSubgoalSpacing))
                {
                    // No required rooms could be placed at all. Try the
                    // door-to-door segment as a final fallback.
                }

                var subgoals = BuildSubgoalList(grid, rooms, startDoorPos, endDoorPos,
                                                startDoorRoom, endDoorRoom);

                var committedSteps = new List<PathStep>();
                int failedSegmentIdx = -1;
                for (int i = 0; i < subgoals.Count - 1; i++)
                {
                    var a = subgoals[i];
                    var b = subgoals[i + 1];
                    var seg = GridAStar.FindPath(
                        grid, a.Pos, b.Pos, a.Room, costFn,
                        blocked: blocked,
                        streakLimits: streakLimits,
                        minStreakLimits: minStreakLimits,
                        maxVerticalRun: maxVerticalRun);
                    if (seg == null) { failedSegmentIdx = i; break; }
                    foreach (var step in seg)
                        grid.Place(step.Cell, step.Anchor.X, step.Anchor.Y, grid.NextGroupId());
                    committedSteps.AddRange(seg);
                }

                if (failedSegmentIdx < 0)
                {
                    var anchors = new List<Point>();
                    foreach (var r in rooms)
                        if (r.IsPlaced) anchors.Add(r.PlacedAnchor);
                    return new PlanResult
                    {
                        Steps = committedSteps,
                        RequiredAnchors = anchors,
                        Success = true,
                    };
                }

                RollbackSteps(grid, committedSteps);

                Subgoal victim = null;
                var aSub = subgoals[failedSegmentIdx];
                var bSub = subgoals[failedSegmentIdx + 1];
                if (!bSub.IsDoor) victim = bSub;
                else if (!aSub.IsDoor) victim = aSub;

                RollbackRequiredRooms(grid, rooms);

                if (victim == null)
                {
                    // Door-to-door segment with no required rooms in
                    // between. Future iterations won't change anything.
                    return new PlanResult { Steps = null, RequiredAnchors = null, Success = false };
                }

                victim.Owner.Idx++;
                if (victim.Owner.Idx >= victim.Owner.Candidates.Count)
                    victim.Owner.Skipped = true;
            }

            RollbackRequiredRooms(grid, rooms);
            return new PlanResult { Steps = null, RequiredAnchors = null, Success = false };
        }

        // Places each non-skipped required room at its current candidate.
        // Advances Idx past candidates that conflict with already-placed
        // rooms or the grid; marks Skipped if all candidates exhausted.
        private static bool PlaceRequiredRooms(DungeonGrid grid, List<RoomState> rooms, int minSubgoalSpacing)
        {
            bool anyPlaced = false;
            foreach (var r in rooms)
            {
                if (r.Skipped) continue;
                while (r.Idx < r.Candidates.Count)
                {
                    var pos = r.Candidates[r.Idx];
                    if (FootprintIsClear(grid, r.Prototype, pos)
                        && !TooCloseToOtherPlaced(rooms, r, pos, minSubgoalSpacing)
                        && r.Prototype.IsValidPlacement(grid, pos))
                    {
                        grid.Place(r.Prototype, pos.X, pos.Y, grid.NextGroupId());
                        r.PlacedAnchor = pos;
                        r.IsPlaced = true;
                        anyPlaced = true;
                        break;
                    }
                    r.Idx++;
                }
                if (!r.IsPlaced) r.Skipped = true;
            }
            return anyPlaced;
        }

        private static bool TooCloseToOtherPlaced(List<RoomState> rooms, RoomState self, Point pos, int minSpacing)
        {
            foreach (var other in rooms)
            {
                if (other == self) continue;
                if (!other.IsPlaced) continue;
                int dx = Math.Abs(pos.X - other.PlacedAnchor.X);
                int dy = Math.Abs(pos.Y - other.PlacedAnchor.Y);
                if (Math.Max(dx, dy) < minSpacing) return true;
            }
            return false;
        }

        private static List<Subgoal> BuildSubgoalList(
            DungeonGrid grid, List<RoomState> rooms,
            Point startDoorPos, Point endDoorPos,
            GridRoom startDoorRoom, GridRoom endDoorRoom)
        {
            var list = new List<Subgoal>
            {
                new Subgoal { Pos = startDoorPos, Room = startDoorRoom, IsDoor = true },
            };
            foreach (var r in rooms)
            {
                if (!r.IsPlaced) continue;
                var slot = grid.GetSlot(r.PlacedAnchor.X, r.PlacedAnchor.Y);
                list.Add(new Subgoal { Pos = r.PlacedAnchor, Room = slot.Room, IsDoor = false, Owner = r });
            }
            list.Add(new Subgoal { Pos = endDoorPos, Room = endDoorRoom, IsDoor = true });
            list.Sort((a, b) => a.Pos.X.CompareTo(b.Pos.X));
            return list;
        }

        // Returns positions on the elevation curve, sorted by curve fit.
        // Excludes positions inside the border ring or too close to a door.
        private static List<Point> GenerateCandidates(
            GridRoom prototype, double[] elevation, int gridCols, int gridRows,
            Point startDoor, Point endDoor, HashSet<Point> blocked, int edgeBorder,
            int minDoorDistance, int maxCount)
        {
            int w = prototype.CellWidth;
            int h = prototype.CellHeight;
            int playableLeft = edgeBorder + 2;
            int playableRight = gridCols - 1 - edgeBorder - w - 1;
            if (playableLeft > playableRight) return new List<Point>();

            var ranked = new List<(Point pos, double dev)>();
            for (int col = playableLeft; col <= playableRight; col++)
            {
                int row = ClampRow((int)Math.Round(elevation[col]), gridRows, edgeBorder);
                if (row + h - 1 >= gridRows - edgeBorder) continue;
                if (row < edgeBorder) continue;

                bool tooCloseToDoor = false;
                for (int sc = 0; sc < w && !tooCloseToDoor; sc++)
                {
                    for (int sr = 0; sr < h && !tooCloseToDoor; sr++)
                    {
                        int cc = col + sc;
                        int cr = row + sr;
                        int distStart = Math.Max(Math.Abs(cc - startDoor.X), Math.Abs(cr - startDoor.Y));
                        int distEnd = Math.Max(Math.Abs(cc - endDoor.X), Math.Abs(cr - endDoor.Y));
                        if (distStart < minDoorDistance || distEnd < minDoorDistance)
                            tooCloseToDoor = true;
                    }
                }
                if (tooCloseToDoor) continue;

                bool blockedHit = false;
                for (int sc = 0; sc < w && !blockedHit; sc++)
                    for (int sr = 0; sr < h && !blockedHit; sr++)
                        if (blocked.Contains(new Point(col + sc, row + sr))) blockedHit = true;
                if (blockedHit) continue;

                double dev = Math.Abs(row - elevation[col]);
                ranked.Add((new Point(col, row), dev));
            }

            ranked.Sort((a, b) => a.dev.CompareTo(b.dev));

            var result = new List<Point>();
            for (int i = 0; i < ranked.Count && result.Count < maxCount; i++)
                result.Add(ranked[i].pos);
            return result;
        }

        private static int ClampRow(int row, int gridRows, int edgeBorder)
        {
            int min = Math.Max(edgeBorder, 1);
            int max = Math.Min(gridRows - 1 - edgeBorder, gridRows - 2);
            return Math.Max(min, Math.Min(max, row));
        }

        private static bool FootprintIsClear(DungeonGrid grid, GridRoom prototype, Point anchor)
        {
            for (int sc = 0; sc < prototype.CellWidth; sc++)
            {
                for (int sr = 0; sr < prototype.CellHeight; sr++)
                {
                    var slot = grid.GetSlot(anchor.X + sc, anchor.Y + sr);
                    if (slot == null || !slot.IsEmpty) return false;
                }
            }
            return true;
        }

        private static void RollbackSteps(DungeonGrid grid, List<PathStep> steps)
        {
            foreach (var step in steps)
                ClearFootprint(grid, step.Cell, step.Anchor);
        }

        private static void RollbackRequiredRooms(DungeonGrid grid, List<RoomState> rooms)
        {
            foreach (var r in rooms)
            {
                if (!r.IsPlaced) continue;
                ClearFootprint(grid, r.Prototype, r.PlacedAnchor);
                r.IsPlaced = false;
            }
        }

        private static void ClearFootprint(DungeonGrid grid, GridRoom room, Point anchor)
        {
            for (int sc = 0; sc < room.CellWidth; sc++)
            {
                for (int sr = 0; sr < room.CellHeight; sr++)
                {
                    var s = grid.GetSlot(anchor.X + sc, anchor.Y + sr);
                    if (s == null) continue;
                    s.Room = null;
                    s.SubCol = 0;
                    s.SubRow = 0;
                    s.GroupId = 0;
                }
            }
        }

        private class RoomState
        {
            public Func<GridRoom> Factory;
            public GridRoom Prototype;
            public List<Point> Candidates;
            public int Idx;
            public bool Skipped;
            public bool IsPlaced;
            public Point PlacedAnchor;
        }

        private class Subgoal
        {
            public Point Pos;
            public GridRoom Room;
            public bool IsDoor;
            public RoomState Owner;
        }
    }
}
