using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>A*-based dungeon generator. See inline phase headers in Build().</summary>
    public static class GridGenerator
    {
        private const int BranchCount = 12;
        private const int RetriesPerBranchSlot = 5;
        private const int MinBranchSpan = 8;     // critical-path indices apart, minimum
        private const int MinBranchDepth = 3;
        private const int MaxBranchDepth = 5;    // depth = MinBranchDepth..MaxBranchDepth (rows below spine)

        // Caps the visual height of any shaft chain (including bookshelf landings between shafts).
        private const int MaxVerticalRun = 2;

        // Width of the un-buildable border ring. Doors live at its inner edge.
        private const int EdgeBorder = 1;

        private const int RepairExtensionBudget = 6;

        // Per-type A* weight. <1 = preferred, >1 = avoided. Default 1.0.
        private static readonly Dictionary<Type, double> TypeWeights = new()
        {
            [typeof(ShaftCell)]       = 1.4,
            [typeof(DescendingStair)] = 0.7,
            [typeof(AscendingStair)]  = 0.7,
            [typeof(FireplaceRoom)] = 1.5,
            [typeof(LoungeRoom)] = 0.3,
            [typeof(CombatRoom)] = 0.7
        };

        // Max consecutive runs. Shafts use MaxVerticalRun instead.
        private static readonly Dictionary<Type, int> StreakLimits = new()
        {
            [typeof(BookshelfCell)] = 4,
            [typeof(CorridorCell)]  = 5,
            [typeof(FireplaceRoom)] = 1
        };

        // Minimum consecutive runs. Once a streak of this type starts, A* must
        // place at least this many before transitioning to another type.
        private static readonly Dictionary<Type, int> MinStreakLimits = new()
        {
            [typeof(BookshelfCell)] = 2
        };

        // Rooms guaranteed to appear in every dungeon. Each entry is pre-placed
        // before spine planning and replaces the closest random spine waypoint
        // so the spine A* must route through it.
        private static readonly List<Func<GridRoom>> RequiredRooms = new()
        {
            () => new FireplaceRoom(),
            () => new CombatRoom(),
        };

        public static void Build(
            Point worldOrigin,
            int gridCols,
            int gridRows,
            List<GridRoom> cellPool,
            int fillTileType,
            int liningTileType,
            Random rand)
        {
            int margin = DungeonGrid.HorizontalPadding;
            var gridOrigin = new Point(worldOrigin.X + margin, worldOrigin.Y + margin);
            var grid = new DungeonGrid(gridCols, gridRows, gridOrigin);

            int totalWidth = gridCols * DungeonGrid.HorizontalSpacing + DungeonGrid.CellTileWidth + margin * 2;
            int totalHeight = gridRows * DungeonGrid.VerticalSpacing + DungeonGrid.CellTileHeight + margin * 2;
            ushort fill = (ushort)fillTileType;

            for (int x = 0; x < totalWidth; x++)
                for (int y = 0; y < totalHeight; y++)
                    WorldGenUtils.PlaceTile(worldOrigin.X + x, worldOrigin.Y + y, fill);

            // ─── Phase 1: critical path (planned by A*) ──────────────────────
            // Door rows hover near baseRow so the spine reads mostly flat
            // instead of zigzagging end to end.
            int doorRowMin = gridRows / 3;
            int doorRowMax = (gridRows * 2) / 3;
            const int MaxDoorRowOffset = 2;

            // 1D elevation curve over columns. Sampled from OpenSimplex noise
            // and remapped to a row band centered on baseRow. The cost
            // function adds a penalty proportional to deviation from it, so
            // A* is pulled toward the curve rather than picking the cheapest
            // flat shortcut. Endpoints are pinned to the actual door rows.
            const int ElevationAmplitude = 5;
            const int MinCurveSpan = 4;
            const int MaxNoiseRetries = 20;
            // Spine retry: if the planned spine ends up flat (small row range)
            // we re-roll noise and try again.
            const int MinSpineSpan = 5;
            const int MaxSpinePlanAttempts = 8;
            // Build retry: if no spine attempt produced a non-orphaning,
            // sufficiently varied path, restart Phase 1 with new door rows.
            const int MaxBuildRetries = 5;

            // Required rooms must sit far enough from either door that the
            // spine A* has room to climb/descend to reach them.
            const int MinDoorDistance = 6;
            // Required rooms must sit apart from each other so each
            // segment has room to satisfy bookshelf min-streak (≥2)
            // before docking. With CombatRoom now 3 wide, two adjacent
            // required rooms need anchor distance ≥ 5 to leave 2 cells
            // of bookshelf landing between them.
            const int MinSubgoalSpacing = 5;

            // Outer-loop retry: Phase 1 + Phase 2 together are an attempt.
            // After Phase 2 we count actual anchors on the grid and accept
            // only if the dungeon ended up densely populated. This is the
            // only signal that reliably distinguishes good runs from bad
            // ones; pre-Phase-2 metrics (spine bookshelf count, free
            // vertical band) all proved tautological in practice.
            const int MaxFullAttempts = 5;
            const int MinTotalAnchors = 60;

            var borderBlocked = BuildBorderBlockedSet(gridCols, gridRows);

            // Elevation pressure: candidates far from the target curve cost
            // more so A* drifts toward the curve. Spine pays the full
            // multiplier so the main run follows the curve. Branches pay a
            // softer multiplier so descents don't have to fight the curve
            // on the climb back up to the spine.
            const double SpineElevationPenalty = 0.4;
            const double BranchElevationPenalty = 0.1;

            // State that survives across attempts so the final accepted
            // configuration is available after the loops exit.
            int baseRow = 0;
            int startRow = 0, endRow = 0;
            Point start = default, endTarget = default;
            DoorRoom startDoor = null;
            double[] elevation = null;
            List<Point> requiredAnchors = null;
            List<PathStep> spineSteps = null;
            EdgeCost spineCostFn = null;
            EdgeCost branchCostFn = null;
            double[,] noiseField = null;
            bool buildAccepted = false;

            // Best-attempt tracking: every spine attempt scores its grid
            // state. The highest-scoring snapshot is restored after the
            // build retry loop so we never end up with the LAST attempt
            // when an earlier one was better.
            int bestScore = int.MinValue;
            (GridRoom room, int subCol, int subRow, int groupId)[,] bestGridSnapshot = null;
            List<PathStep> bestSpineSteps = null;
            double[] bestElevation = null;
            List<Point> bestRequiredAnchors = null;
            int bestBaseRow = 0;
            Point bestStart = default, bestEndTarget = default;
            int bestStartRow = 0, bestEndRow = 0;

            // Outer attempt: full Phase 1 + Phase 2 sequence, judged by
            // post-Phase-2 anchor count. The only metric that empirically
            // separates good runs from bad ones; everything pre-Phase-2
            // turned out tautological.
            bool fullAccepted = false;
            int bestFullAnchors = -1;
            int bestFullBranchesPlaced = 0;
            (GridRoom room, int subCol, int subRow, int groupId)[,] bestFullGridSnapshot = null;
            List<PathStep> bestFullSpineSteps = null;
            List<Point> bestFullRequiredAnchors = null;
            double[] bestFullElevation = null;
            int bestFullBaseRow = 0;
            Point bestFullStart = default, bestFullEndTarget = default;
            int bestFullStartRow = 0, bestFullEndRow = 0;
            int finalBranchesPlaced = 0;
            int finalAnchorsCount = 0;

            for (int fullAttempt = 0; fullAttempt < MaxFullAttempts && !fullAccepted; fullAttempt++)
            {
                // Wipe everything from the previous full attempt so Phase 2
                // cells from the prior run don't survive into this one.
                if (fullAttempt > 0)
                {
                    for (int c = 0; c < gridCols; c++)
                    {
                        for (int r = 0; r < gridRows; r++)
                        {
                            var s = grid.GetSlot(c, r);
                            if (s == null) continue;
                            s.Room = null;
                            s.SubCol = 0;
                            s.SubRow = 0;
                            s.GroupId = 0;
                        }
                    }
                }

                // Reset internal state populated by the Phase 1 loops.
                buildAccepted = false;
                bestScore = int.MinValue;
                bestGridSnapshot = null;
                bestSpineSteps = null;
                bestElevation = null;
                bestRequiredAnchors = null;
                spineSteps = null;
                requiredAnchors = null;

            for (int buildAttempt = 0; buildAttempt < MaxBuildRetries && !buildAccepted; buildAttempt++)
            {
                // Clear every grid slot before re-attempting Phase 1 so the
                // new door positions and required-room placements aren't
                // colliding with leftovers from the previous attempt.
                if (buildAttempt > 0)
                {
                    for (int c = 0; c < gridCols; c++)
                    {
                        for (int r = 0; r < gridRows; r++)
                        {
                            var s = grid.GetSlot(c, r);
                            if (s == null) continue;
                            s.Room = null;
                            s.SubCol = 0;
                            s.SubRow = 0;
                            s.GroupId = 0;
                        }
                    }
                    spineSteps = null;
                    requiredAnchors = null;
                }

                baseRow = rand.Next(doorRowMin, doorRowMax + 1);
                startRow = ClampRow(baseRow + rand.Next(-MaxDoorRowOffset, MaxDoorRowOffset + 1), gridRows);
                endRow = ClampRow(baseRow + rand.Next(-MaxDoorRowOffset, MaxDoorRowOffset + 1), gridRows);
                start = new Point(EdgeBorder, startRow);
                endTarget = new Point(gridCols - 1 - EdgeBorder, endRow);

                startDoor = new DoorRoom { IsFeature = true };
                grid.Place(startDoor, start.X, start.Y, grid.NextGroupId());
                var endDoor = new DoorRoom { IsFeature = true };
                grid.Place(endDoor, endTarget.X, endTarget.Y, grid.NextGroupId());

                for (int spineAttempt = 0; spineAttempt < MaxSpinePlanAttempts; spineAttempt++)
                {
                    // Wipe leftovers from the previous spine attempt: path
                    // steps and required rooms. Doors are reused.
                    if (spineSteps != null)
                    {
                        foreach (var step in spineSteps)
                        {
                            for (int sc = 0; sc < step.Cell.CellWidth; sc++)
                                for (int sr = 0; sr < step.Cell.CellHeight; sr++)
                                {
                                    var s = grid.GetSlot(step.Anchor.X + sc, step.Anchor.Y + sr);
                                    if (s == null) continue;
                                    s.Room = null;
                                    s.SubCol = 0;
                                    s.SubRow = 0;
                                    s.GroupId = 0;
                                }
                        }
                        spineSteps = null;
                    }
                    if (requiredAnchors != null)
                    {
                        foreach (var anchor in requiredAnchors)
                        {
                            var slot = grid.GetSlot(anchor.X, anchor.Y);
                            if (slot == null || slot.IsEmpty) continue;
                            var room = slot.Room;
                            for (int sc = 0; sc < room.CellWidth; sc++)
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
                        requiredAnchors = null;
                    }

                    // Re-roll the elevation noise until the curve spans enough
                    // rows. Cheap (a few microseconds per retry) so we don't
                    // bother bailing early; the cap is just a safety net.
                    for (int retry = 0; retry < MaxNoiseRetries; retry++)
                    {
                        var fnElev = new FastNoiseLite(rand.Next());
                        fnElev.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
                        fnElev.SetFrequency(0.10f);
                        elevation = new double[gridCols];
                        for (int c = 0; c < gridCols; c++)
                        {
                            float n = fnElev.GetNoise(c, 0f); // -1..1
                            elevation[c] = baseRow + n * ElevationAmplitude;
                        }
                        elevation[start.X] = startRow;
                        elevation[endTarget.X] = endRow;

                        double cMin = double.MaxValue, cMax = double.MinValue;
                        for (int c = 0; c < gridCols; c++)
                        {
                            if (elevation[c] < cMin) cMin = elevation[c];
                            if (elevation[c] > cMax) cMax = elevation[c];
                        }
                        if (cMax - cMin >= MinCurveSpan) break;
                    }

                    noiseField = PathfindingCost.BuildSimplexNoiseField(grid.Cols, grid.Rows, rand.Next());
                    EdgeCost noiseCostFn = PathfindingCost.FromNoise(noiseField, TypeWeights);

                    EdgeCost shaftAdjacencyAwareCost = (anchor, candidate) =>
                    {
                        if (candidate is ShaftCell
                            && (ColumnContainsShaft(grid, anchor.X - 1)
                             || ColumnContainsShaft(grid, anchor.X)
                             || ColumnContainsShaft(grid, anchor.X + 1)))
                        {
                            return double.PositiveInfinity;
                        }
                        return noiseCostFn(anchor, candidate);
                    };

                    // Capture the current elevation array in the lambda so
                    // each retry sees its own curve.
                    double[] capturedElevation = elevation;
                    EdgeCost MakeElevationAware(double multiplier) => (anchor, candidate) =>
                    {
                        double baseCost = shaftAdjacencyAwareCost(anchor, candidate);
                        if (double.IsPositiveInfinity(baseCost)) return baseCost;
                        int colSafe = anchor.X < 0 ? 0 : (anchor.X >= gridCols ? gridCols - 1 : anchor.X);
                        double dev = System.Math.Abs(anchor.Y - capturedElevation[colSafe]);
                        return baseCost + dev * multiplier;
                    };

                    spineCostFn = MakeElevationAware(SpineElevationPenalty);
                    branchCostFn = MakeElevationAware(BranchElevationPenalty);

                    bool isFinalSpineAttempt = (spineAttempt == MaxSpinePlanAttempts - 1);

                    // SpinePlanner runs per-segment A* between subgoals
                    // (doors + required rooms), retrying with relocated
                    // required-room candidates on segment failure. On
                    // success the plan is already stamped onto the grid.
                    var plan = SpinePlanner.TryPlanSpine(
                        grid, start, endTarget, startDoor, endDoor,
                        RequiredRooms, elevation, gridCols, gridRows,
                        spineCostFn, borderBlocked, StreakLimits, MinStreakLimits,
                        MaxVerticalRun, EdgeBorder, MinDoorDistance, MinSubgoalSpacing);

                    spineSteps = plan.Steps;
                    requiredAnchors = plan.RequiredAnchors ?? new List<Point>();

                    if (spineSteps == null) continue;

                    int minR = startRow, maxR = startRow;
                    foreach (var step in spineSteps)
                    {
                        if (step.Anchor.Y < minR) minR = step.Anchor.Y;
                        if (step.Anchor.Y > maxR) maxR = step.Anchor.Y;
                    }
                    if (endRow < minR) minR = endRow;
                    if (endRow > maxR) maxR = endRow;
                    bool spanOk = (maxR - minR) >= MinSpineSpan;

                    int missingRequiredCount = RequiredRooms.Count - requiredAnchors.Count;
                    if (missingRequiredCount < 0) missingRequiredCount = 0;

                    int totalAnchors = 0;
                    for (int c = 0; c < gridCols; c++)
                    {
                        for (int r = 0; r < gridRows; r++)
                        {
                            var s = grid.GetSlot(c, r);
                            if (s == null || s.IsEmpty) continue;
                            if (s.SubCol == 0 && s.SubRow == 0) totalAnchors++;
                        }
                    }
                    int spineSpanRows = maxR - minR;

                    int score = 10 * spineSpanRows + totalAnchors;
                    score -= 10000 * missingRequiredCount;
                    if (spineSpanRows < MinSpineSpan) score -= 5000;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestGridSnapshot = new (GridRoom, int, int, int)[gridCols, gridRows];
                        for (int c = 0; c < gridCols; c++)
                        {
                            for (int r = 0; r < gridRows; r++)
                            {
                                var s = grid.GetSlot(c, r);
                                bestGridSnapshot[c, r] = (s.Room, s.SubCol, s.SubRow, s.GroupId);
                            }
                        }
                        bestSpineSteps = new List<PathStep>(spineSteps);
                        bestElevation = (double[])elevation.Clone();
                        bestRequiredAnchors = new List<Point>(requiredAnchors);
                        bestBaseRow = baseRow;
                        bestStart = start;
                        bestEndTarget = endTarget;
                        bestStartRow = startRow;
                        bestEndRow = endRow;
                    }

                    if (spanOk && missingRequiredCount == 0) { buildAccepted = true; break; }
                    if (isFinalSpineAttempt) break;
                }
            }

            // Restore the best snapshot if the final attempt wasn't the best
            // one (or wasn't acceptable). Re-applies grid state, spineSteps,
            // and Phase-1-derived state so Phase 2/3 sees the best configuration.
            if (bestGridSnapshot != null && !buildAccepted)
            {
                for (int c = 0; c < gridCols; c++)
                {
                    for (int r = 0; r < gridRows; r++)
                    {
                        var s = grid.GetSlot(c, r);
                        if (s == null) continue;
                        var (room, sc, sr, gid) = bestGridSnapshot[c, r];
                        s.Room = room;
                        s.SubCol = sc;
                        s.SubRow = sr;
                        s.GroupId = gid;
                    }
                }
                spineSteps = bestSpineSteps;
                elevation = bestElevation;
                requiredAnchors = bestRequiredAnchors;
                baseRow = bestBaseRow;
                start = bestStart;
                endTarget = bestEndTarget;
                startRow = bestStartRow;
                endRow = bestEndRow;

                // Rebuild cost functions against the restored elevation curve.
                noiseField = PathfindingCost.BuildSimplexNoiseField(grid.Cols, grid.Rows, rand.Next());
                EdgeCost noiseCostFn = PathfindingCost.FromNoise(noiseField, TypeWeights);
                EdgeCost shaftAdjacencyAwareCost = (anchor, candidate) =>
                {
                    if (candidate is ShaftCell
                        && (ColumnContainsShaft(grid, anchor.X - 1)
                         || ColumnContainsShaft(grid, anchor.X)
                         || ColumnContainsShaft(grid, anchor.X + 1)))
                    {
                        return double.PositiveInfinity;
                    }
                    return noiseCostFn(anchor, candidate);
                };
                double[] capturedElev = elevation;
                EdgeCost MakeElev(double mult) => (anchor, candidate) =>
                {
                    double baseCost = shaftAdjacencyAwareCost(anchor, candidate);
                    if (double.IsPositiveInfinity(baseCost)) return baseCost;
                    int colSafe = anchor.X < 0 ? 0 : (anchor.X >= gridCols ? gridCols - 1 : anchor.X);
                    double dev = System.Math.Abs(anchor.Y - capturedElev[colSafe]);
                    return baseCost + dev * mult;
                };
                spineCostFn = MakeElev(SpineElevationPenalty);
                branchCostFn = MakeElev(BranchElevationPenalty);
            }

            // Spine cells are already on the grid (SpinePlanner stamps as it
            // plans). Build criticalPath from spineSteps anchors so Phase 2
            // branches have the spine geometry to attach to.
            var criticalPath = new List<Point> { start };
            if (spineSteps != null)
            {
                foreach (var step in spineSteps)
                    criticalPath.Add(step.Anchor);
            }

            // ─── Phase 1.5: branch-combat anchor ─────────────────────────────
            // Pre-place a CombatRoom in a clean spot below the spine so one
            // Phase 2 branch can be deterministically routed through it.
            // Without this, the second combat is purely emergent (A* getting
            // lucky enough to slot one inside a U-shape) and shows up only
            // sometimes. The pre-placed instance is flagged IsFeature=true
            // so neighbors don't have to whitelist it.
            var branchCombat = TryPrePlaceBranchCombat(
                grid, criticalPath, gridCols, gridRows, borderBlocked, rand);

            // ─── Phase 2: branches (loops) ───────────────────────────────────
            // Each slot retries with different endpoints since A* can fail on unlucky pairs.
            int branchesPlaced = 0;
            // Try to route one branch through the pre-placed branch combat
            // first. Counts toward the BranchCount budget on success.
            // Rollback the combat if no route reaches it; an orphan combat
            // would split the connected component.
            if (branchCombat.HasValue)
            {
                bool linked = criticalPath.Count > MinBranchSpan + 1
                    && TryPlaceBranchThroughCombat(
                        grid, criticalPath, branchCombat.Value, branchCostFn,
                        StreakLimits, borderBlocked);
                if (linked)
                {
                    branchesPlaced++;
                }
                else
                {
                    ClearFootprint(grid, branchCombat.Value.room, branchCombat.Value.anchor);
                }
            }
            for (int slot = 0; slot < BranchCount; slot++)
            {
                if (criticalPath.Count <= MinBranchSpan + 1) break;
                for (int attempt = 0; attempt < RetriesPerBranchSlot; attempt++)
                {
                    if (TryPlaceBranchViaAStar(grid, criticalPath, branchCostFn, StreakLimits, borderBlocked, rand, relaxed: false))
                    {
                        branchesPlaced++;
                        break;
                    }
                }
            }

            // Guarantee at least one branch via max-span fallback.
            if (branchesPlaced == 0 && criticalPath.Count > MinBranchSpan + 1)
            {
                if (TryPlaceMaxSpanBranch(grid, criticalPath, branchCostFn, StreakLimits, borderBlocked, rand, relaxed: false)
                    || TryPlaceMaxSpanBranch(grid, criticalPath, branchCostFn, StreakLimits, borderBlocked, rand, relaxed: true))
                {
                    branchesPlaced++;
                }
            }

                // Count anchors across the whole grid. This is the gate
                // that decides whether the full attempt is good enough.
                int totalAnchorsAfterPhase2 = 0;
                for (int c = 0; c < gridCols; c++)
                {
                    for (int r = 0; r < gridRows; r++)
                    {
                        var s = grid.GetSlot(c, r);
                        if (s == null || s.IsEmpty) continue;
                        if (s.SubCol == 0 && s.SubRow == 0) totalAnchorsAfterPhase2++;
                    }
                }

                Terraria.ModLoader.Logging.PublicLogger.Info(
                    $"OvermorrowDungeon fullAttempt {fullAttempt}: branches={branchesPlaced}, anchors={totalAnchorsAfterPhase2} (need {MinTotalAnchors}).");

                if (totalAnchorsAfterPhase2 >= MinTotalAnchors)
                {
                    fullAccepted = true;
                    finalBranchesPlaced = branchesPlaced;
                    finalAnchorsCount = totalAnchorsAfterPhase2;
                    break;
                }

                // Snapshot if this is the best attempt so far. Used as a
                // fallback if every attempt fails the gate.
                if (totalAnchorsAfterPhase2 > bestFullAnchors)
                {
                    bestFullAnchors = totalAnchorsAfterPhase2;
                    bestFullBranchesPlaced = branchesPlaced;
                    bestFullGridSnapshot = new (GridRoom, int, int, int)[gridCols, gridRows];
                    for (int c = 0; c < gridCols; c++)
                    {
                        for (int r = 0; r < gridRows; r++)
                        {
                            var s = grid.GetSlot(c, r);
                            bestFullGridSnapshot[c, r] = (s.Room, s.SubCol, s.SubRow, s.GroupId);
                        }
                    }
                    bestFullSpineSteps = spineSteps != null ? new List<PathStep>(spineSteps) : null;
                    bestFullRequiredAnchors = requiredAnchors != null ? new List<Point>(requiredAnchors) : null;
                    bestFullElevation = elevation != null ? (double[])elevation.Clone() : null;
                    bestFullBaseRow = baseRow;
                    bestFullStart = start;
                    bestFullEndTarget = endTarget;
                    bestFullStartRow = startRow;
                    bestFullEndRow = endRow;
                }
            }

            // Restore the best full attempt if no run cleared the gate.
            if (!fullAccepted && bestFullGridSnapshot != null)
            {
                for (int c = 0; c < gridCols; c++)
                {
                    for (int r = 0; r < gridRows; r++)
                    {
                        var s = grid.GetSlot(c, r);
                        if (s == null) continue;
                        var (room, sc, sr, gid) = bestFullGridSnapshot[c, r];
                        s.Room = room;
                        s.SubCol = sc;
                        s.SubRow = sr;
                        s.GroupId = gid;
                    }
                }
                spineSteps = bestFullSpineSteps;
                elevation = bestFullElevation;
                requiredAnchors = bestFullRequiredAnchors;
                baseRow = bestFullBaseRow;
                start = bestFullStart;
                endTarget = bestFullEndTarget;
                startRow = bestFullStartRow;
                endRow = bestFullEndRow;
                finalBranchesPlaced = bestFullBranchesPlaced;
                finalAnchorsCount = bestFullAnchors;

                // Rebuild branch cost function so downstream phases (2.5 and
                // anything else that takes branchCostFn) have a working
                // reference. Cheap to rebuild.
                noiseField = PathfindingCost.BuildSimplexNoiseField(grid.Cols, grid.Rows, rand.Next());
                EdgeCost noiseCostFn2 = PathfindingCost.FromNoise(noiseField, TypeWeights);
                EdgeCost shaftAdj = (anchor, candidate) =>
                {
                    if (candidate is ShaftCell
                        && (ColumnContainsShaft(grid, anchor.X - 1)
                         || ColumnContainsShaft(grid, anchor.X)
                         || ColumnContainsShaft(grid, anchor.X + 1)))
                    {
                        return double.PositiveInfinity;
                    }
                    return noiseCostFn2(anchor, candidate);
                };
                double[] capturedElevFinal = elevation;
                branchCostFn = (anchor, candidate) =>
                {
                    double baseCost = shaftAdj(anchor, candidate);
                    if (double.IsPositiveInfinity(baseCost)) return baseCost;
                    int colSafe = anchor.X < 0 ? 0 : (anchor.X >= gridCols ? gridCols - 1 : anchor.X);
                    double dev = capturedElevFinal != null
                        ? System.Math.Abs(anchor.Y - capturedElevFinal[colSafe])
                        : 0.0;
                    return baseCost + dev * BranchElevationPenalty;
                };
            }

            Terraria.ModLoader.Logging.PublicLogger.Info(
                $"OvermorrowDungeon final: branches={finalBranchesPlaced}, anchors={finalAnchorsCount}, accepted={fullAccepted}.");

            // ─── Phase 2.5: dead-end repair ──────────────────────────────────
            // Shaft landings get a short A* extension; other dead-ends get capped with walls.
            var sidesToCap = RepairDeadEnds(grid, branchCostFn, StreakLimits, borderBlocked, rand);

            // ─── Phase 3: render ─────────────────────────────────────────────
            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot.IsEmpty) continue;
                    if (slot.SubCol != 0 || slot.SubRow != 0) continue;

                    Point cellOrigin = grid.GridToWorld(col, row);
                    slot.Room.Build(cellOrigin, fillTileType, liningTileType);
                }
            }

            PaddingBuilder.BuildAll(grid, fillTileType);
            DecorateShafts(grid);

            ApplySideCaps(grid, sidesToCap, fillTileType);

            // ─── Phase 4: furniture ──────────────────────────────────────────
            // Runs after padding and side caps so furniture can react to the
            // finished neighbor context (e.g. shaft above/below) without
            // racing against the strip painter.
            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot.IsEmpty) continue;
                    if (slot.SubCol != 0 || slot.SubRow != 0) continue;

                    Point cellOrigin = grid.GridToWorld(col, row);
                    slot.Room.PlaceFurniture(new FurnitureContext(
                        cellOrigin, grid, col, row, fillTileType, liningTileType));
                }
            }

            // Diagnostic grid dump.
            try
            {
                var config = new GenerationConfig
                {
                    BaseRow = baseRow,
                    StartDoor = start,
                    EndDoor = endTarget,
                    SpineWaypoints = new List<Point>(requiredAnchors),
                    RequiredRoomAnchors = new List<Point>(requiredAnchors),
                    Elevation = elevation,
                };
                string dumpFolder = System.IO.Path.Combine(
                    Terraria.Main.SavePath, "OvermorrowDungeonDumps");
                System.IO.Directory.CreateDirectory(dumpFolder);
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(dumpFolder, "good"));
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(dumpFolder, "bad"));
                string fileName = $"dump_{System.DateTime.Now:yyyyMMdd_HHmmss_fff}.txt";
                string dumpPath = System.IO.Path.Combine(dumpFolder, fileName);
                GridDiagnostics.DumpFullGrid(grid, dumpPath, config);
            }
            catch (System.Exception ex)
            {
                Terraria.ModLoader.Logging.PublicLogger.Warn($"GridDiagnostics dump failed: {ex.Message}");
            }

            // Debug: corner markers at each grid cell.
            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    Point p = grid.GridToWorld(col, row);
                    int w = DungeonGrid.CellTileWidth - 1;
                    int h = DungeonGrid.CellTileHeight - 1;
                    WorldGenUtils.PlaceTile(p.X, p.Y, (ushort)TileID.Adamantite);
                    WorldGenUtils.PlaceTile(p.X + w, p.Y, (ushort)TileID.Adamantite);
                    WorldGenUtils.PlaceTile(p.X, p.Y + h, (ushort)TileID.Adamantite);
                    WorldGenUtils.PlaceTile(p.X + w, p.Y + h, (ushort)TileID.Adamantite);
                }
            }
        }

        /// <summary>Picks two spine points at least <see cref="MinBranchSpan"/> columns apart and routes a branch.</summary>
        private static bool TryPlaceBranchViaAStar(
            DungeonGrid grid,
            List<Point> criticalPath,
            EdgeCost costFn,
            IReadOnlyDictionary<Type, int> streakLimits,
            HashSet<Point> blocked,
            Random rand,
            bool relaxed)
        {
            if (criticalPath.Count < 2) return false;

            int iStart = rand.Next(criticalPath.Count);
            var startPos = criticalPath[iStart];

            var candidates = new List<int>();
            for (int i = 0; i < criticalPath.Count; i++)
            {
                if (i == iStart) continue;
                if (Math.Abs(criticalPath[i].X - startPos.X) >= MinBranchSpan)
                    candidates.Add(i);
            }
            if (candidates.Count == 0) return false;

            int iEnd = candidates[rand.Next(candidates.Count)];
            return TryPlaceBranchBetween(grid, criticalPath, iStart, iEnd,
                                         costFn, streakLimits, blocked, rand, relaxed);
        }

        /// <summary>Last-resort branch placement: walks index pairs inward from both spine ends.</summary>
        private static bool TryPlaceMaxSpanBranch(
            DungeonGrid grid,
            List<Point> criticalPath,
            EdgeCost costFn,
            IReadOnlyDictionary<Type, int> streakLimits,
            HashSet<Point> blocked,
            Random rand,
            bool relaxed)
        {
            if (criticalPath.Count < 4) return false;

            // 5x5 pairs from the ends inward.
            for (int frontOffset = 1; frontOffset <= 5; frontOffset++)
            {
                for (int backOffset = 1; backOffset <= 5; backOffset++)
                {
                    int iStart = frontOffset;
                    int iEnd = criticalPath.Count - 1 - backOffset;
                    if (iEnd <= iStart) continue;
                    if (TryPlaceBranchBetween(grid, criticalPath, iStart, iEnd,
                                              costFn, streakLimits, blocked, rand, relaxed))
                        return true;
                }
            }
            return false;
        }

        /// <summary>Validates endpoints, builds U-shape waypoints, runs A*, stamps the result.</summary>
        private static bool TryPlaceBranchBetween(
            DungeonGrid grid,
            List<Point> criticalPath,
            int iStart,
            int iEnd,
            EdgeCost costFn,
            IReadOnlyDictionary<Type, int> streakLimits,
            HashSet<Point> blocked,
            Random rand,
            bool relaxed)
        {
            if (iStart < 0 || iEnd >= criticalPath.Count || iEnd <= iStart) return false;

            var branchStart = criticalPath[iStart];
            var branchEnd   = criticalPath[iEnd];

            // Endpoints must be walkable (not stairs or doors).
            var startSlot = grid.GetSlot(branchStart.X, branchStart.Y);
            var endSlot   = grid.GetSlot(branchEnd.X,   branchEnd.Y);
            if (startSlot == null || startSlot.IsEmpty) return false;
            if (endSlot   == null || endSlot.IsEmpty)   return false;
            if (startSlot.Room is not (BookshelfCell or CorridorCell)) return false;
            if (endSlot.Room   is not (BookshelfCell or CorridorCell)) return false;

            if (Math.Abs(branchEnd.X - branchStart.X) < 2) return false;

            // U-shape waypoints: down, across, implicit climb back up.
            // Half the time the across portion gets a mid-waypoint at a
            // different depth so the branch wanders vertically instead of
            // being a flat rectangle. Relaxed mode skips waypoints entirely.
            List<Point> waypoints = null;
            if (!relaxed)
            {
                int maxDetourY = grid.Rows - 2 - EdgeBorder;
                int depthA = MinBranchDepth + rand.Next(MaxBranchDepth - MinBranchDepth + 1);
                int detourYA = Math.Min(branchStart.Y + depthA, maxDetourY);

                bool wander = rand.Next(2) == 0
                    && Math.Abs(branchEnd.X - branchStart.X) >= 4;

                if (wander)
                {
                    int depthB = MinBranchDepth + rand.Next(MaxBranchDepth - MinBranchDepth + 1);
                    int detourYB = Math.Min(branchStart.Y + depthB, maxDetourY);
                    int midX = (branchStart.X + branchEnd.X) / 2;
                    waypoints = new List<Point>
                    {
                        new Point(branchStart.X, detourYA),
                        new Point(midX,          detourYB),
                        new Point(branchEnd.X,   detourYA),
                    };
                }
                else
                {
                    waypoints = new List<Point>
                    {
                        new Point(branchStart.X, detourYA),
                        new Point(branchEnd.X,   detourYA),
                    };
                }
            }

            // Waypoints must end on a horizontally-walkable cell so the next segment can turn.
            var waypointAcceptableTypes = new HashSet<Type>
            {
                typeof(BookshelfCell),
                typeof(CorridorCell),
            };

            var path = GridAStar.FindPath(grid, branchStart, branchEnd, startSlot.Room, costFn,
                                          waypoints: waypoints,
                                          blocked: blocked,
                                          streakLimits: streakLimits,
                                          minStreakLimits: MinStreakLimits,
                                          waypointAcceptableTypes: waypointAcceptableTypes,
                                          maxVerticalRun: MaxVerticalRun);
            if (path == null) return false;

            foreach (var step in path)
                grid.Place(step.Cell, step.Anchor.X, step.Anchor.Y, grid.NextGroupId());
            return true;
        }

        /// <summary>
        /// Picks a column where the spine spans wide enough for a U-shape
        /// (≥6 columns) and drops a CombatRoom 3-5 rows below the most-
        /// populous spine row. Returns null if no clean spot fits the
        /// IsValidPlacement constraints (door distance, combat spacing).
        /// </summary>
        private static (CombatRoom room, Point anchor)? TryPrePlaceBranchCombat(
            DungeonGrid grid, List<Point> criticalPath, int gridCols, int gridRows,
            HashSet<Point> blocked, Random rand)
        {
            if (criticalPath.Count < MinBranchSpan + 1) return null;

            // Bucket spine cells by row, sort rows by population descending.
            var rowBuckets = new Dictionary<int, List<int>>();
            foreach (var p in criticalPath)
            {
                if (!rowBuckets.TryGetValue(p.Y, out var list))
                {
                    list = new List<int>();
                    rowBuckets[p.Y] = list;
                }
                list.Add(p.X);
            }
            var sortedRows = new List<KeyValuePair<int, List<int>>>(rowBuckets);
            sortedRows.Sort((a, b) => b.Value.Count.CompareTo(a.Value.Count));

            var prototype = new CombatRoom();
            int w = prototype.CellWidth;
            int h = prototype.CellHeight;

            foreach (var kv in sortedRows)
            {
                int spineRow = kv.Key;
                var cols = kv.Value;
                cols.Sort();
                int spanCols = cols[cols.Count - 1] - cols[0];
                // Need at least Combat width + 2 stair landings worth of horizontal span.
                if (spanCols < w + 4) continue;

                for (int depth = MinBranchDepth; depth <= MaxBranchDepth; depth++)
                {
                    int targetRow = spineRow + depth;
                    if (targetRow + h - 1 >= gridRows - EdgeBorder) continue;
                    if (targetRow < EdgeBorder) continue;

                    int colMin = cols[0] + 2;
                    int colMax = cols[cols.Count - 1] - w - 1;
                    if (colMin > colMax) continue;

                    for (int attempt = 0; attempt < 10; attempt++)
                    {
                        int col = rand.Next(colMin, colMax + 1);

                        bool clear = true;
                        for (int sc = 0; sc < w && clear; sc++)
                        {
                            for (int sr = 0; sr < h; sr++)
                            {
                                var pos = new Point(col + sc, targetRow + sr);
                                if (blocked.Contains(pos)) { clear = false; break; }
                                var slot = grid.GetSlot(pos.X, pos.Y);
                                if (slot == null || !slot.IsEmpty) { clear = false; break; }
                            }
                        }
                        if (!clear) continue;

                        var anchor = new Point(col, targetRow);
                        if (!prototype.IsValidPlacement(grid, anchor)) continue;

                        var combat = new CombatRoom { IsFeature = true };
                        grid.Place(combat, col, targetRow, grid.NextGroupId());
                        return (combat, anchor);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Routes a branch from one spine cell down through the pre-placed
        /// combat and back up to another spine cell. The combat is the
        /// middle waypoint; A* dock against it from the left and exits from
        /// the right.
        /// </summary>
        private static bool TryPlaceBranchThroughCombat(
            DungeonGrid grid, List<Point> criticalPath,
            (CombatRoom room, Point anchor) branchCombat,
            EdgeCost costFn, IReadOnlyDictionary<Type, int> streakLimits,
            HashSet<Point> blocked)
        {
            int combatRow = branchCombat.anchor.Y;
            int combatLeft = branchCombat.anchor.X;
            int combatRight = branchCombat.anchor.X + branchCombat.room.CellWidth - 1;

            // Spine cells above the combat that could serve as descent / ascent endpoints.
            var leftCandidates = new List<Point>();
            var rightCandidates = new List<Point>();
            foreach (var p in criticalPath)
            {
                if (p.Y >= combatRow) continue;
                var slot = grid.GetSlot(p.X, p.Y);
                if (slot == null || slot.IsEmpty) continue;
                if (slot.Room is not (BookshelfCell or CorridorCell)) continue;
                if (p.X <= combatLeft - 2) leftCandidates.Add(p);
                else if (p.X >= combatRight + 2) rightCandidates.Add(p);
            }
            if (leftCandidates.Count == 0 || rightCandidates.Count == 0) return false;

            // Prefer endpoints close to the combat for tighter U-shapes.
            leftCandidates.Sort((a, b) =>
                (combatLeft - a.X).CompareTo(combatLeft - b.X));
            rightCandidates.Sort((a, b) =>
                (a.X - combatRight).CompareTo(b.X - combatRight));

            var waypointAcceptableTypes = new HashSet<Type>
            {
                typeof(BookshelfCell),
                typeof(CorridorCell),
                typeof(CombatRoom),
            };

            int leftTries = Math.Min(3, leftCandidates.Count);
            int rightTries = Math.Min(3, rightCandidates.Count);
            for (int li = 0; li < leftTries; li++)
            {
                for (int ri = 0; ri < rightTries; ri++)
                {
                    var startPoint = leftCandidates[li];
                    var endPoint = rightCandidates[ri];
                    var startSlot = grid.GetSlot(startPoint.X, startPoint.Y);
                    if (startSlot == null || startSlot.IsEmpty) continue;

                    var waypoints = new List<Point>
                    {
                        new Point(startPoint.X, combatRow),
                        branchCombat.anchor,
                        new Point(endPoint.X,   combatRow),
                    };

                    var path = GridAStar.FindPath(grid, startPoint, endPoint, startSlot.Room, costFn,
                                                  waypoints: waypoints,
                                                  blocked: blocked,
                                                  streakLimits: streakLimits,
                                                  minStreakLimits: MinStreakLimits,
                                                  waypointAcceptableTypes: waypointAcceptableTypes,
                                                  maxVerticalRun: MaxVerticalRun);
                    if (path == null) continue;

                    foreach (var step in path)
                        grid.Place(step.Cell, step.Anchor.X, step.Anchor.Y, grid.NextGroupId());
                    return true;
                }
            }
            return false;
        }

        /// <summary>Clears every sub-cell of <paramref name="room"/>'s footprint at <paramref name="anchor"/>.</summary>
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

        /// <summary>Clamps a row inside the playable area, leaving room for 2-row stair footprints.</summary>
        private static int ClampRow(int row, int gridRows)
        {
            int min = EdgeBorder;
            int max = gridRows - 1 - EdgeBorder;
            min = Math.Max(min, 1);
            max = Math.Min(max, gridRows - 2);
            return Math.Max(min, Math.Min(max, row));
        }

        /// <summary>Blocks the EdgeBorder ring so A* never places cells there.</summary>
        private static HashSet<Point> BuildBorderBlockedSet(int gridCols, int gridRows)
        {
            var blocked = new HashSet<Point>();
            for (int b = 0; b < EdgeBorder; b++)
            {
                for (int x = 0; x < gridCols; x++)
                {
                    blocked.Add(new Point(x, b));
                    blocked.Add(new Point(x, gridRows - 1 - b));
                }
                for (int y = 0; y < gridRows; y++)
                {
                    blocked.Add(new Point(b, y));
                    blocked.Add(new Point(gridCols - 1 - b, y));
                }
            }
            return blocked;
        }

        // ─── Phase 2.5: dead-end repair / capping ────────────────────────────

        /// <summary>
        /// For each dead-end open side, tries: shaft-landing extension, bookshelf conversion,
        /// bookshelf filler at the empty neighbor, removal of the offending 1x1 connector,
        /// or stone-cap. Iterates until no more progress is made so cascading repairs settle.
        /// Returns the (cell, side) pairs that fell through to capping.
        /// </summary>
        private static HashSet<(Point cell, Direction side)> RepairDeadEnds(
            DungeonGrid grid, EdgeCost costFn,
            IReadOnlyDictionary<Type, int> streakLimits,
            HashSet<Point> blocked, Random rand)
        {
            var sidesToCap = new HashSet<(Point, Direction)>();
            var dirs = new (Direction side, int dx, int dy)[]
            {
                (Direction.Top,    0, -1),
                (Direction.Bottom, 0,  1),
                (Direction.Left,  -1, 0),
                (Direction.Right,  1, 0),
            };

            const int MaxIterations = 20;
            for (int iter = 0; iter < MaxIterations; iter++)
            {
                bool changed = false;
                sidesToCap.Clear();

                var initialCells = new List<(Point pos, GridRoom room)>();
                for (int row = 0; row < grid.Rows; row++)
                {
                    for (int col = 0; col < grid.Cols; col++)
                    {
                        var slot = grid.GetSlot(col, row);
                        if (slot == null || slot.IsEmpty) continue;
                        initialCells.Add((new Point(col, row), slot.Room));
                    }
                }

                foreach (var (pos, room) in initialCells)
                {
                    var slot = grid.GetSlot(pos.X, pos.Y);
                    if (slot == null || slot.IsEmpty) continue;

                    var deadEndSides = new List<Direction>();
                    foreach (var d in dirs)
                    {
                        if (!room.IsOpenSide(slot.SubCol, slot.SubRow, d.side)) continue;
                        var neighbor = grid.GetSlot(pos.X + d.dx, pos.Y + d.dy);
                        if (neighbor == null) continue;
                        if (!neighbor.IsEmpty) continue;
                        deadEndSides.Add(d.side);
                    }

                    if (deadEndSides.Count == 0) continue;

                    var unrepairedSides = new List<Direction>();
                    foreach (var side in deadEndSides)
                    {
                        if (IsShaftLandingDeadEnd(grid, pos, room, side)
                            && TryRepairShaftLanding(grid, pos, side, costFn, streakLimits, blocked, rand))
                        {
                            changed = true;
                            continue;
                        }
                        unrepairedSides.Add(side);
                    }

                    if (unrepairedSides.Count == 0) continue;
                    if (room.AllowsEmptyNeighbors) continue;

                    if (TryConvertToBookshelf(grid, pos)) { changed = true; continue; }

                    // Try placing a Bookshelf at each empty neighbor side. A
                    // bookshelf is the most permissive filler now that vertical
                    // bookshelf-bookshelf adjacency is allowed.
                    var stillUnrepaired = new List<Direction>();
                    foreach (var side in unrepairedSides)
                    {
                        int dx = side == Direction.Left ? -1 : (side == Direction.Right ? 1 : 0);
                        int dy = side == Direction.Top ? -1 : (side == Direction.Bottom ? 1 : 0);
                        if (TryPlaceFillerNeighbor(grid, new Point(pos.X + dx, pos.Y + dy), blocked))
                        {
                            changed = true;
                            continue;
                        }
                        stillUnrepaired.Add(side);
                    }

                    if (stillUnrepaired.Count == 0) continue;

                    // Last resort: remove the offending 1x1 connector so the
                    // dungeon shrinks rather than dangles. Multi-cell pieces
                    // get capped instead since clearing one sub-cell would
                    // orphan the rest of the footprint.
                    if (room.CellWidth == 1 && room.CellHeight == 1)
                    {
                        slot.Room = null;
                        slot.SubCol = 0;
                        slot.SubRow = 0;
                        slot.GroupId = 0;
                        changed = true;
                        continue;
                    }

                    foreach (var side in stillUnrepaired)
                        sidesToCap.Add((pos, side));
                }

                if (!changed) break;
            }

            return sidesToCap;
        }

        /// <summary>
        /// Places a Bookshelf at <paramref name="pos"/> if the slot is empty,
        /// not blocked, and structurally compatible with every non-empty
        /// neighbor. Used as a cosmetic filler so corridor dead-ends face a
        /// real cell rather than empty stone.
        /// </summary>
        private static bool TryPlaceFillerNeighbor(DungeonGrid grid, Point pos, HashSet<Point> blocked)
        {
            var slot = grid.GetSlot(pos.X, pos.Y);
            if (slot == null) return false;
            if (!slot.IsEmpty) return false;
            if (blocked.Contains(pos)) return false;

            var bookshelf = new BookshelfCell();
            var dirs = new (Direction side, int dx, int dy, Direction opposite)[]
            {
                (Direction.Top,    0, -1, Direction.Bottom),
                (Direction.Bottom, 0,  1, Direction.Top),
                (Direction.Left,  -1, 0, Direction.Right),
                (Direction.Right,  1, 0, Direction.Left),
            };

            foreach (var d in dirs)
            {
                var n = grid.GetSlot(pos.X + d.dx, pos.Y + d.dy);
                if (n == null || n.IsEmpty) continue;

                if (!n.Room.IsOpenSide(n.SubCol, n.SubRow, d.opposite)) return false;

                var weAccept = bookshelf.GetAcceptedNeighbors(0, 0, d.side);
                if (weAccept == null || !weAccept.Contains(n.Room.GetType())) return false;

                var theyAccept = n.Room.GetAcceptedNeighbors(n.SubCol, n.SubRow, d.opposite);
                if (theyAccept == null || !theyAccept.Contains(typeof(BookshelfCell))) return false;
            }

            grid.Place(bookshelf, pos.X, pos.Y, grid.NextGroupId());
            return true;
        }

        /// <summary>Swaps a 1x1 connector for a BookshelfCell when no adjacency mismatches result.</summary>
        private static bool TryConvertToBookshelf(DungeonGrid grid, Point pos)
        {
            var slot = grid.GetSlot(pos.X, pos.Y);
            if (slot == null || slot.IsEmpty) return false;

            // Multi-cell pieces would orphan the rest of their footprint.
            if (slot.Room.CellWidth != 1 || slot.Room.CellHeight != 1) return false;

            var bookshelf = new BookshelfCell();
            var dirs = new (Direction side, int dx, int dy, Direction opposite)[]
            {
                (Direction.Top,    0, -1, Direction.Bottom),
                (Direction.Bottom, 0,  1, Direction.Top),
                (Direction.Left,  -1, 0, Direction.Right),
                (Direction.Right,  1, 0, Direction.Left),
            };

            foreach (var d in dirs)
            {
                var n = grid.GetSlot(pos.X + d.dx, pos.Y + d.dy);
                if (n == null || n.IsEmpty) continue;

                // Bookshelf is open on all sides; neighbor's facing side must also be open.
                if (!n.Room.IsOpenSide(n.SubCol, n.SubRow, d.opposite)) return false;

                // Mutual whitelist on the shared edge.
                var weAccept = bookshelf.GetAcceptedNeighbors(0, 0, d.side);
                if (weAccept == null || !weAccept.Contains(n.Room.GetType())) return false;

                var theyAccept = n.Room.GetAcceptedNeighbors(n.SubCol, n.SubRow, d.opposite);
                if (theyAccept == null || !theyAccept.Contains(typeof(BookshelfCell))) return false;
            }

            grid.Place(bookshelf, pos.X, pos.Y, grid.NextGroupId());
            return true;
        }

        /// <summary>True if pos is a Bookshelf vertically adjacent to a ShaftCell with a horizontal dead-end side.</summary>
        private static bool IsShaftLandingDeadEnd(DungeonGrid grid, Point pos, GridRoom room, Direction side)
        {
            if (room is not BookshelfCell) return false;
            if (side != Direction.Left && side != Direction.Right) return false;

            var above = grid.GetSlot(pos.X, pos.Y - 1);
            var below = grid.GetSlot(pos.X, pos.Y + 1);
            bool shaftAbove = above != null && !above.IsEmpty && above.Room is ShaftCell;
            bool shaftBelow = below != null && !below.IsEmpty && below.Room is ShaftCell;
            return shaftAbove || shaftBelow;
        }

        /// <summary>Grows a short extension from a landing into existing dungeon. Returns false if no fit.</summary>
        private static bool TryRepairShaftLanding(DungeonGrid grid, Point landing, Direction outwardSide,
                                                  EdgeCost costFn, IReadOnlyDictionary<Type, int> streakLimits,
                                                  HashSet<Point> blocked, Random rand)
        {
            int dx = outwardSide == Direction.Left ? -1 : (outwardSide == Direction.Right ? 1 : 0);
            int dy = outwardSide == Direction.Top  ? -1 : (outwardSide == Direction.Bottom ? 1 : 0);
            if (dx == 0 && dy == 0) return false;

            for (int step = 1; step <= RepairExtensionBudget; step++)
            {
                int tx = landing.X + dx * step;
                int ty = landing.Y + dy * step;
                var target = grid.GetSlot(tx, ty);
                if (target == null) break;
                if (target.IsEmpty) continue;

                // Target must have an open side facing the landing.
                Direction inverseSide = outwardSide switch
                {
                    Direction.Left => Direction.Right,
                    Direction.Right => Direction.Left,
                    Direction.Top => Direction.Bottom,
                    _ => Direction.Top
                };
                if (!target.Room.IsOpenSide(target.SubCol, target.SubRow, inverseSide))
                    continue;

                var path = GridAStar.FindPath(grid, landing, new Point(tx, ty),
                                              grid.GetSlot(landing.X, landing.Y).Room,
                                              costFn,
                                              blocked: blocked,
                                              streakLimits: streakLimits,
                                              minStreakLimits: MinStreakLimits,
                                              maxVerticalRun: MaxVerticalRun);
                if (path == null) continue;
                if (path.Count == 0) continue;

                foreach (var pstep in path)
                    grid.Place(pstep.Cell, pstep.Anchor.X, pstep.Anchor.Y, grid.NextGroupId());
                return true;
            }

            return false;
        }

        /// <summary>Paints stone over capped sides. Runs after PaddingBuilder to override any opening it placed.</summary>
        private static void ApplySideCaps(DungeonGrid grid,
                                          HashSet<(Point cell, Direction side)> sidesToCap,
                                          int fillTileType)
        {
            ushort fill = (ushort)fillTileType;

            foreach (var (cellPos, side) in sidesToCap)
            {
                var slot = grid.GetSlot(cellPos.X, cellPos.Y);
                if (slot == null || slot.IsEmpty) continue;

                Point cellOrigin = grid.GridToWorld(cellPos.X, cellPos.Y);

                int x, y, w, h;
                switch (side)
                {
                    case Direction.Top:
                        x = cellOrigin.X;
                        y = cellOrigin.Y - DungeonGrid.VerticalPadding;
                        w = DungeonGrid.CellTileWidth;
                        h = DungeonGrid.VerticalPadding;
                        break;
                    case Direction.Bottom:
                        x = cellOrigin.X;
                        y = cellOrigin.Y + DungeonGrid.CellTileHeight;
                        w = DungeonGrid.CellTileWidth;
                        h = DungeonGrid.VerticalPadding;
                        break;
                    case Direction.Left:
                        x = cellOrigin.X - DungeonGrid.HorizontalPadding;
                        y = cellOrigin.Y;
                        w = DungeonGrid.HorizontalPadding;
                        h = DungeonGrid.CellTileHeight;
                        break;
                    case Direction.Right:
                    default:
                        x = cellOrigin.X + DungeonGrid.CellTileWidth;
                        y = cellOrigin.Y;
                        w = DungeonGrid.HorizontalPadding;
                        h = DungeonGrid.CellTileHeight;
                        break;
                }

                for (int lx = 0; lx < w; lx++)
                {
                    for (int ly = 0; ly < h; ly++)
                    {
                        WorldGenUtils.PlaceTile(x + lx, y + ly, fill);
                        WorldGenUtils.ClearWall(x + lx, y + ly);
                    }
                }
            }
        }

        /// <summary>True if any ShaftCell sits in this column.</summary>
        private static bool ColumnContainsShaft(DungeonGrid grid, int col)
        {
            if (col < 0 || col >= grid.Cols) return false;
            for (int row = 0; row < grid.Rows; row++)
            {
                var slot = grid.GetSlot(col, row);
                if (slot != null && !slot.IsEmpty && slot.Room is ShaftCell)
                    return true;
            }
            return false;
        }

        private static void DecorateShafts(DungeonGrid grid)
        {
            int diagonalStairsType = ModContent.TileType<DiagonalStairs>();
            int stairCapType = ModContent.TileType<StairCap>();

            // A passage spans consecutive shafts and any bookshelf landings between them.
            var resolved = new HashSet<Point>();

            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot.IsEmpty || slot.Room is not ShaftCell) continue;
                    if (resolved.Contains(new Point(col, row))) continue;

                    // Walk up through shafts and through a bookshelf landing with another shaft beyond.
                    int topRow = row;
                    while (topRow > 0)
                    {
                        var above = grid.GetSlot(col, topRow - 1);
                        if (above == null || above.IsEmpty) break;

                        if (above.Room is ShaftCell)
                        {
                            topRow--;
                            continue;
                        }

                        if (above.Room is BookshelfCell && topRow >= 2)
                        {
                            var aboveAbove = grid.GetSlot(col, topRow - 2);
                            if (aboveAbove != null && !aboveAbove.IsEmpty && aboveAbove.Room is ShaftCell)
                            {
                                topRow -= 2;
                                continue;
                            }
                        }

                        break;
                    }

                    // Walk down with the mirrored rule.
                    int bottomRow = row;
                    while (bottomRow < grid.Rows - 1)
                    {
                        var below = grid.GetSlot(col, bottomRow + 1);
                        if (below == null || below.IsEmpty) break;

                        if (below.Room is ShaftCell)
                        {
                            bottomRow++;
                            continue;
                        }

                        if (below.Room is BookshelfCell && bottomRow + 2 < grid.Rows)
                        {
                            var belowBelow = grid.GetSlot(col, bottomRow + 2);
                            if (belowBelow != null && !belowBelow.IsEmpty && belowBelow.Room is ShaftCell)
                            {
                                bottomRow += 2;
                                continue;
                            }
                        }

                        break;
                    }

                    for (int r = topRow; r <= bottomRow; r++)
                    {
                        var s = grid.GetSlot(col, r);
                        if (s != null && !s.IsEmpty && s.Room is ShaftCell)
                            resolved.Add(new Point(col, r));
                    }

                    var topRoom = grid.GetSlot(col, topRow - 1);
                    var bottomRoom = grid.GetSlot(col, bottomRow + 1);

                    // Skip decoration if either end is empty; nothing for stairs to lead to.
                    if (topRoom == null || topRoom.IsEmpty || bottomRoom == null || bottomRoom.IsEmpty)
                        continue;

                    Point topRoomOrigin = grid.GridToWorld(col, topRow - 1);
                    Point bottomRoomOrigin = grid.GridToWorld(col, bottomRow + 1);

                    int topY = topRoomOrigin.Y + DungeonGrid.CellTileHeight - 1;
                    int bottomY = bottomRoomOrigin.Y + DungeonGrid.CellTileHeight - 1;

                    int segmentCount = (bottomY - topY) / 10;
                    int shaftCenterX = grid.GridToWorld(col, topRow).X + DungeonGrid.CellTileWidth / 2;
                    int stairX = shaftCenterX - 7;
                    int capX = stairX;

                    for (int s = segmentCount - 1; s >= 0; s--)
                        WorldGen.PlaceObject(stairX, topY + s * 10 + 10, diagonalStairsType);

                    WorldGen.PlaceObject(capX, topY, stairCapType);
                }
            }
        }
    }
}
