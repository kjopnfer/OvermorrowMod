using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria;
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



        public void Build(Point origin, int fillTileType, int liningTileType)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    Tile t = Main.tile[origin.X + x, origin.Y + y];
                    t.HasTile = false;
                }

            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 4; y++)
                {
                    void Place(int wx, int wy) { 
                        Tile t = Main.tile[wx, wy]; t.TileType = woodTile; t.HasTile = true; 
                    }

                    Place(origin.X + x,             origin.Y + y);
                    Place(origin.X + x,             origin.Y + Height - y - 1);
                    Place(origin.X + Width - x - 1, origin.Y + y);
                    Place(origin.X + Width - x - 1, origin.Y + Height - y - 1);
                }

            int woodWall = ModContent.WallType<ArchiveWoodWall>();
            int blueWall = ModContent.WallType<ArchiveWoodWallBlue>();

            for (int x = 0; x < 3; x++)
                for (int y = 0; y < Height; y++)
                {
                    WorldGen.PlaceWall(origin.X + x, origin.Y + y, woodWall, true);
                    WorldGen.PlaceWall(origin.X + Width - x - 1, origin.Y + y, woodWall, true);
                }

            ProceduralUtils.DrawWallPanel(origin.X + 3, origin.Y + 4, 7, Height - 8, woodWall, blueWall);
            ProceduralUtils.DrawWallPanel(origin.X + 11, origin.Y + 4, 16, Height - 8, woodWall, blueWall);
            ProceduralUtils.DrawWallPanel(origin.X + 27, origin.Y + 4, 7, Height - 8, woodWall, blueWall);

            // Middle
            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 3; y++)
                {
                    WorldGen.PlaceWall(origin.X + x + 13, origin.Y + y, woodWall, true);
                    WorldGen.PlaceWall(origin.X + x + 13, origin.Y + y + 31, woodWall, true);
                }

            WorldGen.KillWall(origin.X + 12, origin.Y + 3);
            WorldGen.KillWall(origin.X + 25, origin.Y + 3);
            WorldGen.KillWall(origin.X + 12, origin.Y + 30);
            WorldGen.KillWall(origin.X + 25, origin.Y + 30);

            for (int y = 0; y < 3; y++)
            {
                WorldGen.PlaceWall(origin.X + 11, origin.Y + y, woodWall, true);
                WorldGen.PlaceWall(origin.X + 11, origin.Y + y + 30, woodWall, true);
                WorldGen.PlaceWall(origin.X + 26, origin.Y + y, woodWall, true);
                WorldGen.PlaceWall(origin.X + 26, origin.Y + y + 30, woodWall, true);
            }

        }
    }
}
