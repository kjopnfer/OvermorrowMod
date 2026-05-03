using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class ShaftCell : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        private static readonly GridRoom[] VerticalNeighbors =
        {
            new ShaftCell(),
            new BookshelfCell(),
        };

        protected override GridRoom[] AllowedNeighbors(Direction side) => side switch
        {
            Direction.Top or Direction.Bottom => VerticalNeighbors,
            _ => Array.Empty<GridRoom>(),
        };

        public override bool IsOpenSide(int subCol, int subRow, Direction side) =>
            side == Direction.Top || side == Direction.Bottom;

        public override bool AllowsEmptyNeighbors => false;

        public override void BuildPadding(PaddingContext ctx)
        {
            switch (ctx.Side)
            {
                case Direction.Left:
                case Direction.Right:
                    {
                        ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
                        ushort blueWall = (ushort)ModContent.WallType<ArchiveWoodWallBlue>();
                        PaddingBuilder.PlaceWoodPanelPadding(
                            ctx.X, ctx.Y, ctx.Width, ctx.Height, woodWall, blueWall);

                        int sconceRow = ctx.Y + ctx.Height - 1 - 11;
                        int sconceCol = ctx.X + 3;
                        WorldGen.PlaceObject(sconceCol, sconceRow, ModContent.TileType<WaxSconceEven>());
                        break;
                    }
                case Direction.Top:
                case Direction.Bottom:
                    // PlaceShaftFloorPadding leaves the diagonal-stair gap; FillWoodFloor would seal it.
                    PaddingBuilder.PlaceShaftFloorPadding(
                        ctx.X, ctx.Y, ctx.Width, ctx.Height, ctx.FillTileType);
                    break;
            }
        }

        /// <summary>Vertical neighbors must be empty, another shaft, or a bookshelf.</summary>
        public override bool IsValidPlacement(DungeonGrid grid, Point anchor, System.Func<int, int, GridRoom> pendingLookup = null)
        {
            var above = GetEffectiveRoomAt(grid, pendingLookup, anchor.X, anchor.Y - 1);
            var below = GetEffectiveRoomAt(grid, pendingLookup, anchor.X, anchor.Y + 1);
            if (above != null && above is not (BookshelfCell or ShaftCell))
                return false;
            if (below != null && below is not (BookshelfCell or ShaftCell))
                return false;
            return true;
        }

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = DungeonGrid.CellTileWidth;
            int height = DungeonGrid.CellTileHeight;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);

            int woodWallId = ModContent.WallType<ArchiveWoodWall>();
            int blueWallId = ModContent.WallType<ArchiveWoodWallBlue>();
            ushort woodWall = (ushort)woodWallId;

            // Side edge walls (1 wide each).
            for (int y = 0; y < height; y++)
            {
                WorldGenUtils.SetWall(origin.X, origin.Y + y, woodWall);
                WorldGenUtils.SetWall(origin.X + width - 1, origin.Y + y, woodWall);
            }

            DrawShaftWallPanel(origin.X + 1, origin.Y + 1, 16, 25, woodWall, (ushort)blueWallId);
        }

        private static void DrawShaftWallPanel(int rx, int ry, int w, int h, ushort woodWall, ushort blueWall)
        {
            int drawStartY = ry - 1;
            int drawEndY = ry + h;
            int drawHeight = drawEndY - drawStartY + 1;

            // Top row is an open gap matching the padding above.
            int innerTopCutY = 5;
            int innerBottomCutY = drawHeight - 4;

            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < drawHeight; ly++)
                {
                    int worldX = rx + lx;
                    int worldY = drawStartY + ly;

                    if (ly == 0)
                        continue;

                    bool isOuterBorder = (lx == 0 || lx == w - 1 || ly == drawHeight - 1);
                    bool isGap = !isOuterBorder && (lx == 1 || lx == w - 2 || ly == drawHeight - 2);
                    bool isInner = (lx >= 2 && lx <= w - 3 && ly >= 1 && ly <= drawHeight - 3);
                    bool isCutRow = isInner && (ly == innerTopCutY || ly == innerBottomCutY);

                    if (isOuterBorder)
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
    }
}
