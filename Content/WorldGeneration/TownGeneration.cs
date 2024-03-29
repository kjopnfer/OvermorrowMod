using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Content.Tiles;
using OvermorrowMod.Content.Tiles.TilePiles;
using OvermorrowMod.Content.Tiles.Town;
using OvermorrowMod.Core;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class TownGeneration : ModSystem
    {
        public static Vector2 SojournLocation;
        public override void SaveWorldData(TagCompound tag)
        {
            tag["SojournLocation"] = SojournLocation;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            SojournLocation = tag.Get<Vector2>("SojournLocation");
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int TunnelIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Tunnels"));
            if (TunnelIndex != -1) tasks.Insert(TunnelIndex + 1, new PassLegacy("Sojourn Base", GenerateTownFoundation));
            
            int BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (BiomeIndex != -1) tasks.Insert(BiomeIndex + 1, new PassLegacy("Sojourn Town", GenerateTown));
        }

        private void GenerateTownFoundation(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Excavating Sojourn";

            Vector2 startPosition = new Vector2(Main.maxTilesX * 0.75f, (float)(Main.worldSurface * 0.35f));
            SojournLocation = ModUtils.FindNearestGround(startPosition, false) * 16;
            SojournBase sojournBase = new SojournBase();
            sojournBase.Place(new Point((int)(SojournLocation.X / 16), (int)(SojournLocation.Y / 16 + 65)), GenVars.structures);
        }

        private void GenerateTown(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Raising Sojourn";
            PlaceTown((int)(SojournLocation.X / 16), (int)(SojournLocation.Y / 16 + 65));   
        }

        /// <summary>
        /// First pass of the town to terraform the area where the town will be placed
        /// </summary>
        public static void PlaceTownFoundation(int x, int y)
        {
            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(53, 117, 60)] = TileID.Grass,
                [new Color(90, 51, 37)] = TileID.Dirt,
                [new Color(132, 124, 110)] = TileID.Stone,
            };

            Dictionary<Color, int> WallMapping = new Dictionary<Color, int>
            {
                [new Color(117, 86, 75)] = WallID.Dirt,
                [new Color(67, 85, 69)] = WallID.Grass,
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2,
            };

            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Clear", AssetRequestMode.ImmediateLoad).Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, TileRemoval);
            TileClear.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn", AssetRequestMode.ImmediateLoad).Value;
            Texture2D WallMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Walls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D LiquidMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Liquids", AssetRequestMode.ImmediateLoad).Value;

            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, WallMap, WallMapping, LiquidMap);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            //WorldGen.TileRunner(x - (TileClear.width / 2), y - (TileClear.height / 2) + 30, 25, 1, TileID.ObsidianBrick, true);
            //WorldGen.TileRunner(x + (TileClear.width / 2), y - (TileClear.height / 2) + 45, 25, 1, TileID.ObsidianBrick, true);

            /*for (int i = 0; i < 5; i++)
            {
                //WorldGen.digTunnel(x + (TileClear.width / 2) + (i * 20), y - (TileClear.height / 2) + 40 - (i * 10), 0, -20, 5, 20);
                WorldGen.digTunnel(x - (TileClear.width / 2) - (i * 20), y - (TileClear.height / 2) + 30 - (i * 10), 0, -20, 5, 20);
            }
            //WorldGen.digTunnel(x - (TileClear.width / 2), y - (TileClear.height / 2) + 30, 0, 0, 1, 20);

            for (int i = 0; i < 5; i++)
            {
                WorldGen.digTunnel(x + (TileClear.width / 2) + (i * 20), y - (TileClear.height / 2) + 40 - (i * 10), 0, -20, 6, 20);
            }*/
        }

        private static void PlacePlatforms(int x, int y)
        {
            /*for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    WorldGen.PlaceTile(x + i, y + j, TileID.ObsidianBrick, false, true);
            }*/

            // Setting forced to true gives an object reference error apparently
            #region Lower House
            WorldGen.PlaceTile(x - 2, y - 49, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 2, y - 49, (int)SlopeType.SlopeDownLeft);

            WorldGen.PlaceTile(x - 3, y - 50, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 3, y - 50, (int)SlopeType.SlopeDownLeft);

            WorldGen.PlaceTile(x - 4, y - 51, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 4, y - 51, (int)SlopeType.SlopeDownLeft);

            WorldGen.PlaceTile(x - 5, y - 52, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 5, y - 52, (int)SlopeType.SlopeDownLeft);

            WorldGen.PlaceTile(x - 6, y - 53, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 6, y - 53, (int)SlopeType.SlopeDownLeft);

            WorldGen.PlaceTile(x - 7, y - 54, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 7, y - 54, (int)SlopeType.SlopeDownLeft);

            WorldGen.PlaceTile(x - 6, y - 54, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 5, y - 54, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 4, y - 54, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 3, y - 54, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 2, y - 54, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 7, y - 56, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 6, y - 56, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 7, y - 58, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 6, y - 58, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 7, y - 60, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 6, y - 60, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 5, y - 60, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 4, y - 60, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 3, y - 60, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 2, y - 60, ModContent.TileType<CastlePlatform>(), true, false);
            #endregion

            #region Castle
            WorldGen.PlaceTile(x - 27, y - 101, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 27, y - 101, (int)SlopeType.SlopeDownRight);

            WorldGen.PlaceTile(x - 26, y - 102, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 26, y - 102, (int)SlopeType.SlopeDownRight);

            WorldGen.PlaceTile(x - 25, y - 103, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 25, y - 103, (int)SlopeType.SlopeDownRight);

            WorldGen.PlaceTile(x - 24, y - 104, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 24, y - 104, (int)SlopeType.SlopeDownRight);

            WorldGen.PlaceTile(x - 23, y - 105, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 23, y - 105, (int)SlopeType.SlopeDownRight);

            WorldGen.PlaceTile(x - 22, y - 106, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 22, y - 106, (int)SlopeType.SlopeDownRight);

            WorldGen.PlaceTile(x - 21, y - 107, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 21, y - 107, (int)SlopeType.SlopeDownRight);

            WorldGen.PlaceTile(x - 22, y - 107, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 23, y - 107, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 24, y - 107, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 25, y - 107, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 26, y - 107, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 27, y - 107, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 28, y - 107, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 8, y - 109, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 7, y - 109, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 8, y - 111, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 7, y - 111, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 8, y - 113, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 7, y - 113, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 8, y - 115, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 7, y - 115, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 8, y - 117, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 7, y - 117, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 10, y - 117, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 11, y - 117, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 12, y - 117, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 13, y - 117, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 14, y - 117, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 15, y - 117, ModContent.TileType<CastlePlatform>(), true, false);

            for (int i = 0; i < 6; i++)
            {
                WorldGen.PlaceTile(x - 10 - i, y - 117, ModContent.TileType<CastlePlatform>(), true, false);
            }

            for (int i = 0; i < 8; i += 2)
            {
                WorldGen.PlaceTile(x - 1, y - 117 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.PlaceTile(x, y - 117 - i, ModContent.TileType<CastlePlatform>(), true, false);
            }

            WorldGen.PlaceTile(x + 1, y - 123, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 2, y - 123, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 2, y - 123, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 2, y - 135, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.SlopeTile(x - 2, y - 135, (int)SlopeType.SlopeDownLeft);

            WorldGen.PlaceTile(x - 46, y - 134, ModContent.TileType<CastlePlatform>(), true, false);

            for (int i = 0; i < 6; i++)
            {
                WorldGen.PlaceTile(x + 3 + i, y - 135 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.SlopeTile(x + 3 + i, y - 135 - i, (int)SlopeType.SlopeDownRight);
            }

            for (int i = 0; i < 12; i++)
            {
                WorldGen.PlaceTile(x + 9 - i, y - 140, ModContent.TileType<CastlePlatform>(), true, false);
            }

            WorldGen.PlaceTile(x + 13, y - 142, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x + 14, y - 142, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 20, y - 142, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 19, y - 142, ModContent.TileType<CastlePlatform>(), true, false);

            for (int i = 0; i < 5; i++)
            {
                WorldGen.PlaceTile(x - 10 - i, y - 136 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.SlopeTile(x - 10 - i, y - 136 - i, (int)SlopeType.SlopeDownLeft);
            }

            for (int i = 0; i < 9; i++)
            {
                WorldGen.PlaceTile(x - 15 + i, y - 140, ModContent.TileType<CastlePlatform>(), true, false);
            }

            for (int i = 0; i < 12; i += 2)
            {
                WorldGen.PlaceTile(x - 14, y - 142 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.PlaceTile(x - 13, y - 142 - i, ModContent.TileType<CastlePlatform>(), true, false);
            }

            WorldGen.PlaceTile(x - 15, y - 152, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 12, y - 152, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 19, y - 151, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 20, y - 151, ModContent.TileType<CastlePlatform>(), true, false);

            for (int i = 0; i < 16; i += 2)
            {
                WorldGen.PlaceTile(x - 9, y - 154 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.PlaceTile(x - 8, y - 154 - i, ModContent.TileType<CastlePlatform>(), true, false);
            }

            WorldGen.PlaceTile(x - 10, y - 168, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 7, y - 168, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 19, y - 162, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 18, y - 162, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 17, y - 162, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 3, y - 162, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 2, y - 162, ModContent.TileType<CastlePlatform>(), true, false);

            for (int i = 0; i < 10; i += 2)
            {
                WorldGen.PlaceTile(x - 12, y - 170 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.PlaceTile(x - 11, y - 170 - i, ModContent.TileType<CastlePlatform>(), true, false);
            }

            WorldGen.PlaceTile(x - 13, y - 178, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 10, y - 178, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x + 25, y - 123, ModContent.TileType<CastlePlatform>(), true, false);

            for (int i = 0; i < 10; i += 2)
            {
                WorldGen.PlaceTile(x + 26, y - 123 + i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.PlaceTile(x + 27, y - 123 + i, ModContent.TileType<CastlePlatform>(), true, false);
            }

            WorldGen.PlaceTile(x + 28, y - 123, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x + 36, y - 113, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x + 37, y - 113, ModContent.TileType<CastlePlatform>(), true, false);

            for (int i = 0; i < 8; i++)
            {
                if (i == 5) continue;
                WorldGen.PlaceTile(x + 21 - i, y - 113, ModContent.TileType<CastlePlatform>(), true, false);
            }

            for (int i = 0; i < 16; i += 2)
            {
                WorldGen.PlaceTile(x + 14, y - 113 + i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.PlaceTile(x + 15, y - 113 + i, ModContent.TileType<CastlePlatform>(), true, false);
            }

            WorldGen.PlaceTile(x + 13, y - 109, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x + 13, y - 107, ModContent.TileType<CastlePlatform>(), true, false);

            for (int i = 0; i < 7; i++)
            {
                WorldGen.PlaceTile(x + 18 + i, y - 99 + i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.SlopeTile(x + 18 + i, y - 99 + i, (int)SlopeType.SlopeDownLeft);
            }

            for (int i = 0; i < 6; i++)
            {
                WorldGen.PlaceTile(x + 13 + i, y - 99, ModContent.TileType<CastlePlatform>(), true, false);
            }

            WorldGen.KillTile(x + 25, y - 121);

            for (int j = 0; j < 18; j += 6)
            {
                for (int i = 0; i < 6; i++)
                {
                    WorldGen.PlaceTile(x - 13 + i, y - 118 - j - i, ModContent.TileType<CastlePlatform>(), true, false);
                    WorldGen.SlopeTile(x - 13 + i, y - 118 - j - i, (int)SlopeType.SlopeDownRight);
                }

                for (int i = 0; i < 9; i++)
                {
                    WorldGen.PlaceTile(x - 7 - i, y - 123 - j, ModContent.TileType<CastlePlatform>(), true, false);
                }
            }

            for (int j = 0; j < 18; j += 6)
            {
                for (int i = 0; i < 6; i++)
                {
                    WorldGen.PlaceTile(x - 41 + i, y - 118 - j - i, ModContent.TileType<CastlePlatform>(), true, false);
                    WorldGen.SlopeTile(x - 41 + i, y - 118 - j - i, (int)SlopeType.SlopeDownRight);
                }

                for (int i = 0; i < 10; i++)
                {
                    WorldGen.PlaceTile(x - 34 - i, y - 123 - j, ModContent.TileType<CastlePlatform>(), true, false);
                }
            }

            for (int i = 0; i < 14; i += 2)
            {
                WorldGen.PlaceTile(x - 41, y - 137 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.PlaceTile(x - 40, y - 137 - i, ModContent.TileType<CastlePlatform>(), true, false);
            }

            WorldGen.PlaceTile(x - 39, y - 149, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 42, y - 149, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 31, y - 149, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 30, y - 149, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 31, y - 147, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 30, y - 147, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 31, y - 143, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.KillTile(x - 40, y - 136);


            #endregion

            #region Inn
            for (int i = 0; i < 16; i += 2)
            {
                if (i == 6)
                    for (int j = 0; j < 8; j++)
                        WorldGen.PlaceTile(x + 177 + j, y - 48, ModContent.TileType<CastlePlatform>(), true, false);

                WorldGen.PlaceTile(x + 180, y - 42 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.PlaceTile(x + 181, y - 42 - i, ModContent.TileType<CastlePlatform>(), true, false);
            }

            for (int i = 0; i < 8; i++)
            {
                WorldGen.PlaceTile(x + 178 + i, y - 57 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.SlopeTile(x + 178 + i, y - 57 - i, (int)SlopeType.SlopeDownRight);

                if (i == 7)
                {
                    for (int j = 0; j < 10; j++)
                        WorldGen.PlaceTile(x + 176 + j, y - 64, ModContent.TileType<CastlePlatform>(), true, false);
                }
            }

            for (int i = 0; i < 8; i += 2)
            {
                if (i == 6)
                {
                    WorldGen.PlaceTile(x + 179, y - 72, ModContent.TileType<CastlePlatform>(), true, false);
                    WorldGen.PlaceTile(x + 182, y - 72, ModContent.TileType<CastlePlatform>(), true, false);
                }

                WorldGen.PlaceTile(x + 180, y - 66 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.PlaceTile(x + 181, y - 66 - i, ModContent.TileType<CastlePlatform>(), true, false);
            }

            for (int i = 0; i < 8; i++)
                WorldGen.PlaceTile(x + 177 + i, y - 80, ModContent.TileType<CastlePlatform>(), true, false);

            #endregion

            #region Feyden's House
            WorldGen.KillTile(x - 143, y - 101);

            for (int i = 0; i < 12; i += 2)
            {
                WorldGen.PlaceTile(x - 143, y - 100 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.PlaceTile(x - 142, y - 100 - i, ModContent.TileType<CastlePlatform>(), true, false);
            }

            WorldGen.PlaceTile(x - 141, y - 104, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 145, y - 104, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 145, y - 110, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 144, y - 110, ModContent.TileType<CastlePlatform>(), true, false);

            WorldGen.PlaceTile(x - 114, y - 104, ModContent.TileType<CastlePlatform>(), true, false);
            WorldGen.PlaceTile(x - 113, y - 104, ModContent.TileType<CastlePlatform>(), true, false);

            for (int i = 0; i < 6; i++)
            {
                WorldGen.PlaceTile(x - 121 + i, y - 105 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.SlopeTile(x - 121 + i, y - 105 - i, (int)SlopeType.SlopeDownRight);
            }

            for (int i = 0; i < 6; i++)
            {
                WorldGen.PlaceTile(x - 121 + i, y - 110, ModContent.TileType<CastlePlatform>(), true, false);
            }

            for (int i = 0; i < 4; i++)
                WorldGen.PlaceTile(x - 135 - i, y - 116, ModContent.TileType<CastlePlatform>(), true, false);

            #endregion

            #region Watch Tower
            for (int i = 0; i < 12; i += 2)
            {
                WorldGen.PlaceTile(x - 216, y - 86 - i, ModContent.TileType<CastlePlatform>(), true, false);
                WorldGen.PlaceTile(x - 215, y - 86 - i, ModContent.TileType<CastlePlatform>(), true, false);
            }

            WorldGen.PlaceTile(x - 213, y - 90, ModContent.TileType<CastlePlatform>(), true, false);

            for (int i = 0; i < 19; i++)
                WorldGen.PlaceTile(x - 215 - i, y - 90, ModContent.TileType<CastlePlatform>(), true, false);

            for (int i = 0; i < 17; i++)
                WorldGen.PlaceTile(x - 213 - i, y - 96, ModContent.TileType<CastlePlatform>(), true, false);

            NPC.NewNPC(null, (x - 215 - 20) * 16, (y - 96 + 6) * 16, ModContent.NPCType<Moxley>());
            #endregion
        }

        private static void PlaceFurniture(int x, int y)
        {
            #region Castle
            for (int i = 0; i < 1; i++)
            {
                //WorldGen.PlaceTile(x - 43, y - 109, TileID.ObsidianBrick, true, true);
                //WorldGen.KillTile(x - 43, y - 109);
            }
            //WorldGen.PlaceTile(x - 44, y - 109, TileID.ObsidianBrick, true, true);
            //WorldGen.KillTile(x - 44, y - 109);

            //WorldGen.PlaceTile(x - 43, y - 106, TileID.ObsidianBrick, true, false);
            WorldGen.KillTile(x - 43, y - 109);
            ModUtils.PlaceObject(x - 43, y - 109, (ushort)ModContent.TileType<CastleTable>(), 0, 1);
            ModUtils.PlaceObject(x - 41, y - 109, (ushort)ModContent.TileType<TownChair>(), 0, -1);
            ModUtils.PlaceObject(x - 39, y - 111, (ushort)ModContent.TileType<GarrisonBanner>(), Main.rand.Next(0, 3));
            ModUtils.PlaceObject(x - 32, y - 109, TileID.ClosedDoor);

            ModUtils.PlaceObject(x - 39, y - 103, (ushort)ModContent.TileType<GarrisonBanner>(), Main.rand.Next(0, 3));
            ModUtils.PlaceObject(x - 32, y - 101, TileID.ClosedDoor);

            ModUtils.PlaceObject(x - 18, y - 101, TileID.ClosedDoor);

            // The prison table with the lamp:
            ModUtils.PlaceObject(x - 12, y - 98, (ushort)ModContent.TileType<CastleTable>(), 0, 1);
            ModUtils.PlaceObject(x - 12, y - 100, (ushort)ModContent.TileType<Lamp>());

            ModUtils.PlaceObject(x - 14, y - 108, (ushort)ModContent.TileType<SojournBookcase>(), 0, -1);
            ModUtils.PlaceObject(x - 11, y - 108, (ushort)ModContent.TileType<CastleTable_Alt>(), 0, 1);

            // Job Board
            ModUtils.PlaceObject(x + 4, y - 124, (ushort)ModContent.TileType<SojournBookcase>(), 0, 1);
            ModUtils.PlaceObject(x + 8, y - 124, (ushort)ModContent.TileType<SojournBookcase>(), 0, 1);

            WorldGen.KillTile(x - 1, y - 124);
            ModUtils.PlaceObject(x - 1, y - 124, (ushort)ModContent.TileType<ScribeTable>(), 0, -1);
            ModUtils.PlaceObject(x + 1, y - 124, (ushort)ModContent.TileType<TownChair>(), 0, -1);

            ModUtils.PlaceObject(x + 4, y - 117, (ushort)ModContent.TileType<JobBoard>());

            ModUtils.PlaceObject(x + 12, y - 114, TileID.ClosedDoor);
            ModUtils.PlaceObject(x - 4, y - 108, TileID.ClosedDoor);

            // Right Tower
            ModUtils.PlaceObject(x + 26, y - 124, (ushort)ModContent.TileType<TownChair>(), 0, 1);
            ModUtils.PlaceObject(x + 29, y - 124, (ushort)ModContent.TileType<CastleTable>(), 0, 1);
            ModUtils.PlaceObject(x + 28, y - 126, (ushort)ModContent.TileType<Lamp>());

            ModUtils.PlaceObject(x + 30, y - 118, (ushort)ModContent.TileType<GarrisonBanner>(), Main.rand.Next(0, 3));
            ModUtils.PlaceObject(x + 24, y - 114, (ushort)ModContent.TileType<SojournBookcase>(), 0, -1);
            ModUtils.PlaceObject(x + 30, y - 114, (ushort)ModContent.TileType<GarrisonBed>());

            ModUtils.PlaceObject(x + 28, y - 141, (ushort)ModContent.TileType<SojournFlag>());

            // Left Tower
            ModUtils.PlaceObject(x - 36, y - 136, (ushort)ModContent.TileType<CastleTable>(), 0, 1);
            ModUtils.PlaceObject(x - 36, y - 142, (ushort)ModContent.TileType<GarrisonBanner>(), Main.rand.Next(0, 3));

            ModUtils.PlaceObject(x - 40, y - 150, (ushort)ModContent.TileType<TownChair>(), 0, 1);
            ModUtils.PlaceObject(x - 37, y - 150, (ushort)ModContent.TileType<CastleTable>(), 0, 1);
            ModUtils.PlaceObject(x - 37, y - 152, (ushort)ModContent.TileType<Lamp>());

            // Middle Tower
            ModUtils.PlaceObject(x - 13, y - 153, (ushort)ModContent.TileType<CastleTable_Alt>(), 0, -1);
            ModUtils.PlaceObject(x - 12, y - 161, (ushort)ModContent.TileType<GarrisonBanner>(), Main.rand.Next(0, 3));
            ModUtils.PlaceObject(x - 8, y - 171, (ushort)ModContent.TileType<GarrisonBanner>(), Main.rand.Next(0, 3));

            ModUtils.PlaceObject(x - 10, y - 179, (ushort)ModContent.TileType<TownChair>(), 0, 1);
            ModUtils.PlaceObject(x - 7, y - 179, (ushort)ModContent.TileType<CastleTable>(), 0, 1);
            ModUtils.PlaceObject(x - 8, y - 181, (ushort)ModContent.TileType<Lamp>());

            ModUtils.PlaceObject(x - 10, y - 196, (ushort)ModContent.TileType<SojournFlag>());
            // Dining Room
            ModUtils.PlaceObject(x - 21, y - 129, (ushort)ModContent.TileType<GarrisonBanner>(), Main.rand.Next(0, 3));
            ModUtils.PlaceObject(x - 21, y - 124, (ushort)ModContent.TileType<TownChair>(), 0, -1);

            ModUtils.PlaceObject(x - 29, y - 129, (ushort)ModContent.TileType<GarrisonBanner>(), Main.rand.Next(0, 3));
            ModUtils.PlaceObject(x - 29, y - 124, (ushort)ModContent.TileType<TownChair>(), 0, 1);

            ModUtils.PlaceObject(x - 26, y - 124, (ushort)ModContent.TileType<BanquetTable>());
            ModUtils.PlaceObject(x - 26, y - 126, (ushort)ModContent.TileType<SojournCandelabra>());

            // Beds
            ModUtils.PlaceObject(x - 7, y - 141, (ushort)ModContent.TileType<SojournBookcase>(), 0, 1);
            ModUtils.PlaceObject(x - 10, y - 141, (ushort)ModContent.TileType<CastleTable>(), 0, 1);

            // TODO: Not sure if this is a banner but in case its a painting, this is the thing near the beds
            ModUtils.PlaceObject(x + 3, y - 143, (ushort)ModContent.TileType<GarrisonBanner>(), Main.rand.Next(0, 3));

            // TODO: This is gonna have to face right
            ModUtils.PlaceObject(x - 2, y - 141, (ushort)ModContent.TileType<GarrisonBed>());

            ModUtils.PlaceObject(x + 5, y - 141, (ushort)ModContent.TileType<MaceBed>());
            ModUtils.PlaceObject(x + 7, y - 135, (ushort)ModContent.TileType<BootBed>());
            ModUtils.PlaceObject(x + 1, y - 135, (ushort)ModContent.TileType<CastleTable_Alt>(), 0, 1);

            //WorldGen.KillTile(x - 41, y - 109);
            //WorldGen.KillTile(x - 10, y - 109);

            #endregion
        }

        /// <summary>
        /// Second pass to place buildings, liquids, and walls
        /// </summary>
        public static void PlaceTown(int x, int y)
        {
            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(90, 51, 37)] = TileID.Dirt,
                [new Color(107, 105, 101)] = ModContent.TileType<CastleBrick>(),
                [new Color(170, 109, 48)] = ModContent.TileType<CastleRoof>(),
                [new Color(115, 81, 60)] = TileID.WoodBlock,
                [new Color(67, 65, 64)] = ModContent.TileType<DarkCastleBrick>(),
                [new Color(56, 45, 42)] = TileID.WoodenBeam,
            };

            Dictionary<Color, int> WallMapping = new Dictionary<Color, int>
            {
                [new Color(110, 110, 110)] = WallID.StoneSlab,
                [new Color(68, 55, 50)] = WallID.Wood,
                [new Color(66, 64, 60)] = ModContent.WallType<CastleWall>(),
                [new Color(117, 86, 75)] = WallID.Dirt,
                [new Color(67, 85, 69)] = WallID.Grass,
                [new Color(0, 247, 255)] = ModContent.WallType<EmptyWall>(),
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2,
            };

            // Secondary clear to remove any unexpected objects or tiles that vanilla may have placed nearby it
            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Clear_2", AssetRequestMode.ImmediateLoad).Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Structures", AssetRequestMode.ImmediateLoad).Value;
            Texture2D WallMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Walls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D LiquidMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Liquids", AssetRequestMode.ImmediateLoad).Value;
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, WallMap, WallMapping, LiquidMap);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            PlacePlatforms(x, y);
            PlaceFurniture(x, y);
        }
    }

    public class SojournBase : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            int x = origin.X;
            int y = origin.Y;

            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(53, 117, 60)] = TileID.Grass,
                [new Color(90, 51, 37)] = TileID.Dirt,
                [new Color(132, 124, 110)] = TileID.Stone,
            };

            Dictionary<Color, int> WallMapping = new Dictionary<Color, int>
            {
                [new Color(117, 86, 75)] = WallID.Dirt,
                [new Color(67, 85, 69)] = WallID.Grass,
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2,
            };

            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Clear", AssetRequestMode.ImmediateLoad).Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, TileRemoval);
            TileClear.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn", AssetRequestMode.ImmediateLoad).Value;
            Texture2D WallMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Walls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D LiquidMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Liquids", AssetRequestMode.ImmediateLoad).Value;

            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, WallMap, WallMapping, LiquidMap);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            structures.AddProtectedStructure(new Rectangle(origin.X - (TileClear.width / 2), origin.Y - (TileClear.height), TileClear.width, TileClear.height));

            return true;
        }
    }
}