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
        public TestGenPass(string name, double loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating test world";

            Main.worldSurface = new TestSubworld().Height - 200;
            Main.rockLayer = new TestSubworld().Height;

            int tileType = ModContent.TileType<ArchiveWood>();

            Point pointA = new Point(new TestSubworld().Width / 2 - 100, new TestSubworld().Height / 2 + 25);
            Point pointB = new Point(new TestSubworld().Width / 2 + 100, new TestSubworld().Height / 2 + 15);

            var rooms = ProceduralGenerator.Generate(pointA, pointB, 5, 30, 20, tileType);

            if (rooms.Count > 0)
            {
                Main.spawnTileX = rooms[0].Center.X;
                Main.spawnTileY = rooms[0].Center.Y;
            }
        }
    }
}
