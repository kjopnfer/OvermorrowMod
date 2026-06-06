using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.NPCs;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>A generated connection door: the tile that resolves its entity.</summary>
    public readonly struct DoorPlacement
    {
        public readonly Point DoorTile;

        public DoorPlacement(Point doorTile)
        {
            DoorTile = doorTile;
        }
    }

    /// <summary>
    /// A fully planned dungeon in local grid coordinates, not yet painted into the
    /// world. Produced by <see cref="GridGenerator.Plan"/> and consumed by
    /// <see cref="GridGenerator.Render"/> once the layout has chosen a world origin.
    /// </summary>
    public sealed class DungeonPlan
    {
        /// <summary>Stone border (tiles) painted around the occupied cells on every side.</summary>
        public const int StoneMargin = 24;

        public DungeonGrid Grid;
        public DungeonContent Content;

        /// <summary>Inclusive occupied-cell bounds (the dungeon's real footprint).</summary>
        public Point BoundsMin;
        public Point BoundsMax;

        /// <summary>Local cell the player spawns in when this is the starting dungeon.</summary>
        public Point SpawnAnchor;

        /// <summary>Local door cell per connection direction.</summary>
        public Dictionary<LayoutDirection, Point> DoorAnchors;

        public int FootprintWidth => (BoundsMax.X - BoundsMin.X) * DungeonGrid.HorizontalSpacing + DungeonGrid.CellTileWidth + StoneMargin * 2;
        public int FootprintHeight => (BoundsMax.Y - BoundsMin.Y) * DungeonGrid.VerticalSpacing + DungeonGrid.CellTileHeight + StoneMargin * 2;
    }

    /// <summary>A*-based dungeon generator. Critical-path / spine only.</summary>
    public static class GridGenerator
    {
        /// <summary>
        /// Width of the un-buildable border ring.
        /// </summary>
        private const int EdgeBorder = 3;

        /// <summary>
        /// Empty rows reserved above and below the spine on the scratch grid so forks
        /// have room to branch out. Trimmed away by the footprint crop.
        /// </summary>
        private const int ForkHeadroom = 12;

        /// <summary>
        /// The content being built, used by the room-creating helpers. Assigned at the start of each <see cref="Plan"/> call.
        /// </summary>
        private static DungeonContent ActiveContent;

        // Tuning and required-room set for the current build, assigned from ActiveContent at the start of Build.
        private static IReadOnlyDictionary<Type, double> TypeWeights;
        private static IReadOnlyDictionary<Type, int> StreakLimits;
        private static IReadOnlyDictionary<Type, int> MinStreakLimits;
        private static int MaxVerticalRun;
        private static List<Func<GridRoom>> RequiredRooms;

        /// <summary>
        /// Plans one dungeon in local grid coordinates without painting any tiles. The spine
        /// is generated on a scratch grid with vertical headroom on each side so forks can
        /// branch out; the occupied region is measured into the returned plan's bounds. Call
        /// <see cref="Render"/> with a chosen world origin to paint it.
        /// </summary>
        /// <param name="doorDirections">
        /// The layout directions this dungeon needs a door for. East/West become the spine's
        /// end/start endpoints; every other direction becomes a fork branch toward it.
        /// </param>
        public static DungeonPlan Plan(DungeonContent content, Random rand, IReadOnlyCollection<LayoutDirection> doorDirections)
        {
            var dirSet = new HashSet<LayoutDirection>(doorDirections);
            bool doorAtStart = dirSet.Contains(LayoutDirection.West);
            bool doorAtEnd = dirSet.Contains(LayoutDirection.East);

            ActiveContent = content;
            TypeWeights = content.TypeWeights;
            StreakLimits = content.StreakLimits;
            MinStreakLimits = content.MinStreakLimits;
            MaxVerticalRun = content.MaxVerticalRun;
            RequiredRooms = content.RequiredRooms;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            int margin = DungeonGrid.HorizontalPadding;

            // Scratch grid: the spine's natural size plus ForkHeadroom empty cells on every side,
            // giving forks open canvas to branch into. The spine is centered; the occupied region is
            // measured and cropped by Render, so the unused headroom never reaches the world.
            int gridCols = content.Cols + 2 * ForkHeadroom;
            int gridRows = content.Rows + 2 * ForkHeadroom;
            var grid = new DungeonGrid(gridCols, gridRows, new Point(margin, margin));

            // Phase 1: critical path. Center the spine vertically so the headroom above and
            // below stays clear for forks in either direction.
            int doorRowMin = gridRows / 2 - 3;
            int doorRowMax = gridRows / 2 + 3;
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
            GridRoom startDoor = null;
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
                // Spine endpoints are inset by ForkHeadroom so its natural width stays unchanged while
                // leaving open columns on each side for horizontal (intercardinal) fork branches.
                start = new Point(ForkHeadroom + EdgeBorder, startRow);
                endTarget = new Point(gridCols - 1 - EdgeBorder - ForkHeadroom, endRow);

                // Each endpoint is a door or a plain filler room.
                startDoor = doorAtStart ? content.CreateDoor(true) : content.CreateFiller(true);
                grid.Place(startDoor, start.X, start.Y, grid.NextGroupId());
                var endDoor = doorAtEnd ? content.CreateDoor(true) : content.CreateFiller(true);
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
                                if (s.Room.Type == RoomType.Door) continue;  // doors stay
                                if ((c == start.X && r == start.Y) || (c == endTarget.X && r == endTarget.Y)) continue;  // endpoints stay
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
                        if (candidate.Type == RoomType.VerticalConnector
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
                    if (candidate.Type == RoomType.VerticalConnector
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

            // The player spawns on the floor of a non-door endpoint; if both endpoints are
            // doors, spawn in the most-central spine room instead.
            Point spawnAnchor;
            if (!doorAtStart) spawnAnchor = start;
            else if (!doorAtEnd) spawnAnchor = endTarget;
            else
            {
                spawnAnchor = start;
                int centerCol = gridCols / 2;
                int bestDist = int.MaxValue;
                if (spineSteps != null)
                    foreach (var step in spineSteps)
                    {
                        if (step.Cell.Type is not (RoomType.Filler or RoomType.HorizontalConnector)) continue;
                        int d = System.Math.Abs(step.Anchor.X - centerCol);
                        if (d < bestDist) { bestDist = d; spawnAnchor = step.Anchor; }
                    }
            }

            // Path graph: wraps spine + branches so aux placement can address them by identity.
            var paths = new List<DungeonPath>();
            Point? spineCombatAnchor = null;
            if (requiredAnchors != null)
            {
                foreach (var anchor in requiredAnchors)
                {
                    var slot = grid.GetSlot(anchor.X, anchor.Y);
                    if (slot != null && !slot.IsEmpty && slot.Room.Type == RoomType.Combat)
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

            // Doors keyed by direction (anchors only). Endpoint doors are the spine
            // start/end; every other direction is a fork branch toward it.
            var doorAnchors = new Dictionary<LayoutDirection, Point>();
            if (doorAtStart) doorAnchors[LayoutDirection.West] = start;
            if (doorAtEnd) doorAnchors[LayoutDirection.East] = endTarget;

            // Fork legs route on plain noise with no elevation term, so a branch is free to leave the
            // spine's row rather than being pulled back toward it.
            EdgeCost forkCostFn = PathfindingCost.FromNoise(noiseField, TypeWeights);

            foreach (var dir in dirSet)
            {
                if (dir == LayoutDirection.East || dir == LayoutDirection.West) continue;
                Point? forkAnchor = TryAddForkDoor(grid, paths, dir, start, forkCostFn, borderBlocked, gridCols, gridRows, rand);
                if (forkAnchor.HasValue)
                    doorAnchors[dir] = forkAnchor.Value;
                else
                    Terraria.ModLoader.Logging.PublicLogger.Warn($"OvermorrowDungeon: could not place {dir} fork door; connection unpaired.");
            }

            int finalCombats = CountCombatRooms(grid);
            bool finalMandatory = !HasCombatBypass(grid, start, endTarget);
            bool finalTreeShaped = IsTreeShaped(grid, start);
            Terraria.ModLoader.Logging.PublicLogger.Info($"OvermorrowDungeon final: combats={finalCombats} mandatory={finalMandatory} tree={finalTreeShaped}");

            stopwatch.Stop();
            DumpGrid(grid, baseRow, start, endTarget, requiredAnchors, elevation, stopwatch.ElapsedMilliseconds);

            // Measure the occupied region: the dungeon's real footprint.
            int minCol = int.MaxValue, minRow = int.MaxValue, maxCol = int.MinValue, maxRow = int.MinValue;
            for (int c = 0; c < grid.Cols; c++)
            {
                for (int r = 0; r < grid.Rows; r++)
                {
                    var s = grid.GetSlot(c, r);
                    if (s == null || s.IsEmpty) continue;
                    if (c < minCol) minCol = c;
                    if (c > maxCol) maxCol = c;
                    if (r < minRow) minRow = r;
                    if (r > maxRow) maxRow = r;
                }
            }
            if (minCol > maxCol) { minCol = maxCol = EdgeBorder; minRow = maxRow = gridRows / 2; }

            return new DungeonPlan
            {
                Grid = grid,
                Content = content,
                BoundsMin = new Point(minCol, minRow),
                BoundsMax = new Point(maxCol, maxRow),
                SpawnAnchor = spawnAnchor,
                DoorAnchors = doorAnchors,
            };
        }

        /// <summary>
        /// Paints a planned dungeon into the world at <paramref name="worldOrigin"/> (the
        /// top-left of its footprint), then resolves the spawn tile and the door placements.
        /// </summary>
        public static void Render(DungeonPlan plan, Point worldOrigin, Random rand, out Point spawnTile, out Dictionary<LayoutDirection, DoorPlacement> doors)
        {
            var grid = plan.Grid;
            var content = plan.Content;
            int fillTileType = content.FillTile;
            int liningTileType = content.LiningTile;
            float baseDensity = content.BaseDensity;
            float eliteChance = content.EliteChance;
            IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool> bindings = content.SpawnBindings;
            DungeonPalette palette = content.Palette;
            int margin = DungeonPlan.StoneMargin;

            // Rebase the grid so the occupied region's top-left cell sits one margin inside worldOrigin.
            grid.Origin = new Point(
                worldOrigin.X + margin - plan.BoundsMin.X * DungeonGrid.HorizontalSpacing,
                worldOrigin.Y + margin - plan.BoundsMin.Y * DungeonGrid.VerticalSpacing);

            // Fill stone over the footprint only.
            ushort fill = (ushort)fillTileType;
            for (int x = 0; x < plan.FootprintWidth; x++)
                for (int y = 0; y < plan.FootprintHeight; y++)
                    WorldGenUtils.PlaceTile(worldOrigin.X + x, worldOrigin.Y + y, fill);

            // Render rooms.
            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot.IsEmpty) continue;
                    if (slot.SubCol != 0 || slot.SubRow != 0) continue;

                    Point cellOrigin = grid.GridToWorld(col, row);
                    slot.Room.Build(new BuildContext(cellOrigin, palette, fillTileType, liningTileType));
                }
            }

            PaddingBuilder.BuildAll(grid, fillTileType, palette);
            DecorateShafts(grid);
            ApplySideCaps(grid, CollectDeadEndSides(grid), fillTileType);

            // Place furniture.
            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot.IsEmpty) continue;
                    if (slot.SubCol != 0 || slot.SubRow != 0) continue;

                    Point cellOrigin = grid.GridToWorld(col, row);
                    slot.Room.PlaceFurniture(new FurnitureContext(cellOrigin, grid, col, row, fillTileType, liningTileType, palette));
                }
            }

            // NPC spawning temporarily disabled (was freezing/crashing on movement).
            // To re-enable, restore the spawn-slot harvest + EncounterSelector.Run below.
            //
            // var allSlots = new List<SpawnSlot>();
            // var cellLocalBindings = new Dictionary<Point, IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool>>();
            // for (int col = 0; col < grid.Cols; col++)
            //     for (int row = 0; row < grid.Rows; row++)
            //     {
            //         var slot = grid.GetSlot(col, row);
            //         if (slot.IsEmpty || slot.SubCol != 0 || slot.SubRow != 0) continue;
            //         Point cellOrigin = grid.GridToWorld(col, row);
            //         var ctx = new FurnitureContext(cellOrigin, grid, col, row, fillTileType, liningTileType, palette);
            //         slot.Room.PlaceSpawns(ctx, allSlots);
            //         var local = slot.Room.GetSpawnBindings();
            //         if (local != null) cellLocalBindings[new Point(col, row)] = local;
            //     }
            // EncounterSelector.Run(allSlots, bindings, cellLocalBindings, baseDensity, eliteChance, rand);
            _ = bindings; _ = baseDensity; _ = eliteChance;

            Point spawnCellOrigin = grid.GridToWorld(plan.SpawnAnchor.X, plan.SpawnAnchor.Y);
            spawnTile = new Point(spawnCellOrigin.X + DungeonGrid.CellTileWidth / 2, spawnCellOrigin.Y + DungeonGrid.CellTileHeight - 4);

            doors = new Dictionary<LayoutDirection, DoorPlacement>();
            foreach (var kv in plan.DoorAnchors)
                doors[kv.Key] = MakeDoorPlacement(grid, kv.Value);
        }

        /// <summary>Writes the diagnostic grid dump; failures are logged, not thrown.</summary>
        private static void DumpGrid(DungeonGrid grid, int baseRow, Point start, Point endTarget, List<Point> requiredAnchors, double[] elevation, long elapsedMs)
        {
            try
            {
                var anchors = requiredAnchors ?? new List<Point>();
                var config = new GenerationConfig
                {
                    BaseRow = baseRow,
                    StartDoor = start,
                    EndDoor = endTarget,
                    SpineWaypoints = new List<Point>(anchors),
                    RequiredRoomAnchors = new List<Point>(anchors),
                    Elevation = elevation,
                    ElapsedMilliseconds = elapsedMs,
                };
                string dumpFolder = System.IO.Path.Combine(Terraria.Main.SavePath, "OvermorrowDungeonDumps");
                System.IO.Directory.CreateDirectory(dumpFolder);
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
                    if (nextSlot.Room.Type == RoomType.Combat) continue;
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
                if (slot != null && !slot.IsEmpty && slot.Room.Type == RoomType.Combat)
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
                if (step.Cell.Type is not (RoomType.Filler or RoomType.HorizontalConnector)) continue;
                int x = step.Anchor.X;
                if (x < spineCombatAnchor.X - 1) beforeNodes.Add(step);
                else if (x > spineCombatRight + 1) afterNodes.Add(step);
            }
            if (beforeNodes.Count == 0 || afterNodes.Count == 0) return null;

            var branchCombatProto = ActiveContent.CreateCombat(true);
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

                var branchCombat = ActiveContent.CreateCombat(true);
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

        /// <summary>Minimum cell distance an extra exit door must keep from any other door.</summary>
        private const int MinExitDoorSpacing = 8;

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
                if (a.Cell.Type is not (RoomType.Filler or RoomType.HorizontalConnector)) continue;
                if (b.Cell.Type is not (RoomType.Filler or RoomType.HorizontalConnector)) continue;

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
            var combatProto = ActiveContent.CreateCombat(true);
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

                var auxCombat = ActiveContent.CreateCombat(true);
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
                if (nodeA.Cell.Type is not (RoomType.Filler or RoomType.HorizontalConnector)) continue;

                int dirStart = rand.Next(4);
                for (int di = 0; di < 4; di++)
                {
                    var (dx, dy) = dirs[(dirStart + di) % 4];
                    int depth = MinDeadEndDepth + rand.Next(MaxDeadEndDepth - MinDeadEndDepth + 1);
                    int tx = nodeA.Anchor.X + dx * depth;
                    int ty = nodeA.Anchor.Y + dy * depth;

                    if (tx < EdgeBorder + 1 || tx >= gridCols - EdgeBorder - 1) continue;
                    if (ty < EdgeBorder || ty >= gridRows - EdgeBorder) continue;

                    var chestProto = ActiveContent.CreateTreasure(true);
                    var targetPos = new Point(tx, ty);
                    if (!FootprintAndNeighborsClear(grid, chestProto, targetPos)) continue;
                    if (!chestProto.IsValidPlacement(grid, targetPos)) continue;

                    var chest = ActiveContent.CreateTreasure(true);
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

        // Per-candidate retries of the sub-spine before giving up on that attach cell.
        private const int ForkSpineAttempts = 4;

        /// <summary>
        /// Places a fork door toward <paramref name="dir"/> as a secondary critical path. A short
        /// hand-built punch (shaft + landing) drops from an exposed mid-spine cell out of the dense
        /// spine band into the clear headroom; from that landing an A* sub-spine winds through two of
        /// the dungeon's required rooms to a horizontal door (so it reads like a second spine). Every
        /// attach cell is retried several times for a long, feature-rich branch; only if none can grow
        /// one does a short fallback door place, so the connection is never lost. Attaches away from
        /// the spine start so the door tends toward the middle/end. Returns the door anchor, or null.
        /// </summary>
        private static Point? TryAddForkDoor(DungeonGrid grid, List<DungeonPath> paths, LayoutDirection dir, Point spineStart, EdgeCost costFn, HashSet<Point> borderBlocked, int gridCols, int gridRows, Random rand)
        {
            int parentLimit = System.Math.Min(2, paths.Count);
            if (parentLimit == 0) return null;

            Point delta = dir.Delta();
            int dy = System.Math.Sign(delta.Y);
            if (dy == 0) return null;  // forks always carry a vertical component
            int dx = delta.X == 0 ? 1 : System.Math.Sign(delta.X);

            // Intercardinal forks (a horizontal component in the direction) read as side branches and
            // lean horizontal; only a true North/South fork leans into the vertical headroom.
            bool favorHorizontal = delta.X != 0;

            const int ForkStartMargin = 6;   // keep misc doors away from the entrance

            // Exposed spine fillers at the band's edge: the column is open for three cells in the
            // fork's direction, so a one-pair punch lands in clear headroom. Away from the start so
            // the door tends toward the middle/end rather than the beginning.
            var candidates = new List<PathStep>();
            for (int p = 0; p < parentLimit; p++)
                foreach (var step in paths[p].Steps)
                {
                    if (step.Cell.Type != RoomType.Filler) continue;
                    if (System.Math.Abs(step.Anchor.X - spineStart.X) < ForkStartMargin) continue;
                    int x = step.Anchor.X, y = step.Anchor.Y;
                    if (!IsEmptyCell(grid, x, y + dy)) continue;
                    if (!IsEmptyCell(grid, x, y + 2 * dy)) continue;
                    if (!IsEmptyCell(grid, x, y + 3 * dy)) continue;
                    candidates.Add(step);
                }
            ShuffleInPlace(candidates, rand);
            int tries = System.Math.Min(candidates.Count, AuxNodePickAttempts);

            // First pass: try hard for a long, feature-rich sub-spine. Each attach cell gets several
            // randomized attempts; a chain shorter than the minimum span is rejected and retried.
            for (int t = 0; t < tries; t++)
            {
                int c = candidates[t].Anchor.X;
                int r = candidates[t].Anchor.Y;
                Point shaftPos = new Point(c, r + dy);
                Point b0 = new Point(c, r + 2 * dy);

                var shaft = ActiveContent.CreateVerticalConnector(false);
                var landing = ActiveContent.CreateFiller(false);
                grid.Place(shaft, shaftPos.X, shaftPos.Y, grid.NextGroupId());
                grid.Place(landing, b0.X, b0.Y, grid.NextGroupId());
                var prefix = new List<PathStep> { new PathStep(shaft, shaftPos), new PathStep(landing, b0) };

                for (int attempt = 0; attempt < ForkSpineAttempts; attempt++)
                {
                    Point? doorAnchor = TryHeadroomSpine(grid, b0, landing, dx, dy, favorHorizontal, costFn, borderBlocked, gridCols, gridRows, rand, prefix, paths);
                    if (doorAnchor.HasValue) return doorAnchor;
                }

                ClearFootprint(grid, landing, b0);
                ClearFootprint(grid, shaft, shaftPos);
            }

            // Last resort: a short door beside the landing so the connection is never lost. Rare,
            // since the headroom almost always admits a full sub-spine.
            for (int t = 0; t < tries; t++)
            {
                int c = candidates[t].Anchor.X;
                int r = candidates[t].Anchor.Y;
                Point shaftPos = new Point(c, r + dy);
                Point b0 = new Point(c, r + 2 * dy);
                Point fb = new Point(c + dx, b0.Y);
                if (!InColBounds(gridCols, fb.X)
                    || !IsEmptyCell(grid, fb.X, fb.Y)
                    || !IsEmptyCell(grid, fb.X + dx, fb.Y)
                    || !IsEmptyCell(grid, fb.X, fb.Y - 1)
                    || !IsEmptyCell(grid, fb.X, fb.Y + 1)
                    || DoorWithinDistance(grid, fb, MinExitDoorSpacing))
                    continue;

                var shaft = ActiveContent.CreateVerticalConnector(false);
                var landing = ActiveContent.CreateFiller(false);
                var fbDoor = ActiveContent.CreateDoor(true);
                grid.Place(shaft, shaftPos.X, shaftPos.Y, grid.NextGroupId());
                grid.Place(landing, b0.X, b0.Y, grid.NextGroupId());
                grid.Place(fbDoor, fb.X, fb.Y, grid.NextGroupId());
                var steps = new List<PathStep> { new PathStep(shaft, shaftPos), new PathStep(landing, b0), new PathStep(fbDoor, fb) };
                paths.Add(new DungeonPath(id: paths.Count + 1, role: PathRole.DeadEndBranch, parent: paths[0], steps: steps, combatAnchor: null, placeholderAnchor: fb));
                Terraria.ModLoader.Logging.PublicLogger.Warn($"OvermorrowDungeon: {dir} fork fell back to a short exit (no sub-spine fit).");
                return fb;
            }
            return null;
        }

        // Feature rooms marched along a fork and the minimum it must span (cells, Manhattan from the
        // punch landing to the door) to count as a real branch rather than a stubby exit.
        private const int ForkFeatureCount = 2;
        private const int MinForkSpan = 10;

        /// <summary>
        /// Per-leg reach of a fork sub-spine. An intercardinal fork (a diagonal climb) leans
        /// horizontal so it reads as a side branch rather than a vertical plunge; a true vertical
        /// (North/South) fork leans into the tall headroom instead. Returns the min and span of the
        /// random column and row advance per leg.
        /// </summary>
        private static (int colMin, int colSpan, int rowMin, int rowSpan) ForkReach(bool favorHorizontal)
            => favorHorizontal ? (4, 4, 1, 3) : (1, 2, 3, 5);

        /// <summary>
        /// Routes the sub-spine of a fork through the clear headroom: marches <see cref="ForkFeatureCount"/>
        /// of the dungeon's required rooms outward from the punch landing <paramref name="b0"/>,
        /// A*-linking each (the legs pick up the dungeon's lounges and bookshelves), then a horizontal
        /// door past the last room. An intercardinal fork leans horizontal, a vertical fork leans into
        /// the headroom (<see cref="ForkReach"/>). Rejects (and rolls back) a chain that does not reach
        /// <see cref="MinForkSpan"/> so the result is never a stubby exit. Returns the door anchor, or
        /// null if nothing routed.
        /// </summary>
        private static Point? TryHeadroomSpine(DungeonGrid grid, Point b0, GridRoom startCell, int dx, int dy, bool favorHorizontal, EdgeCost costFn, HashSet<Point> borderBlocked, int gridCols, int gridRows, Random rand, List<PathStep> prefix, List<DungeonPath> paths)
        {
            var factories = new List<Func<GridRoom>>(RequiredRooms ?? new List<Func<GridRoom>>());
            ShuffleInPlace(factories, rand);
            int featureCount = System.Math.Min(factories.Count, ForkFeatureCount);

            var placed = new List<(GridRoom cell, Point anchor)>();
            var steps = new List<PathStep>(prefix);
            Point prevAnchor = b0;
            GridRoom prevCell = startCell;

            for (int f = 0; f < featureCount; f++)
            {
                if (!TryLinkForkNode(grid, factories[f], dx, dy, favorHorizontal, costFn, borderBlocked, gridCols, gridRows, rand, placed, steps, ref prevAnchor, ref prevCell))
                {
                    RollbackChain(grid, placed);
                    return null;
                }
            }

            Point? doorAnchor = TryLinkForkDoor(grid, prevAnchor, prevCell, dx, dy, favorHorizontal, costFn, borderBlocked, gridCols, gridRows, rand, placed, steps);
            if (!doorAnchor.HasValue)
            {
                RollbackChain(grid, placed);
                return null;
            }

            int span = System.Math.Abs(doorAnchor.Value.X - b0.X) + System.Math.Abs(doorAnchor.Value.Y - b0.Y);
            if (span < MinForkSpan)
            {
                RollbackChain(grid, placed);
                return null;
            }

            paths.Add(new DungeonPath(id: paths.Count + 1, role: PathRole.DeadEndBranch, parent: paths[0], steps: steps, combatAnchor: null, placeholderAnchor: doorAnchor.Value));
            return doorAnchor;
        }

        /// <summary>
        /// Places one feature room (built by <paramref name="factory"/>, marked a planner feature so
        /// corridors may dock against it) a leg's reach toward (<paramref name="dx"/>, <paramref name="dy"/>)
        /// from the chain's current end, then A*-routes the corridor that links them. On success the
        /// room and corridor are committed, appended to <paramref name="placed"/>/<paramref name="steps"/>,
        /// and the chain end advances. Returns false (grid left clean for this node) if nothing fit.
        /// </summary>
        private static bool TryLinkForkNode(DungeonGrid grid, Func<GridRoom> factory, int dx, int dy, bool favorHorizontal, EdgeCost costFn, HashSet<Point> borderBlocked, int gridCols, int gridRows, Random rand, List<(GridRoom cell, Point anchor)> placed, List<PathStep> steps, ref Point prevAnchor, ref GridRoom prevCell)
        {
            var proto = factory();
            proto.IsFeature = true;
            int cw = proto.CellWidth;
            int ch = proto.CellHeight;
            var (colMin, colSpan, rowMin, rowSpan) = ForkReach(favorHorizontal);

            const int Attempts = 12;
            for (int i = 0; i < Attempts; i++)
            {
                int col = prevAnchor.X + dx * (colMin + rand.Next(colSpan));
                int row = prevAnchor.Y + dy * (rowMin + rand.Next(rowSpan));
                if (col < EdgeBorder + 1 || col + cw > gridCols - EdgeBorder - 1) continue;
                if (row < EdgeBorder || row + ch > gridRows - EdgeBorder) continue;

                var pos = new Point(col, row);
                if (!FootprintAndNeighborsClear(grid, proto, pos)) continue;
                if (!proto.IsValidPlacement(grid, pos)) continue;

                var node = factory();
                node.IsFeature = true;
                grid.Place(node, pos.X, pos.Y, grid.NextGroupId());

                var leg = GridAStar.FindPath(grid, prevAnchor, pos, prevCell, costFn, blocked: borderBlocked, streakLimits: StreakLimits, minStreakLimits: MinStreakLimits, maxVerticalRun: MaxVerticalRun);
                if (leg == null)
                {
                    ClearFootprint(grid, node, pos);
                    continue;
                }
                foreach (var s in leg)
                {
                    grid.Place(s.Cell, s.Anchor.X, s.Anchor.Y, grid.NextGroupId());
                    placed.Add((s.Cell, s.Anchor));
                    steps.Add(s);
                }
                placed.Add((node, pos));
                steps.Add(new PathStep(node, pos));
                prevAnchor = pos;
                prevCell = node;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Caps the fork with a horizontal door a leg's reach past the chain's end, A*-routing the
        /// final corridor to it. Committed cells are appended to <paramref name="placed"/>/<paramref name="steps"/>.
        /// Returns the door anchor, or null (grid left clean for this attempt) if nothing fit.
        /// </summary>
        private static Point? TryLinkForkDoor(DungeonGrid grid, Point prevAnchor, GridRoom prevCell, int dx, int dy, bool favorHorizontal, EdgeCost costFn, HashSet<Point> borderBlocked, int gridCols, int gridRows, Random rand, List<(GridRoom cell, Point anchor)> placed, List<PathStep> steps)
        {
            var (colMin, colSpan, _, _) = ForkReach(favorHorizontal);

            const int Attempts = 12;
            for (int i = 0; i < Attempts; i++)
            {
                // The door opens left/right, so its approach is horizontal regardless of the fork's bias.
                int col = prevAnchor.X + dx * (colMin + rand.Next(colSpan));
                int row = prevAnchor.Y + dy * rand.Next(3);
                if (!InColBounds(gridCols, col) || !InVerticalBounds(gridRows, row)) continue;

                var pos = new Point(col, row);
                if (DoorWithinDistance(grid, pos, MinExitDoorSpacing)) continue;

                var proto = ActiveContent.CreateDoor(true);
                if (!FootprintAndNeighborsClear(grid, proto, pos)) continue;
                if (!proto.IsValidPlacement(grid, pos)) continue;

                var door = ActiveContent.CreateDoor(true);
                grid.Place(door, pos.X, pos.Y, grid.NextGroupId());

                var leg = GridAStar.FindPath(grid, prevAnchor, pos, prevCell, costFn, blocked: borderBlocked, streakLimits: StreakLimits, minStreakLimits: MinStreakLimits, maxVerticalRun: MaxVerticalRun);
                if (leg == null)
                {
                    ClearFootprint(grid, door, pos);
                    continue;
                }
                foreach (var s in leg)
                {
                    grid.Place(s.Cell, s.Anchor.X, s.Anchor.Y, grid.NextGroupId());
                    placed.Add((s.Cell, s.Anchor));
                    steps.Add(s);
                }
                placed.Add((door, pos));
                steps.Add(new PathStep(door, pos));
                return pos;
            }
            return null;
        }

        private static void RollbackChain(DungeonGrid grid, List<(GridRoom cell, Point anchor)> placed)
        {
            for (int i = placed.Count - 1; i >= 0; i--)
                ClearFootprint(grid, placed[i].cell, placed[i].anchor);
        }

        private static bool IsEmptyCell(DungeonGrid grid, int x, int y)
        {
            var s = grid.GetSlot(x, y);
            return s != null && s.IsEmpty;
        }

        private static bool InVerticalBounds(int gridRows, int row) => row >= EdgeBorder && row <= gridRows - 1 - EdgeBorder;
        private static bool InColBounds(int gridCols, int col) => col >= EdgeBorder && col <= gridCols - 1 - EdgeBorder;

        /// <summary>World tile at the center of the cell at <paramref name="anchor"/>.</summary>
        private static Point CellCenterTile(DungeonGrid grid, Point anchor)
        {
            Point origin = grid.GridToWorld(anchor.X, anchor.Y);
            return new Point(origin.X + DungeonGrid.CellTileWidth / 2, origin.Y + DungeonGrid.CellTileHeight / 2);
        }

        private static DoorPlacement MakeDoorPlacement(DungeonGrid grid, Point doorAnchor) =>
            new DoorPlacement(CellCenterTile(grid, doorAnchor));

        /// <summary>True if any door anchor sits within <paramref name="minCells"/> cells of <paramref name="candidate"/>.</summary>
        private static bool DoorWithinDistance(DungeonGrid grid, Point candidate, int minCells)
        {
            for (int c = 0; c < grid.Cols; c++)
            {
                for (int r = 0; r < grid.Rows; r++)
                {
                    var s = grid.GetSlot(c, r);
                    if (s == null || s.IsEmpty) continue;
                    if (s.Room.Type != RoomType.Door) continue;
                    if (s.SubCol != 0 || s.SubRow != 0) continue;
                    if (System.Math.Abs(c - candidate.X) + System.Math.Abs(r - candidate.Y) < minCells)
                        return true;
                }
            }
            return false;
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
                    if (s.Room.Type == RoomType.Combat && s.SubCol == 0 && s.SubRow == 0) count++;
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
                if (slot != null && !slot.IsEmpty && slot.Room.Type == RoomType.VerticalConnector)
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
                    if (slot.IsEmpty || slot.Room.Type != RoomType.VerticalConnector) continue;
                    if (resolved.Contains(new Point(col, row))) continue;

                    // Walk up through shafts and through a bookshelf landing with another shaft beyond.
                    int topRow = row;
                    while (topRow > 0)
                    {
                        var above = grid.GetSlot(col, topRow - 1);
                        if (above == null || above.IsEmpty) break;

                        if (above.Room.Type == RoomType.VerticalConnector)
                        {
                            topRow--;
                            continue;
                        }

                        if (above.Room.Type == RoomType.Filler && topRow >= 2)
                        {
                            var aboveAbove = grid.GetSlot(col, topRow - 2);
                            if (aboveAbove != null && !aboveAbove.IsEmpty && aboveAbove.Room.Type == RoomType.VerticalConnector)
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

                        if (below.Room.Type == RoomType.VerticalConnector)
                        {
                            bottomRow++;
                            continue;
                        }

                        if (below.Room.Type == RoomType.Filler && bottomRow + 2 < grid.Rows)
                        {
                            var belowBelow = grid.GetSlot(col, bottomRow + 2);
                            if (belowBelow != null && !belowBelow.IsEmpty && belowBelow.Room.Type == RoomType.VerticalConnector)
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
                        if (s != null && !s.IsEmpty && s.Room.Type == RoomType.VerticalConnector)
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
