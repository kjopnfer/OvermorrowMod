using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Connectors
{
    public class FlatCorridor : IProceduralRoom
    {
        private const int CorridorHeight = 8;

        // 0 = wood, 1 = castle/stone, 2 = blue, -1 = gap
        int[] widths = { 1, 2, 1, 4, 1, 2, 1, 7, 1, 2, 1, 4, 1, 2, 1 };
        int[] types = { -1, 0, -1, 1, -1, 0, -1, 2, -1, 0, -1, 1, -1, 0, -1 };

        public int Width
        {
            get { int l = 0; for (int i = 0; i < widths.Length; i++) l += widths[i]; return l; }
        }

        public int Height => CorridorHeight + 1;

        public EdgeSocket Left { get; } = new EdgeSocket(new Point(0, 8), SocketDirection.Left);
        public EdgeSocket Right { get; }
        public EdgeSocket Top => null;
        public EdgeSocket Bottom => null;

        public FlatCorridor()
        {
            Right = new EdgeSocket(new Point(Width - 1, CorridorHeight), SocketDirection.Right);
        }


        public void Build(Point origin, int fillTileType, int liningTileType)
        {
            int floorY = origin.Y + CorridorHeight;
            int ceilingY = origin.Y;

            for (int x = origin.X; x < origin.X + Width; x++)
                for (int y = ceilingY; y <= floorY; y++)
                    WorldGen.KillTile(x, y);

            int woodWall = ModContent.WallType<ArchiveWoodWall>();
            int castleWall = ModContent.WallType<CastleWall>();
            int blueWall = ModContent.WallType<ArchiveWoodWallBlue>();
            int wallTop = ceilingY + 1;
            int wallBottom = floorY - 1;

            int cursor = origin.X;
            for (int i = 0; i < widths.Length; i++)
            {
                if (types[i] >= 0)
                {
                    int wallType = types[i] switch { 0 => woodWall, 1 => castleWall, 2 => blueWall, _ => woodWall };
                    PlaceWallStripe(cursor, wallTop, widths[i], wallBottom, wallType);
                }
                cursor += widths[i];
            }

            for (int x = origin.X + 1; x < origin.X + Width - 1; x++)
            {
                WorldGen.PlaceWall(x, ceilingY - 1, woodWall, true);
                WorldGen.PlaceWall(x, floorY + 1, woodWall, true);
            }
        }

        private static void PlaceWallStripe(int startX, int top, int width, int bottom, int wallType)
        {
            for (int x = startX; x < startX + width; x++)
                for (int y = top; y <= bottom; y++)
                    WorldGen.PlaceWall(x, y, wallType, true);
        }
    }
}
