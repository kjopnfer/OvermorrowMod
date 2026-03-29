using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Rooms
{
    public class ArchiveSmallRoom : IRoomTemplate
    {
        public int Width => 40;
        public int Height => 32;

        private const int StoneEdgeWidth = 4;
        private const int BorderThickness = 4;
        private const int Padding = 10;
        private const int CorridorHeight = 8;

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
            PlaceWoodBorders(position, liningTileType, fillTileType);
            PlaceCastleWalls(position);
            PlaceWallPanels(position);
            AddSockets(room);

            return room;
        }

        private void AddSockets(ProceduralRoom room)
        {
            room.SetEdgeSocket(new EdgeSocket(
                new Point(0, Height - 1),
                SocketDirection.Left,
                _leftSocketAccepted
            ));

            room.SetEdgeSocket(new EdgeSocket(
                new Point(39, Height - 1),
                SocketDirection.Right,
                _rightSocketAccepted
            ));
        }

        private void ClearInterior(Point position)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    WorldGen.KillTile(position.X + x, position.Y + y, false, false, true);
        }

        private void PlaceCastleWalls(Point position)
        {
            int castleWall = ModContent.WallType<CastleWall>();
            for (int x = position.X; x < position.X + Width; x++)
                for (int y = position.Y; y < position.Y + Height; y++)
                    WorldGen.PlaceWall(x, y, castleWall, true);

            int inset = StoneEdgeWidth + 2;
            for (int x = position.X + inset; x < position.X + Width - inset; x++)
                for (int y = position.Y; y < position.Y + Height; y++)
                    WorldGen.KillWall(x, y, false);
        }

        private void PlaceWallPanels(Point position)
        {
            int inset = StoneEdgeWidth + 2;
            int panelStartX = position.X + inset + 1;
            int panelEndX = position.X + Width - inset - 2;
            int panelWidth = panelEndX - panelStartX + 1;

            int woodWall = ModContent.WallType<ArchiveWoodWall>();
            int blueWall = ModContent.WallType<ArchiveWoodWallBlue>();
            ProceduralGenerator.PlaceWallPanels(panelStartX, position.Y, panelWidth, Height, woodWall, blueWall);
        }

        private void PlaceWoodBorders(Point position, int liningTileType, int fillTileType)
        {
            WorldGen.PlaceTile(position.X + StoneEdgeWidth + 1, position.Y, TileID.Adamantite, true, true);
            WorldGen.PlaceTile(position.X + Width - StoneEdgeWidth - 2, position.Y, TileID.Adamantite, true, true);

            for (int x = position.X + StoneEdgeWidth + 1; x <= position.X + Width - StoneEdgeWidth - 2; x++)
            {
                WorldGen.PlaceTile(x, position.Y - 1, ModContent.TileType<ArchiveWood>(), true, true);
                WorldGen.PlaceTile(x, position.Y - 2, ModContent.TileType<ArchiveWood>(), true, true);
                WorldGen.PlaceTile(x, position.Y - 3, ModContent.TileType<ArchiveWood>(), true, true);
                WorldGen.PlaceTile(x, position.Y - 4, ModContent.TileType<ArchiveWood>(), true, true);
            }

            for (int x = position.X + StoneEdgeWidth + 1; x <= position.X + Width - StoneEdgeWidth - 2; x++)
            {
                WorldGen.PlaceTile(x, position.Y + Height, ModContent.TileType<ArchiveWood>(), true, true);
                WorldGen.PlaceTile(x, position.Y + Height + 1, ModContent.TileType<ArchiveWood>(), true, true);
                WorldGen.PlaceTile(x, position.Y + Height + 2, ModContent.TileType<ArchiveWood>(), true, true);
                WorldGen.PlaceTile(x, position.Y + Height + 3, ModContent.TileType<ArchiveWood>(), true, true);
            }
        }
    }
}
