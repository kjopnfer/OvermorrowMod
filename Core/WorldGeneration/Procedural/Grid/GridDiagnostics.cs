using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// Dumps the final state of a generated dungeon grid to a text file:
    ///   - 2D ASCII layout (one char per cell)
    ///   - Per-type cell counts
    ///   - Dead-end corridor list (corridor with empty/stone on left or right)
    ///   - Lateral adjacency mismatches (cell exit pointing at a neighbor that
    ///     doesn't reciprocate — exposes "wall facing open side" disconnects)
    ///   - Connected-component analysis (isolated regions by reciprocal exits)
    ///   - Per-column shaft listing
    ///
    /// Call after <see cref="GridGenerator.Build"/> finishes. Output goes to
    /// the tModLoader root folder so you don't have to hunt for it.
    /// </summary>
    public static class GridDiagnostics
    {
        /// <summary>
        /// Writes a full grid diagnostic report to <paramref name="filePath"/>.
        /// </summary>
        public static void DumpFullGrid(DungeonGrid grid, string filePath)
        {
            var sb = new StringBuilder();

            sb.AppendLine("=== Dungeon Grid Diagnostic ===");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Grid size: {grid.Cols} cols x {grid.Rows} rows");
            sb.AppendLine();

            WriteCellCounts(sb, grid);
            WriteAsciiLayout(sb, grid);
            WriteDeadEndCorridors(sb, grid);
            WriteAdjacencyMismatches(sb, grid);
            WriteConnectedComponents(sb, grid);
            WriteShaftColumns(sb, grid);
            WritePerRowDensity(sb, grid);

            File.WriteAllText(filePath, sb.ToString());
        }

        // ─── Cell counts ────────────────────────────────────────────────────

        private static void WriteCellCounts(StringBuilder sb, DungeonGrid grid)
        {
            int bookshelf = 0, corridor = 0, shaft = 0;
            int descStair = 0, ascStair = 0, door = 0;
            int totalAnchors = 0, totalSlots = 0, emptySlots = 0;

            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Cols; col++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot == null) continue;
                    totalSlots++;
                    if (slot.IsEmpty) { emptySlots++; continue; }
                    if (slot.SubCol != 0 || slot.SubRow != 0) continue;

                    totalAnchors++;
                    switch (slot.Room)
                    {
                        case BookshelfCell:    bookshelf++; break;
                        case CorridorCell:     corridor++;  break;
                        case ShaftCell:        shaft++;     break;
                        case DescendingStair:  descStair++; break;
                        case AscendingStair:   ascStair++;  break;
                        case DoorRoom:         door++;      break;
                    }
                }
            }

            sb.AppendLine("--- Cell counts (anchors only) ---");
            sb.AppendLine($"  Bookshelf:        {bookshelf}");
            sb.AppendLine($"  Corridor:         {corridor}");
            sb.AppendLine($"  Shaft:            {shaft}");
            sb.AppendLine($"  Descending Stair: {descStair}");
            sb.AppendLine($"  Ascending Stair:  {ascStair}");
            sb.AppendLine($"  Door:             {door}");
            sb.AppendLine($"  Total anchors:    {totalAnchors}");
            sb.AppendLine($"  Empty slots:      {emptySlots} / {totalSlots}");
            sb.AppendLine();
        }

        // ─── 2D ASCII layout ────────────────────────────────────────────────

        // Width of the longest stripped type name ("DescendingStair" = 15).
        // Each cell renders as "[name padded to 15]" = 17 chars. Empty/OOB
        // are padded the same way so columns line up.
        private const int CellLabelInnerWidth = 15;

        private static void WriteAsciiLayout(StringBuilder sb, DungeonGrid grid)
        {
            sb.AppendLine("--- Grid layout ---");
            sb.AppendLine("Each cell shows its type wrapped in brackets. '.' = empty, '#' = out-of-bounds.");
            sb.AppendLine($"Note: rows are wide ({grid.Cols} cells x {CellLabelInnerWidth + 2} chars = {grid.Cols * (CellLabelInnerWidth + 2)} chars). Use a wide editor.");
            sb.AppendLine();

            // Column header — col index centered in each cell's width.
            sb.Append("        ");
            for (int col = 0; col < grid.Cols; col++)
            {
                string colLabel = $"c{col}";
                sb.Append(PadCenter(colLabel, CellLabelInnerWidth + 2));
            }
            sb.AppendLine();

            for (int row = 0; row < grid.Rows; row++)
            {
                sb.Append($"  r{row,4} ");
                for (int col = 0; col < grid.Cols; col++)
                {
                    sb.Append(GetCellLabel(grid, col, row));
                }
                sb.AppendLine();
            }
            sb.AppendLine();
        }

        /// <summary>
        /// Returns the cell at (col, row) formatted as "[Typename     ]"
        /// padded to a fixed width so columns align.
        /// </summary>
        private static string GetCellLabel(DungeonGrid grid, int col, int row)
        {
            var slot = grid.GetSlot(col, row);
            string name;
            if (slot == null)            name = "#";
            else if (slot.IsEmpty)       name = ".";
            else
            {
                name = slot.Room.GetType().Name;
                if (name.EndsWith("Cell")) name = name.Substring(0, name.Length - 4);
            }
            return $"[{name.PadRight(CellLabelInnerWidth)}]";
        }

        private static string PadCenter(string s, int width)
        {
            if (s.Length >= width) return s;
            int total = width - s.Length;
            int left = total / 2;
            int right = total - left;
            return new string(' ', left) + s + new string(' ', right);
        }

        // ─── Dead-end corridors ─────────────────────────────────────────────

        private static void WriteDeadEndCorridors(StringBuilder sb, DungeonGrid grid)
        {
            sb.AppendLine("--- Dead-end corridors ---");
            sb.AppendLine("(A corridor whose left or right neighbor is empty/OOB.)");

            int count = 0;
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Cols; col++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot == null || slot.IsEmpty) continue;
                    if (slot.Room is not CorridorCell) continue;

                    string leftDesc  = DescribeCellTerse(grid, col - 1, row);
                    string rightDesc = DescribeCellTerse(grid, col + 1, row);
                    bool leftDead  = leftDesc  == "empty" || leftDesc  == "OOB";
                    bool rightDead = rightDesc == "empty" || rightDesc == "OOB";

                    if (leftDead || rightDead)
                    {
                        count++;
                        sb.AppendLine($"  ({col},{row}) Corridor: left={leftDesc} right={rightDesc}");
                    }
                }
            }
            sb.AppendLine($"  Total: {count}");
            sb.AppendLine();
        }

        // ─── Adjacency mismatches ───────────────────────────────────────────

        private static void WriteAdjacencyMismatches(StringBuilder sb, DungeonGrid grid)
        {
            sb.AppendLine("--- Adjacency mismatches ---");
            sb.AppendLine("(Cell exit points at a neighbor that doesn't reciprocate. Indicates a 'wall facing open side' disconnect.)");

            int count = 0;
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Cols; col++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot == null || slot.IsEmpty) continue;
                    if (slot.SubCol != 0 || slot.SubRow != 0) continue;

                    foreach (var exit in slot.Room.Exits)
                    {
                        int nx = col + exit.CursorDelta.X;
                        int ny = row + exit.CursorDelta.Y;
                        var neighbor = grid.GetSlot(nx, ny);

                        if (neighbor == null)
                        {
                            count++;
                            sb.AppendLine($"  ({col},{row}) {slot.Room.GetType().Name} exit→({nx},{ny}) OOB");
                            continue;
                        }
                        if (neighbor.IsEmpty)
                        {
                            // Empty neighbor isn't strictly a "mismatch" — many
                            // exits land on stone by design (last cell before
                            // arrival, dead-end stretches). Logged for context
                            // but not counted toward connection failures below.
                            continue;
                        }

                        var inverse = new Point(-exit.CursorDelta.X, -exit.CursorDelta.Y);
                        bool reciprocal = false;
                        foreach (var nExit in neighbor.Room.Exits)
                        {
                            if (nExit.CursorDelta == inverse) { reciprocal = true; break; }
                        }

                        if (!reciprocal)
                        {
                            count++;
                            sb.AppendLine($"  ({col},{row}) {slot.Room.GetType().Name} exit→({nx},{ny}) {neighbor.Room.GetType().Name} (no return exit)");
                        }
                    }
                }
            }
            sb.AppendLine($"  Total mismatches: {count}");
            sb.AppendLine();
        }

        // ─── Connected components ───────────────────────────────────────────

        private static void WriteConnectedComponents(StringBuilder sb, DungeonGrid grid)
        {
            sb.AppendLine("--- Connected components ---");
            sb.AppendLine("(Cells reachable via reciprocal exits. >1 component = isolated regions.)");

            var visited = new HashSet<Point>();
            var componentSizes = new List<(int size, Point representative)>();

            for (int row = 0; row < grid.Rows; row++)
            {
                for (int col = 0; col < grid.Cols; col++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot == null || slot.IsEmpty) continue;
                    if (slot.SubCol != 0 || slot.SubRow != 0) continue;

                    var anchor = new Point(col, row);
                    if (visited.Contains(anchor)) continue;

                    int size = FloodFillComponent(grid, anchor, visited);
                    componentSizes.Add((size, anchor));
                }
            }

            sb.AppendLine($"  Total components: {componentSizes.Count}");
            for (int i = 0; i < componentSizes.Count; i++)
            {
                var (size, rep) = componentSizes[i];
                sb.AppendLine($"    #{i + 1}: {size} cells (anchor at {rep})");
            }
            sb.AppendLine();
        }

        private static int FloodFillComponent(DungeonGrid grid, Point seed, HashSet<Point> visited)
        {
            var queue = new Queue<Point>();
            queue.Enqueue(seed);
            visited.Add(seed);
            int size = 0;

            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                size++;

                var slot = grid.GetSlot(p.X, p.Y);
                if (slot == null || slot.IsEmpty) continue;

                foreach (var exit in slot.Room.Exits)
                {
                    int nx = p.X + exit.CursorDelta.X;
                    int ny = p.Y + exit.CursorDelta.Y;
                    var nSlot = grid.GetSlot(nx, ny);
                    if (nSlot == null || nSlot.IsEmpty) continue;

                    var inverse = new Point(-exit.CursorDelta.X, -exit.CursorDelta.Y);
                    bool reciprocal = false;
                    foreach (var nExit in nSlot.Room.Exits)
                    {
                        if (nExit.CursorDelta == inverse) { reciprocal = true; break; }
                    }
                    if (!reciprocal) continue;

                    // Walk up to the neighbor's anchor so we visit each
                    // multi-cell piece once.
                    var nAnchor = new Point(nx - nSlot.SubCol, ny - nSlot.SubRow);
                    if (visited.Contains(nAnchor)) continue;
                    visited.Add(nAnchor);
                    queue.Enqueue(nAnchor);
                }
            }
            return size;
        }

        // ─── Shaft columns ──────────────────────────────────────────────────

        private static void WriteShaftColumns(StringBuilder sb, DungeonGrid grid)
        {
            sb.AppendLine("--- Shaft columns ---");
            int columnsWithShaft = 0;
            int totalShafts = 0;
            for (int col = 0; col < grid.Cols; col++)
            {
                var rows = new List<int>();
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot != null && !slot.IsEmpty && slot.Room is ShaftCell)
                        rows.Add(row);
                }
                if (rows.Count > 0)
                {
                    columnsWithShaft++;
                    totalShafts += rows.Count;
                    sb.AppendLine($"  Col {col,3}: {rows.Count} shaft(s) at rows [{string.Join(", ", rows)}]");
                }
            }
            sb.AppendLine($"  Columns with shafts: {columnsWithShaft}");
            sb.AppendLine($"  Total shaft cells:   {totalShafts}");
            sb.AppendLine();
        }

        // ─── Per-row density ────────────────────────────────────────────────

        private static void WritePerRowDensity(StringBuilder sb, DungeonGrid grid)
        {
            sb.AppendLine("--- Per-row density ---");
            sb.AppendLine("  row | filled / total | %");
            for (int row = 0; row < grid.Rows; row++)
            {
                int filled = 0;
                for (int col = 0; col < grid.Cols; col++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot != null && !slot.IsEmpty) filled++;
                }
                double pct = grid.Cols > 0 ? (filled * 100.0 / grid.Cols) : 0;
                sb.AppendLine($"  r{row,3} | {filled,5} / {grid.Cols,-5} | {pct,5:F1}%");
            }
            sb.AppendLine();
        }

        // ─── Helpers ────────────────────────────────────────────────────────

        private static string DescribeCellTerse(DungeonGrid grid, int col, int row)
        {
            var slot = grid.GetSlot(col, row);
            if (slot == null) return "OOB";
            if (slot.IsEmpty) return "empty";
            return slot.Room.GetType().Name;
        }
    }
}
