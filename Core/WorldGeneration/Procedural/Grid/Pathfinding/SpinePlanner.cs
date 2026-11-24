using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding
{
    /// <summary>
    /// Plans the spine by marching from the start door to the end door: the required rooms get evenly spaced targets
    /// along the march in a random order, the march places each near its target, and an A* leg links it to the previous
    /// room.
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

        private const int OrderShuffles = 8;
        private const int OrderTries = 3;
        private const int LegMinExpansions = 400;
        private const int LegExpansionsPerCell = 60;

        /// <summary>
        /// Lays out the spine. Doors must already be on the grid at <paramref name="startDoorPos"/> and
        /// <paramref name="endDoorPos"/>, with the start door before the end door along <paramref name="march"/>. On
        /// failure every placement made by this call is rolled back, leaving only the doors.
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
        public static PlanResult TryPlanSpine(DungeonGrid grid, GridAStar search, SpineMarch march, Point startDoorPos, Point endDoorPos, GridRoom startDoorRoom, IReadOnlyList<Func<GridRoom>> requiredRoomFactories, EdgeCost costFn, HashSet<Point> blocked, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor, MoveMask forbidden, bool skipUnlinkable, Random rand, Action<string> log)
        {
            var rooms = requiredRoomFactories.Select(factory => (Factory: factory, Prototype: Feature(factory))).ToList();
            for (int order = 0; order < OrderTries; order++)
            {
                List<int> targets = null;
                for (int i = 0; i < OrderShuffles; i++)
                {
                    Shuffle(rooms, rand);
                    targets = Targets(rooms.Select(r => march.Extent(r.Prototype)).ToList(), march.Along(startDoorPos), march.Along(endDoorPos));
                    if (Enumerable.Range(0, rooms.Count).All(r => march.TargetAllowed(grid, rooms[r].Prototype, targets[r], edgeBorder, allowAnchor))) break;
                }

                var result = TryMarch(grid, search, march, startDoorPos, endDoorPos, startDoorRoom, rooms, targets, costFn, blocked, edgeBorder, allowAnchor, forbidden, skipUnlinkable && order == OrderTries - 1, log);
                if (result.HasValue) return result.Value;
            }
            return new PlanResult { Success = false };
        }

        /// <summary>
        /// Places <paramref name="rooms"/> in order at their <paramref name="targets"/>, linking each to the one before,
        /// then links the last to the end door.
        /// </summary>
        /// <returns>
        /// The spine, or null with the grid left as it was.
        /// </returns>
        private static PlanResult? TryMarch(DungeonGrid grid, GridAStar search, SpineMarch march, Point startDoorPos, Point endDoorPos, GridRoom startDoorRoom, List<(Func<GridRoom> Factory, GridRoom Prototype)> rooms, List<int> targets, EdgeCost costFn, HashSet<Point> blocked, int edgeBorder, Func<GridRoom, Point, bool> allowAnchor, MoveMask forbidden, bool skipUnlinkable, Action<string> log)
        {
            var steps = new List<PathStep>();
            var anchors = new List<Point>();
            var placedRooms = new List<(GridRoom Room, Point Anchor)>();
            Point prevAnchor = startDoorPos;
            GridRoom prevRoom = startDoorRoom;

            for (int r = 0; r < rooms.Count; r++)
            {
                var (factory, prototype) = rooms[r];
                bool linked = false;
                foreach (var pos in march.Candidates(grid, prototype, r, targets[r], prevAnchor, prevRoom, endDoorPos, r == rooms.Count - 1, blocked, edgeBorder, allowAnchor))
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
                    linked = true;
                    break;
                }

                if (!linked)
                {
                    log?.Invoke($"SpinePlanner: no linked spot for {prototype.GetType().Name} near {targets[r]} along the march.");
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
        /// Positions along the march that spread rooms of the given extents evenly between the two doors, in order.
        /// </summary>
        private static List<int> Targets(List<int> extents, int start, int end)
        {
            double gap = (double)(end - start - 1 - extents.Sum()) / (extents.Count + 1);
            var targets = new List<int>(extents.Count);
            int extentBefore = 0;
            for (int i = 0; i < extents.Count; i++)
            {
                targets.Add(start + 1 + (int)Math.Round(gap * (i + 1)) + extentBefore);
                extentBefore += extents[i];
            }
            return targets;
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
