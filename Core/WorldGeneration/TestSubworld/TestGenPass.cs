using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
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

            int gridCols = 25;
            int gridRows = 30;
            int startX = 100;
            int startY = centerY - (gridRows * DungeonGrid.VerticalSpacing) / 2;

            GridGenerator.Build(
                worldOrigin: new Point(startX, startY),
                gridCols: gridCols,
                gridRows: gridRows,
                cellPool: cellPool,
                fillTileType: fillTile,
                liningTileType: liningTile,
                rand: rand
            );

            Main.spawnTileX = startX + DungeonGrid.HorizontalPadding + DungeonGrid.CellTileWidth / 2;
            Main.spawnTileY = startY + (gridRows / 2) * DungeonGrid.VerticalSpacing + DungeonGrid.CellTileHeight / 2;
        }
    }
}
