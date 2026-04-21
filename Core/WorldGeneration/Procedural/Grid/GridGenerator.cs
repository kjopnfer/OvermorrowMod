using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// Walker-based dungeon generator using a multi-exit cell model.
    ///
    /// Each cell exposes a list of <see cref="CellExit"/>s (cursor delta +
    /// allowed next cells). The walker picks one per step, weighted toward
    /// the current target. A <c>lastCursor</c> guard prevents the walker
    /// from immediately reversing into the cell it just came from, which is
    /// what makes bidirectional cells (shafts, corridors) behave correctly —
    /// the shaft itself has no notion of "up" or "down", but the walker's
    /// history determines which exit counts as "forward".
    ///
    /// Phase 1: critical path from spawn to east edge.
    /// Phase 2: branches from critical-path points through waypoints back
    ///          to other critical-path points. Branches always close into
    ///          loops (arrival at an already-occupied critical-path cell
    ///          counts as success, no overlap needed).
    /// Phase 3: render (existing pipeline, untouched).
    /// </summary>
    public static class GridGenerator
    {
        private const int BranchCount = 8;
        private const int WaypointsPerBranch = 2;
        private const int MaxStepsPerSegment = 400;
        private const int MaxBranchRetries = 5;

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

            // ─── Phase 1: critical path ──────────────────────────────────────
            int spineRow = gridRows / 2;
            var start = new Point(0, spineRow);
            var endTarget = new Point(gridCols - 1, spineRow);

            // Seed the walker with a bookshelf at `start` — placed directly.
            var firstCell = new BookshelfCell();
            if (!FitsFootprint(grid, firstCell, start)) return; // nothing fits? bail.
            grid.Place(firstCell, start.X, start.Y, grid.NextGroupId());

            var criticalPath = new List<Point> { start };
            WalkFromTo(grid, start, endTarget, firstCell, criticalPath, rand);

            // ─── Phase 2: branches (loops) ───────────────────────────────────
            for (int i = 0; i < BranchCount; i++)
            {
                if (criticalPath.Count < 4) break;
                TryPlaceBranch(grid, criticalPath, rand);
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
        /// Walks from the cell already placed at <paramref name="startCursor"/>
        /// toward <paramref name="target"/>. Mutates <paramref name="visited"/>
        /// with each new cell's cursor position. Stops when the walker arrives
        /// at the target, runs out of steps, or can't make a legal move.
        /// </summary>
        private static void WalkFromTo(
            DungeonGrid grid,
            Point startCursor,
            Point target,
            GridRoom startCell,
            List<Point> visited,
            Random rand)
        {
            var cursor = startCursor;
            var lastCursor = new Point(int.MinValue, int.MinValue); // impossible sentinel
            GridRoom prev = startCell;

            int stepGuard = MaxStepsPerSegment;
            while (cursor != target && stepGuard-- > 0)
            {
                if (!TryStep(grid, cursor, lastCursor, target, prev, rand,
                             out var chosen, out var anchor, out var nextCursor,
                             out bool arriveWithoutPlace))
                {
                    return; // no legal move
                }

                if (arriveWithoutPlace)
                {
                    cursor = target;
                    break;
                }

                grid.Place(chosen, anchor.X, anchor.Y, grid.NextGroupId());
                visited.Add(nextCursor);
                lastCursor = cursor;
                cursor = nextCursor;
                prev = chosen;
            }
        }

        /// <summary>
        /// Tries one walker step from <paramref name="cursor"/>. Picks the
        /// (exit, candidate) pair with the best score. Skips exits that would
        /// backtrack (land on <paramref name="lastCursor"/>) or whose candidate
        /// doesn't fit (out of bounds / overlapping).
        ///
        /// Special arrival case: if some exit lands the cursor exactly on
        /// <paramref name="target"/> and that slot is already occupied (e.g.
        /// the branch end on the critical path), succeed without placing a
        /// cell — the critical-path cell is the arrival point.
        /// </summary>
        private static bool TryStep(
            DungeonGrid grid,
            Point cursor,
            Point lastCursor,
            Point target,
            GridRoom prev,
            Random rand,
            out GridRoom chosen,
            out Point anchor,
            out Point nextCursor,
            out bool arriveWithoutPlace)
        {
            chosen = null;
            anchor = default;
            nextCursor = default;
            arriveWithoutPlace = false;

            double bestScore = double.NegativeInfinity;
            bool bestArrival = false;

            foreach (var exit in prev.Exits)
            {
                var candNext = new Point(cursor.X + exit.CursorDelta.X,
                                         cursor.Y + exit.CursorDelta.Y);

                // Prevent immediate backtrack (don't step onto where we just were).
                if (candNext == lastCursor) continue;

                // Arrival at already-occupied target (e.g. branchEnd on critical path).
                if (candNext == target)
                {
                    var targetSlot = grid.GetSlot(target.X, target.Y);
                    if (targetSlot != null && !targetSlot.IsEmpty)
                    {
                        int arrivalDist = 0;
                        double arrivalScore = -arrivalDist + rand.NextDouble() * 0.5;
                        if (arrivalScore > bestScore)
                        {
                            bestScore = arrivalScore;
                            bestArrival = true;
                            chosen = null;
                            nextCursor = target;
                        }
                        continue;
                    }
                }

                foreach (var candidate in exit.AllowedNext)
                {
                    var candAnchor = new Point(
                        candNext.X + candidate.AnchorOffsetFromCursor.X,
                        candNext.Y + candidate.AnchorOffsetFromCursor.Y);

                    if (!FitsFootprint(grid, candidate, candAnchor)) continue;

                    int dist = Math.Abs(candNext.X - target.X) + Math.Abs(candNext.Y - target.Y);
                    double score = -dist + rand.NextDouble() * 0.5;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestArrival = false;
                        chosen = candidate;
                        anchor = candAnchor;
                        nextCursor = candNext;
                    }
                }
            }

            if (bestArrival)
            {
                arriveWithoutPlace = true;
                return true;
            }
            return chosen != null;
        }

        /// <summary>
        /// Returns true if every slot in the candidate's footprint is in-bounds
        /// and currently empty. No edge-compatibility checking — validity comes
        /// from <see cref="CellExit.AllowedNext"/>.
        /// </summary>
        private static bool FitsFootprint(DungeonGrid grid, GridRoom room, Point anchor)
        {
            for (int sc = 0; sc < room.CellWidth; sc++)
            {
                for (int sr = 0; sr < room.CellHeight; sr++)
                {
                    var slot = grid.GetSlot(anchor.X + sc, anchor.Y + sr);
                    if (slot == null) return false;
                    if (!slot.IsEmpty) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// A rectangle recording what a single grid.Place call covered,
        /// so it can be undone if the attempt fails.
        /// </summary>
        private readonly struct PlacedRect
        {
            public readonly int Col, Row, Width, Height;
            public PlacedRect(int col, int row, int width, int height)
            {
                Col = col; Row = row; Width = width; Height = height;
            }
        }

        /// <summary>
        /// Tries up to <see cref="MaxBranchRetries"/> times to plan and
        /// commit one branch. Each attempt records every cell it stamps;
        /// on failure those cells are wiped so the next retry (and any
        /// later branches) see a clean grid.
        /// </summary>
        private static void TryPlaceBranch(DungeonGrid grid, List<Point> criticalPath, Random rand)
        {
            for (int attempt = 0; attempt < MaxBranchRetries; attempt++)
            {
                var placed = new List<PlacedRect>();
                if (TryPlaceBranchAttempt(grid, criticalPath, rand, placed))
                    return;
                RollbackPlacements(grid, placed);
            }
        }

        /// <summary>
        /// One branch attempt. Returns true only if every segment of the
        /// waypoint chain completed. Every cell stamped during this attempt
        /// is appended to <paramref name="placed"/> so the caller can undo
        /// them on failure.
        /// </summary>
        private static bool TryPlaceBranchAttempt(
            DungeonGrid grid, List<Point> criticalPath, Random rand, List<PlacedRect> placed)
        {
            int iStart = rand.Next(criticalPath.Count - 2);
            int iEnd = iStart + 3 + rand.Next(Math.Max(1, criticalPath.Count - iStart - 3));
            if (iEnd >= criticalPath.Count) iEnd = criticalPath.Count - 1;

            var branchStart = criticalPath[iStart];
            var branchEnd = criticalPath[iEnd];

            // Reject shaft-column adjacency with existing branches.
            for (int dc = -1; dc <= 1; dc++)
            {
                if (ColumnHasShaft(grid, branchStart.X + dc)) return false;
                if (ColumnHasShaft(grid, branchEnd.X + dc)) return false;
            }

            // Randomize the U's depth every retry so different attempts
            // explore different shapes, not just different RNG noise.
            int depth = 3 + rand.Next(3);
            int detourY = Math.Min(branchStart.Y + depth, grid.Rows - 2);

            var waypoints = new List<Point>
            {
                new Point(branchStart.X, detourY),
                new Point(branchEnd.X,   detourY),
                new Point(branchEnd.X,   branchEnd.Y + 1),
            };

            var startSlot = grid.GetSlot(branchStart.X, branchStart.Y);
            if (startSlot == null || startSlot.IsEmpty) return false;

            var targets = new List<Point>(waypoints) { branchEnd };

            var cursor = branchStart;
            GridRoom prev = startSlot.Room;
            var lastCursor = new Point(int.MinValue, int.MinValue);

            foreach (var target in targets)
            {
                int stepGuard = MaxStepsPerSegment;
                while (cursor != target && stepGuard-- > 0)
                {
                    if (!TryStep(grid, cursor, lastCursor, target, prev, rand,
                                 out var chosen, out var anchor, out var nextCursor,
                                 out bool arriveWithoutPlace))
                    {
                        return false; // failed; caller will roll back whatever's in `placed`
                    }

                    if (arriveWithoutPlace)
                    {
                        cursor = target;
                        break;
                    }

                    grid.Place(chosen, anchor.X, anchor.Y, grid.NextGroupId());
                    placed.Add(new PlacedRect(anchor.X, anchor.Y, chosen.CellWidth, chosen.CellHeight));
                    lastCursor = cursor;
                    cursor = nextCursor;
                    prev = chosen;
                }

                // If we ran out of step budget without hitting the target, bail.
                if (cursor != target) return false;
            }

            return true;
        }

        /// <summary>
        /// Undoes every stamped cell recorded during a failed branch attempt,
        /// restoring the slots to empty so the next retry sees a clean grid.
        /// </summary>
        private static void RollbackPlacements(DungeonGrid grid, List<PlacedRect> placed)
        {
            foreach (var r in placed)
            {
                for (int dc = 0; dc < r.Width; dc++)
                {
                    for (int dr = 0; dr < r.Height; dr++)
                    {
                        var slot = grid.GetSlot(r.Col + dc, r.Row + dr);
                        if (slot == null) continue;
                        slot.Room = null;
                        slot.GroupId = 0;
                        slot.SubCol = 0;
                        slot.SubRow = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the given column contains any ShaftCell anywhere
        /// in its length. Used to prevent two branches from placing shafts
        /// in adjacent columns.
        /// </summary>
        private static bool ColumnHasShaft(DungeonGrid grid, int col)
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
