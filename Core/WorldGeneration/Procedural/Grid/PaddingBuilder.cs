using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// Fills the gaps between grid cells with context-appropriate content.
    /// Reads the cell types on both sides of each gap to decide what to render.
    /// </summary>
    public static class PaddingBuilder
    {
        public static void BuildAll(DungeonGrid grid, int fillTileType)
        {
            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            ushort blueWall = (ushort)ModContent.WallType<ArchiveWoodWallBlue>();

            // Horizontal padding between columns
            for (int col = 0; col < grid.Cols - 1; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var left = grid.GetSlot(col, row);
                    var right = grid.GetSlot(col + 1, row);

                    if (left.IsEmpty && right.IsEmpty)
                        continue;

                    Point leftWorld = grid.GridToWorld(col, row);
                    int padX = leftWorld.X + DungeonGrid.CellTileWidth;
                    int padY = leftWorld.Y;

                    if (!left.IsEmpty && !right.IsEmpty && left.GroupId == right.GroupId)
                    {
                        // Same group: internal padding handled by the multi-cell Build
                        continue;
                    }

                    if (!left.IsEmpty && !right.IsEmpty)
                    {
                        if (left.Room is CorridorCell && right.Room is CorridorCell)
                            PlaceCorridorPadding(padX, padY, DungeonGrid.HorizontalPadding, woodWall, blueWall);
                        else if (left.Room is ShaftCell || right.Room is ShaftCell)
                            ClearPadding(padX, padY, DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight);
                        else
                            PlaceWoodPanelPadding(padX, padY, DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight, woodWall, blueWall);
                    }
                    else if (!left.IsEmpty && left.Room is ShaftCell)
                    {
                        ClearPadding(padX, padY, DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight);
                    }
                    else if (!right.IsEmpty && right.Room is ShaftCell)
                    {
                        ClearPadding(padX, padY, DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight);
                    }
                    else
                    {
                        // One side empty: solid wall
                        FillSolid(padX, padY, DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight, fillTileType);
                    }
                }
            }

            // Vertical padding between rows
            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows - 1; row++)
                {
                    var top = grid.GetSlot(col, row);
                    var bottom = grid.GetSlot(col, row + 1);

                    if (top.IsEmpty && bottom.IsEmpty)
                        continue;

                    Point topWorld = grid.GridToWorld(col, row);
                    int padX = topWorld.X;
                    int padY = topWorld.Y + DungeonGrid.CellTileHeight;

                    if (!top.IsEmpty && !bottom.IsEmpty && top.GroupId == bottom.GroupId)
                    {
                        // Same group: internal padding handled by the multi-cell Build
                        continue;
                    }

                    StairBlock topStair = !top.IsEmpty ? top.Room as StairBlock : null;
                    (int woodStartX, int woodWidth) = topStair != null
                        ? topStair.GetFloorPaddingRange(top.SubCol, top.SubRow)
                        : (-1, 0);

                    if (!top.IsEmpty && !bottom.IsEmpty)
                    {
                        bool eitherShaft = top.Room is ShaftCell || bottom.Room is ShaftCell;

                        if (eitherShaft)
                        {
                            PlaceShaftFloorPadding(padX, padY, DungeonGrid.CellTileWidth, DungeonGrid.VerticalPadding, fillTileType);
                        }
                        else if (woodWidth > 0)
                        {
                            FillFloor(padX, padY, DungeonGrid.CellTileWidth, DungeonGrid.VerticalPadding, fillTileType);
                            FillWoodFloor(padX + woodStartX, padY, woodWidth, DungeonGrid.VerticalPadding);
                        }
                        else
                        {
                            FillFloor(padX, padY, DungeonGrid.CellTileWidth, DungeonGrid.VerticalPadding, fillTileType);
                        }
                    }
                    else if (woodWidth > 0)
                    {
                        FillSolid(padX, padY, DungeonGrid.CellTileWidth, DungeonGrid.VerticalPadding, fillTileType);
                        FillWoodFloor(padX + woodStartX, padY, woodWidth, DungeonGrid.VerticalPadding);
                    }
                    else
                    {
                        // One side empty: solid fill
                        FillSolid(padX, padY, DungeonGrid.CellTileWidth, DungeonGrid.VerticalPadding, fillTileType);
                    }
                }
            }
        }

        private static void PlaceCorridorPadding(int x, int y, int w, ushort woodWall, ushort blueWall)
        {
            int ceilingY = y + 17;
            int floorY = ceilingY + 8;

            for (int lx = 0; lx < w; lx++)
                for (int ly = ceilingY; ly <= floorY; ly++)
                    WorldGenUtils.ClearTile(x + lx, ly);

            int wallTop = ceilingY + 1;
            int wallBottom = floorY - 1;
            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = wallTop; ly <= wallBottom; ly++)
                {
                    bool isBorder = false;
                    if (!isBorder)
                        WorldGenUtils.SetWall(x + lx, ly, blueWall);
                }
            }

            for (int lx = 0; lx < w; lx++)
            {
                WorldGenUtils.SetWall(x + lx, ceilingY - 1, woodWall);
                WorldGenUtils.SetWall(x + lx, floorY + 1, woodWall);
            }
        }

        private static void PlaceWoodPanelPadding(int x, int y, int w, int h, ushort woodWall, ushort blueWall)
        {
            // Clear tiles in the padding area
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.ClearTile(x + lx, y + ly);

            ProceduralUtils.DrawWallPanel(x, y, w, h, woodWall, blueWall);
        }

        private static void PlaceShaftFloorPadding(int x, int y, int w, int h, int tileType)
        {
            ushort tile = (ushort)tileType;
            int wallWidth = 2;

            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < h; ly++)
                {
                    if (lx < wallWidth || lx >= w - wallWidth)
                        WorldGenUtils.PlaceTile(x + lx, y + ly, tile);
                    else
                        WorldGenUtils.ClearTile(x + lx, y + ly);
                }
            }

            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            for (int lx = 0; lx < wallWidth; lx++)
                for (int ly = 0; ly < h; ly++)
                {
                    WorldGenUtils.SetWall(x + lx, y + ly, woodWall);
                    WorldGenUtils.SetWall(x + w - 1 - lx, y + ly, woodWall);
                }
        }

        private static void FillWoodFloor(int x, int y, int w, int h)
        {
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.PlaceTile(x + lx, y + ly, woodTile);
        }

        private static void ClearPadding(int x, int y, int w, int h)
        {
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.ClearTile(x + lx, y + ly);
        }

        private static void FillSolid(int x, int y, int w, int h, int tileType)
        {
            ushort tile = (ushort)tileType;
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.PlaceTile(x + lx, y + ly, tile);
        }

        private static void FillFloor(int x, int y, int w, int h, int tileType)
        {
            ushort tile = (ushort)tileType;
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.PlaceTile(x + lx, y + ly, tile);
        }
    }
}
