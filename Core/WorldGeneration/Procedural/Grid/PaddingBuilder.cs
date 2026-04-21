using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
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
                            PlaceShaftSidePadding(padX, padY, DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight, woodWall, blueWall);
                        else
                            PlaceWoodPanelPadding(padX, padY, DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight, woodWall, blueWall);
                    }
                    else if (!left.IsEmpty && left.Room is ShaftCell)
                    {
                        PlaceShaftSidePadding(padX, padY, DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight, woodWall, blueWall);
                    }
                    else if (!right.IsEmpty && right.Room is ShaftCell)
                    {
                        PlaceShaftSidePadding(padX, padY, DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight, woodWall, blueWall);
                    }
                    else if (!left.IsEmpty && left.Room is BookshelfCell)
                    {
                        // Bookshelf claims its right-side padding even when the
                        // adjacent cell is empty — matches the look it has when
                        // sitting next to another bookshelf.
                        PlaceWoodPanelPadding(padX, padY, DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight, woodWall, blueWall);
                    }
                    else if (!right.IsEmpty && right.Room is BookshelfCell)
                    {
                        // Same on the other side — bookshelf claims its left-side padding.
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

                    StairBlock topStair = !top.IsEmpty ? top.Room as StairBlock : null;
                    (int woodStartX, int woodWidth) = topStair != null
                        ? topStair.GetFloorPaddingRange(top.SubCol, top.SubRow)
                        : (-1, 0);

                    bool eitherShaft = (!top.IsEmpty && top.Room is ShaftCell)
                                    || (!bottom.IsEmpty && bottom.Room is ShaftCell);
                    bool bottomIsCorridor = !bottom.IsEmpty && bottom.Room is CorridorCell;

                    // Step-void stair sub-cell (non-landing) above or below this padding keeps it solid.
                    bool bottomIsStairVoid = !bottom.IsEmpty && bottom.Room is StairBlock bs
                                             && !bs.IsTopLandingSubCell(bottom.SubCol, bottom.SubRow);
                    bool topIsStairVoid = !top.IsEmpty && top.Room is StairBlock ts
                                             && !ts.IsBottomLandingSubCell(top.SubCol, top.SubRow);

                    if (eitherShaft)
                    {
                        PlaceShaftFloorPadding(padX, padY, DungeonGrid.CellTileWidth, DungeonGrid.VerticalPadding, fillTileType);
                    }
                    else if (bottomIsCorridor)
                    {
                        // Corridor builds its own wood ceiling; leave the padding as solid fill.
                        FillSolid(padX, padY, DungeonGrid.CellTileWidth, DungeonGrid.VerticalPadding, fillTileType);
                    }
                    else if (bottomIsStairVoid || topIsStairVoid)
                    {
                        // Stair step-void edge on one side: keep the padding as solid stone.
                        FillSolid(padX, padY, DungeonGrid.CellTileWidth, DungeonGrid.VerticalPadding, fillTileType);
                    }
                    else
                    {
                        FillWoodFloor(padX, padY, DungeonGrid.CellTileWidth, DungeonGrid.VerticalPadding);
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

            // Wood tiles forming 4-tile thick top (above ceiling) and bottom (below floor) trim strips.
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            int trim = DungeonGrid.VerticalPadding;
            for (int lx = 0; lx < w; lx++)
            {
                for (int d = 0; d < trim; d++)
                {
                    WorldGenUtils.PlaceTile(x + lx, ceilingY - 1 - d, woodTile);
                    WorldGenUtils.PlaceTile(x + lx, floorY + 1 + d, woodTile);
                }
            }

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

        private static void PlaceWoodPanelPadding(int x, int y, int w, int h, ushort woodWall, ushort blueWall, bool skipAbove = false, bool skipBelow = false)
        {
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            int trim = DungeonGrid.VerticalPadding;

            // Wood ceiling and floor strips framing the panel
            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < trim; ly++)
                {
                    if (!skipAbove)
                        WorldGenUtils.PlaceTile(x + lx, y - trim + ly, woodTile);
                    if (!skipBelow)
                        WorldGenUtils.PlaceTile(x + lx, y + h + ly, woodTile);
                }
            }

            // Clear tiles in the padding area
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.ClearTile(x + lx, y + ly);

            ProceduralUtils.DrawWallPanel(x, y, w, h, woodWall, blueWall);
        }

        private static void PlaceShaftFloorPadding(int x, int y, int w, int h, int tileType)
        {
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            int edgeWidth = 2;

            // Tiles: solid wood at the outer 2-tile edges, cleared in the middle for stair passage
            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < h; ly++)
                {
                    if (lx < edgeWidth || lx >= w - edgeWidth)
                        WorldGenUtils.PlaceTile(x + lx, y + ly, woodTile);
                    else
                        WorldGenUtils.ClearTile(x + lx, y + ly);
                }
            }

            // Walls: fill entire padding with wood, with gap columns just inside the stone edges
            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            for (int lx = 0; lx < w; lx++)
            {
                if (lx == edgeWidth || lx == w - 1 - edgeWidth)
                    continue; // gap columns just inside the stone
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.SetWall(x + lx, y + ly, woodWall);
            }

            // Extend wood wall panels into the adjacent horizontal padding zones so the
            // shaft's side edge walls remain visually continuous across the seam.
            int sidePanelWidth = DungeonGrid.HorizontalPadding;
            for (int lx = 0; lx < sidePanelWidth; lx++)
                for (int ly = 0; ly < h; ly++)
                {
                    WorldGenUtils.SetWall(x - sidePanelWidth + lx, y + ly, woodWall);
                    WorldGenUtils.SetWall(x + w + lx, y + ly, woodWall);
                }
        }

        private static void FillWoodFloor(int x, int y, int w, int h)
        {
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.PlaceTile(x + lx, y + ly, woodTile);
        }

        private static void PlaceShaftSidePadding(int x, int y, int w, int h, ushort woodWall, ushort blueWall)
        {
            // Clear tiles through the padding
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.ClearTile(x + lx, y + ly);

            // Wood floor below and wood ceiling above this horizontal padding strip,
            // matching the vertical padding height so they blend with the shaft's top/bottom padding.
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            int trim = DungeonGrid.VerticalPadding;
            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < trim; ly++)
                {
                    WorldGenUtils.PlaceTile(x + lx, y - trim + ly, woodTile);
                    WorldGenUtils.PlaceTile(x + lx, y + h + ly, woodTile);
                }
            }

            // Narrower version of ShaftCell's DrawShaftWallPanel pattern:
            // outer wood border on sides and bottom, a 1-tile gap inset, and an inner fill
            // split by cut rows into top wood / middle blue / bottom wood sections.
            int drawHeight = h + 2; // panel extends one row above and one row below padY
            int drawStartY = y - 1;
            int innerTopCutY = 5;
            int innerBottomCutY = drawHeight - 4;

            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < drawHeight; ly++)
                {
                    int worldX = x + lx;
                    int worldY = drawStartY + ly;

                    bool isOuterBorder = (lx == 0 || lx == w - 1 || ly == drawHeight - 1);
                    // Top inner gap: inner columns are empty at the top while outer borders stay.
                    bool isTopInnerGap = (ly == 0 || ly == 1) && lx >= 1 && lx <= w - 2;
                    bool isGap = !isOuterBorder && (lx == 1 || lx == w - 2 || ly == drawHeight - 2);
                    bool isInner = (lx >= 2 && lx <= w - 3 && ly >= 2 && ly <= drawHeight - 3);
                    bool isCutRow = isInner && (ly == innerTopCutY || ly == innerBottomCutY);

                    if (isTopInnerGap)
                        continue;
                    else if (isOuterBorder)
                        WorldGenUtils.SetWall(worldX, worldY, woodWall);
                    else if (isGap || isCutRow)
                        continue;
                    else if (isInner)
                    {
                        bool isMiddleSection = ly > innerTopCutY && ly < innerBottomCutY;
                        WorldGenUtils.SetWall(worldX, worldY, isMiddleSection ? blueWall : woodWall);
                    }
                }
            }
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
