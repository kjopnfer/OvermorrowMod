using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class MountainGeneration : ModSystem
    {
        private static int numMCaves;
        private static int[] mCaveX = new int[30];
        private static int[] mCaveY = new int[30];

        public static void GenerateMountains(GenerationProgress progress, GameConfiguration config)
        {
            numMCaves = 0;
            progress.Message = Lang.gen[2].Value;
            int num883 = 0;
            bool isInvalidLocation = false;
            bool flag57 = false;

            // Find a random x-position to generate the mountain at within the inner 50% of the world
            int generateX = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.65));
            while (!flag57)
            {
                flag57 = true;

                float tilePadding = Main.maxTilesX * 0.05f;
                int slimeCavePosition = (int)((Main.maxTilesX / 7 * 4) + tilePadding);

                // Find a different x-position if the chosen location is within 180 tiles of the spawn area
                // Prevent the mountain from generating into the slime cave which is always to the right of the spawn camp
                while (generateX > Main.maxTilesX / 2 - 180 && generateX < slimeCavePosition + 90)
                {
                    generateX = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.65));
                }

                // I have no idea what this does
                for (int num885 = 0; num885 < numMCaves; num885++)
                {
                    if (Math.Abs(generateX - mCaveX[num885]) < 100)
                    {
                        num883++;
                        flag57 = false;
                        break;
                    }
                }

                // I have no idea what this is checking
                if (num883 >= Main.maxTilesX / 5)
                {
                    isInvalidLocation = true;
                    break;
                }
            }

            if (!isInvalidLocation)
            {
                // Finds a valid y-position by looping form the top of the world to the world's surface
                for (int generateY = 0; (double)generateY < Main.worldSurface; generateY++)
                {
                    if (Main.tile[generateX, generateY].HasTile)
                    {
                        // Check within 100 pixels on the chosen location for any sand. If found, do not generate.
                        for (int x = generateX - 50; x < generateX + 50; x++)
                        {
                            for (int y = generateY - 25; y < generateY + 25; y++)
                            {
                                bool checkSand = Main.tile[x, y].HasTile && (Main.tile[x, y].TileType == TileID.Sand || Main.tile[x, y].TileType == TileID.SandstoneBrick || Main.tile[x, y].TileType == TileID.SandStoneSlab);
                                if (checkSand) isInvalidLocation = true;
                            }
                        }

                        if (!isInvalidLocation)
                        {
                            WorldGen.Mountinater(generateX, generateY);
                            mCaveX[numMCaves] = generateX;
                            mCaveY[numMCaves] = generateY;
                            numMCaves++;
                            break;
                        }
                    }
                }
            }
        }

        public static void GenerateMountainCaves(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = Lang.gen[21].Value;
            for (int currentCave = 0; currentCave < numMCaves; currentCave++)
            {
                int i = mCaveX[currentCave];
                int j = mCaveY[currentCave];
                WorldGen.CaveOpenater(i, j);
                WorldGen.Cavinator(i, j, WorldGen.genRand.Next(40, 50));
            }
        }
    }
}