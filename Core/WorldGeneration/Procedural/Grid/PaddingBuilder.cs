using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// Iterates every placed room once and lets each render its outward
    /// padding via <see cref="GridRoom.BuildPadding"/>. The builder itself
    /// owns no rendering decisions: it just computes the four strip
    /// rectangles around each room and dispatches.
    /// <para/>
    /// Rooms are blind to their neighbors. When two rooms share a strip,
    /// both write into it and later writes overwrite earlier ones. Iteration
    /// is column-major then row-major, so for any shared strip the room
    /// further down or further right paints last and wins.
    /// </summary>
    public static class PaddingBuilder
    {
        public static void BuildAll(DungeonGrid grid, int fillTileType, DungeonPalette palette)
        {
            // Collect every room anchor first so we can sort by
            // PaddingPriority before painting. Rooms with higher priority
            // paint last and win shared strips against lower-priority
            // neighbors. Within the same priority, original column-major
            // order is preserved (List.Sort is stable on equal keys when
            // we use a tuple sort key).
            var anchors = new List<(int priority, int col, int row, GridSlot slot)>();
            var processed = new HashSet<int>();

            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    var slot = grid.GetSlot(col, row);
                    if (slot == null || slot.IsEmpty) continue;
                    if (!processed.Add(slot.GroupId)) continue;
                    int anchorCol = col - slot.SubCol;
                    int anchorRow = row - slot.SubRow;
                    anchors.Add((slot.Room.PaddingPriority, anchorCol, anchorRow, slot));
                }
            }

            anchors.Sort((a, b) =>
            {
                int cmp = a.priority.CompareTo(b.priority);
                if (cmp != 0) return cmp;
                cmp = a.col.CompareTo(b.col);
                if (cmp != 0) return cmp;
                return a.row.CompareTo(b.row);
            });

            foreach (var (_, anchorCol, anchorRow, slot) in anchors)
            {
                var room = slot.Room;
                Point anchor = grid.GridToWorld(anchorCol, anchorRow);

                int w = room.FootprintWidth;
                int h = room.FootprintHeight;
                int hp = DungeonGrid.HorizontalPadding;
                int vp = DungeonGrid.VerticalPadding;

                room.BuildPadding(new PaddingContext(
                    Direction.Top,    anchor.X,      anchor.Y - vp, w,  vp, fillTileType, grid, anchorCol, anchorRow, palette));
                room.BuildPadding(new PaddingContext(
                    Direction.Bottom, anchor.X,      anchor.Y + h,  w,  vp, fillTileType, grid, anchorCol, anchorRow, palette));
                room.BuildPadding(new PaddingContext(
                    Direction.Left,   anchor.X - hp, anchor.Y,      hp, h,  fillTileType, grid, anchorCol, anchorRow, palette));
                room.BuildPadding(new PaddingContext(
                    Direction.Right,  anchor.X + w,  anchor.Y,      hp, h,  fillTileType, grid, anchorCol, anchorRow, palette));
            }
        }

        public static void PlaceCorridorPadding(int x, int y, int w, int h, ushort woodWall, ushort blueWall, bool skipAbove = false)
        {
            int ceilingY = y + 17;
            int floorY = ceilingY + 8;

            for (int lx = 0; lx < w; lx++)
                for (int ly = ceilingY; ly <= floorY; ly++)
                    WorldGenUtils.ClearTile(x + lx, ly);

            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            int trim = DungeonGrid.VerticalPadding;
            int stripBottom = y + h;
            for (int lx = 0; lx < w; lx++)
            {
                for (int d = 0; d < trim; d++)
                {
                    int aboveY = ceilingY - 1 - d;
                    int belowY = floorY + 1 + d;
                    if (!skipAbove && aboveY >= y)
                        WorldGenUtils.ReplaceTile(x + lx, aboveY, woodTile);
                    WorldGenUtils.ReplaceTile(x + lx, belowY, woodTile);
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

            // Wall trim above the ceiling and below the floor.
            for (int lx = 0; lx < w; lx++)
            {
                int aboveY = ceilingY - 1;
                int belowY = floorY + 1;
                if (!skipAbove && aboveY >= y)
                    WorldGenUtils.SetWall(x + lx, aboveY, woodWall);
                if (belowY < stripBottom) WorldGenUtils.SetWall(x + lx, belowY, woodWall);
            }
        }

        public static void PlaceWoodPanelPadding(int x, int y, int w, int h, ushort woodWall, ushort blueWall, bool skipAbove = false, bool skipBelow = false)
        {
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            int trim = DungeonGrid.VerticalPadding;

            // Wood ceiling and floor strips framing the panel.
            // ReplaceTile so adjacent padding that already carved an opening
            // (a shaft's vertical padding, a corridor's cleared walkway) is
            // not sealed by this wood trim.
            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < trim; ly++)
                {
                    if (!skipAbove)
                        WorldGenUtils.ReplaceTile(x + lx, y - trim + ly, woodTile);
                    if (!skipBelow)
                        WorldGenUtils.ReplaceTile(x + lx, y + h + ly, woodTile);
                }
            }

            // Clear tiles in the padding area
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.ClearTile(x + lx, y + ly);

            // Clear walls in the strip before drawing the panel. DrawWallPanel
            // intentionally leaves its gap rows and cut rows un-painted, so
            // any wall written into the strip by an earlier helper (e.g. a
            // corridor's blue side wall) would show through those gaps. A
            // pre-clear guarantees the panel reads clean regardless of who
            // painted before us.
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.ClearWall(x + lx, y + ly);

            ProceduralUtils.DrawWallPanel(x, y, w, h, woodWall, blueWall);
        }

        public static void PlaceShaftFloorPadding(int x, int y, int w, int h, int tileType)
        {
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            int edgeWidth = 2;

            // Tiles: solid wood at the outer 2-tile edges, cleared in the middle for stair passage.
            // ReplaceTile on the wood edges so this never seals an opening
            // a neighbor already carved.
            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < h; ly++)
                {
                    if (lx < edgeWidth || lx >= w - edgeWidth)
                        WorldGenUtils.ReplaceTile(x + lx, y + ly, woodTile);
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

        public static void FillWoodFloor(int x, int y, int w, int h)
        {
            // ReplaceTile so a neighbor's earlier carve (a shaft connection,
            // a corridor opening) is preserved through this paint pass.
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.ReplaceTile(x + lx, y + ly, woodTile);
        }

        public static void PlaceShaftSidePadding(int x, int y, int w, int h, ushort woodWall, ushort blueWall)
        {
            // Clear tiles through the padding
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.ClearTile(x + lx, y + ly);

            // Wood floor below and wood ceiling above this horizontal padding strip,
            // matching the vertical padding height so they blend with the shaft's top/bottom padding.
            // ReplaceTile so we do not seal openings a neighbor carved.
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            int trim = DungeonGrid.VerticalPadding;
            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < trim; ly++)
                {
                    WorldGenUtils.ReplaceTile(x + lx, y - trim + ly, woodTile);
                    WorldGenUtils.ReplaceTile(x + lx, y + h + ly, woodTile);
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

        public static void ClearPadding(int x, int y, int w, int h)
        {
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.ClearTile(x + lx, y + ly);
        }

        public static void FillSolid(int x, int y, int w, int h, int tileType)
        {
            ushort tile = (ushort)tileType;
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.PlaceTile(x + lx, y + ly, tile);
        }

        public static void FillFloor(int x, int y, int w, int h, int tileType)
        {
            ushort tile = (ushort)tileType;
            for (int lx = 0; lx < w; lx++)
                for (int ly = 0; ly < h; ly++)
                    WorldGenUtils.PlaceTile(x + lx, y + ly, tile);
        }
    }
}
