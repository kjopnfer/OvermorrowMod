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
    /// Phase 3: render. Each placed cell builds its tiles, padding fills
    ///          the gaps, and shaft chains get diagonal stairs.
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

        // Width (in grid cells) of the un-buildable border around the entire
        // grid. Cells in the outer ring are forbidden so the dungeon's outer
        // edge is always padding/stone, never a cell pressed up against the
        // canvas boundary. Doors live at the inner edge of this border.
        private const int EdgeBorder = 1;

        // Budget (in cells) for a single dead-end repair extension. Kept
        // short so repairs grow a small connecting hallway rather than a
        // whole new branch.
        private const int RepairExtensionBudget = 6;

        // Per-type cost multiplier applied during A* pathfinding.
        // Higher = rarer (more expensive to place). Lower = more common.
        // Default (no entry) is 1.0.
        //
        // Stairs sit below 1.0 so A* still picks them when vertical movement
        // is needed. Without the discount, their 4-cell footprint would
        // always lose to a single-cell shaft. Shafts sit above 1.0 so A*
        // avoids carving long vertical chains except when descent is forced
        // by a waypoint.
        private static readonly Dictionary<Type, double> TypeWeights = new()
        {
            [typeof(ShaftCell)]       = 1.4,
            [typeof(DescendingStair)] = 0.7,
            [typeof(AscendingStair)]  = 0.7,
            [typeof(FireplaceRoom)] = 1.5
        };

        // Max consecutive runs. Shafts use a vertical-run limit instead
        // (see MaxVerticalRun) so the cap covers the visual chain length.
        private static readonly Dictionary<Type, int> StreakLimits = new()
        {
            [typeof(BookshelfCell)] = 3,
            [typeof(CorridorCell)]  = 5,
            [typeof(FireplaceRoom)] = 1
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

            // Doors live at the inner edge of the canvas border
            var start = new Point(EdgeBorder, startRow);
            var endTarget = new Point(gridCols - 1 - EdgeBorder, endRow);

            // Spine waypoints: intermediate columns with row offsets near
            // baseRow so the spine takes the occasional dip or climb without
            // constantly changing elevation. Columns are spread between the
            // inner edges of the border so waypoints never land in the
            // forbidden ring.
            const int SpineWaypointCount = 2;
            const int MaxWaypointRowOffset = 4;
            int playableLeft = EdgeBorder;
            int playableRight = gridCols - 1 - EdgeBorder;
            int playableSpan = playableRight - playableLeft;
            var spineWaypoints = new List<Point>();
            for (int w = 1; w <= SpineWaypointCount; w++)
            {
                int wpCol = playableLeft + (playableSpan * w) / (SpineWaypointCount + 1);
                int wpRow = ClampRow(baseRow + rand.Next(-MaxWaypointRowOffset, MaxWaypointRowOffset + 1), gridRows);
                spineWaypoints.Add(new Point(wpCol, wpRow));
            }

            // Block the entire outer ring of cells. A* refuses to place any
            // candidate whose footprint touches a blocked cell, so the
            // dungeon's outer border is guaranteed empty.
            var borderBlocked = BuildBorderBlockedSet(gridCols, gridRows);

            // Pre-stamp doors at both edges before planning. A* plans from
            // the start door to the end door.
            var startDoor = new DoorRoom();
            grid.Place(startDoor, start.X, start.Y, grid.NextGroupId());

            var endDoor = new DoorRoom();
            grid.Place(endDoor, endTarget.X, endTarget.Y, grid.NextGroupId());

            double[,] noiseField = PathfindingCost.BuildSimplexNoiseField(grid.Cols, grid.Rows, rand.Next());
            EdgeCost noiseCostFn = PathfindingCost.FromNoise(noiseField, TypeWeights);

            // Wrap the noise cost so shaft candidates cost infinity when
            // the candidate's own column or either adjacent column already contains a shaft.
            // This blocks both two shaft chains in neighbouring columns
            // AND a second shaft chain in the same column from a different branch.
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

            // Plan the spine. On success, stamp every step and record the
            // anchor positions so branches can pick endpoints from them.
            // The spine routes through randomized waypoints so it zigzags
            // vertically; each waypoint must end on a cell type that the
            // next segment can keep moving from (bookshelves only, since
            // corridors would dead-end the next segment because they have
            // no vertical exits).
            //
            // Fallback chain: try waypointed first; if that fails, try
            // without waypoints; if THAT fails, try without waypoints AND
            // without the streak/vertical-run caps. A connected spine is
            // required, so a less-shapely spine is preferred over no spine
            // at all.
            var spineWaypointAcceptableTypes = new HashSet<Type> { typeof(BookshelfCell) };
            var criticalPath = new List<Point> { start };

            List<PathStep> spineSteps = GridAStar.FindPath(
                grid, start, endTarget, startDoor, spineCostFn,
                waypoints: spineWaypoints,
                blocked: borderBlocked,
                streakLimits: StreakLimits,
                waypointAcceptableTypes: spineWaypointAcceptableTypes,
                maxVerticalRun: MaxVerticalRun);

            if (spineSteps == null)
            {
                // First fallback: drop waypoints. Spine becomes a direct path with no forced detours.
                spineSteps = GridAStar.FindPath(
                    grid, start, endTarget, startDoor, spineCostFn,
                    blocked: borderBlocked,
                    streakLimits: StreakLimits,
                    maxVerticalRun: MaxVerticalRun);
            }

            if (spineSteps == null)
            {
                // Second fallback: drop streak and vertical-run caps.
                // Path may have long shaft chains or runs of one cell type,
                // but at least the dungeon has a connected spine.
                spineSteps = GridAStar.FindPath(
                    grid, start, endTarget, startDoor, spineCostFn,
                    blocked: borderBlocked);
            }

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
            // with different random endpoints. A* can return null for
            // unlucky pairs (for example, when one endpoint is a stair
            // anchor), and a quick retry usually finds a valid pair.
            int branchesPlaced = 0;
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

            // Guarantee at least one branch. If every regular attempt failed,
            // retry with a maximum-span unrelaxed shape, and if that
            // still fails, drop the waypoints entirely and let A* find any
            // valid loop between the two ends of the spine.
            if (branchesPlaced == 0 && criticalPath.Count > MinBranchSpan + 1)
            {
                if (!TryPlaceMaxSpanBranch(grid, criticalPath, branchCostFn, StreakLimits, borderBlocked, rand, relaxed: false))
                    TryPlaceMaxSpanBranch(grid, criticalPath, branchCostFn, StreakLimits, borderBlocked, rand, relaxed: true);
            }

            // ─── Phase 2.5: dead-end repair ──────────────────────────────────
            // Walk every placed cell. Each open side facing empty stone is a
            // visible dead-end. Shaft-chain landings get a short A* extension
            // so their decorative staircase leads into a real hallway; other
            // dead-ends get rendered as walls in the next phase.
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

            // Seal capped sides with stone walls so dead-end open sides no
            // longer read as broken doorways onto rock.
            ApplySideCaps(grid, sidesToCap, fillTileType);

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
        /// (column distance, not list-index distance, since the spine can
        /// wiggle vertically through stairs and adjacent indices may share
        /// a column). Generates U-shape waypoints in a detour zone below
        /// the spine, runs A* from start through the waypoints back to end,
        /// and stamps every step on success.
        /// </summary>
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
                                         costFn, streakLimits, blocked, rand, relaxed);
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
            HashSet<Point> blocked,
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
                                              costFn, streakLimits, blocked, rand, relaxed))
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
            HashSet<Point> blocked,
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
            // Relaxed mode skips waypoints so A* finds any valid loop.
            List<Point> waypoints = null;
            if (!relaxed)
            {
                int depth = MinBranchDepth + rand.Next(MaxBranchDepth - MinBranchDepth + 1);
                // Keep the detour row inside the playable area, leaving room
                // for 2-row stair footprints to fit without touching the
                // outer border ring.
                int detourY = Math.Min(branchStart.Y + depth, grid.Rows - 2 - EdgeBorder);
                waypoints = new List<Point>
                {
                    new Point(branchStart.X, detourY),
                    new Point(branchEnd.X,   detourY),
                };
            }

            // Branches must terminate each waypoint on a cell that can keep
            // walking horizontally; otherwise the next segment cannot turn.
            // Shafts and stairs only have vertical or single-direction
            // exits, so this whitelist is restricted to bookshelf and
            // corridor.
            var waypointAcceptableTypes = new HashSet<Type>
            {
                typeof(BookshelfCell),
                typeof(CorridorCell),
            };

            var path = GridAStar.FindPath(grid, branchStart, branchEnd, startSlot.Room, costFn,
                                          waypoints: waypoints,
                                          blocked: blocked,
                                          streakLimits: streakLimits,
                                          waypointAcceptableTypes: waypointAcceptableTypes,
                                          maxVerticalRun: MaxVerticalRun);
            if (path == null) return false;

            foreach (var step in path)
                grid.Place(step.Cell, step.Anchor.X, step.Anchor.Y, grid.NextGroupId());
            return true;
        }

        /// <summary>
        /// Clamps a row index into the legal grid range, leaving the
        /// EdgeBorder ring at the top and bottom unbuildable so cells with
        /// 2-row footprints (stairs) still have room to fit inside the
        /// playable area.
        /// </summary>
        private static int ClampRow(int row, int gridRows)
        {
            int min = EdgeBorder;
            int max = gridRows - 1 - EdgeBorder;
            // Stairs need 1 extra row of margin from each edge so their
            // 2-row footprint doesn't overlap the border ring.
            min = Math.Max(min, 1);
            max = Math.Min(max, gridRows - 2);
            return Math.Max(min, Math.Min(max, row));
        }

        /// <summary>
        /// Builds the set of grid positions that A* must never place a cell
        /// on. Currently: every cell in the outer EdgeBorder ring, so the
        /// dungeon's outer edge is always padding/stone instead of pressed
        /// up against the canvas boundary.
        /// </summary>
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
        /// Walks every placed cell. For each open side that faces empty
        /// stone, picks the cheapest repair in priority order:
        /// <list type="number">
        /// <item>Extend a short hallway from a shaft landing into existing
        /// dungeon, so the staircase leads somewhere real.</item>
        /// <item>Convert connector cells (corridor, shaft, stair) to a
        /// standalone bookshelf when the swap is structurally safe.</item>
        /// <item>Cap the side with stone at render time as a last resort.</item>
        /// </list>
        /// Returns the list of (cellAnchor, side) pairs that fell all the
        /// way through to capping.
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

            // Snapshot the current placements first. The extension loop
            // mutates the grid, and feeding repaired cells back into the
            // dead-end scan would create new false dead-ends.
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
                // Re-fetch the slot so SubCol/SubRow are correct (the room
                // reference is shared across the footprint of multi-cell pieces).
                var slot = grid.GetSlot(pos.X, pos.Y);
                if (slot == null || slot.IsEmpty) continue;

                // Collect every open side of this cell whose neighbor is
                // empty in-grid stone. Out-of-bounds is not a dead-end
                // since the border ring is intentional.
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

                // Shaft-landing dead-ends (Bookshelf above/below a shaft
                // chain with empty left/right) get a short A* extension
                // attempt so the staircase leads somewhere real.
                var unrepairedSides = new List<Direction>();
                foreach (var side in deadEndSides)
                {
                    if (IsShaftLandingDeadEnd(grid, pos, room, side)
                        && TryRepairShaftLanding(grid, pos, side, costFn, streakLimits, blocked, rand))
                    {
                        continue;
                    }
                    unrepairedSides.Add(side);
                }

                if (unrepairedSides.Count == 0) continue;

                // Standalone rooms finish their own edges via padding, so
                // an empty neighbor still reads as a finished wall.
                if (room.AllowsEmptyNeighbors) continue;

                // Connector cell with at least one dead-end side. Convert
                // it to a standalone bookshelf when the swap is structurally
                // safe; otherwise fall back to stone-capping each dead-end
                // side at render time.
                if (TryConvertToBookshelf(grid, pos)) continue;

                foreach (var side in unrepairedSides)
                    sidesToCap.Add((pos, side));
            }

            return sidesToCap;
        }

        /// <summary>
        /// Replaces a 1x1 connector cell with a BookshelfCell when the
        /// swap creates no adjacency mismatches. A bookshelf reports every
        /// side open, so each non-empty neighbor's facing side must also
        /// be open and both rooms must whitelist each other on the shared
        /// edge.
        /// </summary>
        private static bool TryConvertToBookshelf(DungeonGrid grid, Point pos)
        {
            var slot = grid.GetSlot(pos.X, pos.Y);
            if (slot == null || slot.IsEmpty) return false;

            // Multi-cell pieces cannot be cleanly replaced by a 1x1
            // bookshelf without orphaning the rest of the footprint.
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

                // Open-side agreement: every bookshelf side will be open,
                // so the neighbor's facing side must also report open.
                if (!n.Room.IsOpenSide(n.SubCol, n.SubRow, d.opposite)) return false;

                // Mutual neighbor whitelist: both rooms must accept the
                // other's type on the shared edge.
                var weAccept = bookshelf.GetAcceptedNeighbors(0, 0, d.side);
                if (weAccept == null || !weAccept.Contains(n.Room.GetType())) return false;

                var theyAccept = n.Room.GetAcceptedNeighbors(n.SubCol, n.SubRow, d.opposite);
                if (theyAccept == null || !theyAccept.Contains(typeof(BookshelfCell))) return false;
            }

            grid.Place(bookshelf, pos.X, pos.Y, grid.NextGroupId());
            return true;
        }

        /// <summary>
        /// Returns true if <paramref name="pos"/> holds a Bookshelf that
        /// sits directly above OR below a ShaftCell, AND the dead-end side
        /// is horizontal (Left or Right). These are the cells whose
        /// decorative diagonal staircase emerges into an open side facing
        /// stone, the case where capping with a wall would seal off the
        /// staircase's destination.
        /// </summary>
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

        /// <summary>
        /// Attempts to grow a short Bookshelf/Corridor extension from the
        /// landing cell in the dead-end direction. The extension's goal is
        /// "any non-empty cell within budget steps that is not part of the
        /// landing's own shaft column", which biases the result toward
        /// connecting into existing dungeon rather than wandering off into
        /// new dead-ends.
        /// <para/>
        /// Returns true on success (cells stamped to the grid), false if no
        /// valid extension fit (caller falls back to wall capping).
        /// </summary>
        private static bool TryRepairShaftLanding(DungeonGrid grid, Point landing, Direction outwardSide,
                                                  EdgeCost costFn, IReadOnlyDictionary<Type, int> streakLimits,
                                                  HashSet<Point> blocked, Random rand)
        {
            int dx = outwardSide == Direction.Left ? -1 : (outwardSide == Direction.Right ? 1 : 0);
            int dy = outwardSide == Direction.Top  ? -1 : (outwardSide == Direction.Bottom ? 1 : 0);
            if (dx == 0 && dy == 0) return false;

            // Look outward up to RepairExtensionBudget cells, find any
            // non-empty cell that's a valid attachment target.
            for (int step = 1; step <= RepairExtensionBudget; step++)
            {
                int tx = landing.X + dx * step;
                int ty = landing.Y + dy * step;
                var target = grid.GetSlot(tx, ty);
                if (target == null) break; // OOB
                if (target.IsEmpty) continue;

                // The target must have an open side facing back toward the
                // landing; otherwise the extension would create a fresh
                // adjacency mismatch.
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
                                              maxVerticalRun: MaxVerticalRun);
                if (path == null) continue;
                if (path.Count == 0) continue; // trivial: same cell

                foreach (var pstep in path)
                    grid.Place(pstep.Cell, pstep.Anchor.X, pstep.Anchor.Y, grid.NextGroupId());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Paints stone over the boundary tiles of every (cell, side) pair
        /// in <paramref name="sidesToCap"/>. Runs AFTER PaddingBuilder so it
        /// can override any opening the padding logic might have placed
        /// when one side of a gap was empty.
        /// </summary>
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

                // Compute the world-space rectangle of the boundary strip
                // between this cell and its (empty) neighbor.
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

                    // A shaft chain that does not have a real room on both
                    // ends has nowhere meaningful for stairs to lead, so
                    // decoration is skipped. A* should not produce these;
                    // if it does, the missing room is the bug to fix
                    // rather than something to paper over with fake stairs.
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
