/*using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class CaveGeneration : ModSystem
    {
        static int i2;
        public static void GenerateSurfaceCaves(GenerationProgress progress, GameConfiguration config)
        {
            
            progress.Message = Lang.gen[10].Value;
            for (int num847 = 0; num847 < (int)((double)Main.maxTilesX * 0.002); num847++)
            {
                i2 = WorldGen.genRand.Next(0, Main.maxTilesX);
                while (((float)i2 > (float)Main.maxTilesX * 0.45f && (float)i2 < (float)Main.maxTilesX * 0.55f) || i2 < leftBeachEnd + 20 || i2 > rightBeachStart - 20)
                {
                    i2 = WorldGen.genRand.Next(0, Main.maxTilesX);
                }
                for (int num848 = 0; (double)num848 < worldSurfaceHigh; num848++)
                {
                    if (Main.tile[i2, num848].HasTile)
                    {
                        WorldGen.TileRunner(i2, num848, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(5, 50), -1, addTile: false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 1f);
                        break;
                    }
                }
            }

            progress.Set(0.2f);
            for (int num849 = 0; num849 < (int)((double)Main.maxTilesX * 0.0007); num849++)
            {
                i2 = WorldGen.genRand.Next(0, Main.maxTilesX);
                while (((float)i2 > (float)Main.maxTilesX * 0.43f && (float)i2 < (float)Main.maxTilesX * 0.57f) || i2 < leftBeachEnd + 20 || i2 > rightBeachStart - 20)
                {
                    i2 = WorldGen.genRand.Next(0, Main.maxTilesX);
                }
                for (int num850 = 0; (double)num850 < worldSurfaceHigh; num850++)
                {
                    if (Main.tile[i2, num850].HasTile)
                    {
                        WorldGen.TileRunner(i2, num850, WorldGen.genRand.Next(10, 15), WorldGen.genRand.Next(50, 130), -1, addTile: false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f);
                        break;
                    }
                }
            }

            progress.Set(0.4f);
            for (int num851 = 0; num851 < (int)((double)Main.maxTilesX * 0.0003); num851++)
            {
                i2 = WorldGen.genRand.Next(0, Main.maxTilesX);
                while (((float)i2 > (float)Main.maxTilesX * 0.4f && (float)i2 < (float)Main.maxTilesX * 0.6f) || i2 < leftBeachEnd + 20 || i2 > rightBeachStart - 20)
                {
                    i2 = WorldGen.genRand.Next(0, Main.maxTilesX);
                }
                for (int num852 = 0; (double)num852 < worldSurfaceHigh; num852++)
                {
                    if (Main.tile[i2, num852].HasTile)
                    {
                        WorldGen.TileRunner(i2, num852, WorldGen.genRand.Next(12, 25), WorldGen.genRand.Next(150, 500), -1, addTile: false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 4f);
                        WorldGen.TileRunner(i2, num852, WorldGen.genRand.Next(8, 17), WorldGen.genRand.Next(60, 200), -1, addTile: false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f);
                        WorldGen.TileRunner(i2, num852, WorldGen.genRand.Next(5, 13), WorldGen.genRand.Next(40, 170), -1, addTile: false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f);
                        break;
                    }
                }
            }

            progress.Set(0.6f);
            for (int num853 = 0; num853 < (int)((double)Main.maxTilesX * 0.0004); num853++)
            {
                i2 = WorldGen.genRand.Next(0, Main.maxTilesX);
                while (((float)i2 > (float)Main.maxTilesX * 0.4f && (float)i2 < (float)Main.maxTilesX * 0.6f) || i2 < leftBeachEnd + 20 || i2 > rightBeachStart - 20)
                {
                    i2 = WorldGen.genRand.Next(0, Main.maxTilesX);
                }
                for (int num854 = 0; (double)num854 < worldSurfaceHigh; num854++)
                {
                    if (Main.tile[i2, num854].HasTile)
                    {
                        WorldGen.TileRunner(i2, num854, WorldGen.genRand.Next(7, 12), WorldGen.genRand.Next(150, 250), -1, addTile: false, 0f, 1f, noYChange: true);
                        break;
                    }
                }
            }

            progress.Set(0.8f);
            float num855 = Main.maxTilesX / 4200;
            for (int num856 = 0; (float)num856 < 5f * num855; num856++)
            {
                try
                {
                    WorldGen.Caverer(WorldGen.genRand.Next(surfaceCavesBeachAvoidance2, Main.maxTilesX - surfaceCavesBeachAvoidance2), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 400));
                }
                catch
                {
                }
            }
        }
    }
}*/