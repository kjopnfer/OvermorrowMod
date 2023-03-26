using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Content.NPCs.Shades;
using OvermorrowMod.Content.Tiles.Ambient;
using OvermorrowMod.Content.Tiles.Underground;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class VanillaBlocking : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sand"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            int CaveIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Surface Caves"));
            if (CaveIndex != -1)
            {
                //tasks.Insert(CaveIndex + 1, new PassLegacy("Test Caves", TestGenerateCaves));
            }

            int TerrainIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Terrain"));
            if (TerrainIndex != -1)
            {
                tasks.Insert(TerrainIndex + 1, new PassLegacy("Test Terrain Base", TestGenerateTerrainBase));
                //tasks.Insert(TerrainIndex + 2, new PassLegacy("Test Terrain", TestGenerateTerrain));
            }

            //RemovePass(tasks, "Terrain");

            RemovePass(tasks, "Tunnels");
            RemovePass(tasks, "Mount Caves");
            RemovePass(tasks, "Small Holes");
            RemovePass(tasks, "Surface Caves");
            RemovePass(tasks, "Dirt Layer Caves");
            RemovePass(tasks, "Rock Layer Caves");
            RemovePass(tasks, "Wavy Caves");

            RemovePass(tasks, "Ocean Sand");
            RemovePass(tasks, "Dunes");

            RemovePass(tasks, "Living Trees");

            // This crashes cause of no tunnels or something
            RemovePass(tasks, "Granite");

            // This forces an infinite loop, not sure what
            RemovePass(tasks, "Wall Variety");
            RemovePass(tasks, "Water Chests");


            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Generate Ice Biome"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Full Desert"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Oasis"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            int DungeonIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (DungeonIndex != -1) tasks.RemoveAt(DungeonIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Corruption"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Trees"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Buried Chests"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

        }

        private void RemovePass(List<GenPass> tasks, string passName)
        {
            int PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals(passName));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);
        }

        private List<string> PassNames = new List<string>()
        {
            "Terrain", "Dunes", "Ocean Sand", "Sand Patches", "Tunnels", "Mount Caves", "Dirt Wall Backgrounds", ""
        };

        private void RemoveAllPasses(List<GenPass> tasks)
        {

        }

        private void RemoveDirt()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    if (Framing.GetTileSafely(i, j).TileType == TileID.Dirt) WorldGen.KillTile(i, j);
                }
            }
        }


        private void TestGenerateTerrain(GenerationProgress progress, GameConfiguration config)
        {
            RemoveDirt();

            FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            /*noise.SetFrequency(0.03f);
            noise.SetFractalOctaves(1);
            noise.SetFractalLacunarity(2f);
            noise.SetFractalGain(0.5f);
            noise.SetFractalWeightedStrength(1f);*/

            noise.SetFrequency(0.03f);
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                //noise.SetFrequency(MathHelper.Lerp(0.005f, 0.005f, Utils.Clamp(i, 0, Main.maxTilesX) / (float)Main.maxTilesX));
                noise.SetFractalOctaves(5);
                noise.SetFractalLacunarity(2f);
                noise.SetFractalGain(1f);
                noise.SetFractalWeightedStrength(MathHelper.Lerp(1f, 1.4f, Utils.Clamp(i, 0, Main.maxTilesX) / (float)Main.maxTilesX));

                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    if (noise.GetNoise(i, j) < 0f) WorldGen.PlaceTile(i, j, TileID.Dirt);
                    //if (noise.GetNoise(i, j) < 0f) WorldGen.KillTile(i, j);
                }
            }

            /*noise.SetNoiseType(FastNoiseLite.NoiseType.Value);

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                noise.SetFrequency(MathHelper.Lerp(0.02f, 0.02f, Utils.Clamp(i, 0, Main.maxTilesX) / (float)Main.maxTilesX));
                noise.SetFractalOctaves(5);
                noise.SetFractalLacunarity(2f);
                noise.SetFractalGain(1f);
                noise.SetFractalWeightedStrength(MathHelper.Lerp(1f, 1f, Utils.Clamp(i, 0, Main.maxTilesX) / (float)Main.maxTilesX));

                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    if (noise.GetNoise(i, j) < 0f) WorldGen.PlaceTile(i, j, TileID.Dirt);
                }
            }*/
        }


        private void TestGenerateTerrainBase(GenerationProgress progress, GameConfiguration config)
        {
            RemoveDirt();

            var logger = OvermorrowModFile.Instance.Logger;

            float maxHeightOffset = -5;
            float maxDepthOffset = 10;

            // Base Terrain
            FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFrequency(0.005f);
            noise.SetFractalOctaves(1);
            noise.SetFractalLacunarity(2f);
            noise.SetFractalGain(0.5f);
            noise.SetFractalWeightedStrength(1f);

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                logger.Debug(noise.GetNoise(i, 0));
                //WorldGen.PlaceTile(i, (int)(200 + MathHelper.Lerp(-100, 100, noise.GetNoise(i, 0))), TileID.ObsidianBrick);
                int yOffset = (int)MathHelper.Lerp(maxHeightOffset, maxDepthOffset, noise.GetNoise(i, 0));
                int yPosition = Main.maxTilesY / 4 + yOffset;
                WorldGen.PlaceTile(i, yPosition, TileID.Dirt, true, true);

                /*while (yPosition > 0)
                {
                    yPosition--;
                    WorldGen.KillTile(i, yPosition);
                }*/

                yPosition = Main.maxTilesY / 4 + yOffset;
                while (yPosition < Main.maxTilesY)
                {
                    yPosition++;
                    if (!Framing.GetTileSafely(i, yPosition).HasTile) WorldGen.PlaceTile(i, yPosition, TileID.Dirt);
                }
            }

            // Smaller Bumps
            /*noise = new FastNoiseLite(WorldGen._genRandSeed + 1);
            noise.SetFrequency(0.02f);

            maxHeightOffset = -5;
            maxDepthOffset = 5;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                logger.Debug(noise.GetNoise(i, 0));
                int yOffset = (int)MathHelper.Lerp(maxHeightOffset, maxDepthOffset, noise.GetNoise(i, 0));
                int yPosition = Main.maxTilesY / 4 + yOffset;
                WorldGen.PlaceTile(i, yPosition, TileID.Dirt, true, true);

                while (yPosition < Main.maxTilesY)
                {
                    yPosition++;
                    if (!Framing.GetTileSafely(i, yPosition).HasTile) WorldGen.PlaceTile(i, yPosition, TileID.Dirt);
                }
            }

            noise = new FastNoiseLite(WorldGen._genRandSeed + 1);
            noise.SetFrequency(0.005f);

            maxHeightOffset = -4;
            maxDepthOffset = 3;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                logger.Debug(noise.GetNoise(i, 0));
                int yOffset = (int)MathHelper.Lerp(maxHeightOffset, maxDepthOffset, noise.GetNoise(i, 0));
                int yPosition = Main.maxTilesY / 4 + yOffset;
                WorldGen.PlaceTile(i, yPosition, TileID.Dirt, true, true);

                while (yPosition < Main.maxTilesY)
                {
                    yPosition++;
                    if (!Framing.GetTileSafely(i, yPosition).HasTile) WorldGen.PlaceTile(i, yPosition, TileID.Dirt);
                }
            }*/
        }

        private void TestGenerateCaves(GenerationProgress progress, GameConfiguration config)
        {
            var logger = OvermorrowModFile.Instance.Logger;

            FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            /*noise.SetFrequency(0.01f);
            noise.SetFractalLacunarity(2f);
            noise.SetFractalGain(0.70f);
            noise.SetFractalWeightedStrength(1f);*/

            noise.SetFrequency(/*0.01f*/0.015f);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalLacunarity(2f);
            noise.SetFractalGain(1f);
            noise.SetFractalWeightedStrength(1.2f);

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    //if (noise.GetNoise(i, j) < 0) WorldGen.PlaceTile(i, j, TileID.ObsidianBrick, true, true);

                    //if (noise.GetNoise(i, j) < -0.1f) WorldGen.KillTile(i, j);

                    if (noise.GetNoise(i, j) < -0.1f) WorldGen.KillTile(i, j);
                }
            }
        }
    }
}