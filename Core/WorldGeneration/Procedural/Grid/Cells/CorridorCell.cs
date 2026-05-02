using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class CorridorCell : GridRoom
    {
        private const int CorridorHeight = 8;
        private const int CeilingOffset = 17;

        public override int CellWidth => 1;
        public override int CellHeight => 1;

        private static readonly GridRoom[] HorizontalNeighbors =
        {
            new BookshelfCell(),
            new CorridorCell(),
            new FireplaceRoom(),
        };

        protected override GridRoom[] AllowedNeighbors(Direction side) => side switch
        {
            Direction.Left or Direction.Right => HorizontalNeighbors,
            _ => Array.Empty<GridRoom>(),
        };

        /// <summary>
        /// Corridors are open on the horizontal sides only. Top and bottom
        /// are walls (the corridor's ceiling and floor).
        /// </summary>
        public override bool IsOpenSide(int subCol, int subRow, Direction side) =>
            side == Direction.Left || side == Direction.Right;

        public override bool AllowsEmptyNeighbors => false;

        public override void BuildPadding(PaddingContext ctx)
        {
            switch (ctx.Side)
            {
                case Direction.Left:
                    PaintHorizontalSide(ctx, neighborCol: ctx.Col - 1, neighborRow: ctx.Row);
                    break;
                case Direction.Right:
                    PaintHorizontalSide(ctx, neighborCol: ctx.Col + 1, neighborRow: ctx.Row);
                    break;

                case Direction.Bottom:
                    // Wood floor below the corridor so the strip continues
                    // the corridor's floor visually rather than reading as
                    // raw stone underneath the cell.
                    PaddingBuilder.FillWoodFloor(ctx.X, ctx.Y, ctx.Width, ctx.Height);
                    break;

                // Top is left as the initial stone fill: above the corridor's
                // own ceiling there is no architectural feature to render.
            }
        }

        /// <summary>
        /// Decides which pattern to paint into the horizontal strip on one
        /// side of the corridor based on the neighbor across that strip.
        /// <para/>
        /// The corridor's blue-wall passage pattern is meaningful only when
        /// it meets another corridor: there it reads as a continuous hallway.
        /// Against an architectural neighbor (bookshelf, door, fireplace,
        /// stair landing) the corridor defers and lets the neighbor's panel
        /// own the strip. Painting the corridor pattern in those cases would
        /// overlay a passage stripe on top of the architectural panel, which
        /// is the bug we are avoiding here.
        /// </summary>
        private static void PaintHorizontalSide(PaddingContext ctx, int neighborCol, int neighborRow)
        {
            var neighborSlot = ctx.Grid?.GetSlot(neighborCol, neighborRow);
            var neighborRoom = neighborSlot?.Room;

            if (neighborRoom is CorridorCell)
            {
                // Corridor-to-corridor: paint the full passage pattern
                // (cleared walkway, blue side walls, wood ceiling and floor
                // trim). Both corridors paint identical content into the
                // shared strip, so order is irrelevant.
                ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
                ushort blueWall = (ushort)ModContent.WallType<ArchiveWoodWallBlue>();
                PaddingBuilder.PlaceCorridorPadding(
                    ctx.X, ctx.Y, ctx.Width, ctx.Height, woodWall, blueWall);
                return;
            }

            // Hook for stair-meets-corridor: a future visual goes here. The
            // structure is ready; fill in the body when you decide what the
            // transition should look like.
            if (neighborRoom is DescendingStair || neighborRoom is AscendingStair)
            {
                // PaintCorridorToStairTransition(ctx);
                return;
            }

            // Any other neighbor (bookshelf, door, fireplace, empty, off-grid):
            // the architectural side owns the strip via its own BuildPadding.
            // Painting nothing here means we leave the strip untouched and let
            // the neighbor's panel render cleanly.
        }

        /// <summary>
        /// A corridor cannot sit directly above or below a shaft, since
        /// shafts only accept bookshelves on their vertical ends.
        /// </summary>
        public override bool IsValidPlacement(DungeonGrid grid, Point anchor, System.Func<int, int, GridRoom> pendingLookup = null)
        {
            var above = GetEffectiveRoomAt(grid, pendingLookup, anchor.X, anchor.Y - 1);
            var below = GetEffectiveRoomAt(grid, pendingLookup, anchor.X, anchor.Y + 1);
            if (above is ShaftCell) return false;
            if (below is ShaftCell) return false;
            return true;
        }

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = DungeonGrid.CellTileWidth;
            int ceilingY = origin.Y + CeilingOffset;
            int floorY = ceilingY + CorridorHeight;

            // Clear only the walkable corridor area
            for (int x = origin.X; x < origin.X + width; x++)
                for (int y = ceilingY; y <= floorY; y++)
                    WorldGenUtils.ClearTile(x, y);

            // Replace the 4 tiles directly above the walkable ceiling with wood.
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            int woodThickness = DungeonGrid.VerticalPadding;
            for (int x = origin.X; x < origin.X + width; x++)
                for (int y = ceilingY - woodThickness; y < ceilingY; y++)
                    WorldGenUtils.PlaceTile(x, y, woodTile);

            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            ushort castleWall = (ushort)ModContent.WallType<CastleWall>();
            ushort blueWall = (ushort)ModContent.WallType<ArchiveWoodWallBlue>();

            // Wall stripe pattern scaled for 18 tiles:
            // gap(1) wood(2) gap(1) castle(3) gap(1) wood(1) blue(3) wood(1) gap(1) castle(3) gap(1)
            int[] widths = { 1, 2, 1, 3, 1, 1, 3, 1, 1, 3, 1 };
            int[] types = { -1, 0, -1, 1, -1, 0, 2, 0, -1, 1, -1 };

            int wallTop = ceilingY + 1;
            int wallBottom = floorY - 1;

            int cursor = origin.X;
            for (int i = 0; i < widths.Length; i++)
            {
                if (types[i] >= 0)
                {
                    ushort wallType = types[i] switch
                    {
                        0 => woodWall,
                        1 => castleWall,
                        2 => blueWall,
                        _ => woodWall
                    };

                    for (int x = cursor; x < cursor + widths[i]; x++)
                        for (int y = wallTop; y <= wallBottom; y++)
                            WorldGenUtils.SetWall(x, y, wallType);
                }
                cursor += widths[i];
            }

            // Ceiling and floor trim
            for (int x = origin.X + 1; x < origin.X + width - 1; x++)
            {
                WorldGenUtils.SetWall(x, ceilingY - 1, woodWall);
                WorldGenUtils.SetWall(x, floorY + 1, woodWall);
            }
        }
    }
}
