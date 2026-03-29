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

            var archiveSmallRoom = new ArchiveSmallRoom(
                leftSocketAccepted: new List<IProcedural> { flatCorridor },
                rightSocketAccepted: new List<IProcedural> { flatCorridor }
            );

            var roomPool = new List<IRoomTemplate> { archiveSmallRoom };

            var rooms = ProceduralChain.Build(
                start: new Point(centerX - 100, centerY),
                target: new Point(centerX + 200, centerY),
                roomCount: 3,
                roomPool: roomPool,
                fillTile, liningTile, rand
            );

            if (rooms.Count > 0)
            {
                Main.spawnTileX = rooms[0].Center.X;
                Main.spawnTileY = rooms[0].FloorY;
            }
        }
    }
}
