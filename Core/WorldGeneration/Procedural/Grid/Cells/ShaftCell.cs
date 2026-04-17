using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class ShaftCell : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        private static readonly HashSet<Type> VerticalAccepted = new()
        {
            typeof(BookshelfCell),
            typeof(ShaftCell)
        };

        public override HashSet<Type> GetAcceptedNeighbors(int subCol, int subRow, Direction side)
        {
            return side switch
            {
                Direction.Top => VerticalAccepted,
                Direction.Bottom => VerticalAccepted,
                _ => null
            };
        }

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = DungeonGrid.CellTileWidth;
            int height = DungeonGrid.CellTileHeight;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);

            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();

            // Wall panels on left and right sides (2 tiles wide each, leaves 14 tiles for stairs)
            for (int x = 0; x < 2; x++)
                for (int y = 0; y < height; y++)
                {
                    WorldGenUtils.SetWall(origin.X + x, origin.Y + y, woodWall);
                    WorldGenUtils.SetWall(origin.X + width - 1 - x, origin.Y + y, woodWall);
                }
        }
    }
}
