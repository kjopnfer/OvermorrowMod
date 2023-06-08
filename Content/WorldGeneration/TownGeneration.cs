using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
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

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int TerrainIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Wavy Caves"));
            if (TerrainIndex != -1)
            {
                tasks.Insert(TerrainIndex + 1, new PassLegacy("Sojourn Base", GenerateTownFoundation));
            }

            int BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (BiomeIndex != -1)
            {
                tasks.Insert(BiomeIndex + 1, new PassLegacy("Sojourn Town", GenerateTown));
            }
        }

        private void GenerateTownFoundation(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Excavating Sojourn";
            bool validArea = false;

            int x = (int)(Main.maxTilesX * 0.75f);
            int y = (int)(Main.worldSurface * 0.35f);


            // Loop downwards until we reach a solid tile
            Tile tile = Framing.GetTileSafely(x, y);
            while (y - 200 < Main.worldSurface * 0.35f)
            {
                y++;
                tile = Framing.GetTileSafely(x, y);
            }

            /*if (Main.tileSolid[tile.TileType])
            {
                validArea = true;
            }*/

            SojournLocation = new Vector2(x, y + 30) * 16;

            for (int _ = 0; _ < 2; _++)
            {
                // this is so fucking stupid
                if (_ == 1)
                    PlaceTownFoundation(x, y + 30);
                else
                { // this isnt supposed to do anything
                    PlaceTownFoundation(x, y + 30);
                }
            }
        }

        private void GenerateTown(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Raising Sojourn";

            // I don't know why the first one doesn't generate dude
            for (int _ = 0; _ < 2; _++)
            {
                // this is so fucking stupid
                if (_ == 1)
                    PlaceTown((int)(SojournLocation.X / 16), (int)(SojournLocation.Y / 16), true);
                else
                { // this isnt supposed to do anything
                    PlaceTown((int)(SojournLocation.X / 16), (int)(SojournLocation.Y / 16));
                }
            }
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
                [new Color(90, 51, 37)] = TileID.Dirt,
            };

            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Clear").Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, TileRemoval);
            TileClear.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn").Value;
            Texture2D LiquidMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Liquids").Value;
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, TileMap, WallMapping, LiquidMap, null, null, null);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            WorldGen.TileRunner(x - (TileClear.width / 2), y - (TileClear.height / 2) + 30, 25, 1, TileID.ObsidianBrick, true);
            WorldGen.TileRunner(x + (TileClear.width / 2), y - (TileClear.height / 2) + 45, 25, 1, TileID.ObsidianBrick, true);

            for (int i = 0; i < 5; i++)
            {
                //WorldGen.digTunnel(x + (TileClear.width / 2) + (i * 20), y - (TileClear.height / 2) + 40 - (i * 10), 0, -20, 5, 20);
                WorldGen.digTunnel(x - (TileClear.width / 2) - (i * 20), y - (TileClear.height / 2) + 30 - (i * 10), 0, -20, 5, 20);
            }
            //WorldGen.digTunnel(x - (TileClear.width / 2), y - (TileClear.height / 2) + 30, 0, 0, 1, 20);

            for (int i = 0; i < 5; i++)
            {
                WorldGen.digTunnel(x + (TileClear.width / 2) + (i * 20), y - (TileClear.height / 2) + 40 - (i * 10), 0, -20, 6, 20);
            }

        }

        private static void PlacePlatforms(int x, int y)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    WorldGen.PlaceTile(x + i, y + j, TileID.ObsidianBrick, false, true);
            }

            //WorldGen.PlaceTile(x - 2, y - 49, TileID.Platforms, true, true);

            // Setting forced to true gives an object reference error apparently
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

            //var test = OvermorrowModFile.Instance.Find<ModTile>("CastlePlatform").Type;
            //WorldGen.PlaceTile(x - 2, y - 49, test, true, true);

        }

        /// <summary>
        /// Second pass to place buildings, liquids, and walls
        /// </summary>
        public static void PlaceTown(int x, int y, bool spawnNPC = false)
        {
            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(90, 51, 37)] = TileID.Dirt,
                //[new Color(132, 124, 110)] = TileID.Stone,
                [new Color(107, 105, 101)] = ModContent.TileType<CastleBrick>(),
                [new Color(170, 109, 48)] = ModContent.TileType<CastleRoof>(),
                [new Color(115, 81, 60)] = TileID.WoodBlock,
                //[new Color(94, 70, 56)] = ModContent.TileType<CastlePlatform>(),
                [new Color(67, 65, 64)] = ModContent.TileType<DarkCastleBrick>(),
            };

            Dictionary<Color, int> WallMapping = new Dictionary<Color, int>
            {
                [new Color(110, 110, 110)] = WallID.StoneSlab,
                [new Color(68, 55, 50)] = WallID.Wood,
                [new Color(66, 64, 60)] = ModContent.WallType<CastleWall>(),
                [new Color(117, 86, 75)] = WallID.Dirt,
                [new Color(67, 85, 69)] = WallID.Grass,
                //[new Color(0, 0, 0)] = ModContent.WallType<CastleWall>(),
            };

            Dictionary<Color, int> SlopeMapping = new Dictionary<Color, int>
            {
                [new Color(0, 255, 0)] = 2, // /|
                [new Color(255, 0, 0)] = 1, // |\    
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2,
                //[new Color(90, 51, 37)] = TileID.Dirt,
            };

            Dictionary<Color, int> WallRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2
            };

            // Secondary clear to remove any unexpected objects or tiles that vanilla may have placed nearby it
            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Clear_2", AssetRequestMode.ImmediateLoad).Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, WallRemoval);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Structures", AssetRequestMode.ImmediateLoad).Value;
            Texture2D WallMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Walls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D LiquidMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/Sojourn_Liquids", AssetRequestMode.ImmediateLoad).Value;
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, WallMap, WallMapping, LiquidMap, null, null, null);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            PlacePlatforms(x, y);
            //WorldGen.TileRunner(x, y - (TileClear.height / 2), 10, 1,)
            /*if (spawnNPC)
            {
                NPC.NewNPC(null, (x - (TileClear.width / 2) + 14) * 16, (y - (TileClear.height) + 26) * 16, ModContent.NPCType<SojournGuard>());

                NPC.NewNPC(null, (x - (TileClear.width / 2) + 110) * 16, (y - (TileClear.height) + 45) * 16, ModContent.NPCType<TownKid>());

                NPC.NewNPC(null, (x - (TileClear.width / 2) + 105) * 16, (y - (TileClear.height) + 44) * 16, ModContent.NPCType<SojournGuard2>());

                NPC.NewNPC(null, (x - (TileClear.width / 2) + 201) * 16, (y - (TileClear.height) + 43) * 16, ModContent.NPCType<SojournGuard2>());
            }*/

            /*WorldGen.PlaceTile(x - (TileClear.width / 2) + 30, y - (TileClear.height) + 35, ModContent.TileType<CartSign>());

            WorldGen.PlaceTile(x - (TileClear.width / 2) + 41, y - (TileClear.height) + 31, ModContent.TileType<CartLamp>());

            for (int i = 0; i < 56; i++)
            {
                for (int j = 0; j < 94; j++)
                {
                    Tile tile = Framing.GetTileSafely(x - (TileClear.width / 2) + i, y - (TileClear.height) + j);
                    if (tile.TileType == TileID.Adamantite)
                    {
                        tile.ClearTile();
                        //Main.tile[x - (TileClear.width / 2) + i, y - (TileClear.height) + j].ClearTile();
                        WorldGen.PlaceTile(x - (TileClear.width / 2) + i, y - (TileClear.height) + j, ModContent.TileType<CastlePlatform>(), false, true);
                        //WorldGen.KillTile(x - (TileClear.width / 2) + i, y - (TileClear.height) + j, true);
                    }
                }
            }*/

            /*WorldGen.PlaceTile(x - (TileClear.width / 2) + 85 + 1, y + 25 - (TileClear.height), TileID.ObsidianBrick, false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 85, y + 25 - (TileClear.height), TileID.ObsidianBrick, false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 85 - 1, y + 25 - (TileClear.height), TileID.ObsidianBrick, false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 85, y + 25 - (TileClear.height) - 1, TileID.ObsidianBrick, false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 85, y + 25 - (TileClear.height) + 1, TileID.ObsidianBrick, false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 85  + 1, y + 25 - (TileClear.height) + 1, TileID.ObsidianBrick, false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 85  - 1, y + 25 - (TileClear.height) + 1, TileID.ObsidianBrick, false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 85  + 1, y + 25 - (TileClear.height) - 1, TileID.ObsidianBrick, false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 85 - 1, y + 25 - (TileClear.height) - 1, TileID.ObsidianBrick, false, true);*/

            /*WorldGen.SlopeTile(x - (TileClear.width / 2) + 85 + 1, y + 25 - (TileClear.height), (int)SlopeType.SlopeDownRight);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 85, y + 25 - (TileClear.height), (int)SlopeType.SlopeDownRight);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 85 - 1, y + 25 - (TileClear.height), (int)SlopeType.SlopeDownRight);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 85, y + 25 - (TileClear.height) - 1, (int)SlopeType.SlopeDownRight);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 85, y + 25 - (TileClear.height) + 1, (int)SlopeType.SlopeDownRight);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 85 + 1, y + 25 - (TileClear.height) + 1, (int)SlopeType.SlopeDownRight);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 85 - 1, y + 25 - (TileClear.height) + 1, (int)SlopeType.SlopeDownRight);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 85 + 1, y + 25 - (TileClear.height) - 1, (int)SlopeType.SlopeDownRight);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 85 - 1, y + 25 - (TileClear.height) - 1, (int)SlopeType.SlopeDownRight);*/


            /*tile = Main.tile[x - (TileClear.width / 2) + 69, y + 22 - (TileClear.height)]; tile.Slope = SlopeType.SlopeUpRight;
            tile = Main.tile[x - (TileClear.width / 2) + 70, y + 21 - (TileClear.height)]; tile.Slope = SlopeType.SlopeUpRight;
            //tile = Main.tile[x - (TileClear.width / 2) + 71, y - (TileClear.height) + 21]; tile.Slope = SlopeType.SlopeUpRight;

            tile = Main.tile[x - (TileClear.width / 2) + 91, y + 26 - (TileClear.height)]; tile.Slope = SlopeType.SlopeUpLeft;
            //tile = Main.tile[x - (TileClear.width / 2) + 95, y - (TileClear.height) + 29]; tile.Slope = SlopeType.SlopeUpLeft;
            //tile = Main.tile[x - (TileClear.width / 2) + 94, y - (TileClear.height) + 28]; tile.Slope = SlopeType.SlopeUpLeft;
            //tile = Main.tile[x - (TileClear.width / 2) + 93, y - (TileClear.height) + 27]; tile.Slope = SlopeType.SlopeUpLeft;
            
            //

            tile = Main.tile[x - (TileClear.width / 2) + 69, y - (TileClear.height) + 24]; tile.Slope = SlopeType.SlopeUpRight;
            tile = Main.tile[x - (TileClear.width / 2) + 70, y - (TileClear.height) + 23]; tile.Slope = SlopeType.SlopeUpRight;
            tile = Main.tile[x - (TileClear.width / 2) + 71, y - (TileClear.height) + 22]; tile.Slope = SlopeType.SlopeUpRight;
            tile = Main.tile[x - (TileClear.width / 2) + 72, y - (TileClear.height) + 21]; tile.Slope = SlopeType.SlopeUpRight;

            tile = Main.tile[x - (TileClear.width / 2) + 95, y - (TileClear.height) + 30]; tile.Slope = SlopeType.SlopeUpLeft;
            tile = Main.tile[x - (TileClear.width / 2) + 94, y - (TileClear.height) + 29]; tile.Slope = SlopeType.SlopeUpLeft;
            tile = Main.tile[x - (TileClear.width / 2) + 93, y - (TileClear.height) + 28]; tile.Slope = SlopeType.SlopeUpLeft;
            tile = Main.tile[x - (TileClear.width / 2) + 92, y - (TileClear.height) + 27]; tile.Slope = SlopeType.SlopeUpLeft;*/

            /*Main.tile[x - (TileClear.width / 2) + 67, y - (TileClear.height) + 23].ClearTile();
            Main.tile[x - (TileClear.width / 2) + 68, y - (TileClear.height) + 22].ClearTile();
            Main.tile[x - (TileClear.width / 2) + 69, y - (TileClear.height) + 21].ClearTile();
            Main.tile[x - (TileClear.width / 2) + 70, y - (TileClear.height) + 20].ClearTile();

            Main.tile[x - (TileClear.width / 2) + 95, y - (TileClear.height) + 29].ClearTile();
            Main.tile[x - (TileClear.width / 2) + 94, y - (TileClear.height) + 28].ClearTile();
            Main.tile[x - (TileClear.width / 2) + 93, y - (TileClear.height) + 27].ClearTile();
            Main.tile[x - (TileClear.width / 2) + 92, y - (TileClear.height) + 26].ClearTile();

            WorldGen.PlaceTile(x - (TileClear.width / 2) + 67, y - (TileClear.height) + 23, ModContent.TileType<CastlePlatform>(), false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 68, y - (TileClear.height) + 22, ModContent.TileType<CastlePlatform>(), false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 69, y - (TileClear.height) + 21, ModContent.TileType<CastlePlatform>(), false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 70, y - (TileClear.height) + 20, ModContent.TileType<CastlePlatform>(), false, true);

            WorldGen.PlaceTile(x - (TileClear.width / 2) + 95, y - (TileClear.height) + 29, ModContent.TileType<CastlePlatform>(), false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 94, y - (TileClear.height) + 28, ModContent.TileType<CastlePlatform>(), false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 93, y - (TileClear.height) + 27, ModContent.TileType<CastlePlatform>(), false, true);
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 92, y - (TileClear.height) + 26, ModContent.TileType<CastlePlatform>(), false, true);

            WorldGen.SlopeTile(x - (TileClear.width / 2) + 67, y + 23 - (TileClear.height), 2);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 68, y + 22 - (TileClear.height), 2);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 69, y + 21 - (TileClear.height), 2);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 70, y + 20 - (TileClear.height), 2);

            WorldGen.SlopeTile(x - (TileClear.width / 2) + 95, y + 29 - (TileClear.height), 1);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 94, y + 28 - (TileClear.height), 1);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 93, y + 27 - (TileClear.height), 1);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 92, y + 26 - (TileClear.height), 1);*/

            // 68, 24
            //Tile tile = Framing.GetTileSafely(x - (TileClear.width / 2) + 67, y + 23 - (TileClear.height));
            //tile.Slope = SlopeType.SlopeUpRight;
            //WorldGen.SlopeTile(x - (TileClear.width / 2) + 67, y + 23 - (TileClear.height), (int)SlopeType.SlopeUpRight);
            ////tile = Framing.GetTileSafely(x - (TileClear.width / 2) + 67, y + 22 - (TileClear.height));
            ////tile.Slope = SlopeType.SlopeUpRight;
            //WorldGen.SlopeTile(x - (TileClear.width / 2) + 68, y + 22 - (TileClear.height), (int)SlopeType.SlopeDownRight);
            //WorldGen.SlopeTile(x - (TileClear.width / 2) + 69, y + 21 - (TileClear.height), (int)SlopeType.SlopeDownRight);
            //WorldGen.SlopeTile(x - (TileClear.width / 2) + 70, y + 20 - (TileClear.height), (int)SlopeType.SlopeDownRight);

            //TileGen.Generate(x - (TileClear.width / 2) + 69, y - (TileClear.height) + 24, true, true, true);
            /*WorldGen.SlopeTile(x - (TileClear.width / 2) + 68, y + 23 - (TileClear.height), 2);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 69, y + 22 - (TileClear.height), 2);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 70, y + 21 - (TileClear.height), 2);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 71, y + 20 - (TileClear.height), 2);

            WorldGen.SlopeTile(x - (TileClear.width / 2) + 94, y + 29 - (TileClear.height), 1);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 93, y + 28 - (TileClear.height), 1);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 92, y + 27 - (TileClear.height), 1);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 91, y + 26 - (TileClear.height), 1);

            WorldGen.SlopeTile(x - (TileClear.width / 2) + 67, y + 23 - (TileClear.height), 2);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 68, y + 22 - (TileClear.height), 2);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 69, y + 21 - (TileClear.height), 2);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 70, y + 20 - (TileClear.height), 2);

            WorldGen.SlopeTile(x - (TileClear.width / 2) + 95, y + 29 - (TileClear.height), 1);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 94, y + 28 - (TileClear.height), 1);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 93, y + 27 - (TileClear.height), 1);
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 92, y + 26 - (TileClear.height), 1);*/
            /*ModContent.GetInstance<CartLampTE>().Place(x - (TileClear.width / 2) + 12, y - (TileClear.height) + 19);

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    WorldGen.PlaceTile(x - (TileClear.width / 2) + i, y - (TileClear.height) + 26 + j, TileID.Dirt, false, true);
                }
            }*/
        }

    }
}