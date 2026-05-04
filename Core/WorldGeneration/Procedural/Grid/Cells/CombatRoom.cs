using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    /// <summary>
    /// Mandatory-encounter room. Same 2x1 footprint and bookshelf-sandwich
    /// connection rules as <see cref="LoungeRoom"/>, currently rendered as a
    /// fully cleared, wall-less space so it visually pops in the dungeon
    /// during placement testing. Decoration and enemy spawns will be added
    /// once placement is dialled in.
    /// </summary>
    public class CombatRoom : GridRoom
    {
        public override int CellWidth => 2;
        public override int CellHeight => 1;

        // Painted last so combat's corridor-style entry passages win the
        // shared strip against neighboring bookshelves' wood-panel padding.
        public override int PaddingPriority => 10;

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = FootprintWidth;
            int height = FootprintHeight;

            // Clear tiles AND walls across the entire footprint so the room
            // reads as a void box; easy to spot during testing.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);
                    WorldGenUtils.ClearWall(origin.X + x, origin.Y + y);
                }
            }
        }

        public override void BuildPadding(PaddingContext ctx)
        {
            // Side padding uses corridor-style geometry (low 8-tile-tall
            // passage with wood ceiling/floor trim) so the combat room
            // reads as a closed-off square: only a small entry/exit
            // opening breaches each side. Once combat starts those
            // openings can be sealed with doors. When the side faces
            // empty stone (no neighbor cell), fall back to the standard
            // wood-panel padding instead so the unused side reads like
            // a normal room edge.
            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            ushort blueWall = (ushort)ModContent.WallType<ArchiveWoodWallBlue>();

            switch (ctx.Side)
            {
                case Direction.Left:
                case Direction.Right:
                {
                    // Wipe whatever the neighbor's BuildPadding wrote into
                    // the shared strip first, then refill with the dungeon
                    // fill tile so the painters below have stone to carve
                    // through. Without the refill the strip is empty air
                    // and the corridor's wood trim (which uses ReplaceTile)
                    // wouldn't have anything to replace, leaving an obvious
                    // gap above and below the entry passage. PaddingBuilder
                    // iterates column-major, so combat lands here AFTER its
                    // left-side neighbor.
                    ushort fill = (ushort)ctx.FillTileType;
                    for (int lx = 0; lx < ctx.Width; lx++)
                    {
                        for (int ly = 0; ly < ctx.Height; ly++)
                        {
                            WorldGenUtils.ClearWall(ctx.X + lx, ctx.Y + ly);
                            WorldGenUtils.PlaceTile(ctx.X + lx, ctx.Y + ly, fill);
                        }
                    }

                    int neighborCol = ctx.Side == Direction.Left
                        ? ctx.Col - 1
                        : ctx.Col + CellWidth;
                    var neighborSlot = ctx.Grid?.GetSlot(neighborCol, ctx.Row);
                    bool hasNeighbor = neighborSlot != null && !neighborSlot.IsEmpty;

                    if (hasNeighbor)
                    {
                        PaddingBuilder.PlaceCorridorPadding(
                            ctx.X, ctx.Y, ctx.Width, ctx.Height, woodWall, blueWall);
                    }
                    else
                    {
                        PaddingBuilder.PlaceWoodPanelPadding(
                            ctx.X, ctx.Y, ctx.Width, ctx.Height, woodWall, blueWall);
                    }
                    break;
                }
                case Direction.Top:
                case Direction.Bottom:
                    PaddingBuilder.FillWoodFloor(ctx.X, ctx.Y, ctx.Width, ctx.Height);
                    break;
            }
        }

        // Combat rooms accept the standard architectural neighbors so the
        // spine and branches have flexibility in routing through them.
        private static readonly GridRoom[] HorizontalNeighbors =
        {
            new BookshelfCell(),
            new CorridorCell(),
            new DescendingStair(),
            new AscendingStair(),
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
