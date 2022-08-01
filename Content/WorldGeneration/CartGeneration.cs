using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Content.Tiles;
using OvermorrowMod.Content.Tiles.Carts;
using OvermorrowMod.Content.Tiles.DesertTemple;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class CartGeneration : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int DesertIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (DesertIndex != -1)
            {
                tasks.Insert(DesertIndex + 1, new PassLegacy("Cart Zones", GenerateCarts));
            }

            base.ModifyWorldGenTasks(tasks, ref totalWeight);
        }

        private void GenerateCarts(GenerationProgress progress, GameConfiguration config)
        {
            int[] TileBlacklist = { TileID.Trees, 43, 44, 226 };


            progress.Message = "Setting up Merchant Zones";

            // Places cart zones based on the size of the world
            for (int i = 0; i < 3; i++)
            {
                bool validArea = false;

                // Pick a random spot in the world that is in midair
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)(Main.worldSurface * 0.35f), (int)(Main.worldSurface * 0.4f));

                while (!validArea)
                {
                    // Loop downwards until we reach a solid tile
                    Tile tile = Framing.GetTileSafely(x, y);
                    while (!tile.HasTile)
                    {
                        y++;
                        tile = Framing.GetTileSafely(x, y);
                    }

                    // We have the tile but we want to check if its a grass block, if it isn't restart the process
                    if (tile.TileType == TileID.Grass && tile.WallType == WallID.None && tile.TileType != TileID.Trees && Main.tileSolid[tile.TileType])
                    {
                        validArea = true;
                    }
                    else
                    {
                        // Pick a random spot in the world that is in midair
                        x = WorldGen.genRand.Next(0, Main.maxTilesX);
                        y = WorldGen.genRand.Next((int)(Main.worldSurface * 0.35f), (int)(Main.worldSurface * 0.5f));
                    }
                }

                PlaceCart(x, y);
            }
        }

        public static void PlaceCart(int x, int y)
        {
            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(102, 57, 49)] = ModContent.TileType<AstroGray>(),
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2
            };

            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/CartZone").Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, TileRemoval);
            TileClear.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/CartZone").Value;
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            // This shit blows up if I try to do anything with anything I'm scared
            WorldGen.PlaceTile(x - (TileClear.width / 2), y - (TileClear.height) + 23, ModContent.TileType<CartSign>());
            WorldGen.PlaceTile(x - (TileClear.width / 2) + 12, y - (TileClear.height) + 19, ModContent.TileType<CartLamp>());
            ModContent.GetInstance<CartLampTE>().Place(x - (TileClear.width / 2) + 12, y - (TileClear.height) + 19);

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    WorldGen.PlaceTile(x - (TileClear.width / 2) + i, y - (TileClear.height) + 26 + j, TileID.Adamantite, false, true);
                }
            }
        }
    }
}