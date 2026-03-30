using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Connectors
{
    public class FlatCorridor : IProcedural
    {
        private const int CorridorHeight = 8;

        int[] widths = { 1, 2, 1, 4, 1, 2, 1, 7, 1, 2, 1, 4, 1, 2, 1 };

        // shit code lets goo
        // 0 = wood, 1 = castle/stone, 2 = blue, -1 = gap
        int[] types = { -1, 0, -1, 1, -1, 0, -1, 2, -1, 0, -1, 1, -1, 0, -1 };

        public SocketAnchor Build(SocketAnchor entry, int fillTileType, int liningTileType)
        {
            int startX = entry.Position.X;
            int floorY = entry.Position.Y;

            int length = 0;
            for (int i = 0; i < widths.Length; i++) length += widths[i];

            int endX = startX + length;
            int ceilingY = floorY - CorridorHeight;

            for (int x = startX; x <= endX; x++)
                for (int y = ceilingY; y <= floorY; y++)
                    WorldGen.KillTile(x, y);

            int woodWall = ModContent.WallType<ArchiveWoodWall>();
            int castleWall = ModContent.WallType<CastleWall>();
            int blueWall = ModContent.WallType<ArchiveWoodWallBlue>();
            int wallTop = ceilingY + 1;
            int wallBottom = floorY - 1;

            int cursor = startX;

            for (int i = 0; i < widths.Length; i++)
            {
                if (types[i] >= 0)
                {
                    int wallType = types[i] switch { 0 => woodWall, 1 => castleWall, 2 => blueWall, _ => woodWall };
                    PlaceWallStripe(cursor, wallTop, widths[i], wallBottom, wallType);
                }
                cursor += widths[i];
            }

            // Wood walls inside the floor and ceiling to force the pattern
            for (int x = startX + 1; x < startX + length - 1; x++)
            {
                WorldGen.PlaceWall(x, floorY - CorridorHeight - 1, woodWall, true);
                WorldGen.PlaceWall(x, floorY + 1, woodWall, true);
            }

            return new SocketAnchor
            {
                Position = new Point(endX, floorY),
                Facing = SocketDirection.Right
            };
        }

        private static void PlaceWallStripe(int startX, int top, int width, int bottom, int wallType)
        {
            for (int x = startX; x < startX + width; x++)
                for (int y = top; y <= bottom; y++)
                    WorldGen.PlaceWall(x, y, wallType, true);
        }
    }
}
