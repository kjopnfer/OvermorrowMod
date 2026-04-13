using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells;
using System;
using System.Collections.Generic;
using Terraria.ID;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// Generates a dungeon on a DungeonGrid by placing cells and building them.
    /// Currently supports horizontal-only chains on a single row.
    /// </summary>
    public static class GridGenerator
    {
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

            // Place cells in a horizontal chain on the middle row
            int startRow = gridRows / 2;
            PlaceHorizontalChain(grid, cellPool, startRow, 0, gridCols, rand);

            // Build all placed cells
            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot.IsEmpty)
                        continue;

                    // Only build from the top-left sub-cell of each piece
                    if (slot.SubCol != 0 || slot.SubRow != 0)
                        continue;

                    Point cellOrigin = grid.GridToWorld(col, row);
                    slot.Room.Build(cellOrigin, fillTileType, liningTileType);
                }
            }

            // Build padding between cells
            PaddingBuilder.BuildAll(grid, fillTileType);

            // Debug: place corner markers at each grid cell
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

        private const int MaxConsecutiveCorridor = 2;
        private const int MaxConsecutiveBookshelf = 3;

        private static void PlaceHorizontalChain(
            DungeonGrid grid,
            List<GridRoom> cellPool,
            int row,
            int startCol,
            int endCol,
            Random rand)
        {
            int consecutiveCorridors = 0;
            int consecutiveBookshelves = 0;

            for (int col = startCol; col < endCol; col++)
            {
                var slot = grid.GetSlot(col, row);
                if (!slot.IsEmpty)
                    continue;

                var shuffled = new List<GridRoom>(cellPool);
                Shuffle(shuffled, rand);

                bool placed = false;
                bool isFirstCol = (col == startCol);
                bool isLastCol = (col == endCol - 1);

                foreach (var candidate in shuffled)
                {
                    bool isCorridor = candidate is CorridorCell;

                    if (isCorridor && (isFirstCol || isLastCol))
                        continue;

                    if (isCorridor && consecutiveCorridors >= MaxConsecutiveCorridor)
                        continue;

                    if (candidate is BookshelfCell && consecutiveBookshelves >= MaxConsecutiveBookshelf)
                        continue;

                    if (grid.CanPlace(candidate, col, row))
                    {
                        int groupId = grid.NextGroupId();
                        grid.Place(candidate, col, row, groupId);

                        if (candidate is CorridorCell)
                        {
                            consecutiveCorridors++;
                            consecutiveBookshelves = 0;
                        }
                        else if (candidate is BookshelfCell)
                        {
                            consecutiveBookshelves++;
                            consecutiveCorridors = 0;
                        }
                        else
                        {
                            consecutiveCorridors = 0;
                            consecutiveBookshelves = 0;
                        }

                        placed = true;
                        break;
                    }
                }

                if (!placed)
                    break;
            }
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
