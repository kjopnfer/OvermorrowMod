using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Rooms
{
    public class ArchiveSmallRoom : IProceduralRoom
    {
        public int Width => 83;
        public int Height => 26;

        public EdgeSocket Left { get; }
        public EdgeSocket Right { get; }
        public EdgeSocket Top { get; }
        public EdgeSocket Bottom { get; }

        public ArchiveSmallRoom(
            List<IProceduralRoom> leftAccepted,
            List<IProceduralRoom> rightAccepted,
            List<IProceduralRoom> downAccepted = null)
        {
            Left = new EdgeSocket(new Point(0, Height - 1), SocketDirection.Left, leftAccepted);
            Right = new EdgeSocket(new Point(Width - 1, Height - 1), SocketDirection.Right, rightAccepted);
            Top = new EdgeSocket(new Point(Width / 2, 0), SocketDirection.Up);
            Bottom = new EdgeSocket(new Point(Width / 2, Height - 1), SocketDirection.Down, downAccepted);
        }


        public SocketAnchor Build(Point origin, int fillTileType, int liningTileType)
        {
            ClearInterior(origin);

            int cursor = origin.X;
            cursor += PlaceWoodPanel(cursor, origin.Y);
            cursor += PlaceBookPanel(cursor, origin.Y, 18);
            cursor += PlaceWoodPanel(cursor, origin.Y);
            cursor += PlaceBookPanel(cursor, origin.Y, 19);
            cursor += PlaceWoodPanel(cursor, origin.Y);
            cursor += PlaceBookPanel(cursor, origin.Y, 18);
            cursor += PlaceWoodPanel(cursor, origin.Y);

            WorldGen.PlaceTile(origin.X + Width / 2, origin.Y, TileID.Adamantite, true, true);
            WorldGen.PlaceTile(origin.X + Width / 2, origin.Y + Height - 1, TileID.Adamantite, true, true);
            WorldGen.PlaceTile(origin.X, origin.Y + Height - 1, TileID.Adamantite, true, true);
            WorldGen.PlaceTile(origin.X + Width - 1, origin.Y + Height - 1, TileID.Adamantite, true, true);

            return new SocketAnchor
            {
                Position = new Point(origin.X + Right.RelativePosition.X, origin.Y + Right.RelativePosition.Y),
                Facing = SocketDirection.Right
            };
        }

        private int PlaceWoodPanel(int startX, int startY)
        {
            int w = 7;
            int woodWall = ModContent.WallType<ArchiveWoodWall>();
            int blueWall = ModContent.WallType<ArchiveWoodWallBlue>();
            DrawWallPanel(startX, startY, w, Height, woodWall, blueWall);
            return w;
        }

        private int PlaceBookPanel(int startX, int startY, int w)
        {
            int frameWall = ModContent.WallType<ArchiveBookWallFrame>();
            int bookWall = ModContent.WallType<ArchiveBookWall>();
            int woodWall = ModContent.WallType<ArchiveWoodWall>();
            int bookHeight = 20;

            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < Height; ly++)
                {
                    int worldX = startX + lx;
                    int worldY = startY + ly;

                    if (lx == 0 || lx == w - 1)
                    {
                        WorldGen.PlaceWall(worldX, worldY, woodWall, true);
                        continue;
                    }

                    int bookStart = Height - bookHeight;
                    if (ly < bookStart)
                    {
                        WorldGen.PlaceWall(worldX, worldY, woodWall, true);
                        continue;
                    }

                    int bookLy = ly - bookStart;
                    bool isBorder = (lx == 1 || lx == w - 2 || bookLy == 0 || bookLy == bookHeight - 1);
                    int fromBottom = (bookHeight - 1) - bookLy;
                    bool isShelfRow = (fromBottom % 4 == 0);
                    WorldGen.PlaceWall(worldX, worldY, (isBorder || isShelfRow) ? frameWall : bookWall, true);
                }
            }

            return w;
        }

        private void ClearInterior(Point origin)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    WorldGen.KillTile(origin.X + x, origin.Y + y, false, false, true);
        }

        /// <summary>
        /// Nested rectangle wall panel: outer wood border, empty gap, inner fill with cut rows and blue middle.
        /// </summary>
        private static void DrawWallPanel(int rx, int ry, int w, int h, int woodWall, int blueWall)
        {
            int drawStartY = ry - 1;
            int drawEndY = ry + h;
            int drawHeight = drawEndY - drawStartY + 1;

            // Row indices where horizontal cuts split the inner fill into 3 sections
            int innerTopCutY = 6;
            int innerBottomCutY = drawHeight - 4;

            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < drawHeight; ly++)
                {
                    int worldX = rx + lx;
                    int worldY = drawStartY + ly;

                    bool isOuterBorder = (lx == 0 || lx == w - 1 || ly == 0 || ly == drawHeight - 1);
                    bool isGap = !isOuterBorder && (lx == 1 || lx == w - 2 || ly == 1 || ly == drawHeight - 2);
                    bool isInner = (lx >= 2 && lx <= w - 3 && ly >= 2 && ly <= drawHeight - 3);
                    bool isCutRow = isInner && (ly == innerTopCutY || ly == innerBottomCutY);

                    if (isOuterBorder)
                        WorldGen.PlaceWall(worldX, worldY, woodWall, true);
                    else if (isGap || isCutRow) { }
                    else if (isInner)
                    {
                        bool isMiddleSection = ly > innerTopCutY && ly < innerBottomCutY;
                        WorldGen.PlaceWall(worldX, worldY, isMiddleSection ? blueWall : woodWall, true);
                    }
                }
            }
        }
    }
}
