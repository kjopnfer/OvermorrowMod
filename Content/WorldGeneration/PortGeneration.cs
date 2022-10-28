using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Content.Tiles.Town;
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
    public class PortGeneration : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int WaterChestIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Water Chests"));
            if (WaterChestIndex != -1)
            {
                tasks.Insert(WaterChestIndex + 1, new PassLegacy("Ocean Port", GeneratePort));
            }
        }

        private void GeneratePort(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Creating the Port";

            int x = 100;
            int y = (int)(Main.worldSurface * 0.35f);

            Tile tile = Framing.GetTileSafely(x, y);

            bool validTile = false;
            while (!validTile) // Loop downward until the surface of the ocean is found
            {
                if (tile.LiquidType == LiquidID.Water && tile.LiquidAmount == byte.MaxValue)
                {
                    y--;
                    validTile = true;

                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            WorldGen.PlaceTile(x + i, y + j, TileID.ObsidianBrick);
                        }
                    }
                }
                else
                {
                    y++;
                    tile = Framing.GetTileSafely(x, y);

                    //WorldGen.PlaceTile(x, y, TileID.ObsidianBrick);
                }
            }

            validTile = false;
            while (!validTile) // Loop to the right until the beach is found
            {
                if (tile.HasTile && tile.TileType == TileID.Sand && tile.LiquidAmount <= 0)
                {
                    validTile = true;                    
                }
                else
                {
                    x++;
                    tile = Framing.GetTileSafely(x, y);

                    //WorldGen.PlaceTile(x, y, TileID.ObsidianBrick);
                }
            }

            WorldGen.PlaceTile(x, y, TileID.Adamantite, false, true);

            // the names so nice you say it twice
            // why the FUCK do i have to generate this twice to make it even show up in the world
            for (int _ = 0; _ < 2; _++)
            {
                PlacePort(x + 45, y + 10);
            }
        }

        public static void PlacePort(int x, int y)
        {
            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(50, 22, 3)] = TileID.WoodenBeam,
                [new Color(197, 130, 57)] = ModContent.TileType<CastleRoof>(),
                [new Color(115, 78, 48)] = TileID.WoodBlock,
                [new Color(248, 209, 118)] = TileID.Sand,
                [new Color(105, 99, 94)] = ModContent.TileType<CastleBrick>(),
                [new Color(67, 65, 64)] = ModContent.TileType<DarkCastleBrick>(),
            };

            Dictionary<Color, int> WallMapping = new Dictionary<Color, int>
            {
                [new Color(73, 64, 56)] = ModContent.WallType<CastleWall>()
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2
            };

            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/PortArena_Clear").Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, TileRemoval);
            //TileClear.Generate(x - TileClear.width, y - (TileClear.height / 2), true, true);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/PortArena").Value;
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, TileMap, WallMapping, null, null);
            TileGen.Generate(x - TileGen.width, y - TileGen.height, true, true);

            WorldGen.PlaceTile(x - TileGen.width, y - TileGen.height, TileID.Adamantite, false, true);
        }
    }
}