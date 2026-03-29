using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Rooms
{
    public class ArchiveSmallRoom : IRoomTemplate
    {
        public int Width => 82;
        public int Height => 26;

        private Point LeftSocketRel => new Point(0, Height - 1);
        private Point RightSocketRel => new Point(Width - 1, Height - 1);

        private readonly List<IProcedural> _leftSocketAccepted;
        private readonly List<IProcedural> _rightSocketAccepted;

        public ArchiveSmallRoom(List<IProcedural> leftSocketAccepted, List<IProcedural> rightSocketAccepted)
        {
            _leftSocketAccepted = leftSocketAccepted;
            _rightSocketAccepted = rightSocketAccepted;
        }

        public Point AlignTo(SocketAnchor anchor)
        {
            Point socketRel = anchor.Facing.Opposite() switch
            {
                SocketDirection.Left => LeftSocketRel,
                SocketDirection.Right => RightSocketRel,
                _ => LeftSocketRel
            };
            return new Point(anchor.Position.X - socketRel.X, anchor.Position.Y - socketRel.Y);
        }

        public ProceduralRoom Generate(Point position, int fillTileType, int liningTileType)
        {
            var room = new ProceduralRoom(position, Width, Height);

            ClearInterior(position);

            int cursor = position.X;
            cursor += PlaceWoodPanel(cursor, position.Y);
            cursor += PlaceBookPanel(cursor, position.Y);
            cursor += PlaceWoodPanel(cursor, position.Y);
            cursor += PlaceBookPanel(cursor, position.Y);
            cursor += PlaceWoodPanel(cursor, position.Y);
            cursor += PlaceBookPanel(cursor, position.Y);
            cursor += PlaceWoodPanel(cursor, position.Y);

            AddSockets(room);

            return room;
        }

        private void AddSockets(ProceduralRoom room)
        {
            room.SetEdgeSocket(new EdgeSocket(
                LeftSocketRel,
                SocketDirection.Left,
                _leftSocketAccepted
            ));

            room.SetEdgeSocket(new EdgeSocket(
                RightSocketRel,
                SocketDirection.Right,
                _rightSocketAccepted
            ));
        }

        private int PlaceWoodPanel(int startX, int startY)
        {
            int w = 7;
            int woodWall = ModContent.WallType<ArchiveWoodWall>();
            int blueWall = ModContent.WallType<ArchiveWoodWallBlue>();
            DrawWallPanel(startX, startY, w, Height, woodWall, blueWall);
            return w;
        }

        private int PlaceBookPanel(int startX, int startY)
        {
            int frameWall = ModContent.WallType<ArchiveBookWallFrame>();
            int bookWall = ModContent.WallType<ArchiveBookWall>();
            int woodWall = ModContent.WallType<ArchiveWoodWall>();

            int w = 18;
            int bookHeight = 20;

            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < Height; ly++)
                {
                    int worldX = startX + lx;
                    int worldY = startY + ly;

                    // Left and right columns are wood padding
                    if (lx == 0 || lx == w - 1)
                    {
                        WorldGen.PlaceWall(worldX, worldY, woodWall, true);
                        continue;
                    }

                    // Top area is wood
                    int bookStart = Height - bookHeight;
                    if (ly < bookStart)
                    {
                        WorldGen.PlaceWall(worldX, worldY, woodWall, true);
                        continue;
                    }

                    // Book area: frame border + shelf rows, book fill
                    int bookLy = ly - bookStart;
                    bool isBorder = (lx == 1 || lx == w - 2 || bookLy == 0 || bookLy == bookHeight - 1);
                    int fromBottom = (bookHeight - 1) - bookLy;
                    bool isShelfRow = (fromBottom % 4 == 0);
                    WorldGen.PlaceWall(worldX, worldY, (isBorder || isShelfRow) ? frameWall : bookWall, true);
                }
            }

            return w;
        }

        private void ClearInterior(Point position)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    WorldGen.KillTile(position.X + x, position.Y + y, false, false, true);
        }

        /// <summary>
        /// Draws a nested rectangle wall panel:
        /// Rect 1 (outer): wood border
        /// Rect 2: empty gap (1 tile inset)
        /// Rect 3 (inner): wood fill, with horizontal cut rows and a blue middle section
        /// </summary>
        private static void DrawWallPanel(int rx, int ry, int w, int h, int woodWall, int blueWall)
        {
            int drawStartY = ry - 1;
            int drawEndY = ry + h;
            int drawHeight = drawEndY - drawStartY + 1;

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
                    {
                        WorldGen.PlaceWall(worldX, worldY, woodWall, true);
                    }
                    else if (isGap || isCutRow)
                    {
                        // Empty
                    }
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
