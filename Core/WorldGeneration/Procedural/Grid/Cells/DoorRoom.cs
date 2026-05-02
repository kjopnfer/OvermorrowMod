using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    /// <summary>
    /// The entrance/exit cell that holds a door linking this dungeon to
    /// another area (a second procedural section, a hand-crafted room, etc.).
    /// Both sides are horizontal connections: one side sits against the
    /// grid boundary (the portal side), the other faces the dungeon. The
    /// cell is agnostic about which is which; placement at an edge column
    /// makes it implicit via the walker's out-of-bounds rejection.
    /// </summary>
    public class DoorRoom : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

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

        /// <summary>
        /// Doors open horizontally only. Top and bottom are walls.
        /// </summary>
        public override bool IsOpenSide(int subCol, int subRow, Direction side) =>
            side == Direction.Left || side == Direction.Right;

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
                case Direction.Bottom:
                    PaddingBuilder.FillWoodFloor(ctx.X, ctx.Y, ctx.Width, ctx.Height);
                    break;
            }
        }

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = DungeonGrid.CellTileWidth;
            int height = DungeonGrid.CellTileHeight;

            // Tiles cleared so the slot is walkable. The door object itself
            // is not yet placed by this build pass.
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);
        }
    }
}
