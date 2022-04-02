using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Content.Tiles;
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
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class StoneLayers : ModWorld
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int HiveIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Hives"));
            if (HiveIndex != -1)
            {
                tasks.Insert(HiveIndex + 1, new PassLegacy("Mine Index", MoleMines)); // Because the rocks get smoothed in later passes
            }

            int RockIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Moss"));
            if (RockIndex != -1)
            {
                tasks.Insert(RockIndex + 1, new PassLegacy("Rock Layers", RockLayers));
                //tasks.Insert(RockIndex + 2, new PassLegacy("Mine Index", MoleMines)); // Because the rocks get smoothed in later passes
            }

            int MicroIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (MicroIndex != -1)
            {
                tasks.Insert(MicroIndex + 1, new PassLegacy("Nest Index", CrawlerNests));
            }
        }

        // Store the actual ID of the tile when selecting an origin
        public static int TileSelected;
        private void MoleMines(GenerationProgress progress)
        {
            progress.Message = "Digging out Moleman Mines";
            int[] ValidTiles = { TileID.Mud, TileID.Stone, TileID.IceBlock, TileID.Granite, TileID.Sandstone, ModContent.TileType<CrunchyStone>() };

            for (int i = 0; i < 36; i++)
            {
                int x = WorldGen.genRand.Next(600, Main.maxTilesX - 600);
                int y = WorldGen.genRand.Next((int)(WorldGen.rockLayer - 50), Main.maxTilesY - 200);

                MoleMine mine = new MoleMine();

                // This shit doesn't actually check blocks properly
                Tile tile = Framing.GetTileSafely(x, y);
                while (!mine.Place(new Point(x, y), WorldGen.structures) && !tile.active() && !ValidTiles.Contains(tile.type))
                {
                    x = WorldGen.genRand.Next(600, Main.maxTilesX - 600);
                    y = WorldGen.genRand.Next((int)(WorldGen.rockLayer - 50), Main.maxTilesY - 200);

                    tile = Framing.GetTileSafely(x, y);
                }
            }
        }

        private void CrawlerNests(GenerationProgress progress)
        {
            progress.Message = "Creating Crawler Nests";

            for (int i = 0; i < 16; i++)
            {
                int x = WorldGen.genRand.Next(600, Main.maxTilesX - 600);
                int y = WorldGen.genRand.Next(WorldGen.lavaLine - 20, Main.maxTilesY - 200);

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
            progress.Message = "Generating Rock Layers";

            #region CrunchyRock
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (y > WorldGen.lavaLine - 50 && y < Main.maxTilesY)
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

            Texture2D TileMap = ModContent.GetTexture(AssetDirectory.WorldGen + "Textures/CrawlerNest");
            Texture2D LiquidMap = ModContent.GetTexture(AssetDirectory.WorldGen + "Textures/CrawlerNest_Liquids");

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

    public class MoleMine : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            #region Texture Mapping
            // This uses weighted probability, basically works by adding together the weights and returning an element in Get()
            // A MineType of 1 will have a 1 / (1 + 4 + 5) = 1 / 10, or 10% chance of being chosen
            WeightedRandom<int> RandomType = new WeightedRandom<int>(Main.rand);
            RandomType.Add(1, 5);
            RandomType.Add(2, 4);
            RandomType.Add(3, 1);
            int MineType = RandomType.Get();

            Vector2 Dimensions = Vector2.Zero;
            switch (MineType)
            {
                case 1:
                    Dimensions = new Vector2(32, 24);
                    break;
                case 2:
                    Dimensions = new Vector2(38, 42);
                    break;
                case 3:
                    Dimensions = new Vector2(51, 97);
                    break;
            }

            int StoneType = Main.tile[origin.X, origin.Y].type;
            int WoodType = TileID.WoodBlock;
            switch (StoneType)
            {
                case TileID.IceBlock:
                case TileID.Slush:
                case TileID.SnowBlock:
                    WoodType = TileID.BorealWood;
                    break;
                case TileID.HardenedSand:
                case TileID.Sandstone:
                case TileID.Sand:
                    WoodType = TileID.Sandstone;
                    break;
                case TileID.Mud:
                    WoodType = TileID.RichMahogany;
                    break;
                case TileID.Granite:
                    WoodType = TileID.GraniteBlock;
                    break;
                case TileID.Dirt: // This is here because it keeps choosing dirt as the stone type despite there being NO dirt
                    StoneType = TileID.Stone;
                    break;
            }

            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                //[new Color(94, 92, 89)] = ModContent.TileType<CrunchyStone>(),
                [new Color(94, 92, 89)] = StoneType,
                [new Color(100, 78, 59)] = WoodType,
                [new Color(80, 51, 41)] = TileID.Platforms,
                [new Color(90, 79, 62)] = TileID.Rope,
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(50, 41, 45)] = -2,
            };

            Texture2D ClearMap = ModContent.GetTexture(AssetDirectory.WorldGen + "Textures/MoleMine_" + MineType + "_Clear");
            Texture2D TileMap = ModContent.GetTexture(AssetDirectory.WorldGen + "Textures/MoleMine_" + MineType);

            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval);
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, null, null);

            #endregion

            #region Generation
            int x = origin.X - (TileClear.width / 2);
            int y = origin.Y - (TileGen.height / 2);

            if (!structures.CanPlace(new Rectangle(x, y, (int)Dimensions.X, (int)Dimensions.Y)))
            {
                return false;
            }

            TileClear.Generate(x, y, true, true);
            TileGen.Generate(x, y, true, true);

            #endregion

            #region Miscellaneous 
            // Place chests, modify slopes, and add pots based on the type of texture selected
            int chestIndex;
            switch (MineType)
            {
                case 1:
                    ModUtils.PlaceObject(x + 12, y + 9, TileID.Torches);
                    ModUtils.PlaceObject(x + 23, y + 12, TileID.Torches);

                    WorldGen.PlaceTile(x + 11, y + 13, TileID.Pots, true); ;

                    ModUtils.PlaceObject(x + 14, y + 13, ModContent.TileType<LargePot>(), WorldGen.genRand.Next(4));
                    break;
                case 2:
                    chestIndex = WorldGen.PlaceChest(x + 16, y + 30, style: 1);
                    Tier2ChestItems(chestIndex);

                    ModUtils.PlaceObject(x + 13, y + 13, TileID.Torches);
                    ModUtils.PlaceObject(x + 23, y + 22, TileID.Torches);
                    ModUtils.PlaceObject(x + 9, y + 33, TileID.Torches);

                    WorldGen.PlaceTile(x + 29, y + 14, TileID.Pots, true);
                    WorldGen.PlaceTile(x + 30, y + 25, TileID.Pots, true);
                    WorldGen.PlaceTile(x + 23, y + 30, TileID.Pots, true);

                    ModUtils.PlaceObject(x + 7, y + 14, ModContent.TileType<LargePot>(), WorldGen.genRand.Next(4));
                    ModUtils.PlaceObject(x + 26, y + 30, ModContent.TileType<LargePot>(), WorldGen.genRand.Next(4));
                    break;
                case 3:
                    chestIndex = WorldGen.PlaceChest(x + 29, y + 49, style: 1);
                    Tier1ChestItems(chestIndex);

                    ModUtils.PlaceObject(x + 23, y + 16, TileID.Torches);
                    ModUtils.PlaceObject(x + 34, y + 12, TileID.Torches);
                    ModUtils.PlaceObject(x + 8, y + 34, TileID.Torches);
                    ModUtils.PlaceObject(x + 23, y + 36, TileID.Torches);
                    ModUtils.PlaceObject(x + 23, y + 56, TileID.Torches);
                    ModUtils.PlaceObject(x + 37, y + 62, TileID.Torches);
                    ModUtils.PlaceObject(x + 39, y + 48, TileID.Torches);

                    WorldGen.PlaceTile(x + 12, y + 34, TileID.Pots, true);
                    WorldGen.PlaceTile(x + 12, y + 19, TileID.Pots, true);
                    WorldGen.PlaceTile(x + 36, y + 28, TileID.Pots, true);
                    WorldGen.PlaceTile(x + 35, y + 36, TileID.Pots, true);
                    WorldGen.PlaceTile(x + 24, y + 65, TileID.Pots, true);
                    WorldGen.PlaceTile(x + 48, y + 54, TileID.Pots, true);
                    WorldGen.PlaceTile(x + 40, y + 37, TileID.Pots, true);

                    ModUtils.PlaceObject(x + 32, y + 36, ModContent.TileType<LargePot>(), WorldGen.genRand.Next(4));
                    ModUtils.PlaceObject(x + 45, y + 54, ModContent.TileType<LargePot>(), WorldGen.genRand.Next(4));

                    //Main.tile[x + 30, y + 65].leftSlope();
                    //Main.tile[x + 31, y + 64].leftSlope();
                    //Main.tile[x + 32, y + 63].leftSlope();
                    //Main.tile[x + 33, y + 62].leftSlope();
                    //Main.tile[x + 34, y + 61].leftSlope();
                    //Main.tile[x + 35, y + 60].leftSlope();
                    //Main.tile[x + 36, y + 59].leftSlope();

                    break;
            }
            #endregion

            return true;
        }

        private void Tier1ChestItems(int chestIndex)
        {
            if (chestIndex != 1)
            {
                Chest chest = Main.chest[chestIndex];

                var itemsToAdd = new List<(int type, int stack)>();
                itemsToAdd.Add((ItemID.LavaCharm, 1));

                int specialItem = new WeightedRandom<int>(
                    Tuple.Create((int)ItemID.HermesBoots, 2.0),
                    Tuple.Create((int)ItemID.CloudinaBottle, 2.0),
                    Tuple.Create((int)ItemID.ShoeSpikes, 1.0),
                    Tuple.Create((int)ItemID.MagicMirror, 1.0),
                    Tuple.Create((int)ItemID.BandofRegeneration, 1.0)
                );

                itemsToAdd.Add((specialItem, 1));

                if (Main.rand.NextBool(3))
                {
                    itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(2, 5)));
                }

                if (Main.rand.NextBool(5))
                {
                    itemsToAdd.Add((ItemID.AngelStatue, 1));
                }

                if (Main.rand.NextBool(3))
                {
                    itemsToAdd.Add((ItemID.Rope, Main.rand.Next(100, 150)));
                }

                if (Main.rand.NextBool(2))
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            if (WorldGen.GoldTierOre == TileID.Gold)
                            {
                                itemsToAdd.Add((ItemID.GoldBar, Main.rand.Next(9, 15)));
                            }
                            else
                            {
                                itemsToAdd.Add((ItemID.PlatinumBar, Main.rand.Next(9, 15)));
                            }
                            break;
                        case 1:
                            if (WorldGen.SilverTierOre == TileID.Silver)
                            {
                                itemsToAdd.Add((ItemID.SilverBar, Main.rand.Next(9, 15)));
                            }
                            else
                            {
                                itemsToAdd.Add((ItemID.TungstenBar, Main.rand.Next(9, 15)));
                            }
                            break;
                    }
                }

                if (Main.rand.NextBool(2))
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.WoodenArrow, 50));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.Shuriken, 50));
                            break;
                    }
                }

                itemsToAdd.Add((ItemID.HealingPotion, Main.rand.Next(3, 6)));

                if (Main.rand.NextBool(2))
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(2, 3)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(2, 3)));
                            break;
                        case 2:
                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(2, 3)));
                            break;
                    }
                }

                itemsToAdd.Add((ItemID.Torch, Main.rand.Next(20, 30)));
                itemsToAdd.Add((ItemID.RecallPotion, Main.rand.Next(1, 3)));
                itemsToAdd.Add((ItemID.SilverCoin, Main.rand.Next(60, 90)));

                // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                int chestItemIndex = 0;
                foreach (var itemToAdd in itemsToAdd)
                {
                    Item item = new Item();
                    item.SetDefaults(itemToAdd.type);
                    item.stack = itemToAdd.stack;
                    chest.item[chestItemIndex] = item;
                    chestItemIndex++;
                    if (chestItemIndex >= 40)
                        break; // Make sure not to exceed the capacity of the chest
                }
            }
        }

        private void Tier2ChestItems(int chestIndex)
        {
            if (chestIndex != 1)
            {
                Chest chest = Main.chest[chestIndex];

                var itemsToAdd = new List<(int type, int stack)>();
                int specialItem = new WeightedRandom<int>(
                    Tuple.Create((int)ItemID.HermesBoots, 2.0),
                    Tuple.Create((int)ItemID.CloudinaBottle, 2.0),
                    Tuple.Create((int)ItemID.ShoeSpikes, 1.0),
                    Tuple.Create((int)ItemID.MagicMirror, 1.0),
                    Tuple.Create((int)ItemID.BandofRegeneration, 1.0)
                );

                itemsToAdd.Add((specialItem, 1));

                if (Main.rand.NextBool(3))
                {
                    itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(2, 5)));
                }

                if (Main.rand.NextBool(5))
                {
                    itemsToAdd.Add((ItemID.AngelStatue, 1));
                }

                if (Main.rand.NextBool(3))
                {
                    itemsToAdd.Add((ItemID.Rope, Main.rand.Next(100, 150)));
                }

                if (Main.rand.NextBool(2))
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            if (WorldGen.GoldTierOre == TileID.Gold)
                            {
                                itemsToAdd.Add((ItemID.GoldBar, Main.rand.Next(9, 15)));
                            }
                            else
                            {
                                itemsToAdd.Add((ItemID.PlatinumBar, Main.rand.Next(9, 15)));
                            }
                            break;
                        case 1:
                            if (WorldGen.SilverTierOre == TileID.Silver)
                            {
                                itemsToAdd.Add((ItemID.SilverBar, Main.rand.Next(9, 15)));
                            }
                            else
                            {
                                itemsToAdd.Add((ItemID.TungstenBar, Main.rand.Next(9, 15)));
                            }
                            break;
                    }
                }

                if (Main.rand.NextBool(2))
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.WoodenArrow, 50));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.Shuriken, 50));
                            break;
                    }
                }

                itemsToAdd.Add((ItemID.HealingPotion, Main.rand.Next(3, 6)));

                if (Main.rand.NextBool(2))
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(2, 3)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(2, 3)));
                            break;
                        case 2:
                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(2, 3)));
                            break;
                    }
                }

                itemsToAdd.Add((ItemID.Torch, Main.rand.Next(20, 30)));
                itemsToAdd.Add((ItemID.RecallPotion, Main.rand.Next(1, 3)));
                itemsToAdd.Add((ItemID.SilverCoin, Main.rand.Next(60, 90)));

                // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                int chestItemIndex = 0;
                foreach (var itemToAdd in itemsToAdd)
                {
                    Item item = new Item();
                    item.SetDefaults(itemToAdd.type);
                    item.stack = itemToAdd.stack;
                    chest.item[chestItemIndex] = item;
                    chestItemIndex++;
                    if (chestItemIndex >= 40)
                        break; // Make sure not to exceed the capacity of the chest
                }
            }
        }
    }
}