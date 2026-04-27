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
    /// <summary>
    /// A*-based dungeon generator.
    ///
    /// Phase 1: pre-stamps west and east doors, then plans the critical path
    ///          between them with A*, biased by an OpenSimplex noise field
    ///          and constrained by streak rules (no long runs of one cell type).
    /// Phase 2: plans branches as additional A* paths between two random
    ///          critical-path cells, routed through waypoints to form loops.
    ///          Each branch reuses the same cost field and streak rules so
    ///          the dungeon stays visually coherent.
    /// Phase 3: render — each placed cell builds its tiles, padding fills
    ///          the gaps, shaft chains get diagonal stairs.
    /// </summary>
    public static class GridGenerator
    {
        private const int BranchCount = 8;
        private const int RetriesPerBranchSlot = 3;
        private const int MinBranchSpan = 8;     // critical-path indices apart, minimum
        private const int MinBranchDepth = 3;
        private const int MaxBranchDepth = 5;    // depth = MinBranchDepth..MaxBranchDepth (rows below spine)

        // Cap on consecutive vertical moves the walker can make in any single
        // A* path. Limits the visual height of any shaft chain (with or
        // without bookshelf landings between shafts) to this many rows.
        // Applied to both the spine and every branch.
        private const int MaxVerticalRun = 2;

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
            // Pick one base row for the dungeon. Door rows and waypoint rows
            // sit close to this base, so the spine reads as "mostly flat with
            // occasional height changes" instead of zigzagging end to end.
            int doorRowMin = gridRows / 3;
            int doorRowMax = (gridRows * 2) / 3;
            int baseRow = rand.Next(doorRowMin, doorRowMax + 1);

            // Door rows can drift slightly off the base so the entry/exit
            // aren't always at the same elevation. Small offset only.
            const int MaxDoorRowOffset = 2;
            int startRow = ClampRow(baseRow + rand.Next(-MaxDoorRowOffset, MaxDoorRowOffset + 1), gridRows);
            int endRow   = ClampRow(baseRow + rand.Next(-MaxDoorRowOffset, MaxDoorRowOffset + 1), gridRows);
            var start = new Point(0, startRow);
            var endTarget = new Point(gridCols - 1, endRow);

            // Spine waypoints — intermediate columns with row offsets near
            // baseRow so the spine takes the occasional dip or climb without
            // constantly changing elevation.
            const int SpineWaypointCount = 2;
            const int MaxWaypointRowOffset = 4;
            var spineWaypoints = new List<Point>();
            for (int w = 1; w <= SpineWaypointCount; w++)
            {
                int wpCol = (gridCols * w) / (SpineWaypointCount + 1);
                int wpRow = ClampRow(baseRow + rand.Next(-MaxWaypointRowOffset, MaxWaypointRowOffset + 1), gridRows);
                spineWaypoints.Add(new Point(wpCol, wpRow));
            }

            // Pre-stamp doors at both edges. A* plans a path from the west
            // door to the east door; the east door's pre-existence is what
            // triggers the type-match arrival check inside A*.
            var startDoor = new DoorRoom();
            grid.Place(startDoor, start.X, start.Y, grid.NextGroupId());

            var endDoor = new DoorRoom();
            grid.Place(endDoor, endTarget.X, endTarget.Y, grid.NextGroupId());

            // Build a smooth per-cell cost map. Same field is reused for every
            // path planned this generation so the spine and branches feel
            // visually coherent (they all flow around the same expensive zones).
            //
            // Type weights tilt A* toward a stair/shaft mix without making
            // stairs so cheap that A* prefers them over flat horizontal travel.
            //
            // Per row of pure descent (cost ÷ rows dropped):
            //   - Shaft:  1 cell × 2.0 noise × 1.4 = ~2.8 per row
            //   - Stair:  4 cells × 2.0 × 0.7 = 5.6 per stair = 5.6 per row drop
            // Per 2 cols of horizontal advance:
            //   - Bookshelves only: 2 × 2.0 = 4.0
            //   - Stair down-right: 5.6 (drops 1 row as side effect — wasted)
            // So stairs lose against flat horizontal but stay viable for
            // vertical demand. Result: spine stays mostly flat, dips into
            // stairs only when waypoints force a row change.
            var typeWeights = new Dictionary<Type, double>
            {
                [typeof(ShaftCell)]       = 1.4,
                [typeof(DescendingStair)] = 0.7,
                [typeof(AscendingStair)]  = 0.7,
            };

            double[,] noiseField = PathfindingCost.BuildSimplexNoiseField(grid.Cols, grid.Rows, rand.Next());
            EdgeCost noiseCostFn = PathfindingCost.FromNoise(noiseField, typeWeights);

            // Wrap the noise cost so shaft candidates cost infinity (i.e. are
            // unreachable) when the candidate's own column or either adjacent
            // column already contains a shaft. This blocks both two shaft
            // chains in neighbouring columns AND a second shaft chain in the
            // same column from a different branch.
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

            EdgeCost spineCostFn = shaftAdjacencyAwareCost;
            EdgeCost branchCostFn = shaftAdjacencyAwareCost;

            // Streak rules: max 3 bookshelves and 5 corridors in a row.
            // Applied to both spine and branches. Shafts use a vertical-run
            // limit instead (see MaxVerticalRun) so the cap covers the visual
            // chain length, not just the count of consecutive shaft cells.
            var streakLimits = new Dictionary<Type, int>
            {
                [typeof(BookshelfCell)] = 3,
                [typeof(CorridorCell)]  = 5,
            };

            // Plan the spine. On success, stamp every step and record the
            // anchor positions so branches can pick endpoints from them.
            // The spine routes through randomized waypoints so it zigzags
            // vertically; each waypoint must end on a cell type that the
            // next segment can keep moving from (bookshelves only — corridors
            // would dead-end the next segment because they have no vertical
            // exits).
            var spineWaypointAcceptableTypes = new HashSet<Type> { typeof(BookshelfCell) };
            var criticalPath = new List<Point> { start };
            var spineSteps = GridAStar.FindPath(grid, start, endTarget, startDoor, spineCostFn,
                                                waypoints: spineWaypoints,
                                                streakLimits: streakLimits,
                                                waypointAcceptableTypes: spineWaypointAcceptableTypes,
                                                maxVerticalRun: MaxVerticalRun);
            if (spineSteps != null)
            {
                foreach (var step in spineSteps)
                {
                    grid.Place(step.Cell, step.Anchor.X, step.Anchor.Y, grid.NextGroupId());
                    criticalPath.Add(step.Anchor);
                }
            }

            // ─── Phase 2: branches (loops) ───────────────────────────────────
            // Each "slot" gets up to RetriesPerBranchSlot internal attempts
            // with different random endpoints — A* can return null for
            // unlucky pairs (e.g. when one endpoint is a stair anchor) and
            // a quick retry usually finds a valid pair.
            int branchesPlaced = 0;
            for (int slot = 0; slot < BranchCount; slot++)
            {
                if (criticalPath.Count <= MinBranchSpan + 1) break;
                for (int attempt = 0; attempt < RetriesPerBranchSlot; attempt++)
                {
                    if (TryPlaceBranchViaAStar(grid, criticalPath, branchCostFn, streakLimits, rand, relaxed: false))
                    {
                        branchesPlaced++;
                        break;
                    }
                }
            }

            // Guarantee at least one branch. If every regular attempt failed,
            // try harder with a maximum-span unrelaxed shape, and if that
            // still fails, drop the waypoints entirely and let A* find any
            // valid loop between the two ends of the spine.
            if (branchesPlaced == 0 && criticalPath.Count > MinBranchSpan + 1)
            {
                if (!TryPlaceMaxSpanBranch(grid, criticalPath, branchCostFn, streakLimits, rand, relaxed: false))
                    TryPlaceMaxSpanBranch(grid, criticalPath, branchCostFn, streakLimits, rand, relaxed: true);
            }

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

            // Diagnostic dump of the final grid state. Output sits next to
            // the existing shaft dump in the tModLoader root folder.
            try
            {
                string dumpPath = System.IO.Path.Combine(
                    Terraria.Main.SavePath, "OvermorrowDungeonGridDump.txt");
                GridDiagnostics.DumpFullGrid(grid, dumpPath);
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

        /// <summary>
        /// Plans one branch via A*. Picks two random points on the critical
        /// path that are at least <see cref="MinBranchSpan"/> COLUMNS apart
        /// (column distance, not list-index distance — the spine can wiggle
        /// vertically through stairs, so adjacent indices may be on the same
        /// column). Generates U-shape waypoints in a detour zone below the
        /// spine, runs A* from start through the waypoints back to end, and
        /// stamps every step on success.
        /// </summary>
        private static bool TryPlaceBranchViaAStar(
            DungeonGrid grid,
            List<Point> criticalPath,
            EdgeCost costFn,
            IReadOnlyDictionary<Type, int> streakLimits,
            Random rand,
            bool relaxed)
        {
            if (criticalPath.Count < 2) return false;

            int iStart = rand.Next(criticalPath.Count);
            var startPos = criticalPath[iStart];

            // Collect every other index whose column is far enough away.
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
                                         costFn, streakLimits, rand, relaxed);
        }

        /// <summary>
        /// Last-resort branch placement. Walks pairs of indices inward from
        /// both ends of the critical path until a valid pair is found and
        /// successfully placed, or the search budget runs out. Used when
        /// every regular branch attempt failed.
        /// </summary>
        private static bool TryPlaceMaxSpanBranch(
            DungeonGrid grid,
            List<Point> criticalPath,
            EdgeCost costFn,
            IReadOnlyDictionary<Type, int> streakLimits,
            Random rand,
            bool relaxed)
        {
            if (criticalPath.Count < 4) return false;

            // Try (1, last-1), (2, last-1), (1, last-2), (2, last-2), etc.
            // Up to 5x5 = 25 pairs from the ends inward.
            for (int frontOffset = 1; frontOffset <= 5; frontOffset++)
            {
                for (int backOffset = 1; backOffset <= 5; backOffset++)
                {
                    int iStart = frontOffset;
                    int iEnd = criticalPath.Count - 1 - backOffset;
                    if (iEnd <= iStart) continue;
                    if (TryPlaceBranchBetween(grid, criticalPath, iStart, iEnd,
                                              costFn, streakLimits, rand, relaxed))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Shared branch-placement logic used by both the regular and
        /// fallback branch flows. Validates the two indexed endpoints,
        /// builds the U-shape waypoints (or skips them in relaxed mode),
        /// runs A*, and stamps the result.
        /// </summary>
        private static bool TryPlaceBranchBetween(
            DungeonGrid grid,
            List<Point> criticalPath,
            int iStart,
            int iEnd,
            EdgeCost costFn,
            IReadOnlyDictionary<Type, int> streakLimits,
            Random rand,
            bool relaxed)
        {
            if (iStart < 0 || iEnd >= criticalPath.Count || iEnd <= iStart) return false;

            var branchStart = criticalPath[iStart];
            var branchEnd   = criticalPath[iEnd];

            // Both endpoints must be 1x1 walkable cells (not stair anchors,
            // not doors). Branches can't cleanly leave from a stair because
            // stairs only have one exit direction.
            var startSlot = grid.GetSlot(branchStart.X, branchStart.Y);
            var endSlot   = grid.GetSlot(branchEnd.X,   branchEnd.Y);
            if (startSlot == null || startSlot.IsEmpty) return false;
            if (endSlot   == null || endSlot.IsEmpty)   return false;
            if (startSlot.Room is not (BookshelfCell or CorridorCell)) return false;
            if (endSlot.Room   is not (BookshelfCell or CorridorCell)) return false;

            // Need horizontal separation so the U-shape has room to go down,
            // across, and back up.
            if (Math.Abs(branchEnd.X - branchStart.X) < 2) return false;

            // U-shape waypoints: drop down at the start column, then across
            // at the bottom, then implicit climb back up to branchEnd.
            // Relaxed mode skips waypoints — A* finds any valid loop.
            List<Point> waypoints = null;
            if (!relaxed)
            {
                int depth = MinBranchDepth + rand.Next(MaxBranchDepth - MinBranchDepth + 1);
                int detourY = Math.Min(branchStart.Y + depth, grid.Rows - 2);
                waypoints = new List<Point>
                {
                    new Point(branchStart.X, detourY),
                    new Point(branchEnd.X,   detourY),
                };
            }

            // Branches must terminate each waypoint on a cell that can keep
            // walking horizontally — otherwise the next segment can't turn.
            // Shafts and stairs only have vertical or single-direction exits,
            // so we whitelist bookshelf and corridor here.
            var waypointAcceptableTypes = new HashSet<Type>
            {
                typeof(BookshelfCell),
                typeof(CorridorCell),
            };

            var path = GridAStar.FindPath(grid, branchStart, branchEnd, startSlot.Room, costFn,
                                          waypoints: waypoints,
                                          streakLimits: streakLimits,
                                          waypointAcceptableTypes: waypointAcceptableTypes,
                                          maxVerticalRun: MaxVerticalRun);
            if (path == null) return false;

            foreach (var step in path)
                grid.Place(step.Cell, step.Anchor.X, step.Anchor.Y, grid.NextGroupId());
            return true;
        }

        /// <summary>
        /// Clamps a row index into the legal grid range, leaving a 1-row
        /// margin from the top and bottom so cells with vertical footprints
        /// (e.g. 2-row stairs) still have room to fit.
        /// </summary>
        private static int ClampRow(int row, int gridRows)
        {
            return Math.Max(1, Math.Min(gridRows - 2, row));
        }

        /// <summary>
        /// Returns true if the given column contains any ShaftCell anywhere
        /// in its length. Used by the shaft-adjacency cost rule to keep
        /// shafts from being placed in columns next to existing shaft chains.
        /// </summary>
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

            // Tracks which shaft cells are already part of a processed passage.
            // A "passage" can span multiple consecutive shafts AND bookshelf
            // landings between them, so a single pass may decorate many shafts.
            var resolved = new HashSet<Point>();

            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot.IsEmpty || slot.Room is not ShaftCell) continue;
                    if (resolved.Contains(new Point(col, row))) continue;

                    // Walk up: step through shafts; also step through a single
                    // bookshelf if there's another shaft on its far side
                    // (that bookshelf acts as a landing inside the passage).
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

                    // Mark every shaft cell in the passage so later iterations
                    // don't redecorate any of them.
                    for (int r = topRow; r <= bottomRow; r++)
                    {
                        var s = grid.GetSlot(col, r);
                        if (s != null && !s.IsEmpty && s.Room is ShaftCell)
                            resolved.Add(new Point(col, r));
                    }

                    var topRoom = grid.GetSlot(col, topRow - 1);
                    var bottomRoom = grid.GetSlot(col, bottomRow + 1);

                    // A shaft chain that doesn't have a real room on both
                    // ends has nowhere meaningful for stairs to lead, so we
                    // skip decoration. A* should not produce these — if it
                    // does, the missing room is the bug to fix, not the
                    // stairs to fake.
                    if (topRoom == null || topRoom.IsEmpty || bottomRoom == null || bottomRoom.IsEmpty)
                        continue;

                    Point topRoomOrigin = grid.GridToWorld(col, topRow - 1);
                    Point bottomRoomOrigin = grid.GridToWorld(col, bottomRow + 1);

                    int topY = topRoomOrigin.Y + DungeonGrid.CellTileHeight - 1;
                    int bottomY = bottomRoomOrigin.Y + DungeonGrid.CellTileHeight - 1;

                    int segmentCount = (bottomY - topY) / 10;
                    int shaftCenterX = grid.GridToWorld(col, topRow).X + DungeonGrid.CellTileWidth / 2;
                    int stairX = shaftCenterX - 7;
                    int capX = shaftCenterX - 2;

                    for (int s = segmentCount - 1; s >= 0; s--)
                        WorldGen.PlaceObject(stairX, topY + s * 10 + 10, diagonalStairsType);

                    WorldGen.PlaceObject(capX, topY, stairCapType);
                }
            }
        }
    }
}
