using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Tiles;
using OvermorrowMod.Tiles.Ambient.WaterCave;
using OvermorrowMod.Tiles.Block;
using OvermorrowMod.Tiles.TrapOre;
using OvermorrowMod.WardenClass.Accessories;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.NPCs.Bosses.Goblin;

namespace OvermorrowMod
{
    public class OvermorrowWorld : ModWorld
    {
        // Bosses
        public static bool downedDarude;
        public static bool downedTree;
        public static bool downedDrippler;
        public static bool downedDrake;
        public static bool downedLady;
        public static bool downedKnight;

        // Boss Attacks
        public static bool DripplerCircle;
        public static bool DripladShoot = false;
        public static bool dripPhase2;
        public static bool dripPhase3;
        public static int loomingdripplerdeadcount;
        public static int RotatingDripladAttackCounter;

        // Biomes
        public static int floodedCaves;

        // These are here because we can't have nice things
        public static int marbleBiome;
        public static int graniteBiome;

        private bool placedBook = false;
        private bool placedwep = false;
        private bool placedtele = false;



        public override void Initialize()
        {
            downedTree = false;
            downedDarude = false;
            downedDrippler = false;
            downedDrake = false;
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            floodedCaves = tileCounts[ModContent.TileType<GlowBlock>()];
            marbleBiome = tileCounts[TileID.MarbleBlock];
            graniteBiome = tileCounts[TileID.GraniteBlock];
        }

        public override TagCompound Save()
        {
            var downed = new List<string>();

            if (downedTree)
            {
                downed.Add("Iorich");
            }

            if (downedDarude)
            {
                downed.Add("Dharuud");
            }

            if (downedDrippler)
            {
                downed.Add("Dripplord");
            }

            if (downedDrake)
            {
                downed.Add("Storm Drake");
            }


            return new TagCompound
            {
                ["downed"] = downed
            };

        }

        public override void Load(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            downedTree = downed.Contains("Iorich");
            downedDarude = downed.Contains("Dharuud");
            downedDrippler = downed.Contains("Dripplord");
            downedDrake = downed.Contains("Storm Drake");
        }

        public override void LoadLegacy(BinaryReader reader)
        {
            int loadVersion = reader.ReadInt32();
            if (loadVersion == 0)
            {
                BitsByte flags = reader.ReadByte();
                downedTree = flags[0];
                downedDarude = flags[1];
                downedDrippler = flags[2];
                downedDrake = flags[3];
            }
            else
            {
                mod.Logger.WarnFormat("Overmorrow: Unknown loadVersion: {0}", loadVersion);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedTree;
            flags[1] = downedDarude;
            flags[2] = downedDrippler;
            flags[3] = downedDrake;

            writer.Write(flags);

            /*
      			Remember that Bytes/BitsByte only have 8 entries. If you have more than 8 flags you want to sync, use multiple BitsByte:
      				This is wrong:
      			flags[8] = downed9thBoss; // an index of 8 is nonsense.
      				This is correct:
      			flags[7] = downed8thBoss;
      			writer.Write(flags);
      			BitsByte flags2 = new BitsByte(); // create another BitsByte
      			flags2[0] = downed9thBoss; // start again from 0
      			// up to 7 more flags here
      			writer.Write(flags2); // write this byte
      			*/

            //If you prefer, you can use the BitsByte constructor approach as well.
            //writer.Write(saveVersion);
            //BitsByte flags = new BitsByte(downedAbomination, downedPuritySpirit);
            //writer.Write(flags);

            // This is another way to do the same thing, but with bitmasks and the bitwise OR assignment operator (the |=)
            // Note that 1 and 2 here are bit masks. The next values in the pattern are 4,8,16,32,64,128. If you require more than 8 flags, make another byte.
            //writer.Write(saveVersion);
            //byte flags = 0;
            //if (downedAbomination)
            //{
            //	flags |= 1;
            //}
            //if (downedPuritySpirit)
            //{
            //	flags |= 2;
            //}
            //writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedTree = flags[0];
            downedDarude = flags[1];
            downedDrippler = flags[2];
            downedDrake = flags[3];

            // As mentioned in NetSend, BitBytes can contain 8 values. If you have more, be sure to read the additional data:
            // BitsByte flags2 = reader.ReadByte();
            // downed9thBoss = flags[0];
        }

        public override void PostWorldGen()
        {
            // Place items in Gold Chests
            int[] itemsToPlaceInDungeonChests = { ModContent.ItemType<ReaperBook>() };
            int itemsToPlaceInDungeonChestsChoice = 0;
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                // If you look at the sprite for Chests by extracting Tiles_21.xnb, you'll see that the 12th chest is the Ice Chest. Since we are counting from 0, this is where 11 comes from. 36 comes from the width of each tile including padding. 
                // For a Locked Dungeon Chest, it is the 3rd chest therefore the value will be 2.
                if (chest != null && Main.tile[chest.x, chest.y].type == TileID.Containers && Main.tile[chest.x, chest.y].frameX == 2 * 36)
                {
                    if (!placedBook) // Guarantees at least one book in a Dungeon Chest
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (inventoryIndex == 0)
                            {
                                chest.item[inventoryIndex].SetDefaults(itemsToPlaceInDungeonChests[itemsToPlaceInDungeonChestsChoice]);
                                itemsToPlaceInDungeonChestsChoice = (itemsToPlaceInDungeonChestsChoice + 1) % itemsToPlaceInDungeonChests.Length;
                                // Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(Main.rand.Next(itemsToPlaceInIceChests));
                                break;
                            }
                        }
                        placedBook = true;
                    }
                    else
                    {
                        if (Main.rand.Next(5) == 0)
                        {
                            for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                            {
                                if (inventoryIndex == 1)
                                {
                                    chest.item[inventoryIndex].SetDefaults(itemsToPlaceInDungeonChests[itemsToPlaceInDungeonChestsChoice]);
                                    itemsToPlaceInDungeonChestsChoice = (itemsToPlaceInDungeonChestsChoice + 1) % itemsToPlaceInDungeonChests.Length;
                                    // Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(Main.rand.Next(itemsToPlaceInIceChests));
                                    break;
                                }
                            }
                        }
                    }
                }



                int[] itemsToPlaceInGranChests = { ModContent.ItemType<GraniteChomper>() };
                int itemsToPlaceInGranChestsChoice = 0;
                if (chest != null && Main.tile[chest.x, chest.y].type == TileID.Containers && Main.tile[chest.x, chest.y].frameX == 50 * 36)
                {
                    if (!placedwep) // Guarantees at least one book in a Dungeon Chest
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (inventoryIndex == 1)
                            {
                                chest.item[inventoryIndex].SetDefaults(itemsToPlaceInGranChests[itemsToPlaceInGranChestsChoice]);
                                itemsToPlaceInGranChestsChoice = (itemsToPlaceInGranChestsChoice + 1) % itemsToPlaceInGranChests.Length;
                                // Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(Main.rand.Next(itemsToPlaceInIceChests));
                                break;
                            }
                        }
                        placedwep = true;
                    }
                    else
                    {
                        if (Main.rand.Next(3) == 1)
                        {
                            for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                            {
                                if (inventoryIndex == 1)
                                {
                                    chest.item[inventoryIndex].SetDefaults(itemsToPlaceInGranChests[itemsToPlaceInGranChestsChoice]);
                                    itemsToPlaceInGranChestsChoice = (itemsToPlaceInGranChestsChoice + 1) % itemsToPlaceInGranChests.Length;
                                    // Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(Main.rand.Next(itemsToPlaceInIceChests));
                                    break;
                                }
                            }
                        }
                    }
                }





                int[] itemsToPlaceInMarbChests = { ModContent.ItemType<WarpRocket>() };
                int itemsToPlaceInGranMarbleChoice = 0;
                if (chest != null && Main.tile[chest.x, chest.y].type == TileID.Containers && Main.tile[chest.x, chest.y].frameX == 51 * 36)
                {
                    if (!placedtele) // Guarantees at least one book in a Dungeon Chest
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (inventoryIndex == 1)
                            {
                                chest.item[inventoryIndex].SetDefaults(itemsToPlaceInMarbChests[itemsToPlaceInGranMarbleChoice]);
                                itemsToPlaceInGranMarbleChoice = (itemsToPlaceInGranMarbleChoice + 1) % itemsToPlaceInMarbChests.Length;
                                // Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(Main.rand.Next(itemsToPlaceInIceChests));
                                break;
                            }
                        }
                        placedtele = true;
                    }
                    else
                    {
                        if (Main.rand.Next(2) == 1)
                        {
                            for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                            {
                                if (inventoryIndex == 1)
                                {
                                    chest.item[inventoryIndex].SetDefaults(itemsToPlaceInMarbChests[itemsToPlaceInGranMarbleChoice]);
                                    itemsToPlaceInGranMarbleChoice = (itemsToPlaceInGranMarbleChoice + 1) % itemsToPlaceInMarbChests.Length;
                                    // Alternate approach: Random instead of cyclical: chest.item[inventoryIndex].SetDefaults(Main.rand.Next(itemsToPlaceInIceChests));
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }

        // Worldgen Debugging
        public static bool JustPressed(Keys key)
        {
            return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
        }

        public override void PostUpdate()
        {
            /*if (JustPressed(Keys.D1))
            {
                TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            }*/
        }

        private void TestMethod(int x, int y)
        {
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
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
                tasks.Insert(WetJungle + 1, new PassLegacy("WaterCaveGeneration", WaterCaveFinder));
            }



            int TowerS = tasks.FindIndex(genpass => genpass.Name.Equals("Spawn Point"));
            if (TowerS != -1)
            {
                tasks.Insert(TowerS + 1, new PassLegacy("NONONONO", TowerStart));
            }




            int TempleS = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (TempleS != -1)
            {
                tasks.Insert(TempleS + 1, new PassLegacy("Mddd", TempleStart));
            }

        }

        bool placedtower = false;
        private void TempleStart(GenerationProgress progress)
        {
            progress.Message = "Generating Sky Ships";
            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 0.000001); k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next(100, (int)Main.maxTilesY / 7);

                //bottom
                for (int i = 0; i < 11; i++)
                {
                    WorldGen.PlaceTile(x + i, y, 311);
                }

                for (int i = 0; i < 9; i++)
                {
                    WorldGen.PlaceTile(x + 10 + i, y - 1, 311);
                }

                WorldGen.PlaceTile(x + 1, y - 1, 376);
                WorldGen.PlaceTile(x + 10, y - 2, 376);
                WorldGen.PlaceTile(x + 12, y - 2, 376);
                WorldGen.PlaceTile(x + 14, y - 2, 376);
                WorldGen.PlaceTile(x + 16, y - 2, 376);

                //front
                WorldGen.PlaceTile(x + 18, y - 2, 311);
                WorldGen.PlaceTile(x + 19, y - 2, 311);
                WorldGen.PlaceTile(x + 19, y - 3, 311);
                WorldGen.PlaceTile(x + 20, y - 3, 311);
                WorldGen.PlaceTile(x + 20, y - 4, 311);
                WorldGen.PlaceTile(x + 21, y - 4, 311);
                WorldGen.PlaceTile(x + 21, y - 5, 311);
                WorldGen.PlaceTile(x + 22, y - 5, 311);
                WorldGen.PlaceTile(x + 22, y - 6, 311);
                WorldGen.PlaceTile(x + 23, y - 6, 311);
                WorldGen.PlaceTile(x + 23, y - 7, 311);
                WorldGen.PlaceTile(x + 24, y - 7, 311);


                //back
                for (int i = 0; i < 7; i++)
                {
                    WorldGen.PlaceTile(x, y - i, 311);
                }


                //top
                for (int i = 0; i < 4; i++)
                {
                    WorldGen.PlaceTile(x + i, y - 7, 311);
                }

                for (int i = 0; i < 3; i++)
                {
                    WorldGen.PlaceTile(x + i + 4, y - 7, 19);
                }

                for (int i = 0; i < 6; i++)
                {
                    WorldGen.PlaceTile(x + 5, y - i - 1, 213);
                }

                for (int i = 0; i < 18; i++)
                {
                    WorldGen.PlaceTile(x + i + 7, y - 7, 311);
                }



                //sail
                for (int i = 0; i < 9; i++)
                {
                    WorldGen.PlaceWall(x + 12, y - 8 - i, 106);
                }

                for (int j = 0; j < 5; j++)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        WorldGen.PlaceWall(x + 8 + i, y - 12 - j, 148);
                    }
                }


                //walls
                for (int i = 0; i < 21; i++)
                {
                    WorldGen.PlaceWall(x + i + 1, y - 6, 42);
                }

                for (int i = 0; i < 20; i++)
                {
                    WorldGen.PlaceWall(x + i + 1, y - 5, 42);
                }

                for (int i = 0; i < 19; i++)
                {
                    WorldGen.PlaceWall(x + i + 1, y - 4, 42);
                }

                for (int i = 0; i < 18; i++)
                {
                    WorldGen.PlaceWall(x + i + 1, y - 3, 42);
                }

                for (int i = 0; i < 17; i++)
                {
                    WorldGen.PlaceWall(x + i + 1, y - 2, 42);
                }

                for (int i = 0; i < 9; i++)
                {
                    WorldGen.PlaceWall(x + i + 1, y - 1, 42);
                }
            }










            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 0.0000008); k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next(125, (int)Main.maxTilesY / 7);



                int wallypt = 42;
                int tileypt = 311;


                //bottom
                for (int i = 0; i < 18; i++)
                {
                    WorldGen.PlaceTile(x - i - 6, y, tileypt);
                }


                //front
                for (int i = 0; i < 3; i++)
                {
                    WorldGen.PlaceTile(x - 23 - i, y - 1, tileypt);
                }

                for (int i = 0; i < 2; i++)
                {
                    WorldGen.PlaceTile(x - 25, y - 2 - i, tileypt);
                }

                for (int i = 0; i < 3; i++)
                {
                    WorldGen.PlaceTile(x - 26, y - 3 - i, tileypt);
                }


                //back
                for (int i = 0; i < 3; i++)
                {
                    WorldGen.PlaceTile(x - 6 + i, y - 1, tileypt);
                }

                for (int i = 0; i < 2; i++)
                {
                    WorldGen.PlaceTile(x - 4, y - 2 - i, tileypt);
                }

                for (int i = 0; i < 3; i++)
                {
                    WorldGen.PlaceTile(x - 3, y - 3 - i, tileypt);
                }



                //top
                for (int i = 0; i < 6; i++)
                {
                    WorldGen.PlaceTile(x - 23 - i, y - 6, tileypt);
                }

                for (int i = 0; i < 5; i++)
                {
                    WorldGen.PlaceTile(x - 25 - i, y - 7, tileypt);
                }


                for (int i = 0; i < 5; i++)
                {
                    WorldGen.PlaceTile(x - 18 - i, y - 6, 19);
                }


                for (int i = 0; i < 17; i++)
                {
                    WorldGen.PlaceTile(x - 1 - i, y - 6, tileypt);
                }


                for (int i = 0; i < 2; i++)
                {
                    WorldGen.PlaceTile(x - 12, y - 5 + i, tileypt);
                }

                WorldGen.PlaceTile(x - 12, y - 1, 10);

                for (int i = 0; i < 9; i++)
                {
                    WorldGen.PlaceTile(x - 35 + i, y - 8, tileypt);
                }




                //Room
                for (int i = 0; i < 5; i++)
                {
                    WorldGen.PlaceTile(x - 1, y - 7 - i, tileypt);
                }

                for (int i = 0; i < 14; i++)
                {
                    WorldGen.PlaceTile(x - i, y - 12, tileypt);
                }

                for (int i = 0; i < 2; i++)
                {
                    WorldGen.PlaceTile(x - 12, y - 11 + i, tileypt);
                }

                WorldGen.PlaceTile(x - 12, y - 7, 10);






                //walls
                for (int i = 0; i < 16; i++)
                {
                    WorldGen.PlaceWall(x - i - 7, y - 1, wallypt);
                }

                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        WorldGen.PlaceWall(x - i - 5, y - 2 - j, wallypt);
                    }
                }

                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 22; i++)
                    {
                        WorldGen.PlaceWall(x - i - 4, y - 4 - j, wallypt);
                    }
                }

                for (int j = 0; j < 5; j++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        WorldGen.PlaceWall(x - i - 2, y - 7 - j, wallypt);
                    }
                }



                //sail
                WorldGen.PlaceTile(x - 17, y - 25, tileypt);
                WorldGen.PlaceTile(x - 16, y - 25, tileypt);

                WorldGen.PlaceTile(x - 17, y - 20, tileypt);
                WorldGen.PlaceTile(x - 16, y - 20, tileypt);

                for (int i = 0; i < 7; i++)
                {
                    WorldGen.PlaceTile(x - 6, y - 13 - i, 214);
                }

                WorldGen.PlaceTile(x - 6, y - 20, tileypt);
                WorldGen.PlaceTile(x - 5, y - 20, 19);
                WorldGen.PlaceTile(x - 4, y - 20, 19);

                for (int i = 0; i < 11; i++)
                {
                    WorldGen.PlaceTile(x - 27, y - 9 - i, 214);
                }

                WorldGen.PlaceTile(x - 27, y - 20, tileypt);
                WorldGen.PlaceTile(x - 28, y - 20, 19);
                WorldGen.PlaceTile(x - 29, y - 20, 19);

                for (int i = 0; i < 11; i++)
                {
                    WorldGen.PlaceTile(x - 27, y - 9 - i, 214);
                }

                for (int i = 0; i < 20; i++)
                {
                    WorldGen.PlaceTile(x - 7 - i, y - 20, 19);
                }


                for (int i = 0; i < 20; i++)
                {
                    WorldGen.PlaceTile(x - 7 - i, y - 25, 19);
                }

                for (int i = 0; i < 10; i++)
                {
                    WorldGen.PlaceWall(x - 17, y - 18 - i, 4);
                    WorldGen.PlaceWall(x - 16, y - 18 - i, 4);
                }

                for (int i = 0; i < 8; i++)
                {
                    WorldGen.PlaceWall(x - 5, y - 19 - i, 148);
                }

                for (int i = 0; i < 8; i++)
                {
                    WorldGen.PlaceWall(x - 29, y - 19 - i, 148);
                }

                for (int i = 0; i < 6; i++)
                {
                    WorldGen.PlaceWall(x - 4, y - 20 - i, 148);
                }

                for (int i = 0; i < 6; i++)
                {
                    WorldGen.PlaceWall(x - 30, y - 20 - i, 148);
                }

                for (int i = 0; i < 10; i++)
                {
                    WorldGen.PlaceWall(x - 6, y - 18 - i, 4);
                }

                for (int i = 0; i < 10; i++)
                {
                    WorldGen.PlaceWall(x - 28, y - 18 - i, 4);
                }

                for (int j = 0; j < 10; j++)
                {
                    for (int i = 0; i < 22; i++)
                    {
                        WorldGen.PlaceWall(x - i - 6, y - 18 - j, 148);
                    }
                }


                WorldGen.PlaceTile(x - 6, y - 25, tileypt);
                WorldGen.PlaceTile(x - 5, y - 25, 19);
                WorldGen.PlaceTile(x - 4, y - 25, 19);

                WorldGen.PlaceTile(x - 27, y - 25, tileypt);
                WorldGen.PlaceTile(x - 28, y - 25, 19);
                WorldGen.PlaceTile(x - 29, y - 25, 19);


                //objects
                WorldGen.PlaceTile(x - 16, y - 3, 240, style: 47);

                WorldGen.PlaceTile(x - 20, y - 1, 376);
                WorldGen.PlaceTile(x - 22, y - 1, 376);
                WorldGen.PlaceTile(x - 26, y - 2, 376);
                WorldGen.PlaceTile(x - 24, y - 2, 376);


                WorldGen.PlaceTile(x - 6, y - 2, 376, style: Main.rand.Next(10));

                WorldGen.PlaceTile(x - 6, y - 7, 21, style: 28);
            }


        }


            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY)); k++)
            {
                if (!placedtower)
                {
                    int x = WorldGen.genRand.Next(380, Main.maxTilesX - 380);
                    int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, (int)WorldGen.rockLayer + Main.maxTilesY / 12);

                    int randY = Main.rand.Next(10, 20);
                    int randX = Main.rand.Next(14, 15);



                    for (int fuckyou = 0; fuckyou < 150; fuckyou++)
                    {
                        Tile tile = Framing.GetTileSafely(x, y);
                        if (tile.active() && !WorldGen.SolidTile(x + 1, y - 1 - fuckyou) && tile.type == 53 || tile.active() && !WorldGen.SolidTile(x + 1, y - 1 - fuckyou) && tile.type == 2 || tile.active() && !WorldGen.SolidTile(x + 1, y - 1 - fuckyou) && tile.type == 60 || tile.active() && !WorldGen.SolidTile(x + 1, y - 1 - fuckyou) && tile.type == 147 || tile.active() && !WorldGen.SolidTile(x + 1, y - 1 - fuckyou) && tile.type == 2 || tile.active() && !WorldGen.SolidTile(x + 1, y - 1 - fuckyou) && tile.type == 60 || tile.active() && !WorldGen.SolidTile(x + 1, y - 1 - fuckyou) && tile.type == 60 || tile.active() && !WorldGen.SolidTile(x + 1, y - 1 - fuckyou) && tile.type == 23 || tile.active() && !WorldGen.SolidTile(x + 1, y - 1 - fuckyou) && tile.type == 199)
                        {

                            for (int j = 0; j < randY + 3; j++)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    WorldGen.KillTile(x - 4 + i, y - j);
                                }
                            }


                            for (int j = 0; j < randY + 12; j++)
                            {
                                for (int i = 0; i < 39; i++)
                                {
                                    WorldGen.KillTile(x - 19 + i, y - randY - j);
                                }
                            }

                            for (int i = 0; i < randY; i++)
                            {
                                WorldGen.PlaceTile(x - 3, y - i - 4, 325);
                            }

                            for (int i = 0; i < randY; i++)
                            {
                                WorldGen.PlaceTile(x + 3, y - i - 4, 325);
                            }

                            for (int i = 0; i < 6; i++)
                            {
                                WorldGen.PlaceTile(x - 3 + i, y, 53);
                            }

                            for (int i = 0; i < randY + 4; i++)
                            {
                                WorldGen.PlaceTile(x, y - i, 213);
                            }


                            WorldGen.PlaceTile(x - 3, y - 1, 10);
                            WorldGen.PlaceTile(x + 3, y - 1, 10);

                        NPC.NewNPC(x * 16, (y - randY - 8) * 16, ModContent.NPCType<WarningBossG>());

                            for (int j = 0; j < 5; j++)
                            {
                                for (int i = 0; i < randY + 6; i++)
                                {
                                    WorldGen.PlaceWall(x + j - 2, y - i - 4, 4);
                                }
                            }


                            for (int i = 0; i < randX; i++)
                            {
                                WorldGen.PlaceTile(x + i + 3, y - randY - 4, 325);
                                WorldGen.PlaceTile(x - i - 3, y - randY - 4, 325);
                            }

                            for (int i = 0; i < 7; i++)
                            {
                                WorldGen.PlaceTile(x + randX + 3, y - i - randY - 4, 325);
                                WorldGen.PlaceTile(x - randX - 3, y - i - randY - 4, 325);
                            }


                            for (int i = 0; i < 6; i++)
                            {
                                WorldGen.PlaceTile(x - 3 + i, y - randY - 4, 19);
                            }



                            for (int i = 0; i < 7; i++)
                            {
                                WorldGen.PlaceTile(x + randX + 3 - i, y - randY - 11 - i, 325);
                                WorldGen.PlaceTile(x - randX - 3 + i, y - randY - 11 - i, 325);

                                WorldGen.PlaceTile(x + randX + 2 - i, y - randY - 11 - i, 325);
                                WorldGen.PlaceTile(x - randX - 2 + i, y - randY - 11 - i, 325);

                            }

                            for (int i = 0; i < 10; i++)
                            {
                                WorldGen.PlaceTile(x - i, y - randY - 17, 325);
                                WorldGen.PlaceTile(x + i, y - randY - 17, 325);
                            }


                            for (int j = 0; j < 6; j++)
                            {
                                for (int i = 0; i < 33; i++)
                                {
                                    WorldGen.PlaceWall(x - randX - 2 + i, y - randY - 5 - j, 4);
                                }
                            }





                            for (int i = 0; i < 33 - 2; i++)
                            {
                                WorldGen.PlaceWall(x - randX + i - 1, y - randY - 11, 4);
                            }

                            for (int i = 0; i < 31 - 2; i++)
                            {
                                WorldGen.PlaceWall(x - randX + i - 0, y - randY - 12, 4);
                            }


                            for (int i = 0; i < 29 - 2; i++)
                            {
                                WorldGen.PlaceWall(x - randX + i + 1, y - randY - 13, 4);
                            }

                            for (int i = 0; i < 27 - 2; i++)
                            {
                                WorldGen.PlaceWall(x - randX + i + 2, y - randY - 14, 4);
                            }

                            for (int i = 0; i < 25 - 2; i++)
                            {
                                WorldGen.PlaceWall(x - randX + i + 3, y - randY - 15, 4);
                            }

                            for (int i = 0; i < 23 - 2; i++)
                            {
                                WorldGen.PlaceWall(x - randX + i + 4, y - randY - 16, 4);
                            }
                            placedtower = true;
                        }
                    }
                }
            }
        }










        int randSize = Main.rand.Next(140, 150);
        bool notInvalid = true;
        bool notNear = true;
        private void WaterCaveFinder(GenerationProgress progress)
        {
            progress.Message = "Flooding the Jungle";

            // small world size is 4200x1200 , medium multiplies every axis by 1.5 , and large multiplies every axis by 2.0

            int x = WorldGen.genRand.Next(600, Main.maxTilesX - 500);
            int y = WorldGen.genRand.Next((int)WorldGen.rockLayer + 100, (Main.maxTilesY == 2400) ? WorldGen.lavaLine - 300 : WorldGen.lavaLine - 150);

            while (true)
            {
                // We want to be in the jungle
                if (Main.tile[x, y].type != TileID.JungleGrass)
                {
                    notInvalid = false;
                }

                // While loop to find a valid tile
                if (!Main.tile[x, y].active() || Main.tile[x, y].type == TileID.SnowBlock || Main.tile[x, y].type == TileID.IceBlock || Main.tile[x, y].type == TileID.BlueDungeonBrick ||
                    Main.tile[x, y].type == TileID.Sand || Main.tile[x, y].type == TileID.HardenedSand || Main.tile[x, y].type == TileID.GreenDungeonBrick || Main.tile[x, y].type == TileID.PinkDungeonBrick)
                {
                    notInvalid = false;
                }

                int[] TileBlacklist = { 41, 43, 44, 226 };

                int xCoord = x;
                int yCoord = y;

                int xAxis = xCoord;
                int yAxis = yCoord;

                // For loop to check the Jungle Temple, or The Dungeon with a distance of randSize + 10
                for (int j = 0; j < randSize + 10; j++)
                {
                    //int fade = 0;
                    yAxis++;
                    xAxis = xCoord;
                    // Draws bottom right quadrant
                    for (int i = 0; i < randSize - j; i++)
                    {
                        xAxis++;
                        if (Main.tile[xAxis, yAxis] != null)
                        {
                            if (TileBlacklist.Contains(Main.tile[xAxis, yAxis].type))
                            {
                                notNear = false;
                            }
                        }
                    }

                    // Reset the xAxis, offset by 1 to fill in the gap
                    xAxis = xCoord + 1;

                    // Draws bottom left quadrant
                    for (int i = 0; i < randSize - j; i++)
                    {
                        xAxis--;
                        if (Main.tile[xAxis, yAxis] != null)
                        {
                            if (TileBlacklist.Contains(Main.tile[xAxis, yAxis].type))
                            {
                                notNear = false;
                            }
                        }
                    }
                }


                // Reset the y axis, offset by 1 to fill in the gap
                yAxis = yCoord + 1;

                for (int j = 0; j < randSize + 10; j++)
                {
                    yAxis--;
                    xAxis = xCoord;

                    // Draws top right quadrant
                    for (int i = 0; i < randSize - j; i++)
                    {
                        xAxis++;
                        if (Main.tile[xAxis, yAxis] != null)
                        {
                            if (TileBlacklist.Contains(Main.tile[xAxis, yAxis].type))
                            {
                                notNear = false;
                            }
                        }
                    }

                    // Reset the xAxis, offset by 1 to fill in the gap
                    xAxis = xCoord + 1;

                    // Draws top left quadrant
                    for (int i = 0; i < randSize - j; i++)
                    {
                        xAxis--;
                        if (Main.tile[xAxis, yAxis] != null)
                        {
                            if (TileBlacklist.Contains(Main.tile[xAxis, yAxis].type))
                            {
                                notNear = false;
                            }
                        }
                    }
                }

                // The selected location is invalid or is near an invalid location
                if (!notInvalid || !notNear)
                {
                    // Reset the flags
                    notInvalid = true;
                    notNear = true;

                    // Get new coordinates
                    x = WorldGen.genRand.Next(600, Main.maxTilesX - 500);
                    y = WorldGen.genRand.Next((int)WorldGen.rockLayer + 120, WorldGen.lavaLine - 200);
                }
                else
                {
                    // Break out of checker loop
                    break;
                }
            }

            GenerateWaterCave(x, y);
        }

        private void GenerateWaterCave(int x, int y)
        {
            int[] TileBlacklist = { 41, 43, 44, 226 };
            Point point = new Point(x, y);

            // small world size is 4200x1200 , medium multiplies every axis by 1.5 , and large multiplies every axis by 2.0
            int xCoord = x;
            int yCoord = y;

            int xAxis = xCoord;
            int yAxis = yCoord;


            for (int j = 0; j < randSize; j++)
            {
                //int fade = 0;
                yAxis++;
                xAxis = xCoord;
                // Draws bottom right quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis++;
                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (!TileBlacklist.Contains(Main.tile[xAxis, yAxis].type))
                        {
                            WorldGen.KillTile(xAxis, yAxis);
                            WorldGen.PlaceTile(xAxis, yAxis, ModContent.TileType<GlowBlock>());

                            Tile wall = Framing.GetTileSafely(xAxis, yAxis);
                            wall.wall = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }
                }

                // Reset the xAxis, offset by 1 to fill in the gap
                xAxis = xCoord + 1;

                // Draws bottom left quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis--;
                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (!TileBlacklist.Contains(Main.tile[xAxis, yAxis].type))
                        {
                            WorldGen.KillTile(xAxis, yAxis);
                            WorldGen.PlaceTile(xAxis, yAxis, ModContent.TileType<GlowBlock>());

                            Tile wall = Framing.GetTileSafely(xAxis, yAxis);
                            wall.wall = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }
                }
            }


            // Reset the y axis, offset by 1 to fill in the gap
            yAxis = yCoord + 1;

            for (int j = 0; j < randSize; j++)
            {
                yAxis--;
                xAxis = xCoord;

                // Draws top right quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis++;
                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (!TileBlacklist.Contains(Main.tile[xAxis, yAxis].type))
                        {
                            WorldGen.KillTile(xAxis, yAxis);
                            WorldGen.PlaceTile(xAxis, yAxis, ModContent.TileType<GlowBlock>());

                            Tile wall = Framing.GetTileSafely(xAxis, yAxis);
                            wall.wall = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }
                }

                // Reset the xAxis, offset by 1 to fill in the gap
                xAxis = xCoord + 1;

                // Draws top left quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis--;
                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (!TileBlacklist.Contains(Main.tile[xAxis, yAxis].type))
                        {
                            WorldGen.KillTile(xAxis, yAxis);
                            WorldGen.PlaceTile(xAxis, yAxis, ModContent.TileType<GlowBlock>());

                            Tile wall = Framing.GetTileSafely(xAxis, yAxis);
                            wall.wall = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }
                }
            }

            // Generate the circle shape
            WorldUtils.Gen(point, new Shapes.Circle(WorldGen.genRand.Next(25, 30)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true)));

            // Create random circles
            for (int i = 0; i < WorldGen.genRand.Next(14, 19); i++)
            {
                ShapeData circleShapeData = new ShapeData();
                int randX = x + WorldGen.genRand.Next(-10, 10) * 8;
                int randY = y + WorldGen.genRand.Next(-10, 10) * 8;
                Point randPoint = new Point(randX, randY); WorldUtils.Gen(randPoint, new Shapes.Circle(WorldGen.genRand.Next(4, 11)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true).Output(circleShapeData)));
                WorldUtils.Gen(new Point(randX, randY), new ModShapes.All(circleShapeData), Actions.Chain(new Modifiers.RectangleMask(-20, 20, 0, 5), new Modifiers.IsEmpty(), new Actions.SetLiquid()));
            }

            // Create smol circles
            for (int i = 0; i < WorldGen.genRand.Next(14, 19); i++)
            {
                ShapeData circleShapeData = new ShapeData();
                int randX = x + WorldGen.genRand.Next(-10, 10) * 8;
                int randY = y + WorldGen.genRand.Next(-10, 10) * 8;
                Point randPoint = new Point(randX, randY);
                WorldUtils.Gen(randPoint, new Shapes.Circle(WorldGen.genRand.Next(3, 5)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true).Output(circleShapeData)));
                WorldUtils.Gen(new Point(randX, randY), new ModShapes.All(circleShapeData), Actions.Chain(new Modifiers.RectangleMask(-20, 20, 0, 5), new Modifiers.IsEmpty(), new Actions.SetLiquid()));
            }

            // Dig out to the sides
            WorldGen.digTunnel(x, y, -0.65f, 0, WorldGen.genRand.Next(70, 80), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0, 0.5f, WorldGen.genRand.Next(70, 80), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0, -0.5f, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, -0.65f, 0.5f, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0.5f, -0.65f, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0.5f, 0, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0.5f, 0.5f, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(2, 5), false);
            WorldGen.digTunnel(x, y, 0, 0.5f, WorldGen.genRand.Next(70, 90), WorldGen.genRand.Next(3, 6), false);


            // Um, additional tunnels I guess
            WorldGen.digTunnel(x, y, 1, 0, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, 0, 1, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, 0, -1, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, -1, 1, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, -1, -1, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, 1, 1, 120, Main.rand.Next(2, 5), true);
            WorldGen.digTunnel(x, y, 1, -1, 120, Main.rand.Next(2, 5), true);

            // Poke holes
            for (int j = 0; j < randSize; j++)
            {
                yAxis++;
                xAxis = xCoord;
                // Draws bottom right quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis++;

                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (j > randSize - 20)
                        {
                            if (WorldGen.genRand.Next(2100) == 0)
                            {
                                WorldUtils.Gen(new Point(xAxis, yAxis), new Shapes.Circle(WorldGen.genRand.Next(2, 5)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true)));
                            }
                        }
                        else
                        {
                            if (WorldGen.genRand.Next(2800) == 0)
                            {
                                WorldUtils.Gen(new Point(xAxis, yAxis), new Shapes.Circle(WorldGen.genRand.Next(2, 5)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true)));
                            }
                        }

                        if (WorldGen.genRand.Next(3000) == 0)
                        {
                            WorldGen.digTunnel(xAxis, yAxis, WorldGen.genRand.NextFloat(-1, 1), WorldGen.genRand.NextFloat(-1, 1), WorldGen.genRand.Next(30, 40), Main.rand.Next(2, 6), Main.rand.Next(10) == 0 ? true : false);
                        }
                    }
                }

                // Reset the xAxis, offset by 1 to fill in the gap
                xAxis = xCoord + 1;

                // Draws bottom left quadrant
                for (int i = 0; i < randSize - j; i++)
                {
                    xAxis--;
                    if (Main.tile[xAxis, yAxis] != null)
                    {
                        if (j > randSize - 20)
                        {
                            if (WorldGen.genRand.Next(2100) == 0)
                            {
                                WorldUtils.Gen(new Point(xAxis, yAxis), new Shapes.Circle(WorldGen.genRand.Next(2, 5)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true)));
                            }
                        }
                        else
                        {
                            if (WorldGen.genRand.Next(2800) == 0)
                            {
                                WorldUtils.Gen(new Point(xAxis, yAxis), new Shapes.Circle(WorldGen.genRand.Next(2, 5)), Actions.Chain(new Modifiers.Blotches(8, 0.4), new Actions.ClearTile(frameNeighbors: true)));
                            }
                        }

                        if (WorldGen.genRand.Next(3000) == 0)
                        {
                            WorldGen.digTunnel(xAxis, yAxis, WorldGen.genRand.NextFloat(-1, 1), WorldGen.genRand.NextFloat(-1, 1), WorldGen.genRand.Next(30, 40), Main.rand.Next(2, 6), Main.rand.Next(10) == 0 ? true : false);
                        }
                    }
                }
            }
            // Determine random number of MUD generated
            int numMud = WorldGen.genRand.Next(99, 109);
            int generatedMud = 0;
            while (generatedMud < numMud)
            {
                // Choose random coordinate
                int i = WorldGen.genRand.Next(0, Main.maxTilesX);
                int j = WorldGen.genRand.Next(0, Main.maxTilesY);

                // Strength controls size
                // Steps control interations
                Tile tile = Framing.GetTileSafely(i, j);
                if (tile.active() && tile.type == ModContent.TileType<GlowBlock>())
                {
                    //WorldGen.OreRunner(i, j, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 3), (ushort)ModContent.TileType<WaterCaveOre>());
                    WorldGen.TileRunner(i, j, WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(8, 18), ModContent.TileType<CaveMud>(), false, WorldGen.genRand.Next(-1, 1), WorldGen.genRand.Next(-1, 1));
                    generatedMud++; // Increment success
                }
            }

            // Determine random number of ores generated
            int numOres = WorldGen.genRand.Next(69, 79);
            int generatedOres = 0;
            while (generatedOres < numOres)
            {
                // Choose random coordinate
                int i = WorldGen.genRand.Next(0, Main.maxTilesX);
                int j = WorldGen.genRand.Next(0, Main.maxTilesY);

                // Strength controls size
                // Steps control interations
                Tile tile = Framing.GetTileSafely(i, j);
                if (tile.active() && (tile.type == ModContent.TileType<GlowBlock>() || tile.type == ModContent.TileType<WaterCaveOre>()))
                {
                    //WorldGen.OreRunner(i, j, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(2, 4), (ushort)ModContent.TileType<WaterCaveOre>());
                    WorldGen.TileRunner(i, j, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), ModContent.TileType<WaterCaveOre>());
                    generatedOres++; // Increment success
                }
            }
        }

        private void ManaStoneOres(GenerationProgress progress)
        {
            progress.Message = "Generating Modded Ores";
            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, Main.maxTilesY); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.

                // Strength controls size
                // Steps control interations
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.active() && tile.type == TileID.Stone)
                {
                    WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 8), 1, ModContent.TileType<ManaStone>());
                }
            }

            // Erudite Generation
            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 0.00025); k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.maxTilesY);

                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(2, 4), WorldGen.genRand.Next(2, 6), ModContent.TileType<EruditeTile>());
            }

            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 0.0014); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.
                int x = WorldGen.genRand.Next(0, Main.maxTilesX / 6);
                int y = WorldGen.genRand.Next((int)WorldGen.worldSurfaceLow, Main.maxTilesY); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.

                // Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place. Feel free to experiment with strength and step to see the shape they generate.
                WorldGen.PlaceTile(x, y, ModContent.TileType<HerosAltar>());
            }

            // Fake Ores
            int[] ValidTiles = { TileID.Stone, TileID.IceBlock, TileID.SnowBlock, TileID.Mud, TileID.HardenedSand };
            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.maxTilesY - 200); // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.

                // Strength controls size
                // Steps control interations
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.active() && ValidTiles.Contains(tile.type))
                {
                    if (WorldGen.GoldTierOre == TileID.Gold)
                    {
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 8), 1, ModContent.TileType<FakeiteGold>());
                    }
                    else
                    {
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 8), 1, ModContent.TileType<FakeitePlatinum>());
                    }
                }
            }

        }

        private void GenerateAmbientObjects(GenerationProgress progress)
        {
            // Place ambient objects for the Flooded Caverns
            for (int i = 0; i < Main.maxTilesY * 45; i++)
            {
                int[] rockFormations = { ModContent.TileType<Rock1>(), ModContent.TileType<Rock2>(), ModContent.TileType<Rock3>(), ModContent.TileType<Rock4>(), ModContent.TileType<Stalagmite1>(), ModContent.TileType<Stalagmite2>(), ModContent.TileType<Stalagmite3>(), ModContent.TileType<Stalagmite4>(), ModContent.TileType<Stalagmite5>() };
                int x = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
                int y = WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.maxTilesY - 200);
                int type = rockFormations[Main.rand.Next(9)];
                if (Main.tile[x, y].type == ModContent.TileType<GlowBlock>())
                {
                    WorldGen.PlaceObject(x, y, (ushort)type);
                    NetMessage.SendObjectPlacment(-1, x, y, (ushort)type, 0, 0, -1, -1);
                }
            }
        }

        private void GenerateAltar(GenerationProgress progress)
        {
            /*Point point = new Point(x, y);
           
            ShapeData circleShapeData = new ShapeData();
            ShapeData halfCircleShapeData = new ShapeData();
            ShapeData circleShapeData2 = new ShapeData();

            // Generate the circle shape
            WorldUtils.Gen(point, new Shapes.Circle(20), Actions.Chain(new Modifiers.Blotches(2, 0.4), new Actions.ClearTile(frameNeighbors: true).Output(circleShapeData)));

            // Generate the platform
            WorldUtils.Gen(point, new Shapes.Circle(8), Actions.Chain(new Modifiers.Blotches(2, 0.4), new Actions.SetTile(TileID.Dirt).Output(circleShapeData2)));

            // Clear the top half of the shape
            WorldUtils.Gen(point, new Shapes.HalfCircle(12), Actions.Chain(new Actions.ClearTile(frameNeighbors: true).Output(halfCircleShapeData)));

            // Remove the top half of the shape from the shape data
            circleShapeData2.Subtract(halfCircleShapeData, point, point);
            WorldUtils.Gen(point, new ModShapes.OuterOutline(circleShapeData2), Actions.Chain(new Actions.SetTile(TileID.Grass), new Actions.SetFrames(frameNeighbors: true)));

            // Place background
            WorldUtils.Gen(point, new ModShapes.All(circleShapeData), new Actions.PlaceWall(WallID.LivingLeaf));
            WorldUtils.Gen(point, new Shapes.Circle(7), Actions.Chain(new Modifiers.RadialDither(0.5f, 0.2f), new Actions.PlaceWall(WallID.Flower)));

            // Place grass
            WorldUtils.Gen(point, new ModShapes.InnerOutline(circleShapeData), Actions.Chain(new Actions.SetTile(TileID.LivingWood), new Actions.SetFrames(frameNeighbors: true)));

            // Place water
            WorldUtils.Gen(new Point(point.X, point.Y + 2), new ModShapes.All(circleShapeData), Actions.Chain(new Modifiers.RectangleMask(-20, 20, 0, 5), new Modifiers.IsEmpty(), new Actions.SetLiquid()));

            // Place special tile

            // Using PlaceObject instead of PlaceTile works for some apparent reason
            WorldGen.PlaceObject(point.X, point.Y - 1, ModContent.TileType<DruidAltar>(), mute: false, 0);
            //WorldGen.PlaceTile(point.X, point.Y - 6, TileID.LunarCraftingStation, mute: false, forced: true, -1, 0);
            //WorldGen.PlaceTile(point.X, point.Y - 3, ModContent.TileType<DruidAltar>(), mute: false, forced: true, -1, 0);
            WorldGen.PlaceTile(point.X - 5, point.Y - 1, TileID.Campfire, mute: false, forced: true, -1, 0);
            WorldGen.PlaceTile(point.X + 5, point.Y - 1, TileID.Campfire, mute: false, forced: true, -1, 0);

            // Place plants
            WorldUtils.Gen(new Point(x, y - 1), new ModShapes.All(halfCircleShapeData), Actions.Chain(new Modifiers.Offset(0, -1), new Modifiers.OnlyTiles(TileID.Dirt), new Modifiers.Offset(0, -1), new ActionGrass()));*/
        }
    }
}