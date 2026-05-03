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
    public class LoungeRoom : GridRoom
    {
        public override int CellWidth => 2;
        public override int CellHeight => 1;

        // Caps and spacing rules used by IsValidPlacement.
        private const int MaxInstancesPerDungeon = 2;
        private const int MinSpacingBetweenLounges = 5;
        private const int MinDistanceFromDoor = 2;

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

            // Layout from pixel2.png. Three colored panels (6, 10, 6) sit
            // contiguous in the middle, flanked by 2-wide wood-only panels,
            // 3-wide wood columns, gaps, and 2-wide castle edges.
            //
            //   castle(2) gap wood(3) gap panel(2) gap panel(6) gap panel(10)
            //   gap panel(6) gap panel(2) gap wood(3) gap castle(2)
            int cursor = 0;
            cursor += DrawSolidWall(origin.X + cursor, origin.Y, height, 2, castleWall);
            cursor += DrawWallGap (origin.X + cursor, origin.Y, height, 1);
            cursor += DrawSolidWall(origin.X + cursor, origin.Y, height, 3, woodWall);
            cursor += DrawWallGap (origin.X + cursor, origin.Y, height, 1);
            cursor += DrawColoredBody(origin.X + cursor, origin.Y, height, 2, woodWall, blueWall);
            cursor += DrawWallGap (origin.X + cursor, origin.Y, height, 1);
            cursor += DrawColoredBody(origin.X + cursor, origin.Y, height, 6, woodWall, blueWall);
            cursor += DrawWallGap (origin.X + cursor, origin.Y, height, 1);
            cursor += DrawColoredBody(origin.X + cursor, origin.Y, height, 10, woodWall, blueWall);
            cursor += DrawWallGap (origin.X + cursor, origin.Y, height, 1);
            cursor += DrawColoredBody(origin.X + cursor, origin.Y, height, 6, woodWall, blueWall);
            cursor += DrawWallGap (origin.X + cursor, origin.Y, height, 1);
            cursor += DrawColoredBody(origin.X + cursor, origin.Y, height, 2, woodWall, blueWall);
            cursor += DrawWallGap (origin.X + cursor, origin.Y, height, 1);
            cursor += DrawSolidWall(origin.X + cursor, origin.Y, height, 3, woodWall);
            cursor += DrawWallGap (origin.X + cursor, origin.Y, height, 1);
            cursor += DrawSolidWall(origin.X + cursor, origin.Y, height, 2, castleWall);

            // Furniture: PlaceLoungeArea spans roughly x..x+23, so anchor
            // at origin.X + 10 to center the lounge inside the 44-wide room.
            int floorRow = origin.Y + height - 1;
            GrandArchiveRoom.PlaceLoungeArea(origin.X + 10, floorRow, GrandArchiveRoom.RoomID.Yellow);
        }

        private static int DrawSolidWall(int worldX, int worldY, int height, int width, ushort wall)
        {
            for (int dx = 0; dx < width; dx++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.SetWall(worldX + dx, worldY + y, wall);
            return width;
        }

        private static int DrawWallGap(int worldX, int worldY, int height, int width)
        {
            for (int dx = 0; dx < width; dx++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.ClearWall(worldX + dx, worldY + y);
            return width;
        }

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
                    PaddingBuilder.FillWoodFloor(ctx.X, ctx.Y, ctx.Width, ctx.Height);
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

        // Lounges must always sit between bookshelves, so they accept
        // bookshelves only on both horizontal sides.
        private static readonly GridRoom[] HorizontalNeighbors =
        {
            new BookshelfCell(),
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

        /// <summary>
        /// Cap total lounges, enforce minimum spacing between them, and keep
        /// them away from doors. Counts unique room instances across the
        /// committed grid AND in-progress placements via pendingLookup.
        /// </summary>
        public override bool IsValidPlacement(DungeonGrid grid, Point anchor,
                                              Func<int, int, GridRoom> pendingLookup = null)
        {
            // Bounded proximity scan covers door distance and lounge spacing.
            int radius = Math.Max(MinSpacingBetweenLounges, MinDistanceFromDoor);
            int cMin = Math.Max(0, anchor.X - radius);
            int cMax = Math.Min(grid.Cols - 1, anchor.X + radius);
            int rMin = Math.Max(0, anchor.Y - radius);
            int rMax = Math.Min(grid.Rows - 1, anchor.Y + radius);

            for (int c = cMin; c <= cMax; c++)
            {
                for (int r = rMin; r <= rMax; r++)
                {
                    var room = GetEffectiveRoomAt(grid, pendingLookup, c, r);
                    if (room == null) continue;
                    int dist = Math.Max(Math.Abs(c - anchor.X), Math.Abs(r - anchor.Y));
                    if (room is LoungeRoom && dist < MinSpacingBetweenLounges) return false;
                    if (room is DoorRoom && dist < MinDistanceFromDoor) return false;
                }
            }

            // Lounge count cap: cheap grid-only pass (no pendingLookup walk).
            var seen = new HashSet<GridRoom>();
            int loungeCount = 0;
            for (int c = 0; c < grid.Cols; c++)
            {
                for (int r = 0; r < grid.Rows; r++)
                {
                    var slot = grid.GetSlot(c, r);
                    if (slot == null || slot.IsEmpty) continue;
                    if (slot.Room is LoungeRoom && seen.Add(slot.Room)) loungeCount++;
                }
            }
            return loungeCount < MaxInstancesPerDungeon;
        }
    }
}
