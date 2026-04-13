using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
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

            int gridCols = 8;
            int gridRows = 3;
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

            // Test StairBlock placement (below the horizontal chain)
            int stairW = DungeonGrid.CellTileWidth * 2 + DungeonGrid.HorizontalPadding;
            int stairH = DungeonGrid.CellTileHeight * 2 + DungeonGrid.VerticalPadding;
            int margin = DungeonGrid.HorizontalPadding;
            int stairX = startX;
            int stairY = startY + gridRows * DungeonGrid.VerticalSpacing + margin * 2;
            var stairBlock = new StairBlock(descendLeftToRight: true);
            ushort fill = (ushort)fillTile;
            for (int x = 0; x < stairW + margin * 2; x++)
                for (int y = 0; y < stairH + margin * 2; y++)
                    WorldGenUtils.PlaceTile(stairX + x, stairY + y, fill);
            Point stairOrigin = new Point(stairX + margin, stairY + margin);
            stairBlock.Build(stairOrigin, fillTile, liningTile);

            // Corner markers for the 2x2 stair block sub-cells
            ushort marker = (ushort)Terraria.ID.TileID.Adamantite;
            for (int sc = 0; sc < 2; sc++)
            {
                for (int sr = 0; sr < 2; sr++)
                {
                    int cx = stairOrigin.X + sc * DungeonGrid.HorizontalSpacing;
                    int cy = stairOrigin.Y + sr * DungeonGrid.VerticalSpacing;
                    int w = DungeonGrid.CellTileWidth - 1;
                    int h = DungeonGrid.CellTileHeight - 1;
                    WorldGenUtils.PlaceTile(cx, cy, marker);
                    WorldGenUtils.PlaceTile(cx + w, cy, marker);
                    WorldGenUtils.PlaceTile(cx, cy + h, marker);
                    WorldGenUtils.PlaceTile(cx + w, cy + h, marker);
                }
            }

            Main.spawnTileX = startX + DungeonGrid.CellTileWidth / 2;
            Main.spawnTileY = startY + (gridRows / 2) * DungeonGrid.VerticalSpacing + DungeonGrid.CellTileHeight / 2;
        }
    }
}
