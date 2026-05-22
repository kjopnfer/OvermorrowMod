using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.NPCs;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells;
using System;
using System.Collections.Generic;
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

            int fillTile = ModContent.TileType<CastleBrick>();
            int liningTile = ModContent.TileType<ArchiveWood>();

            int centerX = new TestSubworld().Width / 2;
            int centerY = new TestSubworld().Height / 2;

            var rand = new Random(Environment.TickCount);

            var cellPool = new List<GridRoom>
            {
                new BookshelfCell(),
                new BookshelfCell(),
                new BookshelfCell(),
                new CorridorCell()
            };

            int gridCols = 35;
            int gridRows = 30;
            int startX = 100;
            int startY = centerY - (gridRows * DungeonGrid.VerticalSpacing) / 2;

            var bindings = new Dictionary<(byte R, byte G, byte B), SpawnPool>
            {
                [(255, 0, 0)] = ArchiveSpawnPool.BaseGroundPool,
                [(221, 255, 0)] = ArchiveSpawnPool.WallPool,
            };

            ArchiveSpawnPool.Initialize();

            try
            {
                GridGenerator.Build(new Point(startX, startY), gridCols, gridRows, cellPool, fillTile, liningTile, rand, baseDensity: 1.0f, eliteChance: 0.10f, bindings: bindings, out Point startDoorTile);
                Main.spawnTileX = startDoorTile.X;
                Main.spawnTileY = startDoorTile.Y;
            }
            finally
            {
                Common.TextureMapping.TexGen.ClearCache();
            }
        }
    }
}
