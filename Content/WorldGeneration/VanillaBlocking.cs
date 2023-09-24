using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Base;
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
                tasks.Insert(TerrainIndex + 1, new PassLegacy("Test Terrain Base", TestGenerateTerrainBase));
                //tasks.Insert(TerrainIndex + 2, new PassLegacy("Test Terrain Base 2", TestGenerateTerrainLayer));

                //tasks.Insert(TerrainIndex + 3, new PassLegacy("Test Terrain Tunnels", TestGenerateTerrainTunnels));
                //tasks.Insert(TerrainIndex + 1, new PassLegacy("Test Terrain", TestGenerateTerrain));
            }

            //RemovePass(tasks, "Terrain");

            //RemovePass(tasks, "Tunnels");
            //RemovePass(tasks, "Mount Caves");
            RemovePass(tasks, "Small Holes");
            //RemovePass(tasks, "Surface Caves");
            //RemovePass(tasks, "Dirt Layer Caves");
            RemovePass(tasks, "Rock Layer Caves");
            RemovePass(tasks, "Wavy Caves");

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
            //RemoveDirt();

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
                    if (noise.GetNoise(i, j) < 0f) WorldGen.PlaceTile(i, j, TileID.ObsidianBrick, true, true);
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

        private void TestGenerateTerrainTunnels(GenerationProgress progress, GameConfiguration config)
        {
            FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFractalType(FastNoiseLite.FractalType.PingPong);
            noise.SetFrequency(0.005f);
            noise.SetFractalOctaves(5);
            noise.SetFractalLacunarity(2f);
            noise.SetFractalGain(0f);
            noise.SetFractalWeightedStrength(1f);
            noise.SetFractalPingPongStrength(2f);
            noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.BasicGrid);
            noise.SetDomainWarpAmp(100f);

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.rockLayer - 20; j++)
                {
                    if (noise.GetNoise(i, j) < -0.9f) WorldGen.digTunnel(i, j, 0, 0, 1, Main.rand.Next(1, 7));
                }
            }
        }

        private void TestGenerateTerrainLayer(GenerationProgress progress, GameConfiguration config)
        {
            FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFrequency(0.01f);
            noise.SetFractalOctaves(1);
            noise.SetFractalLacunarity(2f);
            noise.SetFractalGain(0.5f);
            noise.SetFractalWeightedStrength(1f);

            float maxHeightOffset = -10;
            float maxDepthOffset = 30;

            FastNoiseLite noise2 = new FastNoiseLite(WorldGen._genRandSeed + 4);
            noise2.SetFrequency(0.005f);

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                //WorldGen.PlaceTile(i, (int)(200 + MathHelper.Lerp(-100, 100, noise.GetNoise(i, 0))), TileID.ObsidianBrick);
                int yOffset = (int)MathHelper.Lerp(maxHeightOffset, maxDepthOffset, noise.GetNoise(i, 0)) - 15;
                int yPosition = Main.maxTilesY / 4 + yOffset;
                //WorldGen.PlaceTile(i, yPosition, TileID.Dirt, true, true);

                if (noise2.GetNoise(i, 0) < 0)
                    WorldGen.TileRunner(i, yPosition, Main.rand.Next(25, 40), 1, TileID.Dirt, true);
            }

            noise.SetFrequency(0.01f);
            noise.SetSeed(WorldGen._genRandSeed + 1);
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                //WorldGen.PlaceTile(i, (int)(200 + MathHelper.Lerp(-100, 100, noise.GetNoise(i, 0))), TileID.ObsidianBrick);
                int yOffset = (int)MathHelper.Lerp(maxHeightOffset, maxDepthOffset, noise.GetNoise(i, 0)) - 50;
                int yPosition = Main.maxTilesY / 4 + yOffset;

                if (noise2.GetNoise(i, 0) < 0)
                    WorldGen.TileRunner(i, yPosition, MathHelper.Lerp(9, 20, noise2.GetNoise(i, 0) / -1f), 1, TileID.Dirt, true);
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
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                int maxAmplitude = (int)MathHelper.Lerp(25, 75, Math.Abs(noise2.GetNoise(x, 0)));
                //int whatever = (int)MathHelper.Lerp(-5, 0, Math.Abs(noise2.GetNoise(x + 8, 0)));
                int whatever = 0;

                int yOffset = (int)MathHelper.Lerp(0, maxAmplitude, Math.Abs(amplitudeNoise.GetNoise(x, 0)));
                int yPosition = (int)(Main.worldSurface - 45 - yOffset);

                for (int y = yPosition + whatever; y < Main.worldSurface; y++)
                {
                    WorldGen.PlaceTile(x, y, TileID.ObsidianBrick, true, true);
                    //if (noise2.GetNoise(x, y) > 0) WorldGen.PlaceTile(x, y, TileID.ObsidianBrick, true, true);
                }
            }
        }

        private void TestGenerateTerrainBase(GenerationProgress progress, GameConfiguration config)
        {
            //RemoveDirt();
            FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise.SetFractalOctaves(6);
            noise.SetFractalLacunarity(4f);
            noise.SetFrequency(0.025f);
            noise.SetFractalGain(0.1f);

            var logger = OvermorrowModFile.Instance.Logger;

            Vector2 startPoint = new Vector2(Main.maxTilesX / 2f, (float)Main.worldSurface);
            Vector2 endPoint = startPoint + new Vector2(150, 150);

            //float degrees = MathHelper.Lerp(-90, 90, noise.GetNoise();
            WorldGen.PlaceTile((int)startPoint.X, (int)startPoint.Y, TileID.ObsidianBrick, true, true);
            WorldGen.PlaceTile((int)endPoint.X, (int)endPoint.Y, TileID.ObsidianBrick, true, true);

            PerlinWorm worm = new PerlinWorm(startPoint, endPoint);
            worm.Update();

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

    public class PerlinWorm
    {
        private Vector2 direction;
        private Vector2 position;
        private Vector2 endPosition;
        public float weight = 0.6f;
        public PerlinWorm(Vector2 startPosition, Vector2 endPosition)
        {
            position = startPosition;
            this.endPosition = endPosition;
        }

        public Vector2 MoveTowardsEndpoint()
        {
            var logger = OvermorrowModFile.Instance.Logger;

            Vector2 direction = GetDirection();
            var directionToEndpoint = Vector2.Normalize(endPosition - position);
            //var endDirection = Vector2.Normalize(direction * (1 - weight) + directionToEndpoint * weight);
            var endDirection = Vector2.Normalize(direction * (1 - weight) + directionToEndpoint * weight);

            position += endDirection;

            return position;
        }

        private Vector2 GetDirection()
        {
            var logger = OvermorrowModFile.Instance.Logger;

            FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
            noise.SetFractalOctaves(6);
            noise.SetFractalLacunarity(4f);
            noise.SetFrequency(0.025f);
            noise.SetFractalGain(0.1f);

            float degrees = MathHelper.Lerp(-90, 90, noise.GetNoise(position.X, position.Y));
            direction = Vector2.One.RotatedBy(degrees);

            return direction;
        }

        public void Update()
        {
            int maxTries = 300;
            while (Vector2.Distance(endPosition, position) > 1 && maxTries > 0)
            {
                MoveTowardsEndpoint();

                var logger = OvermorrowModFile.Instance.Logger;
                WorldGen.PlaceTile((int)position.X, (int)position.Y, TileID.ObsidianBrick, true, true);
                maxTries--;
            }
        }
    }
}