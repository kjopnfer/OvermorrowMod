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
            bool flag56 = false;
            bool flag57 = false;

            int num884 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
            while (!flag57)
            {
                flag57 = true;
                while (num884 > Main.maxTilesX / 2 - 90 && num884 < Main.maxTilesX / 2 + 90)
                {
                    num884 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
                }
                for (int num885 = 0; num885 < numMCaves; num885++)
                {
                    if (Math.Abs(num884 - mCaveX[num885]) < 100)
                    {
                        num883++;
                        flag57 = false;
                        break;
                    }
                }
                if (num883 >= Main.maxTilesX / 5)
                {
                    flag56 = true;
                    break;
                }
            }

            if (!flag56)
            {
                for (int num886 = 0; (double)num886 < Main.worldSurface; num886++)
                {
                    if (Main.tile[num884, num886].HasTile)
                    {
                        for (int num887 = num884 - 50; num887 < num884 + 50; num887++)
                        {
                            for (int num888 = num886 - 25; num888 < num886 + 25; num888++)
                            {
                                bool checkSand = Main.tile[num887, num888].HasTile && (Main.tile[num887, num888].TileType == TileID.Sand || Main.tile[num887, num888].TileType == TileID.SandstoneBrick || Main.tile[num887, num888].TileType == TileID.SandStoneSlab);
                                if (checkSand) flag56 = true;
                            }
                        }

                        if (!flag56)
                        {
                            WorldGen.Mountinater(num884, num886);
                            mCaveX[numMCaves] = num884;
                            mCaveY[numMCaves] = num886;
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
            for (int num655 = 0; num655 < numMCaves; num655++)
            {
                int i4 = mCaveX[num655];
                int j4 = mCaveY[num655];
                WorldGen.CaveOpenater(i4, j4);
                WorldGen.Cavinator(i4, j4, WorldGen.genRand.Next(40, 50));
            }
        }
    }
}