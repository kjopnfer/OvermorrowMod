using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
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

            int centerY = new TestSubworld().Height / 2;

            var rand = new Random(Environment.TickCount);

            DungeonContent content = new ArchiveContent();
            int gridCols = content.Cols;
            int gridRows = content.Rows;

            int dungeonWidth = gridCols * DungeonGrid.HorizontalSpacing + DungeonGrid.CellTileWidth + DungeonGrid.HorizontalPadding * 2;
            int dungeonHeight = gridRows * DungeonGrid.VerticalSpacing + DungeonGrid.CellTileHeight + DungeonGrid.HorizontalPadding * 2;
            int baseY = centerY - (gridRows * DungeonGrid.VerticalSpacing) / 2;
            int step = dungeonWidth + 60;

            // Three dungeons chained left to right with a vertical zig-zag so the run is not a straight line.
            var origins = new[]
            {
                new Point(100, baseY - 150),
                new Point(100 + step, baseY + 150),
                new Point(100 + step * 2, baseY - 50),
            };
            try
            {
                var totalSw = System.Diagnostics.Stopwatch.StartNew();

                ArchiveDoor_TE previousExit = null;
                int nextDoorId = 9001;

                for (int i = 0; i < origins.Length; i++)
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    GridGenerator.Build(origins[i], gridCols, gridRows, content, rand, out Point doorTile);
                    sw.Stop();

                    Terraria.ModLoader.Logging.PublicLogger.Info($"OvermorrowDungeon chain build {i + 1}/{origins.Length} at ({origins[i].X},{origins[i].Y}): {sw.ElapsedMilliseconds} ms, cumulative {totalSw.ElapsedMilliseconds} ms");

                    if (i == 0)
                    {
                        Main.spawnTileX = doorTile.X;
                        Main.spawnTileY = doorTile.Y;
                    }

                    if (!FindDungeonDoors(origins[i], dungeonWidth, dungeonHeight, doorTile, out ArchiveDoor_TE entrance, out ArchiveDoor_TE exit))
                        continue;

                    entrance.DoorID = nextDoorId++;
                    entrance.PairedDoor = -1;
                    exit.DoorID = nextDoorId++;
                    exit.PairedDoor = -1;

                    if (previousExit != null)
                    {
                        previousExit.PairedDoor = entrance.DoorID;
                        entrance.PairedDoor = previousExit.DoorID;
                    }

                    previousExit = exit;
                }

                totalSw.Stop();
                Terraria.ModLoader.Logging.PublicLogger.Info($"OvermorrowDungeon chain total for {origins.Length} dungeons: {totalSw.ElapsedMilliseconds} ms");
            }
            finally
            {
                Common.TextureMapping.TexGen.ClearCache();
            }
        }

        private static bool FindDungeonDoors(Point origin, int width, int height, Point entranceTile, out ArchiveDoor_TE entrance, out ArchiveDoor_TE exit)
        {
            entrance = null;
            exit = null;

            var doors = new List<ArchiveDoor_TE>();
            foreach (var te in TileEntity.ByID.Values)
            {
                if (te is not ArchiveDoor_TE door) continue;
                if (door.Position.X >= origin.X && door.Position.X <= origin.X + width && door.Position.Y >= origin.Y && door.Position.Y <= origin.Y + height)
                    doors.Add(door);
            }

            if (doors.Count < 2)
            {
                Terraria.ModLoader.Logging.PublicLogger.Warn($"OvermorrowDungeon chain: found {doors.Count} doors in dungeon rect, expected 2; skipping link.");
                return false;
            }

            entrance = doors[0];
            int bestDistSq = DistanceSq(entrance.Position, entranceTile);
            foreach (var door in doors)
            {
                int distSq = DistanceSq(door.Position, entranceTile);
                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    entrance = door;
                }
            }

            foreach (var door in doors)
            {
                if (door != entrance)
                {
                    exit = door;
                    break;
                }
            }

            return exit != null;
        }

        private static int DistanceSq(Point16 a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }
    }
}
