using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Common.Pathfinding;
using OvermorrowMod.Content.Tiles;
using OvermorrowMod.Content.Tiles.Ambient.WaterCave;
using OvermorrowMod.Content.Tiles.Ores;
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

        public override void PostDrawTiles()
        {
            // SharedAIState.State2x2.Visualize();
        }

        // Worldgen Debugging
        public static bool JustPressed(Keys key)
        {
            return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
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

        private void ManaStoneOres(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Generating Modded Ores";
            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.

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
                int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);

                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(2, 4), WorldGen.genRand.Next(2, 6), ModContent.TileType<EruditeTile>());
            }

            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 0.0014); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.
                int x = WorldGen.genRand.Next(0, Main.maxTilesX / 6);
                int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.

                // Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place. Feel free to experiment with strength and step to see the shape they generate.
                WorldGen.PlaceTile(x, y, ModContent.TileType<HerosAltar>());
            }
        }

        private void GenerateAmbientObjects(GenerationProgress progress, GameConfiguration config)
        {
            // Place ambient objects for the Flooded Caverns
            for (int i = 0; i < Main.maxTilesY * 45; i++)
            {
                int[] rockFormations = { ModContent.TileType<Rock1>(), ModContent.TileType<Rock2>(), ModContent.TileType<Rock3>(), ModContent.TileType<Rock4>(), ModContent.TileType<Stalagmite1>(), ModContent.TileType<Stalagmite2>(), ModContent.TileType<Stalagmite3>(), ModContent.TileType<Stalagmite4>(), ModContent.TileType<Stalagmite5>() };
                int x = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
                int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY - 200);
                int type = rockFormations[Main.rand.Next(9)];
                if (Main.tile[x, y].TileType == ModContent.TileType<GlowBlock>())
                {
                    WorldGen.PlaceObject(x, y, (ushort)type);
                    NetMessage.SendObjectPlacement(-1, x, y, (ushort)type, 0, 0, -1, -1);
                }
            }
        }
        #endregion
    }
}