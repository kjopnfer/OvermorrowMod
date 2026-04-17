using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class BookshelfCell : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        private static readonly HashSet<Type> HorizontalAccepted = new()
        {
            typeof(BookshelfCell),
            typeof(CorridorCell),
            typeof(StairBlock)
        };

        private static readonly HashSet<Type> VerticalAccepted = new()
        {
            typeof(ShaftCell)
        };

        public override HashSet<Type> GetAcceptedNeighbors(int subCol, int subRow, Direction side)
        {
            return side switch
            {
                Direction.Left => HorizontalAccepted,
                Direction.Right => HorizontalAccepted,
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

            PlaceBookPanel(origin.X, origin.Y, width, height);
        }

        /// <summary>
        /// Places a bookshelf wall panel. Wood frame on the sides, book wall fill
        /// with shelf rows every 4 tiles from bottom.
        /// Extracted from ArchiveSmallRoom.PlaceBookPanel.
        /// </summary>
        private static void PlaceBookPanel(int startX, int startY, int w, int h)
        {
            ushort frameWall = (ushort)ModContent.WallType<ArchiveBookWallFrame>();
            ushort bookWall = (ushort)ModContent.WallType<ArchiveBookWall>();
            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            int bookHeight = 20;

            // 1-tile wood panel on each side, 16-tile book area in the middle
            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < h; ly++)
                {
                    int worldX = startX + lx;
                    int worldY = startY + ly;

                    if (lx == 0 || lx == w - 1)
                    {
                        WorldGenUtils.SetWall(worldX, worldY, woodWall);
                        continue;
                    }

                    int bookStart = h - bookHeight;
                    if (ly < bookStart)
                    {
                        WorldGenUtils.SetWall(worldX, worldY, woodWall);
                        continue;
                    }

                    int bookLx = lx - 1;
                    int bookW = w - 2;
                    int bookLy = ly - bookStart;
                    bool isBorder = (bookLx == 0 || bookLx == bookW - 1 || bookLy == 0 || bookLy == bookHeight - 1);
                    int fromBottom = (bookHeight - 1) - bookLy;
                    bool isShelfRow = (fromBottom % 4 == 0);
                    WorldGenUtils.SetWall(worldX, worldY, (isBorder || isShelfRow) ? frameWall : bookWall);
                }
            }
        }
    }
}
