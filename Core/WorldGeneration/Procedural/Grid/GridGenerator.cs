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
    /// Generates a dungeon on a DungeonGrid by placing cells and building them.
    /// Supports horizontal chains with vertical branching via StairBlocks.
    /// </summary>
    public static class GridGenerator
    {
        private const int BranchChance = 3;
        private const int MaxDepth = 2;

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

            int startRow = gridRows / 2;
            PlaceSpineWithBranches(grid, cellPool, startRow, 0, gridCols, rand, 0);

            // Place shafts below bookshelf cells
            PlaceShafts(grid, rand);

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

            // Place diagonal stairs in shaft columns
            DecorateShafts(grid);

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

        private const int ShaftChance = 3;

        private static void PlaceShafts(DungeonGrid grid, Random rand)
        {
            var shaftCell = new ShaftCell();

            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot.IsEmpty || slot.Room is not BookshelfCell)
                        continue;

                    if (rand.Next(10) >= ShaftChance)
                        continue;

                    int shaftRow = row + 1;
                    int bottomRow = shaftRow + 1;

                    // Both slots must be in bounds and empty before committing
                    if (shaftRow >= grid.Rows || bottomRow >= grid.Rows)
                        continue;

                    var shaftSlot = grid.GetSlot(col, shaftRow);
                    var bottomSlot = grid.GetSlot(col, bottomRow);
                    if (shaftSlot == null || !shaftSlot.IsEmpty)
                        continue;
                    if (bottomSlot == null || !bottomSlot.IsEmpty)
                        continue;

                    if (!grid.CanPlace(shaftCell, col, shaftRow))
                        continue;

                    int groupId = grid.NextGroupId();
                    grid.Place(shaftCell, col, shaftRow, groupId);

                    var bottomBookshelf = new BookshelfCell();
                    if (grid.CanPlace(bottomBookshelf, col, bottomRow))
                    {
                        int bottomGroupId = grid.NextGroupId();
                        grid.Place(bottomBookshelf, col, bottomRow, bottomGroupId);
                    }
                    else
                    {
                        // Bookshelf can't fit; remove the shaft to avoid an orphaned shaft
                        shaftSlot.Room = null;
                        shaftSlot.GroupId = 0;
                    }
                }
            }
        }

        private static void DecorateShafts(DungeonGrid grid)
        {
            int diagonalStairsType = ModContent.TileType<DiagonalStairs>();
            int stairCapType = ModContent.TileType<StairCap>();

            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot.IsEmpty || slot.Room is not ShaftCell)
                        continue;

                    // Find the top of this shaft chain (walk up through consecutive shafts)
                    int topRow = row;
                    while (topRow > 0)
                    {
                        var above = grid.GetSlot(col, topRow - 1);
                        if (above != null && !above.IsEmpty && above.Room is ShaftCell)
                            topRow--;
                        else
                            break;
                    }

                    // Only process from the topmost shaft in a chain
                    if (row != topRow)
                        continue;

                    // Find the bottom of the chain
                    int bottomRow = row;
                    while (bottomRow < grid.Rows - 1)
                    {
                        var below = grid.GetSlot(col, bottomRow + 1);
                        if (below != null && !below.IsEmpty && below.Room is ShaftCell)
                            bottomRow++;
                        else
                            break;
                    }

                    // Find the room above the top shaft (for floor Y)
                    var topRoom = grid.GetSlot(col, topRow - 1);
                    // Find the room below the bottom shaft (for floor Y)
                    var bottomRoom = grid.GetSlot(col, bottomRow + 1);

                    if (topRoom == null || topRoom.IsEmpty || bottomRoom == null || bottomRoom.IsEmpty)
                        continue;

                    Point topRoomOrigin = grid.GridToWorld(col, topRow - 1);
                    Point bottomRoomOrigin = grid.GridToWorld(col, bottomRow + 1);

                    // Top Y = floor of the room above the shaft
                    int topY = topRoomOrigin.Y + DungeonGrid.CellTileHeight - 1;
                    // Bottom Y = floor of the room below the shaft
                    int bottomY = bottomRoomOrigin.Y + DungeonGrid.CellTileHeight - 1;

                    int segmentCount = (bottomY - topY) / 10;
                    int shaftCenterX = grid.GridToWorld(col, topRow).X + DungeonGrid.CellTileWidth / 2;
                    int stairX = shaftCenterX - 7;
                    int capX = shaftCenterX - 2;

                    // Place stairs from bottom to top
                    for (int s = segmentCount - 1; s >= 0; s--)
                        WorldGen.PlaceObject(stairX, topY + s * 10 + 10, diagonalStairsType);

                    // Place cap at top of chain
                    WorldGen.PlaceObject(capX, topY, stairCapType);
                }
            }
        }

        private const int MaxConsecutiveCorridor = 2;
        private const int MaxConsecutiveBookshelf = 3;

        private static void PlaceSpineWithBranches(
            DungeonGrid grid,
            List<GridRoom> cellPool,
            int row,
            int startCol,
            int endCol,
            Random rand,
            int depth)
        {
            int consecutiveCorridors = 0;
            int consecutiveBookshelves = 0;
            int cellsSinceBranch = 0;

            for (int col = startCol; col < endCol; col++)
            {
                var slot = grid.GetSlot(col, row);
                if (!slot.IsEmpty)
                    continue;

                // Try placing a stair to branch to another row
                if (depth < MaxDepth && cellsSinceBranch >= 2 && rand.Next(10) < BranchChance)
                {
                    bool goDown = row + 1 < grid.Rows;
                    bool goUp = row - 1 >= 0;

                    if (goDown || goUp)
                    {
                        bool descend = goDown && (!goUp || rand.Next(2) == 0);
                        int stairRow = descend ? row : row - 1;

                        var stairBlock = new StairBlock(descendLeftToRight: descend);
                        if (grid.CanPlace(stairBlock, col, stairRow))
                        {
                            int groupId = grid.NextGroupId();
                            grid.Place(stairBlock, col, stairRow, groupId);

                            int newRow = descend ? row + 1 : row - 1;
                            int chainStart = col + 2;
                            if (chainStart < endCol)
                                PlaceSpineWithBranches(grid, cellPool, newRow, chainStart, endCol, rand, depth + 1);

                            // Skip past the 2-wide stair and continue on the same row
                            col += 1;
                            cellsSinceBranch = 0;
                            consecutiveCorridors = 0;
                            consecutiveBookshelves = 0;
                            continue;
                        }
                    }
                }

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

                        cellsSinceBranch++;
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
