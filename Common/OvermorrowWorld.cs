using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Common.Pathfinding;
using OvermorrowMod.Content.Tiles;
using OvermorrowMod.Content.Tiles.Ambient.WaterCave;
using OvermorrowMod.Content.Tiles.Ores;
using OvermorrowMod.Content.Tiles.TrapOre;
using OvermorrowMod.Content.Tiles.WaterCave;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Common
{
    public partial class OvermorrowWorld : ModSystem
    {
        // Boss Attacks
        public static bool DripplerCircle;
        public static bool DripladShoot = false;
        public static bool dripPhase2;
        public static bool dripPhase3;
        public static int loomingdripplerdeadcount;
        public static int RotatingDripladAttackCounter;

        // Biomes

        private bool placedGranite = false;
        private bool placedtele = false;
        private bool placedclaw = false;



        #region chest shit i nede to move somewhere else
        public override void PostWorldGen()
        {
            /*for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile.TileType == TileID.Pots)
                    {
                        tile.TileType = (ushort)ModContent.TileType<LargePot>();
                    }
                }
            }*/
        }
        #endregion

        public static WalkPathFinder pf = new WalkPathFinder(2, 2, 1000, 200);
        private int curX;
        private int curY;

        public override void PostUpdateTime()
        {
            /* int x = (int)(Main.CurrentPlayer.position.X / 16f);
            int y = (int)(Main.CurrentPlayer.position.Y / 16f);

            if (curX == 0 && curY == 0)
            {
                curX = Main.spawnTileX;
                curY = Main.spawnTileY;
            }

            var path = pf.Next(curX, curY, x, y);

            if (path == null || path.Cost == 0)
            {
                curX = Main.spawnTileX;
                curY = Main.spawnTileY;
            }
            else
            {
                curX = path.XTarget;
                curY = path.YTarget;
                Dust.NewDust(new Vector2(curX * 16f, curY * 16f), 8, 8, DustID.WitherLightning);
            }

            base.PostUpdateTime(); */
        }

        public override void PostDrawTiles()
        {
            // pf.Visualize(0);
        }

        // Worldgen Debugging
        public static bool JustPressed(Keys key)
        {
            return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex + 1, new PassLegacy("Mana Stone Ores", ManaStoneOres));
                tasks.Insert(ShiniesIndex + 2, new PassLegacy("Ambient Objects", GenerateAmbientObjects));
            }

            int WetJungle = tasks.FindIndex(genpass => genpass.Name.Equals("Wet Jungle"));
            if (WetJungle != -1)
            {
                //tasks.Insert(WetJungle + 1, new PassLegacy("WaterCaveGeneration", WaterCaveFinder));
            }
        }

        #region oldworldgen shit i need to move somewhere else

        int randSize = Main.rand.Next(140, 150);
        bool notInvalid = true;
        bool notNear = true;
        private void WaterCaveFinder(GenerationProgress progress)
        {
            progress.Message = "Flooding the Jungle";

            // small world size is 4200x1200 , medium multiplies every axis by 1.5 , and large multiplies every axis by 2.0

            int x = WorldGen.genRand.Next(600, Main.maxTilesX - 500);
            int y = WorldGen.genRand.Next((int)WorldGen.rockLayer + 100, (Main.maxTilesY == 2400) ? WorldGen.lavaLine - 300 : WorldGen.lavaLine - 150);

            while (true)
            {
                // We want to be in the jungle
                if (Main.tile[x, y].TileType != TileID.JungleGrass)
                {
                    notInvalid = false;
                }

                // While loop to find a valid tile
                if (!Main.tile[x, y].HasTile || Main.tile[x, y].TileType == TileID.SnowBlock || Main.tile[x, y].TileType == TileID.IceBlock || Main.tile[x, y].TileType == TileID.BlueDungeonBrick ||
                    Main.tile[x, y].TileType == TileID.Sand || Main.tile[x, y].TileType == TileID.HardenedSand || Main.tile[x, y].TileType == TileID.GreenDungeonBrick || Main.tile[x, y].TileType == TileID.PinkDungeonBrick)
                {
                    notInvalid = false;
                }

                int[] TileBlacklist = { 41, 43, 44, 226 };

                int xCoord = x;
                int yCoord = y;

                int xAxis = xCoord;
                int yAxis = yCoord;

                // For loop to check the Jungle Temple, or The Dungeon with a distance of randSize + 10
                for (int j = 0; j < randSize + 10; j++)
                {
                    //int fade = 0;
                    yAxis++;
                    xAxis = xCoord;
                    // Draws bottom right quadrant
                    for (int i = 0; i < randSize - j; i++)
                    {
                        xAxis++;
                        if (Main.tile[xAxis, yAxis] != null)
                        {
                            if (TileBlacklist.Contains(Main.tile[xAxis, yAxis].TileType))
                            {
                                notNear = false;
                            }
                        }
                    }

                    // Reset the xAxis, offset by 1 to fill in the gap
                    xAxis = xCoord + 1;

                    // Draws bottom left quadrant
                    for (int i = 0; i < randSize - j; i++)
                    {
                        xAxis--;
                        if (Main.tile[xAxis, yAxis] != null)
                        {
                            if (TileBlacklist.Contains(Main.tile[xAxis, yAxis].TileType))
                            {
                                notNear = false;
                            }
                        }
                    }
                }


                // Reset the y axis, offset by 1 to fill in the gap
                yAxis = yCoord + 1;

                for (int j = 0; j < randSize + 10; j++)
                {
                    yAxis--;
                    xAxis = xCoord;

                    // Draws top right quadrant
                    for (int i = 0; i < randSize - j; i++)
                    {
                        xAxis++;
                        if (Main.tile[xAxis, yAxis] != null)
                        {
                            if (TileBlacklist.Contains(Main.tile[xAxis, yAxis].TileType))
                            {
                                notNear = false;
                            }
                        }
                    }

                    // Reset the xAxis, offset by 1 to fill in the gap
                    xAxis = xCoord + 1;

                    // Draws top left quadrant
                    for (int i = 0; i < randSize - j; i++)
                    {
                        xAxis--;
                        if (Main.tile[xAxis, yAxis] != null)
                        {
                            if (TileBlacklist.Contains(Main.tile[xAxis, yAxis].TileType))
                            {
                                notNear = false;
                            }
                        }
                    }
                }

                // The selected location is invalid or is near an invalid location
                if (!notInvalid || !notNear)
                {
                    // Reset the flags
                    notInvalid = true;
                    notNear = true;

                    // Get new coordinates
                    x = WorldGen.genRand.Next(600, Main.maxTilesX - 500);
                    y = WorldGen.genRand.Next((int)WorldGen.rockLayer + 120, WorldGen.lavaLine - 200);
                }
                else
                {
                    // Break out of checker loop
                    break;
                }
            }

            GenerateWaterCave(x, y);
        }

        private void GenerateWaterCave(int x, int y)
        {
            int[] TileBlacklist = { 41, 43, 44, 226 };
            Point point = new Point(x, y);

            // small world size is 4200x1200 , medium multiplies every axis by 1.5 , and large multiplies every axis by 2.0
            int xCoord = x;
            int yCoord = y;

            int xAxis = xCoord;
            int yAxis = yCoord;


            for (int j = 0; j < randSize; j++)
            {
                //int fade = 0;
                yAxis++;
                xAxis = xCoord;
                // Draws bottom right quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis++;
                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (!TileBlacklist.Contains(Main.tile[xAxis, yAxis].TileType))
                        {
                            WorldGen.KillTile(xAxis, yAxis);
                            WorldGen.PlaceTile(xAxis, yAxis, ModContent.TileType<GlowBlock>());

                            Tile wall = Framing.GetTileSafely(xAxis, yAxis);
                            wall.WallType = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }
                }

                // Reset the xAxis, offset by 1 to fill in the gap
                xAxis = xCoord + 1;

                // Draws bottom left quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis--;
                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (!TileBlacklist.Contains(Main.tile[xAxis, yAxis].TileType))
                        {
                            WorldGen.KillTile(xAxis, yAxis);
                            WorldGen.PlaceTile(xAxis, yAxis, ModContent.TileType<GlowBlock>());

                            Tile wall = Framing.GetTileSafely(xAxis, yAxis);
                            wall.WallType = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }
                }
            }


            // Reset the y axis, offset by 1 to fill in the gap
            yAxis = yCoord + 1;

            for (int j = 0; j < randSize; j++)
            {
                yAxis--;
                xAxis = xCoord;

                // Draws top right quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis++;
                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (!TileBlacklist.Contains(Main.tile[xAxis, yAxis].TileType))
                        {
                            WorldGen.KillTile(xAxis, yAxis);
                            WorldGen.PlaceTile(xAxis, yAxis, ModContent.TileType<GlowBlock>());

                            Tile wall = Framing.GetTileSafely(xAxis, yAxis);
                            wall.WallType = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }
                }

                // Reset the xAxis, offset by 1 to fill in the gap
                xAxis = xCoord + 1;

                // Draws top left quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis--;
                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (!TileBlacklist.Contains(Main.tile[xAxis, yAxis].TileType))
                        {
                            WorldGen.KillTile(xAxis, yAxis);
                            WorldGen.PlaceTile(xAxis, yAxis, ModContent.TileType<GlowBlock>());

                            Tile wall = Framing.GetTileSafely(xAxis, yAxis);
                            wall.WallType = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }
                }
            }

            // Generate the circle shape
            WorldUtils.Gen(point, new Shapes.Circle(WorldGen.genRand.Next(25, 30)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true)));

            // Create random circles
            for (int i = 0; i < WorldGen.genRand.Next(14, 19); i++)
            {
                ShapeData circleShapeData = new ShapeData();
                int randX = x + WorldGen.genRand.Next(-10, 10) * 8;
                int randY = y + WorldGen.genRand.Next(-10, 10) * 8;
                Point randPoint = new Point(randX, randY); WorldUtils.Gen(randPoint, new Shapes.Circle(WorldGen.genRand.Next(4, 11)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true).Output(circleShapeData)));
                WorldUtils.Gen(new Point(randX, randY), new ModShapes.All(circleShapeData), Actions.Chain(new Modifiers.RectangleMask(-20, 20, 0, 5), new Modifiers.IsEmpty(), new Actions.SetLiquid()));
            }

            // Create smol circles
            for (int i = 0; i < WorldGen.genRand.Next(14, 19); i++)
            {
                ShapeData circleShapeData = new ShapeData();
                int randX = x + WorldGen.genRand.Next(-10, 10) * 8;
                int randY = y + WorldGen.genRand.Next(-10, 10) * 8;
                Point randPoint = new Point(randX, randY);
                WorldUtils.Gen(randPoint, new Shapes.Circle(WorldGen.genRand.Next(3, 5)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true).Output(circleShapeData)));
                WorldUtils.Gen(new Point(randX, randY), new ModShapes.All(circleShapeData), Actions.Chain(new Modifiers.RectangleMask(-20, 20, 0, 5), new Modifiers.IsEmpty(), new Actions.SetLiquid()));
            }

            // Dig out to the sides
            WorldGen.digTunnel(x, y, -0.65f, 0, WorldGen.genRand.Next(70, 80), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0, 0.5f, WorldGen.genRand.Next(70, 80), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0, -0.5f, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, -0.65f, 0.5f, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0.5f, -0.65f, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0.5f, 0, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0.5f, 0.5f, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0, 0.5f, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(3, 6), false);


            // Um, additional tunnels I guess
            WorldGen.digTunnel(x, y, 1, 0, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, 0, 1, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, 0, -1, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, -1, 1, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, -1, -1, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, 1, 1, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, 1, -1, 120, Main.rand.Next(2, 5), true);

            // Poke holes
            for (int j = 0; j < randSize; j++)
            {
                yAxis++;
                xAxis = xCoord;
                // Draws bottom right quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis++;

                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (j > randSize - 20)
                        {
                            if (WorldGen.genRand.NextBool(2100))
                            {
                                WorldUtils.Gen(new Point(xAxis, yAxis), new Shapes.Circle(WorldGen.genRand.Next(2, 5)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true)));
                            }
                        }
                        else
                        {
                            if (WorldGen.genRand.NextBool(2800))
                            {
                                WorldUtils.Gen(new Point(xAxis, yAxis), new Shapes.Circle(WorldGen.genRand.Next(2, 5)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true)));
                            }
                        }

                        if (WorldGen.genRand.NextBool(3000))
                        {
                            WorldGen.digTunnel(xAxis, yAxis, WorldGen.genRand.NextFloat(-1, 1), WorldGen.genRand.NextFloat(-1, 1), WorldGen.genRand.Next(30, 40), Main.rand.Next(2, 6), Main.rand.NextBool(10));
                        }
                    }
                }

                // Reset the xAxis, offset by 1 to fill in the gap
                xAxis = xCoord + 1;

                // Draws bottom left quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis--;
                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (j > randSize - 20)
                        {
                            if (WorldGen.genRand.NextBool(2100))
                            {
                                WorldUtils.Gen(new Point(xAxis, yAxis), new Shapes.Circle(WorldGen.genRand.Next(2, 5)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true)));
                            }
                        }
                        else
                        {
                            if (WorldGen.genRand.NextBool(2800))
                            {
                                WorldUtils.Gen(new Point(xAxis, yAxis), new Shapes.Circle(WorldGen.genRand.Next(2, 5)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true)));
                            }
                        }

                        if (WorldGen.genRand.NextBool(3000))
                        {
                            WorldGen.digTunnel(xAxis, yAxis, WorldGen.genRand.NextFloat(-1, 1), WorldGen.genRand.NextFloat(-1, 1), WorldGen.genRand.Next(30, 40), Main.rand.Next(2, 6), Main.rand.NextBool(10));
                        }
                    }
                }
            }
            // Determine random number of MUD generated
            int numMud = WorldGen.genRand.Next(99, 109);
            int generatedMud = 0;
            while (generatedMud < numMud)
            {
                // Choose random coordinate
                int i = WorldGen.genRand.Next(0, Main.maxTilesX);
                int j = WorldGen.genRand.Next(0, Main.maxTilesY);

                // Strength controls size
                // Steps control interations
                Tile tile = Framing.GetTileSafely(i, j);
                if (tile.HasTile && tile.TileType == ModContent.TileType<GlowBlock>())
                {
                    //WorldGen.OreRunner(i, j, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 3), (ushort)ModContent.TileType<WaterCaveOre>());
                    WorldGen.TileRunner(i, j, WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(8, 18), ModContent.TileType<CaveMud>(), false, WorldGen.genRand.Next(-1, 1), WorldGen.genRand.Next(-1, 1));
                    generatedMud++; // Increment success
                }
            }

            // Determine random number of ores generated
            int numOres = WorldGen.genRand.Next(69, 79);
            int generatedOres = 0;
            while (generatedOres < numOres)
            {
                // Choose random coordinate
                int i = WorldGen.genRand.Next(0, Main.maxTilesX);
                int j = WorldGen.genRand.Next(0, Main.maxTilesY);

                // Strength controls size
                // Steps control interations
                Tile tile = Framing.GetTileSafely(i, j);
                if (tile.HasTile && (tile.TileType == ModContent.TileType<GlowBlock>() || tile.TileType == ModContent.TileType<WaterCaveOre>()))
                {
                    //WorldGen.OreRunner(i, j, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(2, 4), (ushort)ModContent.TileType<WaterCaveOre>());
                    WorldGen.TileRunner(i, j, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), ModContent.TileType<WaterCaveOre>());
                    generatedOres++; // Increment success
                }
            }
        }

        private void ManaStoneOres(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Generating Modded Ores";
            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, Main.maxTilesY); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.

                // Strength controls size
                // Steps control interations
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && tile.TileType == TileID.Stone)
                {
                    WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 8), 1, ModContent.TileType<ManaStone>());
                }
            }

            // Erudite Generation
            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 0.00025); k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.maxTilesY);

                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(2, 4), WorldGen.genRand.Next(2, 6), ModContent.TileType<EruditeTile>());
            }

            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 0.0014); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.
                int x = WorldGen.genRand.Next(0, Main.maxTilesX / 6);
                int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, Main.maxTilesY); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.

                // Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place. Feel free to experiment with strength and step to see the shape they generate.
                WorldGen.PlaceTile(x, y, ModContent.TileType<HerosAltar>());
            }

            // Fake Ores
            int[] ValidTiles = { TileID.Stone, TileID.IceBlock, TileID.SnowBlock, TileID.Mud, TileID.HardenedSand };
            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.maxTilesY - 200); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.

                // Strength controls size
                // Steps control interations
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && ValidTiles.Contains(tile.TileType))
                {
                    if (WorldGen.SavedOreTiers.Gold == TileID.Gold)
                    {
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 8), 1, ModContent.TileType<FakeiteGold>());
                    }
                    else
                    {
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 8), 1, ModContent.TileType<FakeitePlatinum>());
                    }
                }
            }

        }

        private void GenerateAmbientObjects(GenerationProgress progress, GameConfiguration config)
        {
            // Place ambient objects for the Flooded Caverns
            for (int i = 0; i < Main.maxTilesY * 45; i++)
            {
                int[] rockFormations = { ModContent.TileType<Rock1>(), ModContent.TileType<Rock2>(), ModContent.TileType<Rock3>(), ModContent.TileType<Rock4>(), ModContent.TileType<Stalagmite1>(), ModContent.TileType<Stalagmite2>(), ModContent.TileType<Stalagmite3>(), ModContent.TileType<Stalagmite4>(), ModContent.TileType<Stalagmite5>() };
                int x = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
                int y = WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.maxTilesY - 200);
                int type = rockFormations[Main.rand.Next(9)];
                if (Main.tile[x, y].TileType == ModContent.TileType<GlowBlock>())
                {
                    WorldGen.PlaceObject(x, y, (ushort)type);
                    NetMessage.SendObjectPlacment(-1, x, y, (ushort)type, 0, 0, -1, -1);
                }
            }
        }
        #endregion
    }
}