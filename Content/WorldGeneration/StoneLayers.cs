using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Content.Tiles.Underground;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class StoneLayers : ModWorld
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int RockIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Moss"));
            if (RockIndex != -1)
            {
                tasks.Insert(RockIndex + 1, new PassLegacy("Rock Layers", RockLayers));
            }

            int NestIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (NestIndex != -1)
            {
                tasks.Insert(NestIndex + 1, new PassLegacy("Nest Index", CrawlerNests));
            }
        }

        private void CrawlerNests(GenerationProgress progress)
        {
            progress.Message = "Creating Crawler Nests";

            // Generate Nests
            for (int i = 0; i < 32; i++)
            {
                int x = WorldGen.genRand.Next(600, Main.maxTilesX - 600);
                int y = WorldGen.genRand.Next(Main.maxTilesY - 400, Main.maxTilesY - 200);

                CrawlerNest nest = new CrawlerNest();
                Tile tile = Framing.GetTileSafely(x, y);

                while (!nest.Place(new Point(x, y), WorldGen.structures) && tile.type != ModContent.TileType<CrunchyStone>())
                {
                    x = WorldGen.genRand.Next(600, Main.maxTilesX - 600);
                    y = WorldGen.genRand.Next(Main.maxTilesY - 400, Main.maxTilesY - 200);

                    tile = Framing.GetTileSafely(x, y);
                }
            }
        }

        private void RockLayers(GenerationProgress progress)
        {
            progress.Message = "Creating Rock Layers";

            #region CrunchyRock
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (y > Main.maxTilesY - 400 && y < Main.maxTilesY - 200)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (tile.type == TileID.Stone)
                        {
                            tile.type = (ushort)ModContent.TileType<CrunchyStone>();
                        }
                    }
                }
            }
            #endregion
        }
    }

    public class CrawlerNest : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            if (!structures.CanPlace(new Rectangle(origin.X, origin.Y, 48, 36)))
            {
                return false;
            }

            #region Texture Mapping
            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(76, 70, 69)] = ModContent.TileType<CrunchyStone>(),
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(34, 31, 32)] = -2,
            };

            Texture2D TileMap = ModContent.GetTexture(AssetDirectory.WorldGen + "CrawlerNest");
            Texture2D LiquidMap = ModContent.GetTexture(AssetDirectory.WorldGen + "CrawlerNest_Liquids");

            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(TileMap, TileRemoval);
            TileClear.Generate(origin.X - (TileClear.width / 2), origin.Y - (TileClear.height / 2), true, true);

            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, null, null, LiquidMap);
            TileGen.Generate(origin.X - (TileGen.width / 2), origin.Y - (TileGen.height / 2), true, true);
            #endregion


            #region Miscellaneous 
            Main.tile[origin.X - (TileClear.width / 2) + 9, origin.Y - (TileClear.height / 2) + 13].halfBrick(true);
            Main.tile[origin.X - (TileClear.width / 2) + 17, origin.Y - (TileClear.height / 2) + 10].halfBrick(true);
            Main.tile[origin.X - (TileClear.width / 2) + 39, origin.Y - (TileClear.height / 2) + 13].halfBrick(true);

            ModUtils.PlaceObject(origin.X - (TileClear.width / 2) + 23, origin.Y - (TileClear.height / 2) + 23, (ushort)ModContent.TileType<RockEgg>());

            #endregion

            return true;
        }
    }
}