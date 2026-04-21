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

        private static readonly HashSet<Type> HorizontalAccepted = new()
        {
            typeof(BookshelfCell),
            typeof(CorridorCell)
        };

        public override HashSet<Type> GetAcceptedNeighbors(int subCol, int subRow, Direction side)
        {
            return side switch
            {
                Direction.Left => HorizontalAccepted,
                Direction.Right => HorizontalAccepted,
                _ => null
            };
        }

        public override CellExit[] Exits => new[]
        {
            new CellExit(new Point( 1, 0), new GridRoom[] { new BookshelfCell(), new CorridorCell() }),
            new CellExit(new Point(-1, 0), new GridRoom[] { new BookshelfCell(), new CorridorCell() }),
        };

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
            int[] types =  { -1, 0, -1, 1, -1, 0, 2, 0, -1, 1, -1 };

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
