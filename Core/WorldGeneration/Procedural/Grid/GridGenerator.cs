using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>A*-based dungeon generator. Critical-path / spine only.</summary>
    public static class GridGenerator
    {
        // Caps the visual height of any shaft chain (including bookshelf landings between shafts).
        private const int MaxVerticalRun = 2;

        // Width of the un-buildable border ring. Doors live at its inner edge.
        private const int EdgeBorder = 1;

        // Per-type A* weight. <1 = preferred, >1 = avoided. Default 1.0.
        private static readonly Dictionary<Type, double> TypeWeights = new()
        {
            [typeof(ShaftCell)] = 1.4,
            [typeof(DescendingStair)] = 0.7,
            [typeof(AscendingStair)] = 0.7,
            [typeof(FireplaceRoom)] = 1.5,
            [typeof(LoungeRoom)] = 0.3,
            [typeof(CombatRoom)] = 0.7
        };

        // Max consecutive runs. Shafts use MaxVerticalRun instead.
        private static readonly Dictionary<Type, int> StreakLimits = new()
        {
            [typeof(BookshelfCell)] = 4,
            [typeof(CorridorCell)] = 5,
            [typeof(FireplaceRoom)] = 1
        };

        // Minimum consecutive runs. Once a streak of this type starts, A* must
        // place at least this many before transitioning to another type.
        private static readonly Dictionary<Type, int> MinStreakLimits = new()
        {
            [typeof(BookshelfCell)] = 2
        };

        // Rooms guaranteed to appear on the spine. Each entry is pre-placed
        // before spine planning and replaces the closest random spine waypoint.
        private static readonly List<Func<GridRoom>> RequiredRooms = new()
        {
            () => new FireplaceRoom(),
            () => new CombatRoom(),
            () => new WritingRoom(),
        };

        public static void Build(Point worldOrigin, int gridCols, int gridRows, List<GridRoom> cellPool, int fillTileType, int liningTileType, Random rand, out Point startDoorTile)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            int margin = DungeonGrid.HorizontalPadding;
            var gridOrigin = new Point(worldOrigin.X + margin, worldOrigin.Y + margin);
            var grid = new DungeonGrid(gridCols, gridRows, gridOrigin);

            int totalWidth = gridCols * DungeonGrid.HorizontalSpacing + DungeonGrid.CellTileWidth + margin * 2;
            int totalHeight = gridRows * DungeonGrid.VerticalSpacing + DungeonGrid.CellTileHeight + margin * 2;
            ushort fill = (ushort)fillTileType;

            for (int x = 0; x < totalWidth; x++)
                for (int y = 0; y < totalHeight; y++)
                    WorldGenUtils.PlaceTile(worldOrigin.X + x, worldOrigin.Y + y, fill);

            // Phase 1: critical path
            int doorRowMin = gridRows / 3;
            int doorRowMax = (gridRows * 2) / 3;
            const int MaxDoorRowOffset = 2;

            const int ElevationAmplitude = 5;
            const int MinCurveSpan = 4;
            const int MaxNoiseRetries = 20;
            const int MinSpineSpan = 5;
            const int MaxSpinePlanAttempts = 8;
            const int MaxBuildRetries = 5;
            const int MinDoorDistance = 6;
            const int MinSubgoalSpacing = 2;
            const double SpineElevationPenalty = 0.4;

            var borderBlocked = BuildBorderBlockedSet(gridCols, gridRows);

            int baseRow = 0;
            int startRow = 0, endRow = 0;
            Point start = default, endTarget = default;
            DoorRoom startDoor = null;
            double[] elevation = null;
            List<Point> requiredAnchors = null;
            List<PathStep> spineSteps = null;
            EdgeCost spineCostFn = null;
            double[,] noiseField = null;
            bool buildAccepted = false;

            // Highest-scoring spine snapshot, restored if no attempt is accepted.
            int bestScore = int.MinValue;
            (GridRoom room, int subCol, int subRow, int groupId)[,] bestGridSnapshot = null;
            List<PathStep> bestSpineSteps = null;
            double[] bestElevation = null;
            List<Point> bestRequiredAnchors = null;
            int bestBaseRow = 0;
            Point bestStart = default, bestEndTarget = default;
            int bestStartRow = 0, bestEndRow = 0;
            BranchPlacement? bestBranchPlacement = null;

            for (int buildAttempt = 0; buildAttempt < MaxBuildRetries && !buildAccepted; buildAttempt++)
            {
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
                    // Clear everything from the previous attempt except the doors.
                    if (spineSteps != null || requiredAnchors != null)
                    {
                        for (int c = 0; c < gridCols; c++)
                        {
                            for (int r = 0; r < gridRows; r++)
                            {
                                var s = grid.GetSlot(c, r);
                                if (s == null || s.IsEmpty) continue;
                                if (s.Room is DoorRoom) continue;  // doors stay
                                s.Room = null;
                                s.SubCol = 0;
                                s.SubRow = 0;
                                s.GroupId = 0;
                            }
                        }
                        spineSteps = null;
                        requiredAnchors = null;
                    }

                    // Re-roll noise until the curve has enough vertical range.
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

                    double[] capturedElevation = elevation;
                    spineCostFn = (anchor, candidate) =>
                    {
                        double baseCost = shaftAdjacencyAwareCost(anchor, candidate);
                        if (double.IsPositiveInfinity(baseCost)) return baseCost;
                        int colSafe = anchor.X < 0 ? 0 : (anchor.X >= gridCols ? gridCols - 1 : anchor.X);
                        double dev = System.Math.Abs(anchor.Y - capturedElevation[colSafe]);
                        return baseCost + dev * SpineElevationPenalty;
                    };

                    bool isFinalSpineAttempt = (spineAttempt == MaxSpinePlanAttempts - 1);

                    var plan = SpinePlanner.TryPlanSpine(grid, start, endTarget, startDoor, endDoor, RequiredRooms, elevation, gridCols, gridRows, spineCostFn, borderBlocked, StreakLimits, MinStreakLimits, MaxVerticalRun, EdgeBorder, MinDoorDistance, MinSubgoalSpacing);

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

                    // Spine must have exactly one Combat with no bypass around it.
                    int spineCombatCount = CountCombatRooms(grid);
                    bool spineHasOneCombat = spineCombatCount == 1;
                    bool combatMandatory = !HasCombatBypass(grid, start, endTarget);

                    BranchPlacement? branchPlacement = null;
                    if (spineHasOneCombat && combatMandatory)
                        branchPlacement = TryPlaceBranchThroughSecondCombat(grid, spineSteps, requiredAnchors, spineCostFn, borderBlocked, gridCols, gridRows, rand);
                    bool branchPlaced = branchPlacement.HasValue;

                    int score = 10 * spineSpanRows + totalAnchors;
                    score -= 10000 * missingRequiredCount;
                    if (spineSpanRows < MinSpineSpan) score -= 5000;
                    if (!spineHasOneCombat) score -= 8000;
                    if (!combatMandatory) score -= 9000;
                    if (!branchPlaced) score -= 7000;

                    Terraria.ModLoader.Logging.PublicLogger.Info($"OvermorrowDungeon spine attempt build={buildAttempt} spine={spineAttempt}: span={spineSpanRows} required={requiredAnchors.Count}/{RequiredRooms.Count} spineCombat={spineCombatCount} mandatory={combatMandatory} branch={branchPlaced} score={score}");

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
                        bestBranchPlacement = branchPlacement.HasValue
                            ? new BranchPlacement
                            {
                                Steps = new List<PathStep>(branchPlacement.Value.Steps),
                                CombatAnchor = branchPlacement.Value.CombatAnchor,
                            }
                            : (BranchPlacement?)null;
                    }

                    if (spanOk && missingRequiredCount == 0 && spineHasOneCombat
                        && combatMandatory && branchPlaced)
                    {
                        buildAccepted = true;
                        break;
                    }
                    if (isFinalSpineAttempt) break;
                }
            }

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

                // Rebuild cost function for downstream consumers.
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
                spineCostFn = (anchor, candidate) =>
                {
                    double baseCost = shaftAdjacencyAwareCost(anchor, candidate);
                    if (double.IsPositiveInfinity(baseCost)) return baseCost;
                    int colSafe = anchor.X < 0 ? 0 : (anchor.X >= gridCols ? gridCols - 1 : anchor.X);
                    double dev = capturedElev != null
                        ? System.Math.Abs(anchor.Y - capturedElev[colSafe])
                        : 0.0;
                    return baseCost + dev * SpineElevationPenalty;
                };
            }

            // Spawn point at the start door's center.
            Point startDoorOrigin = grid.GridToWorld(start.X, start.Y);
            startDoorTile = new Point(startDoorOrigin.X + DungeonGrid.CellTileWidth / 2, startDoorOrigin.Y + DungeonGrid.CellTileHeight / 2);

            // Path graph: wraps spine + branches so aux placement can address them by identity.
            var paths = new List<DungeonPath>();
            Point? spineCombatAnchor = null;
            if (requiredAnchors != null)
            {
                foreach (var anchor in requiredAnchors)
                {
                    var slot = grid.GetSlot(anchor.X, anchor.Y);
                    if (slot != null && !slot.IsEmpty && slot.Room is CombatRoom)
                    {
                        spineCombatAnchor = anchor;
                        break;
                    }
                }
            }
            var spinePath = new DungeonPath(id: 1, role: PathRole.Spine, parent: null, steps: spineSteps ?? new List<PathStep>(), combatAnchor: spineCombatAnchor);
            paths.Add(spinePath);

            if (bestBranchPlacement.HasValue)
            {
                var bp = bestBranchPlacement.Value;
                paths.Add(new DungeonPath(id: 2, role: PathRole.ClosedLoopBranch, parent: spinePath, steps: bp.Steps, combatAnchor: bp.CombatAnchor));
            }

            // Phase 2: aux branches
            TryAddAuxBranches(grid, paths, spineCostFn, borderBlocked, gridCols, gridRows, rand);

            // Phase 3: side cap scan
            var sidesToCap = CollectDeadEndSides(grid);

            int finalCombats = CountCombatRooms(grid);
            bool finalMandatory = !HasCombatBypass(grid, start, endTarget);
            bool finalTreeShaped = IsTreeShaped(grid, start);
            Terraria.ModLoader.Logging.PublicLogger.Info($"OvermorrowDungeon final: combats={finalCombats} mandatory={finalMandatory} tree={finalTreeShaped}");

            // Phase 4: render rooms
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

            // Phase 5: place furniture
            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot.IsEmpty) continue;
                    if (slot.SubCol != 0 || slot.SubRow != 0) continue;

                    Point cellOrigin = grid.GridToWorld(col, row);
                    slot.Room.PlaceFurniture(new FurnitureContext(cellOrigin, grid, col, row, fillTileType, liningTileType));
                }
            }

            // Diagnostic grid dump.
            stopwatch.Stop();
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
                    ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                };
                string dumpFolder = System.IO.Path.Combine(Terraria.Main.SavePath, "OvermorrowDungeonDumps");
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

        /// <summary>True if there's a start-to-end route that bypasses every CombatRoom.</summary>
        private static bool HasCombatBypass(DungeonGrid grid, Point startDoor, Point endDoor)
        {
            var visited = new HashSet<Point> { startDoor };
            var queue = new Queue<Point>();
            queue.Enqueue(startDoor);
            var dirs = new (Direction side, int dx, int dy)[]
            {
                (Direction.Right,  1,  0),
                (Direction.Left,  -1,  0),
                (Direction.Bottom, 0,  1),
                (Direction.Top,    0, -1),
            };
            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                if (p == endDoor) return true;
                var slot = grid.GetSlot(p.X, p.Y);
                if (slot == null || slot.IsEmpty) continue;
                foreach (var d in dirs)
                {
                    if (!slot.Room.IsOpenSide(slot.SubCol, slot.SubRow, d.side)) continue;
                    var next = new Point(p.X + d.dx, p.Y + d.dy);
                    if (visited.Contains(next)) continue;
                    var nextSlot = grid.GetSlot(next.X, next.Y);
                    if (nextSlot == null || nextSlot.IsEmpty) continue;
                    if (nextSlot.Room is CombatRoom) continue;
                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }
            return false;
        }


        /// <summary>True if the cell graph reachable from <paramref name="seed"/> has no cycles.</summary>
        private static bool IsTreeShaped(DungeonGrid grid, Point seed)
        {
            var seedSlot = grid.GetSlot(seed.X, seed.Y);
            if (seedSlot == null || seedSlot.IsEmpty) return true;

            var parent = new Dictionary<Point, Point> { [seed] = seed };
            var queue = new Queue<Point>();
            queue.Enqueue(seed);
            var dirs = new (Direction side, int dx, int dy)[]
            {
                (Direction.Right,  1,  0),
                (Direction.Left,  -1,  0),
                (Direction.Bottom, 0,  1),
                (Direction.Top,    0, -1),
            };
            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                var slot = grid.GetSlot(p.X, p.Y);
                if (slot == null || slot.IsEmpty) continue;
                foreach (var d in dirs)
                {
                    if (!slot.Room.IsOpenSide(slot.SubCol, slot.SubRow, d.side)) continue;
                    var n = new Point(p.X + d.dx, p.Y + d.dy);
                    var nSlot = grid.GetSlot(n.X, n.Y);
                    if (nSlot == null || nSlot.IsEmpty) continue;
                    if (parent.TryGetValue(n, out var par))
                    {
                        // Visited. Cycle iff it's not the parent we came from.
                        if (par != p && parent[p] != n) return false;
                    }
                    else
                    {
                        parent[n] = p;
                        queue.Enqueue(n);
                    }
                }
            }
            return true;
        }

        /// <summary>Combined path steps and combat anchor for a placed branch.</summary>
        private struct BranchPlacement
        {
            public List<PathStep> Steps;
            public Point CombatAnchor;
        }

        /// <summary>
        /// Places a second CombatRoom off the spine and routes two legs back
        /// to spine nodes on either side of the spine's combat. Rolls back
        /// fully and returns null on failure.
        /// </summary>
        private static BranchPlacement? TryPlaceBranchThroughSecondCombat(DungeonGrid grid, List<PathStep> spineSteps, List<Point> requiredAnchors, EdgeCost costFn, HashSet<Point> borderBlocked, int gridCols, int gridRows, Random rand)
        {
            Point spineCombatAnchor = default;
            bool found = false;
            foreach (var anchor in requiredAnchors)
            {
                var slot = grid.GetSlot(anchor.X, anchor.Y);
                if (slot != null && !slot.IsEmpty && slot.Room is CombatRoom)
                {
                    spineCombatAnchor = anchor;
                    found = true;
                    break;
                }
            }
            if (!found) return null;

            int spineCombatRight = spineCombatAnchor.X + 2;  // CombatRoom is 3 wide

            // Partition non-feature bookshelf/corridor spine cells around the combat.
            var beforeNodes = new List<PathStep>();
            var afterNodes = new List<PathStep>();
            foreach (var step in spineSteps)
            {
                if (step.Cell.IsFeature) continue;
                if (step.Cell is not (BookshelfCell or CorridorCell)) continue;
                int x = step.Anchor.X;
                if (x < spineCombatAnchor.X - 1) beforeNodes.Add(step);
                else if (x > spineCombatRight + 1) afterNodes.Add(step);
            }
            if (beforeNodes.Count == 0 || afterNodes.Count == 0) return null;

            var branchCombatProto = new CombatRoom { IsFeature = true };
            int bw = branchCombatProto.CellWidth;
            int bh = branchCombatProto.CellHeight;
            const int BranchDepthMin = 4;
            const int BranchDepthMax = 6;
            const int MaxPositionAttempts = 20;
            int beforeCap = System.Math.Min(8, beforeNodes.Count);
            int afterCap = System.Math.Min(8, afterNodes.Count);

            for (int posAttempt = 0; posAttempt < MaxPositionAttempts; posAttempt++)
            {
                int depth = BranchDepthMin + rand.Next(BranchDepthMax - BranchDepthMin + 1);
                bool below = rand.Next(2) == 0;
                int row = below ? spineCombatAnchor.Y + depth : spineCombatAnchor.Y - depth;
                if (row < EdgeBorder || row + bh - 1 >= gridRows - EdgeBorder)
                    row = below ? spineCombatAnchor.Y - depth : spineCombatAnchor.Y + depth;
                if (row < EdgeBorder) continue;
                if (row + bh - 1 >= gridRows - EdgeBorder) continue;

                int colJitter = rand.Next(-6, 7);
                int col = spineCombatAnchor.X + colJitter;
                if (col < EdgeBorder + 1) continue;
                if (col + bw > gridCols - EdgeBorder - 1) continue;

                var posCandidate = new Point(col, row);
                if (!FootprintAndNeighborsClear(grid, branchCombatProto, posCandidate)) continue;
                if (!branchCombatProto.IsValidPlacement(grid, posCandidate)) continue;

                var branchCombat = new CombatRoom { IsFeature = true };
                grid.Place(branchCombat, posCandidate.X, posCandidate.Y, grid.NextGroupId());

                ShuffleInPlace(beforeNodes, rand);
                ShuffleInPlace(afterNodes, rand);

                BranchPlacement? success = null;
                for (int aIdx = 0; aIdx < beforeCap && !success.HasValue; aIdx++)
                {
                    var nodeA = beforeNodes[aIdx];
                    var leg1 = GridAStar.FindPath(grid, nodeA.Anchor, posCandidate, nodeA.Cell, costFn, blocked: borderBlocked, streakLimits: StreakLimits, minStreakLimits: MinStreakLimits, maxVerticalRun: MaxVerticalRun);
                    if (leg1 == null) continue;

                    foreach (var step in leg1)
                        grid.Place(step.Cell, step.Anchor.X, step.Anchor.Y, grid.NextGroupId());

                    List<PathStep> leg2 = null;
                    for (int bIdx = 0; bIdx < afterCap; bIdx++)
                    {
                        var nodeB = afterNodes[bIdx];
                        leg2 = GridAStar.FindPath(grid, posCandidate, nodeB.Anchor, branchCombat, costFn, blocked: borderBlocked, streakLimits: StreakLimits, minStreakLimits: MinStreakLimits, maxVerticalRun: MaxVerticalRun);
                        if (leg2 != null) break;
                    }

                    if (leg2 != null)
                    {
                        foreach (var step in leg2)
                            grid.Place(step.Cell, step.Anchor.X, step.Anchor.Y, grid.NextGroupId());
                        var combined = new List<PathStep>(leg1.Count + leg2.Count);
                        combined.AddRange(leg1);
                        combined.AddRange(leg2);
                        success = new BranchPlacement
                        {
                            Steps = combined,
                            CombatAnchor = posCandidate,
                        };
                    }
                    else
                    {
                        foreach (var step in leg1)
                            ClearFootprint(grid, step.Cell, step.Anchor);
                    }
                }

                if (success.HasValue) return success;

                // Every (nodeA, nodeB) at this position failed to close.
                // Roll the combat back and try a fresh position.
                ClearFootprint(grid, branchCombat, posCandidate);
            }

            return null;
        }

        // Aux branch tuning
        private const int MaxAuxBranchAttempts = 4;
        private const int MaxAuxNodeManhattan = 10;
        private const int MinAuxNodeManhattan = 3;
        private const int MinDeadEndDepth = 3;
        private const int MaxDeadEndDepth = 6;
        // Per-aux-branch internal retries.
        private const int AuxNodePickAttempts = 12;
        private const int AuxCombatPositionAttempts = 8;

        /// <summary>
        /// Best-effort aux branches off the spine or branch 1 only. Each is a
        /// closed loop or a chest-terminated dead end.
        /// </summary>
        private static void TryAddAuxBranches(DungeonGrid grid, List<DungeonPath> paths, EdgeCost costFn, HashSet<Point> borderBlocked, int gridCols, int gridRows, Random rand)
        {
            int parentLimit = System.Math.Min(2, paths.Count);
            if (parentLimit == 0) return;

            int placed = 0;
            for (int attempt = 0; attempt < MaxAuxBranchAttempts; attempt++)
            {
                var parent = paths[rand.Next(parentLimit)];
                bool closedLoop = rand.Next(2) == 0;
                int newId = paths.Count + 1;

                DungeonPath result = closedLoop
                    ? TryAddClosedLoopAux(grid, parent, newId, costFn, borderBlocked, gridCols, gridRows, rand)
                    : TryAddDeadEndAux(grid, parent, newId, costFn, borderBlocked, gridCols, gridRows, rand);

                if (result != null)
                {
                    paths.Add(result);
                    placed++;
                }
            }

            Terraria.ModLoader.Logging.PublicLogger.Info($"OvermorrowDungeon aux branches placed: {placed}/{MaxAuxBranchAttempts}");
        }

        /// <summary>
        /// Picks two nearby parent nodes and routes a closed-loop branch
        /// between them. Loops crossing the parent's combat get their own combat.
        /// </summary>
        private static DungeonPath TryAddClosedLoopAux(DungeonGrid grid, DungeonPath parent, int newId, EdgeCost costFn, HashSet<Point> borderBlocked, int gridCols, int gridRows, Random rand)
        {
            if (parent.Steps.Count < 2) return null;

            for (int npa = 0; npa < AuxNodePickAttempts; npa++)
            {
                var a = parent.Steps[rand.Next(parent.Steps.Count)];
                var b = parent.Steps[rand.Next(parent.Steps.Count)];
                if (a.Anchor == b.Anchor) continue;
                if (a.Cell is not (BookshelfCell or CorridorCell)) continue;
                if (b.Cell is not (BookshelfCell or CorridorCell)) continue;

                int manhattan = System.Math.Abs(a.Anchor.X - b.Anchor.X) + System.Math.Abs(a.Anchor.Y - b.Anchor.Y);
                if (manhattan < MinAuxNodeManhattan || manhattan > MaxAuxNodeManhattan) continue;

                bool needsCombat = parent.ClosedLoopAcrossCombat(a.Anchor, b.Anchor);

                DungeonPath placed = needsCombat
                    ? TryAuxAcrossCombat(grid, parent, a, b, newId, costFn, borderBlocked, gridCols, gridRows, rand)
                    : TryAuxSameSide(grid, parent, a, b, newId, costFn, borderBlocked, rand);
                if (placed != null) return placed;
            }
            return null;
        }

        /// <summary>Same-side closed loop: a single A* leg, no new combat.</summary>
        private static DungeonPath TryAuxSameSide(DungeonGrid grid, DungeonPath parent, PathStep a, PathStep b, int newId, EdgeCost costFn, HashSet<Point> borderBlocked, Random rand)
        {
            var legSteps = GridAStar.FindPath(grid, a.Anchor, b.Anchor, a.Cell, costFn, blocked: borderBlocked, streakLimits: StreakLimits, minStreakLimits: MinStreakLimits, maxVerticalRun: MaxVerticalRun);
            if (legSteps == null) return null;

            foreach (var s in legSteps)
                grid.Place(s.Cell, s.Anchor.X, s.Anchor.Y, grid.NextGroupId());

            return new DungeonPath(id: newId, role: PathRole.ClosedLoopBranch, parent: parent, steps: legSteps, combatAnchor: null);
        }

        /// <summary>Across-combat closed loop: place a third combat, route two A* legs through it.</summary>
        private static DungeonPath TryAuxAcrossCombat(DungeonGrid grid, DungeonPath parent, PathStep a, PathStep b, int newId, EdgeCost costFn, HashSet<Point> borderBlocked, int gridCols, int gridRows, Random rand)
        {
            var combatProto = new CombatRoom { IsFeature = true };
            int cw = combatProto.CellWidth;
            int ch = combatProto.CellHeight;
            int midX = (a.Anchor.X + b.Anchor.X) / 2;
            int midY = (a.Anchor.Y + b.Anchor.Y) / 2;

            for (int posAttempt = 0; posAttempt < AuxCombatPositionAttempts; posAttempt++)
            {
                int col = midX + rand.Next(-3, 4);
                int row = midY + rand.Next(-3, 4);

                if (col < EdgeBorder + 1 || col + cw > gridCols - EdgeBorder - 1) continue;
                if (row < EdgeBorder || row + ch > gridRows - EdgeBorder) continue;

                var pos = new Point(col, row);
                if (!FootprintAndNeighborsClear(grid, combatProto, pos)) continue;
                if (!combatProto.IsValidPlacement(grid, pos)) continue;

                var auxCombat = new CombatRoom { IsFeature = true };
                grid.Place(auxCombat, pos.X, pos.Y, grid.NextGroupId());

                var leg1 = GridAStar.FindPath(grid, a.Anchor, pos, a.Cell, costFn, blocked: borderBlocked, streakLimits: StreakLimits, minStreakLimits: MinStreakLimits, maxVerticalRun: MaxVerticalRun);
                if (leg1 == null)
                {
                    ClearFootprint(grid, auxCombat, pos);
                    continue;
                }

                foreach (var s in leg1)
                    grid.Place(s.Cell, s.Anchor.X, s.Anchor.Y, grid.NextGroupId());

                var leg2 = GridAStar.FindPath(grid, pos, b.Anchor, auxCombat, costFn, blocked: borderBlocked, streakLimits: StreakLimits, minStreakLimits: MinStreakLimits, maxVerticalRun: MaxVerticalRun);
                if (leg2 == null)
                {
                    foreach (var s in leg1)
                        ClearFootprint(grid, s.Cell, s.Anchor);
                    ClearFootprint(grid, auxCombat, pos);
                    continue;
                }

                foreach (var s in leg2)
                    grid.Place(s.Cell, s.Anchor.X, s.Anchor.Y, grid.NextGroupId());

                var combined = new List<PathStep>(leg1.Count + leg2.Count);
                combined.AddRange(leg1);
                combined.AddRange(leg2);

                return new DungeonPath(id: newId, role: PathRole.ClosedLoopBranch, parent: parent, steps: combined, combatAnchor: pos);
            }
            return null;
        }

        /// <summary>Dead-end aux branch terminating in a ChestRoom.</summary>
        private static DungeonPath TryAddDeadEndAux(DungeonGrid grid, DungeonPath parent, int newId, EdgeCost costFn, HashSet<Point> borderBlocked, int gridCols, int gridRows, Random rand)
        {
            if (parent.Steps.Count == 0) return null;

            var dirs = new (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

            for (int npa = 0; npa < AuxNodePickAttempts; npa++)
            {
                var nodeA = parent.Steps[rand.Next(parent.Steps.Count)];
                if (nodeA.Cell is not (BookshelfCell or CorridorCell)) continue;

                int dirStart = rand.Next(4);
                for (int di = 0; di < 4; di++)
                {
                    var (dx, dy) = dirs[(dirStart + di) % 4];
                    int depth = MinDeadEndDepth + rand.Next(MaxDeadEndDepth - MinDeadEndDepth + 1);
                    int tx = nodeA.Anchor.X + dx * depth;
                    int ty = nodeA.Anchor.Y + dy * depth;

                    if (tx < EdgeBorder + 1 || tx >= gridCols - EdgeBorder - 1) continue;
                    if (ty < EdgeBorder || ty >= gridRows - EdgeBorder) continue;

                    var chestProto = new ChestRoom { IsFeature = true };
                    var targetPos = new Point(tx, ty);
                    if (!FootprintAndNeighborsClear(grid, chestProto, targetPos)) continue;
                    if (!chestProto.IsValidPlacement(grid, targetPos)) continue;

                    var chest = new ChestRoom { IsFeature = true };
                    grid.Place(chest, tx, ty, grid.NextGroupId());

                    var legSteps = GridAStar.FindPath(grid, nodeA.Anchor, targetPos, nodeA.Cell, costFn, blocked: borderBlocked, streakLimits: StreakLimits, minStreakLimits: MinStreakLimits, maxVerticalRun: MaxVerticalRun);
                    if (legSteps == null)
                    {
                        ClearFootprint(grid, chest, targetPos);
                        continue;
                    }

                    foreach (var s in legSteps)
                        grid.Place(s.Cell, s.Anchor.X, s.Anchor.Y, grid.NextGroupId());

                    return new DungeonPath(id: newId, role: PathRole.DeadEndBranch, parent: parent, steps: legSteps, combatAnchor: null, placeholderAnchor: targetPos);
                }
            }
            return null;
        }

        /// <summary>
        /// True iff the room's footprint and every cardinal neighbor cell are
        /// empty. Required before pre-placing features so no foreign cell can
        /// fuse into the placement via a shared open border.
        /// </summary>
        private static bool FootprintAndNeighborsClear(DungeonGrid grid, GridRoom proto, Point anchor)
        {
            int w = proto.CellWidth;
            int h = proto.CellHeight;

            for (int sc = 0; sc < w; sc++)
            {
                for (int sr = 0; sr < h; sr++)
                {
                    var slot = grid.GetSlot(anchor.X + sc, anchor.Y + sr);
                    if (slot == null || !slot.IsEmpty) return false;
                }
            }

            var dirs = new (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
            for (int sc = 0; sc < w; sc++)
            {
                for (int sr = 0; sr < h; sr++)
                {
                    int ix = anchor.X + sc;
                    int iy = anchor.Y + sr;
                    foreach (var (dx, dy) in dirs)
                    {
                        int nx = ix + dx;
                        int ny = iy + dy;
                        // Skip cells inside the room's own footprint.
                        if (nx >= anchor.X && nx < anchor.X + w
                         && ny >= anchor.Y && ny < anchor.Y + h) continue;
                        var slot = grid.GetSlot(nx, ny);
                        if (slot == null) continue;   // off-grid is empty
                        if (!slot.IsEmpty) return false;
                    }
                }
            }
            return true;
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

        /// <summary>Fisher-Yates in-place shuffle.</summary>
        private static void ShuffleInPlace<T>(List<T> list, Random rand)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary>Counts CombatRoom anchors on the grid (sub-cells excluded).</summary>
        private static int CountCombatRooms(DungeonGrid grid)
        {
            int count = 0;
            for (int c = 0; c < grid.Cols; c++)
            {
                for (int r = 0; r < grid.Rows; r++)
                {
                    var s = grid.GetSlot(c, r);
                    if (s == null || s.IsEmpty) continue;
                    if (s.Room is CombatRoom && s.SubCol == 0 && s.SubRow == 0) count++;
                }
            }
            return count;
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

        /// <summary>Every (cell, side) where the cell has an open side facing an empty neighbor.</summary>
        private static HashSet<(Point cell, Direction side)> CollectDeadEndSides(DungeonGrid grid)
        {
            var sides = new HashSet<(Point, Direction)>();
            var dirs = new (Direction side, int dx, int dy)[]
            {
                (Direction.Top,    0, -1),
                (Direction.Bottom, 0,  1),
                (Direction.Left,  -1, 0),
                (Direction.Right,  1, 0),
            };
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Cols; col++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot == null || slot.IsEmpty) continue;
                    foreach (var d in dirs)
                    {
                        if (!slot.Room.IsOpenSide(slot.SubCol, slot.SubRow, d.side)) continue;
                        var n = grid.GetSlot(col + d.dx, row + d.dy);
                        if (n != null && !n.IsEmpty) continue;
                        sides.Add((new Point(col, row), d.side));
                    }
                }
            }
            return sides;
        }

        /// <summary>Paints stone over capped sides. Runs after PaddingBuilder.</summary>
        private static void ApplySideCaps(DungeonGrid grid, HashSet<(Point cell, Direction side)> sidesToCap, int fillTileType)
        {
            ushort fill = (ushort)fillTileType;

            foreach (var (cellPos, side) in sidesToCap)
            {
                var slot = grid.GetSlot(cellPos.X, cellPos.Y);
                if (slot == null || slot.IsEmpty) continue;
                if (slot.Room.OwnsPadding) continue;

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
                    {
                        int sy = topY + s * 10 + 10;
                        ClearObjectFootprint(stairX, sy, 14, 10);
                        WorldGen.PlaceObject(stairX, sy, diagonalStairsType);
                    }

                    ClearObjectFootprint(capX, topY, 5, 4);
                    WorldGen.PlaceObject(capX, topY, stairCapType);
                }
            }
        }

        /// <summary>Clears the tile footprint for an object placed with origin at bottom-left.</summary>
        private static void ClearObjectFootprint(int x, int yBottom, int width, int height)
        {
            int yTop = yBottom - (height - 1);
            for (int lx = 0; lx < width; lx++)
                for (int ly = 0; ly < height; ly++)
                    WorldGenUtils.ClearTile(x + lx, yTop + ly);
        }
    }
}
