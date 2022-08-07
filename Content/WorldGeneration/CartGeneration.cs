using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Content.Tiles.Carts;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class CartGeneration : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Quick Cleanup"));
            if (BiomeIndex != -1)
            {
                tasks.Insert(BiomeIndex + 1, new PassLegacy("Ruined Town", GenerateTown));
            }

            base.ModifyWorldGenTasks(tasks, ref totalWeight);
        }

        private void GenerateTown(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Creating Town";

            // I don't know why the first one doesn't generate dude
            for (int _ = 0; _ < 2; _++)
            {
                bool validArea = false;

                // Pick a random spot in the world that is in midair
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = (int)(Main.worldSurface * 0.35f);

                // Make sure that the x coordinate chosen isn't within the spawn point
                while (x > Main.spawnTileX - 50 && x < Main.spawnTileY + 50)
                {
                    x = WorldGen.genRand.Next(0, Main.maxTilesX);
                }

                while (!validArea)
                {
                    // Loop downwards until we reach a solid tile
                    Tile tile = Framing.GetTileSafely(x, y);
                    while (!tile.HasTile)
                    {
                        y++;
                        tile = Framing.GetTileSafely(x, y);
                    }

                    Tile aboveTile = Framing.GetTileSafely(x, y - 1);
                    // We have the tile but we want to check if its a grass block, if it isn't restart the process
                    if (tile.TileType == TileID.Dirt && tile.WallType == WallID.None && !aboveTile.HasTile && Main.tileSolid[tile.TileType])
                    {
                        validArea = true;
                    }
                    else
                    {
                        // Reset and try again.
                        x = WorldGen.genRand.Next(0, Main.maxTilesX);
                        y = WorldGen.genRand.Next((int)(Main.worldSurface * 0.35f), (int)(Main.worldSurface * 0.5f));

                        // Make sure that the x coordinate chosen isn't within the spawn point
                        while (x > Main.spawnTileX - 50 && x < Main.spawnTileY + 50)
                        {
                            x = WorldGen.genRand.Next(0, Main.maxTilesX);
                        }
                    }
                }

                PlaceTown(x, y + 60);
            }
        }

        public static void PlaceTown(int x, int y)
        {
            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(69, 132, 64)] = TileID.Grass,
                [new Color(101, 67, 41)] = TileID.Dirt,
                [new Color(84, 68, 55)] = TileID.ClayBlock,
                [new Color(128, 128, 128)] = TileID.Stone,
                [new Color(110, 113, 117)] = TileID.StoneSlab,
                [new Color(105, 99, 94)] = ModContent.TileType<CastleBrick>(),
                [new Color(117, 70, 46)] = ModContent.TileType<CastleRoof>(),
                //[new Color(93, 70, 52)] = ModContent.TileType<PlaceholderPlatform>(),
                [new Color(69, 87, 78)] = TileID.EbonstoneBrick,
            };

            Dictionary<Color, int> WallMapping = new Dictionary<Color, int>
            {
                [new Color(70, 67, 72)] = WallID.StoneSlab,
                [new Color(49, 45, 51)] = WallID.GrayBrick,
                [new Color(73, 64, 56)] = WallID.Wood,
                [new Color(40, 34, 29)] = WallID.BorealWood,
                [new Color(70, 67, 72)] = WallID.Stone,
                [new Color(42, 50, 46)] = WallID.EbonstoneBrick,
                [new Color(87, 43, 20)] = WallID.RedBrick,
                [new Color(93, 70, 52)] = WallID.BorealWood,
            };

            Dictionary<Color, int> SlopeMapping = new Dictionary<Color, int>
            {
                [new Color(0, 255, 0)] = 2, // /|
                [new Color(255, 0, 0)] = 1, // |\    
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2
            };

            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/CastleTown_Clear").Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, TileRemoval);
            TileClear.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/CastleTown").Value;
            Texture2D SlopeMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/CastleTown_Slope").Value;
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, TileMap, WallMapping, null, null, SlopeMap, SlopeMapping);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            WorldGen.PlaceTile(x - (TileClear.width / 2) + 30, y - (TileClear.height) + 35, ModContent.TileType<CartSign>());

            WorldGen.PlaceTile(x - (TileClear.width / 2) + 41, y - (TileClear.height) + 31, ModContent.TileType<CartLamp>());

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

            Main.tile[x - (TileClear.width / 2) + 67, y - (TileClear.height) + 23].ClearTile();
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
            WorldGen.SlopeTile(x - (TileClear.width / 2) + 92, y + 26 - (TileClear.height), 1);

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