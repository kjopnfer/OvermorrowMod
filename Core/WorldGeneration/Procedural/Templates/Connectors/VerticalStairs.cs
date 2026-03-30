using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Connectors
{
    public class VerticalStairs : IProceduralRoom
    {
        public int Width => 38;
        public int Height => 34;

        public EdgeSocket Left => null;
        public EdgeSocket Right => null;
        public EdgeSocket Top { get; } = new EdgeSocket(new Point(19, 0), SocketDirection.Up);
        public EdgeSocket Bottom { get; } = new EdgeSocket(new Point(19, 33), SocketDirection.Down);


        public SocketAnchor Build(Point origin, int fillTileType, int liningTileType)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    WorldGen.KillTile(origin.X + x, origin.Y + y, false, false, true);

            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 4; y++)
                    WorldGen.PlaceTile(origin.X + x, origin.Y + y, TileID.Emerald, true, true);

            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 4; y++)
                    WorldGen.PlaceTile(origin.X + x, origin.Y + Height - y - 1, TileID.Diamond, true, true);

            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 4; y++)
                    WorldGen.PlaceTile(origin.X + Width - x - 1, origin.Y + y, TileID.Emerald, true, true);

            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 4; y++)
                    WorldGen.PlaceTile(origin.X + Width - x - 1, origin.Y + Height - y - 1, TileID.Diamond, true, true);

            for (int x = 0; x < 3; x++)
                for (int y = 0; y < Height; y++)
                    WorldGen.PlaceWall(origin.X + x, origin.Y + y, ModContent.WallType<ArchiveWoodWall>(), true);

            return new SocketAnchor
            {
                Position = new Point(origin.X + Bottom.RelativePosition.X, origin.Y + Bottom.RelativePosition.Y),
                Facing = SocketDirection.Down
            };
        }
    }
}
