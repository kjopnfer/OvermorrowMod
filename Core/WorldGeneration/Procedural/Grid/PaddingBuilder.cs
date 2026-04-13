using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
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
                        // Two different cells: wood panel wall between them
                        PlaceWoodPanelPadding(padX, padY, DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight, woodWall, blueWall);
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

                    if (!top.IsEmpty && !bottom.IsEmpty)
                    {
                        // Two different cells vertically: floor/ceiling tiles
                        FillFloor(padX, padY, DungeonGrid.CellTileWidth, DungeonGrid.VerticalPadding, fillTileType);
                    }
                    else
                    {
                        // One side empty: solid fill
                        FillSolid(padX, padY, DungeonGrid.CellTileWidth, DungeonGrid.VerticalPadding, fillTileType);
                    }
                }
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
