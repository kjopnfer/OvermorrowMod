using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.WorldGeneration.Procedural;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Connectors;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Rooms;
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

            var flatCorridor = new FlatCorridor();
            var verticalStairs = new VerticalStairs();
            var descendingStairs = new DescendingStairs();
            var ascendingStairs = new AscendingStairs();

            var archiveSmallRoom = new ArchiveSmallRoom(
                leftAccepted: new List<IProceduralRoom> { flatCorridor },
                rightAccepted: new List<IProceduralRoom> { flatCorridor, descendingStairs, ascendingStairs },
                downAccepted: new List<IProceduralRoom> { verticalStairs }
            );

            var roomPool = new List<IProceduralRoom> { archiveSmallRoom };

            int startX = 100;
            int startY = centerY;

            ProceduralChain.Build(
                start: new Point(startX, startY),
                target: new Point(startX + 200, startY),
                roomCount: 6,
                roomPool: roomPool,
                fillTile, liningTile, rand
            );

            Main.spawnTileX = startX + archiveSmallRoom.Width / 2;
            Main.spawnTileY = startY + archiveSmallRoom.Height / 2;
        }
    }
}
