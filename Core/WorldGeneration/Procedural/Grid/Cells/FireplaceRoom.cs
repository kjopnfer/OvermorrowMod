using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Content.WorldGeneration.Archives;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class FireplaceRoom : GridRoom
    {
        public override int CellWidth => 2;
        public override int CellHeight => 1;

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = FootprintWidth;   // 44
            int height = FootprintHeight; // 26

            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            ushort blueWall = (ushort)ModContent.WallType<ArchiveWoodWallBlue>();
            ushort castleWall = (ushort)ModContent.WallType<CastleWall>();

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);

            // Mirrored layout, 18 + 8 firebox + 18 = 44.
            //   Sub-cell:   castle(2) gap(1) wood(3) gap(1) panel(2) gap(1) panel(6) gap(1) wood(1)
            //   Middle:     gap(1) castle(6) gap(1)
            int cursor = 0;
            cursor += DrawSolidWall(origin.X + cursor, origin.Y, height, 2, castleWall);
            cursor += DrawWallGap(origin.X + cursor, origin.Y, height, 1);
            cursor += DrawSolidWall(origin.X + cursor, origin.Y, height, 3, woodWall);
            cursor += DrawWallGap(origin.X + cursor, origin.Y, height, 1);
            cursor += DrawColoredBody(origin.X + cursor, origin.Y, height, 2, woodWall, blueWall);
            cursor += DrawWallGap(origin.X + cursor, origin.Y, height, 1);
            cursor += DrawColoredBody(origin.X + cursor, origin.Y, height, 6, woodWall, blueWall);
            cursor += DrawWallGap(origin.X + cursor, origin.Y, height, 1);
            cursor += DrawSolidWall(origin.X + cursor, origin.Y, height, 1, woodWall);

            cursor += DrawWallGap(origin.X + cursor, origin.Y, height, 1);
            cursor += DrawSolidWall(origin.X + cursor, origin.Y, height, 6, castleWall);
            cursor += DrawWallGap(origin.X + cursor, origin.Y, height, 1);

            cursor += DrawSolidWall(origin.X + cursor, origin.Y, height, 1, woodWall);
            cursor += DrawWallGap(origin.X + cursor, origin.Y, height, 1);
            cursor += DrawColoredBody(origin.X + cursor, origin.Y, height, 6, woodWall, blueWall);
            cursor += DrawWallGap(origin.X + cursor, origin.Y, height, 1);
            cursor += DrawColoredBody(origin.X + cursor, origin.Y, height, 2, woodWall, blueWall);
            cursor += DrawWallGap(origin.X + cursor, origin.Y, height, 1);
            cursor += DrawSolidWall(origin.X + cursor, origin.Y, height, 3, woodWall);
            cursor += DrawWallGap(origin.X + cursor, origin.Y, height, 1);
            cursor += DrawSolidWall(origin.X + cursor, origin.Y, height, 2, castleWall);

            // Anchored so PlaceCozyArea's pillars bracket cols 19-24 where the Fireplace tile sits.
            int floorRow = origin.Y + height - 1;
            GrandArchiveRoom.PlaceCozyArea(origin.X + 8, floorRow, GrandArchiveRoom.RoomID.Yellow);
            WorldGen.PlaceObject(origin.X + 19, floorRow, ModContent.TileType<Fireplace>());
        }

        /// <summary>Paints a solid block of one wall type.</summary>
        private static int DrawSolidWall(int worldX, int worldY, int height, int width, ushort wall)
        {
            for (int dx = 0; dx < width; dx++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.SetWall(worldX + dx, worldY + y, wall);
            return width;
        }

        /// <summary>Clears walls in a vertical strip (gap columns).</summary>
        private static int DrawWallGap(int worldX, int worldY, int height, int width)
        {
            for (int dx = 0; dx < width; dx++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.ClearWall(worldX + dx, worldY + y);
            return width;
        }

        /// <summary>Draws a colored panel body. Vertical framing is supplied by surrounding gap columns.</summary>
        private static int DrawColoredBody(int worldX, int worldY, int height,
                                            int bodyWidth, ushort woodWall, ushort blueWall)
        {
            for (int dx = 0; dx < bodyWidth; dx++)
            {
                for (int y = 0; y < height; y++)
                {
                    int wx = worldX + dx;
                    int wy = worldY + y;

                    if (y == 0 || y == 5 || y == 23 || y == 25)
                        continue;
                    else if (y >= 1 && y <= 4)
                        WorldGenUtils.SetWall(wx, wy, woodWall);
                    else if (y == 24)
                        WorldGenUtils.SetWall(wx, wy, woodWall);
                    else
                        WorldGenUtils.SetWall(wx, wy, blueWall);
                }
            }
            return bodyWidth;
        }

        public override void BuildPadding(PaddingContext ctx)
        {
            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            ushort blueWall = (ushort)ModContent.WallType<ArchiveWoodWallBlue>();

            switch (ctx.Side)
            {
                case Direction.Left:
                case Direction.Right:
                    PaddingBuilder.PlaceWoodPanelPadding(
                        ctx.X, ctx.Y, ctx.Width, ctx.Height, woodWall, blueWall);
                    break;
                case Direction.Top:
                    // ReplaceTile keeps any cleared opening intact.
                    PaddingBuilder.FillWoodFloor(ctx.X, ctx.Y, ctx.Width, ctx.Height);
                    PaintFullWidthWoodRow(ctx, rowOffset: ctx.Height - 1);
                    break;
                case Direction.Bottom:
                    PaintSeamFloor(ctx);
                    // Trim line directly under the room; overrides PaintSeamFloor's seam-skip on this row.
                    PaintFullWidthWoodRow(ctx, rowOffset: 0);
                    break;
            }
        }

        /// <summary>Paints one row of ArchiveWoodWall across the full padding strip.</summary>
        private static void PaintFullWidthWoodRow(PaddingContext ctx, int rowOffset)
        {
            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            int wy = ctx.Y + rowOffset;
            for (int dx = 0; dx < ctx.Width; dx++)
                WorldGenUtils.SetWall(ctx.X + dx, wy, woodWall);
        }

        /// <summary>Paints wood across the strip, leaving the inner 6 cols of the internal seam as stone.</summary>
        private static void PaintSeamFloor(PaddingContext ctx)
        {
            ushort wood = (ushort)ModContent.TileType<ArchiveWood>();
            int seamInteriorStart = DungeonGrid.CellTileWidth + 1;
            int seamInteriorEnd = DungeonGrid.CellTileWidth + DungeonGrid.HorizontalPadding - 1;

            for (int lx = 0; lx < ctx.Width; lx++)
            {
                if (lx >= seamInteriorStart && lx < seamInteriorEnd) continue;
                for (int ly = 0; ly < ctx.Height; ly++)
                    WorldGenUtils.ReplaceTile(ctx.X + lx, ctx.Y + ly, wood);
            }
        }

        private static readonly GridRoom[] HorizontalNeighbors =
        {
            new BookshelfCell(),
            new CorridorCell(),
            new DescendingStair(),
            new AscendingStair(),
            new FireplaceRoom(),
        };

        protected override GridRoom[] AllowedNeighbors(Direction side) => side switch
        {
            Direction.Left or Direction.Right => HorizontalNeighbors,
            _ => Array.Empty<GridRoom>(),
        };

        public override bool IsOpenSide(int subCol, int subRow, Direction side)
        {
            if (IsInternalEdge(subCol, subRow, side)) return false;
            if (subCol == 0 && side == Direction.Left) return true;
            if (subCol == 1 && side == Direction.Right) return true;
            return false;
        }

        public override bool IsInternalEdge(int subCol, int subRow, Direction side)
        {
            if (subCol == 0 && side == Direction.Right) return true;
            if (subCol == 1 && side == Direction.Left) return true;
            return false;
        }
    }
}