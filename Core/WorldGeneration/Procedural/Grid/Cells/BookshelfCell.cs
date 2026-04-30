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

        /// <summary>
        /// Bookshelves are architectural rooms and accept connections on
        /// every cardinal side. The actual neighbor type is constrained
        /// separately by GetAcceptedNeighbors.
        /// </summary>
        public override bool IsOpenSide(int subCol, int subRow, Direction side) => true;

        public override CellExit[] Exits => new[]
        {
            // Horizontal exits
            new CellExit(new Point( 1, 0), new GridRoom[]
            {
                new BookshelfCell(),
                new CorridorCell(),
                new DescendingStair(),
                new AscendingStair(),
            }),
            new CellExit(new Point(-1, 0), new GridRoom[]
            {
                new BookshelfCell(),
                new CorridorCell(),
            }),

            // Vertical exits
            new CellExit(new Point(0,  1), new GridRoom[] { new ShaftCell() }),
            new CellExit(new Point(0, -1), new GridRoom[] { new ShaftCell() }),
        };

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = DungeonGrid.CellTileWidth;
            int height = DungeonGrid.CellTileHeight;

            // The cell's wood floor and ceiling are produced by
            // PaddingBuilder filling the vertical padding strips above and
            // below the cell. Drawing wood inside the cell here would
            // double-thicken those strips.
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);

            PlaceBookPanel(origin.X, origin.Y, width, height);
        }

        /// <summary>
        /// Places a bookshelf wall panel: wood frame on the sides, book wall
        /// fill with shelf rows every 4 tiles from the bottom.
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
