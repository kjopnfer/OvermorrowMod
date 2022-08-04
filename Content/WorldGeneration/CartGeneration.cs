using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Base;
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
            int BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Gems"));
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
                [new Color(105, 99, 94)] = TileID.GrayBrick,
                [new Color(117, 70, 46)] = TileID.RedDynastyShingles,
                [new Color(93, 70, 52)] = TileID.Platforms,
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

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2
            };

            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/CastleTown_Clear").Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, TileRemoval);
            TileClear.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/CastleTown").Value;
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, TileMap, WallMapping);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true, true);

            // This shit blows up if I try to do anything with anything I'm scared
            /*WorldGen.PlaceTile(x - (TileClear.width / 2), y - (TileClear.height) + 23, ModContent.TileType<CartSign>());
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 12, y - (TileClear.height) + 19, ModContent.TileType<CartLamp>());
            ModContent.GetInstance<CartLampTE>().Place(x - (TileClear.width / 2) + 12, y - (TileClear.height) + 19);

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