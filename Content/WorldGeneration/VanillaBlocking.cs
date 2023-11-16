using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Common.WorldGeneration;
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
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
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
                tasks.Insert(TerrainIndex + 1, new PassLegacy("Test Terrain Tunnels", TestGenerateTunnels));
                tasks.Insert(TerrainIndex + 2, new PassLegacy("Test Terrain Base", TestGenerateTerrainBase));

                //tasks.Insert(TerrainIndex + 2, new PassLegacy("Test Terrain Base 2", TestGenerateTerrainLayer));

                //tasks.Insert(TerrainIndex + 3, new PassLegacy("Test Terrain Tunnels", TestGenerateTerrainTunnels));
                //tasks.Insert(TerrainIndex + 1, new PassLegacy("Test Terrain", TestGenerateTerrain));
            }

            // Re-add Vanilla Passes that have been modified
            int TunnelIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Tunnels"));
            if (TunnelIndex != -1)
            {
                tasks.Insert(TunnelIndex + 1, new PassLegacy("Mount Caves", MountainGeneration.GenerateMountains));
            }

            int SlushIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Slush"));
            if (SlushIndex != -1)
            {
                tasks.Insert(SlushIndex + 1, new PassLegacy("Mountain Caves", MountainGeneration.GenerateMountainCaves));
            }


            //RemovePass(tasks, "Terrain");

            //RemovePass(tasks, "Tunnels");
            RemovePass(tasks, "Mount Caves"); 
            RemovePass(tasks, "Mountain Caves");

            RemovePass(tasks, "Small Holes");
            //RemovePass(tasks, "Surface Caves");
            //RemovePass(tasks, "Dirt Layer Caves");

            //RemovePass(tasks, "Rock Layer Caves");
            //RemovePass(tasks, "Wavy Caves");

            RemovePass(tasks, "Ocean Sand");
            RemovePass(tasks, "Dunes");

            // This slows down the generation
            RemovePass(tasks, "Jungle Temple");
            RemovePass(tasks, "Hives");

            // Re-add these back later
            RemovePass(tasks, "Living Trees");
            RemovePass(tasks, "Moss");
            RemovePass(tasks, "Traps");

            // This crashes cause of no tunnels or something
            //RemovePass(tasks, "Granite");

            // This forces an infinite loop, not sure what
            RemovePass(tasks, "Wall Variety");
            RemovePass(tasks, "Water Chests");


            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Generate Ice Biome"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Full Desert"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Oasis"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Living Trees"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Hives"));
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
            for (int x = 0; x < Main.maxTilesX; x += 15)
            {
                for (int y = 0; y < Main.maxTilesY; y += 10)
                {
                    if (Framing.GetTileSafely(x, y).TileType == TileID.Dirt)
                        WorldGen.digTunnel(x, y, 0, 0, 1, 18, false);
                }
            }
        }

        private void GenerateSlopes()
        {
            // Amplitude
            float maxHeightOffset = -15;
            float maxDepthOffset = 15;

            FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFractalOctaves(8);
            noise.SetFractalLacunarity(2f);

            noise.SetFrequency(0.025f);

            FastNoiseLite noise2 = new FastNoiseLite(WorldGen._genRandSeed);
            noise2.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            noise2.SetFractalOctaves(2);
            noise2.SetFractalLacunarity(3f);
            noise2.SetFrequency(0.001f);

            int seed = Main.rand.Next(-10000, 10000);
            float heightMultiplier = 2f;
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                int yOffset = (int)MathHelper.Lerp(maxHeightOffset, maxDepthOffset, noise.GetNoise(x, 0));
                int yPosition = (int)(Main.worldSurface - 45 + yOffset);

                float height = noise.GetNoise((x + seed) * 0.005f, seed * 0.005f) * heightMultiplier;
                for (int y = yPosition; y < Main.worldSurface; y++)
                {
                    WorldGen.PlaceTile(x, y, TileID.ObsidianBrick, true, true);
                    //if (noise.GetNoise(x, y) >= noise2.GetNoise(x, y)) WorldGen.PlaceTile(x, y, TileID.ObsidianBrick, true, true);
                }
            }
        }

        private float EaseInElastic(float x)
        {
            const float c4 = (float)((2 * Math.PI) / 3);

            return (float)(x == 0
              ? 0
              : x == 1
              ? 1
              : -Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * c4));
        }

        private void GenerateCliff()
        {
            FastNoiseLite amplitudeNoise = new FastNoiseLite(WorldGen._genRandSeed);
            amplitudeNoise.SetNoiseType(FastNoiseLite.NoiseType.Value);
            amplitudeNoise.SetFractalOctaves(8);
            amplitudeNoise.SetFractalLacunarity(4f);
            amplitudeNoise.SetFrequency(0.02f);

            float xStart = Main.maxTilesX / 3 * 2;
            float scale = 0.5f;

            int cliffPositionX = 0; // Stores the last x offset for the cliff top to draw
            for (int y = (int)Main.worldSurface; y > Main.worldSurface * 0.5f; y--)
            {
                int xOffsetLower = (int)MathHelper.Lerp(55, 3, (float)((y - 23) / (Main.worldSurface * 0.5f - 23)));
                int maxAmplitude = (int)MathHelper.Lerp(125, 5, (float)((y - 23) / (Main.worldSurface * 0.5f - 23)));
                int xOffset = (int)MathHelper.Lerp(xOffsetLower, maxAmplitude, amplitudeNoise.GetNoise(y, 0) * scale);

                int xPosition = (int)(xStart - 45 - xOffset);
                for (int x = xPosition; x < xStart; x++)
                {
                    WorldGen.PlaceTile(x, y, ModContent.TileType<SmoothStone>(), true, true);
                }

                cliffPositionX = xPosition;
            }

            //GenerateCliffTop(cliffPositionX, (int)(Main.worldSurface * 0.5f));
        }

        private void GenerateCliffTop(int xOrigin, int yOrigin)
        {
            // Surface Terrain
            FastNoiseLite amplitudeNoise = new FastNoiseLite(WorldGen._genRandSeed);
            amplitudeNoise.SetNoiseType(FastNoiseLite.NoiseType.Value);
            amplitudeNoise.SetFractalOctaves(8);
            amplitudeNoise.SetFractalLacunarity(2f);
            amplitudeNoise.SetFrequency(0.01f);

            FastNoiseLite noise2 = new FastNoiseLite(WorldGen._genRandSeed);
            noise2.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise2.SetFractalOctaves(6);
            noise2.SetFractalLacunarity(4f);
            noise2.SetFrequency(0.025f);
            noise2.SetFractalGain(0.1f);
            noise2.SetDomainWarpType(FastNoiseLite.DomainWarpType.BasicGrid);
            noise2.SetDomainWarpAmp(2);

            int xOffsetTiles = (Main.maxTilesX / 3 * 2) - xOrigin;
            var logger = OvermorrowModFile.Instance.Logger;
            for (int x = xOrigin; x < Main.maxTilesX; x++)
            {
                int lowerBound = 0;
                int upperBound = 25;

                // Used to make the terrain start sloping downwards from the cliff
                float progress = Utils.Clamp((x - xOrigin) / (Main.maxTilesX * 0.1f), 0, 1f);
                int yOriginOffset = x < xOffsetTiles + xOrigin ? 6 : (int)MathHelper.Lerp(0, (float)Main.worldSurface * 0.25f, progress);

                int maxAmplitude = (int)MathHelper.Lerp(lowerBound, upperBound, Math.Abs(noise2.GetNoise(x, 0)));
                int yOffset = (int)MathHelper.Lerp(0, maxAmplitude, Math.Abs(amplitudeNoise.GetNoise(x, 0)));
                int yPosition = (int)(yOrigin - 15 - yOffset + yOriginOffset);

                float yHeight = x < xOffsetTiles + xOrigin ? yOrigin : (float)Main.worldSurface;
                //logger.Error(x + " / " + xOffsetTiles);



                for (int y = yPosition; y <= yHeight; y++)
                {
                    WorldGen.PlaceTile(x, y, TileID.Dirt, true, true);
                }
            }
        }

        // Peaks are rounded, general terrain is smoother
        private void GenerateSurface()
        {
            // Surface Terrain
            FastNoiseLite amplitudeNoise = new FastNoiseLite(WorldGen._genRandSeed);
            amplitudeNoise.SetNoiseType(FastNoiseLite.NoiseType.Value);
            amplitudeNoise.SetFractalOctaves(8);
            amplitudeNoise.SetFractalLacunarity(2f);
            amplitudeNoise.SetFrequency(0.01f);

            FastNoiseLite noise2 = new FastNoiseLite(WorldGen._genRandSeed);
            noise2.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise2.SetFractalOctaves(6);
            noise2.SetFractalLacunarity(4f);
            noise2.SetFrequency(0.025f);
            noise2.SetFractalGain(0.1f);
            noise2.SetDomainWarpType(FastNoiseLite.DomainWarpType.BasicGrid);
            noise2.SetDomainWarpAmp(2);

            //int maxHeightOffset = 0;
            //int maxDepthOffset = 25;
            //int maxAmplitude = 25;
            //for (int x = 0; x < Main.maxTilesX; x++)
            for (int x = 0; x < Main.maxTilesX / 3 * 2; x++)
            {
                int lowerBound = 25;
                int upperBound = 75;
                int yOffsetLower = 0;
                if (x >= Main.maxTilesX / 3 && x <= Main.maxTilesX / 3 * 2) // Middle of the world
                {
                    lowerBound = (int)MathHelper.Lerp(25, 3, Utils.Clamp((x - (Main.maxTilesX / 3)) / (Main.maxTilesX * 0.1f), 0, 1f));
                    upperBound = (int)MathHelper.Lerp(75, 25, Utils.Clamp((x - (Main.maxTilesX / 3)) / (Main.maxTilesX * 0.1f), 0, 1f));

                    //maxAmplitude = (int)MathHelper.Lerp(3, 25, Math.Abs(noise2.GetNoise(x, 0)));
                }

                if (x >= Main.maxTilesX / 3 * 2)
                {
                    lowerBound = 45;
                    upperBound = 75;
                    yOffsetLower = 150; // Increase the height by 45 blocks
                }

                int maxAmplitude = (int)MathHelper.Lerp(lowerBound, upperBound, Math.Abs(noise2.GetNoise(x, 0)));

                //int whatever = (int)MathHelper.Lerp(-5, 0, Math.Abs(noise2.GetNoise(x + 8, 0)));
                int yOffset = (int)MathHelper.Lerp(yOffsetLower, maxAmplitude, Math.Abs(amplitudeNoise.GetNoise(x, 0)) * 0.4f);
                int yPosition = (int)(Main.worldSurface - 45 - yOffset);
                for (int y = yPosition; y < Main.worldSurface; y++)
                {
                    int size = Main.rand.Next(4, 9);
                    WorldGen.TileRunner(x, y, size, 1, TileID.ObsidianBrick, true);
                    //WorldGen.PlaceTile(x, y, TileID.ObsidianBrick, true, true);
                    //if (noise2.GetNoise(x, y) > 0) WorldGen.PlaceTile(x, y, TileID.ObsidianBrick, true, true);
                }
            }
        }

        private void TestGenerateTerrainBase(GenerationProgress progress, GameConfiguration config)
        {
            //RemoveDirt();
            //GenerateSurface();
            //GenerateCliff();

            return;
            //noise.SetFrequency(0.015f);
            //noise.SetFractalGain(2.5f);
            //noise.SetFractalOctaves(8);
            //noise.SetFractalLacunarity(2f);
            //noise.SetFractalWeightedStrength(1f);

            // Rolling hill type shit
            /*for (int i = 0; i < Main.maxTilesX; i++)
            {
                //WorldGen.PlaceTile(i, (int)(200 + MathHelper.Lerp(-100, 100, noise.GetNoise(i, 0))), TileID.ObsidianBrick);
                int yOffset = (int)MathHelper.Lerp(maxHeightOffset, maxDepthOffset, noise.GetNoise(i, 0));
                int yPosition = Main.maxTilesY / 4 + yOffset;
                WorldGen.PlaceTile(i, yPosition, TileID.ObsidianBrick, true, true);
                //WorldGen.TileRunner(i, yPosition, Main.rand.Next(4, 18), 1, TileID.ObsidianBrick, true);

                // noise.SetFrequency(MathHelper.Lerp(0.025f, 0.005f, i / (float)Main.maxTilesX));
                if (i <= Main.maxTilesX / 3) // Left Side
                {
                    //noise.SetFrequency(MathHelper.Lerp(0.025f, 0.005f, i / (Main.maxTilesX / 3f)));
                    noise.SetFrequency(0.005f);

                    noise.SetFractalLacunarity(8f);
                    //maxHeightOffset = -60;
                    maxHeightOffset = MathHelper.Lerp(-75, -25, i / (Main.maxTilesX / 3f));
                    maxDepthOffset = 0;
                }
                else if (i >= Main.maxTilesX / 3 * 2) // Right Side
                {
                    //noise.SetFrequency(MathHelper.Lerp(0.025f, 0.005f, (i - (Main.maxTilesX / 3 * 2)) / (Main.maxTilesX / 3f)));
                    noise.SetFrequency(0.005f);
                    noise.SetFractalOctaves(2);
                    noise.SetFractalLacunarity(4f);
                    //maxHeightOffset = -5;
                    //maxDepthOffset = 20;
                    maxHeightOffset = -60;
                    maxDepthOffset = 0;
                }
                else // Middle
                {
                    noise.SetFractalOctaves(3);
                    noise.SetFractalLacunarity(3f);
                    maxHeightOffset = -25;
                    //maxDepthOffset = 10;
                }

                yPosition = Main.maxTilesY / 4 + yOffset;
                while (yPosition < Main.maxTilesY)
                {
                    yPosition++;
                    if (!Framing.GetTileSafely(i, yPosition).HasTile) WorldGen.PlaceTile(i, yPosition, TileID.Dirt, false, true);
                }
            }

            return;

            // Smaller Bumps
            noise = new FastNoiseLite(WorldGen._genRandSeed + 1);
            noise.SetFrequency(0.02f);

            maxHeightOffset = -5;
            maxDepthOffset = 5;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int yOffset = (int)MathHelper.Lerp(maxHeightOffset, maxDepthOffset, noise.GetNoise(i, 0));
                int yPosition = Main.maxTilesY / 4 + yOffset;
                //WorldGen.PlaceTile(i, yPosition, TileID.Dirt, true, true);
                WorldGen.TileRunner(i, yPosition, Main.rand.Next(1, 3), 1, TileID.Dirt, true);

                while (yPosition < Main.maxTilesY)
                {
                    yPosition++;
                    if (!Framing.GetTileSafely(i, yPosition).HasTile) WorldGen.PlaceTile(i, yPosition, TileID.Dirt);
                }
            }

            // Large Hills
            noise = new FastNoiseLite(WorldGen._genRandSeed + 2);
            noise.SetFrequency(0.01f);
            noise.SetFractalGain(0.5f);

            maxHeightOffset = -15;
            maxDepthOffset = 5;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int yOffset = (int)MathHelper.Lerp(maxHeightOffset, maxDepthOffset, noise.GetNoise(i, 0));
                int yPosition = Main.maxTilesY / 4 + yOffset;
                //WorldGen.PlaceTile(i, yPosition, TileID.Dirt, true, true);
                WorldGen.TileRunner(i, yPosition, Main.rand.Next(9, 15), 1, TileID.Dirt, true);

                while (yPosition < Main.maxTilesY)
                {
                    yPosition++;
                    if (!Framing.GetTileSafely(i, yPosition).HasTile) WorldGen.PlaceTile(i, yPosition, TileID.Dirt);
                    //if (!Framing.GetTileSafely(i, yPosition).HasTile) WorldGen.TileRunner(i, yPosition, Main.rand.Next(1, 10), 1, TileID.Dirt);
                }
            }

            // Terrain Holes
            /*noise = new FastNoiseLite(WorldGen._genRandSeed + 3);
            noise.SetFrequency(0.015f);
            noise.SetFractalLacunarity(2f);
            noise.SetFractalGain(1f);
            noise.SetFractalWeightedStrength(1.2f);

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = Main.maxTilesY / 4 - 20; j < Main.maxTilesY / 4 + 70; j++)
                {
                    if (noise.GetNoise(i, j) < -0.1f && Framing.GetTileSafely(i, j).TileType == TileID.Dirt) WorldGen.digTunnel(i, j, 0, 0, 1, Main.rand.Next(1, 10));
                    //if (noise.GetNoise(i, j) < -0.1f && Framing.GetTileSafely(i, j).TileType == TileID.Dirt) WorldGen.KillTile(i, j);
                }
            }*/
        }

        private void TestGenerateTunnels(GenerationProgress progress, GameConfiguration config)
        {
            RemoveDirt();

            var logger = OvermorrowModFile.Instance.Logger;

            // Lerp from 0.7 to 0.9 back to 0.7
            Vector2 startPoint = new Vector2(0, (float)Main.worldSurface * 0.6f);
            Vector2 endPoint = startPoint + new Vector2(300, 0);
            int repeat = 64;

            /*FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise.SetFractalOctaves(6);
            //noise.SetFractalOctaves(1);
            noise.SetFractalLacunarity(4f);
            //noise.SetFractalLacunarity(1f);

            // Decreasing frequency makes the curves smoother
            noise.SetFrequency(0.015f);
            noise.SetFractalGain(0.5f);*/

            FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFractalOctaves(6);
            //noise.SetFractalOctaves(1);
            noise.SetFractalLacunarity(4);
            //noise.SetFractalLacunarity(1f);
            // Decreasing frequency makes the curves smoother
            noise.SetFrequency(0.025f);
            noise.SetFractalGain(0.1f);

            SurfaceBuilder worm = new SurfaceBuilder(startPoint, endPoint, noise, repeat);
            worm.trueStartPoint = endPoint;
            worm.Run(out _);
        }

    }
}