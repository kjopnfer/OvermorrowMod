using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.WorldGeneration.Procedural;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldGeneration.TestSubworld
{
    public class TestGenPass : GenPass
    {
        const int RoomCount = 5;
        const int MinRoomWidth = 35;
        const int MaxRoomWidth = 55;
        const int MinRoomHeight = 20;
        const int MaxRoomHeight = 30;
        const int CorridorHeight = 8;

        public TestGenPass(string name, double loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating test world";

            Main.worldSurface = new TestSubworld().Height - 200;
            Main.rockLayer = new TestSubworld().Height;

            int fillTile = ModContent.TileType<CastleBrick>();
            int liningTile = ModContent.TileType<ArchiveWood>();

            Point pointA = new Point(new TestSubworld().Width / 2 - 150, new TestSubworld().Height / 2 + 90);
            Point pointB = new Point(new TestSubworld().Width / 2 + 100, new TestSubworld().Height / 2);

            var rooms = ProceduralGenerator.Generate(
                pointA, pointB, RoomCount,
                MinRoomWidth, MaxRoomWidth,
                MinRoomHeight, MaxRoomHeight,
                CorridorHeight,
                fillTile, liningTile);

            if (rooms.Count > 0)
            {
                Main.spawnTileX = rooms[0].Center.X;
                Main.spawnTileY = rooms[0].Center.Y;
            }
        }
    }
}
