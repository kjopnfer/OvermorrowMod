using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Localization;
using System.Threading;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using System.IO;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using OvermorrowMod.WardenClass.Accessories;
using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Tiles;

namespace OvermorrowMod
{
    public class OvermorrowWorld : ModWorld
    {
        public static bool downedDarude;
        public static bool downedTree;
        public static bool downedDrippler;
        public static bool downedDrake;

        private bool placedBook = false;

        public override void Initialize()
        {
            downedTree = false;
            downedDarude = false;
            downedDrippler = false;
            downedDrake = false;
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
                                if (inventoryIndex == 0)
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
            Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);

            Point point = new Point(x, y);
            //WorldUtils.Gen(point, new Shapes.Mound(14, 14), new Actions.SetTile(TileID.LunarBlockSolar));

            /*ShapeData shapeData = new ShapeData();
            WorldUtils.Gen(point, new Shapes.Circle(5, 5), new Actions.Blank().Output(shapeData));
            WorldUtils.Gen(point, new Shapes.Circle(5, 5), Actions.Chain(new GenAction[]
            {
                new Modifiers.Offset(15, 0),
                new Actions.Blank().Output(shapeData)
            }));

            WorldUtils.Gen(point, new Shapes.Rectangle(6, 25), Actions.Chain(new GenAction[]
            {
                new Modifiers.Offset(5, -25),
                new Actions.Blank().Output(shapeData)
            }));

            WorldUtils.Gen(point, new Shapes.Mound(7, 10), Actions.Chain(new GenAction[]
            {
                new Modifiers.Offset(7, -25),
                new Actions.Blank().Output(shapeData)
            }));
            WorldUtils.Gen(point, new ModShapes.InnerOutline(shapeData, true), new Actions.SetTile(TileID.LunarBlockSolar, true));*/
            //Point surfacePoint;

            //WorldUtils.Gen(point, new Shapes.Mound(14, 14), Actions.Chain(new Actions.SetTile(TileID.LunarBlockStardust)));
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
            WorldGen.PlaceTile(point.X, point.Y, TileID.LargePiles2, mute: true, forced: false, -1, 17);
            WorldGen.PlaceTile(point.X - 5, point.Y, TileID.Campfire, mute: true, forced: true, -1, 0);
            WorldGen.PlaceTile(point.X + 5, point.Y, TileID.Campfire, mute: true, forced: true, -1, 0);

            // Place plants
            WorldUtils.Gen(point, new ModShapes.All(halfCircleShapeData), Actions.Chain(new Modifiers.Offset(0, -1), new Modifiers.OnlyTiles(TileID.Dirt), new Modifiers.Offset(0, -1), new ActionGrass()));
            /*WorldGen.TileRunner(x, y, 20, 1, TileID.WoodBlock, true);
            WorldGen.digTunnel(x, y, 0, 0, 1, 10);*/
        
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int GenPass = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
            if (GenPass != -1)
            {
                tasks.Insert(GenPass + 1, new PassLegacy("Generating Glowing Lakes", GenerateGlowingLakes));
            }
        }

        bool notInvalid = true;
        bool notNear = true;
        private void GenerateGlowingLakes(GenerationProgress progress)
        {
            // Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Generating Glowing Lakes";

            /*
             * In a small world, this math results in 4200 * 1200 * 0.0000009, which is about 4. 
             * This means that we'll run the code inside the for loop 4 times. 
             * Since we are scaling by both dimensions of the world size, the amount spawned will adjust automatically to different world sizes for a consistent distribution of ores.
             */
            for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 9E-07); i++)
            {
                int x = WorldGen.genRand.Next(600, Main.maxTilesX - 500);
                int y = WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.maxTilesY == 2400 ? WorldGen.lavaLine - 125 : WorldGen.lavaLine - 75); // Check if world is large

                while (true)
                {
                    // While loop to find a valid tile
                    if (!Main.tile[x, y].active() || Main.tile[x, y].type == TileID.SnowBlock || Main.tile[x, y].type == TileID.IceBlock || Main.tile[x, y].type == TileID.BlueDungeonBrick ||
                        Main.tile[x, y].type == TileID.Sand || Main.tile[x, y].type == TileID.HardenedSand || Main.tile[x, y].type == TileID.GreenDungeonBrick || Main.tile[x, y].type == TileID.PinkDungeonBrick)
                    {
                        notInvalid = false;
                    }

                    // For loop to check for nearby Glowing Lakes, The Jungle Temple, or The Dungeon with a distance of 60
                    for (int ii = 0; ii < 60; ii++)
                    {
                        if (Main.tile[x + ii, y].type == ModContent.TileType<GlowBlock>() || Main.tile[x + ii, y].type == TileID.GreenDungeonBrick || Main.tile[x + ii, y].type == TileID.PinkDungeonBrick || 
                            Main.tile[x + ii, y].type == TileID.BlueDungeonBrick || Main.tile[x + ii, y].type == TileID.LihzahrdBrick)
                        {
                            notNear = false;
                        }

                        if (Main.tile[x - ii, y].type == ModContent.TileType<GlowBlock>() || Main.tile[x - ii, y].type == TileID.GreenDungeonBrick || Main.tile[x - ii, y].type == TileID.PinkDungeonBrick || 
                            Main.tile[x - ii, y].type == TileID.BlueDungeonBrick || Main.tile[x - ii, y].type == TileID.LihzahrdBrick)
                        {
                            notNear = false;
                        }

                        for (int ij = 0; ij < 60; ij++)
                        {
                            // -> v
                            if (Main.tile[x + ii, y + ij].type == ModContent.TileType<GlowBlock>() || Main.tile[x + ii, y + ij].type == TileID.GreenDungeonBrick || Main.tile[x + ii, y + ij].type == TileID.PinkDungeonBrick ||
                            Main.tile[x + ii, y + ij].type == TileID.BlueDungeonBrick || Main.tile[x + ii, y + ij].type == TileID.LihzahrdBrick)
                            {
                                notNear = false;
                            }

                            // <- v
                            if (Main.tile[x - ii, y + ij].type == ModContent.TileType<GlowBlock>() || Main.tile[x - ii, y + ij].type == TileID.GreenDungeonBrick || Main.tile[x - ii, y + ij].type == TileID.PinkDungeonBrick ||
                            Main.tile[x - ii, y + ij].type == TileID.BlueDungeonBrick || Main.tile[x - ii, y + ij].type == TileID.LihzahrdBrick)
                            {
                                notNear = false;
                            }

                            // -> ^
                            if (Main.tile[x + ii, y - ij].type == ModContent.TileType<GlowBlock>() || Main.tile[x + ii, y - ij].type == TileID.GreenDungeonBrick || Main.tile[x + ii, y - ij].type == TileID.PinkDungeonBrick ||
                            Main.tile[x + ii, y - ij].type == TileID.BlueDungeonBrick || Main.tile[x + ii, y - ij].type == TileID.LihzahrdBrick)
                            {
                                notNear = false;
                            }

                            // <- ^
                            if (Main.tile[x - ii, y - ij].type == ModContent.TileType<GlowBlock>() || Main.tile[x - ii, y - ij].type == TileID.GreenDungeonBrick || Main.tile[x - ii, y - ij].type == TileID.PinkDungeonBrick ||
                            Main.tile[x - ii, y - ij].type == TileID.BlueDungeonBrick || Main.tile[x - ii, y - ij].type == TileID.LihzahrdBrick)
                            {
                                notNear = false;
                            }
                        }
                    }

                    if(!notInvalid || !notNear)
                    {
                        // Reset the flags
                        notInvalid = true;
                        notNear = true;

                        // Get new coordinates
                        x = WorldGen.genRand.Next(600, Main.maxTilesX - 500);
                        y = WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.maxTilesY - 600);
                    }
                    else
                    {
                        break;
                    }
                }

                GenerateGlowingLake(x, y);
            }

            // This goes through each GlowBlock and places a wall behind it
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile checkTiles = Framing.GetTileSafely(i, j);
                    if (checkTiles.type == ModContent.TileType<GlowBlock>() && checkTiles.wall != ModContent.WallType<GlowWall>())
                    {
                        checkTiles.wall = (ushort)ModContent.WallType<GlowWall>();
                    }
                }
            }
        }

        private void GenerateGlowingLake(int x, int y)
        {
            int randSize = Main.rand.Next(36, 60);
            for (int i = 0; i < randSize; i++)
            {
                // Runs across X forwards
                WorldGen.TileRunner(x + i, y, Main.rand.Next(21, 32), 8, ModContent.TileType<GlowBlock>(), i > randSize - 6 ? true : false, Main.rand.Next(10, 15), Main.rand.Next(1, 4));
                // Runs down Y
                WorldGen.TileRunner(x + i, y + ((randSize - 5) - i), Main.rand.Next(29, 40), 7, ModContent.TileType<GlowBlock>(), i > randSize - 6 ? true : false, Main.rand.Next(0, 5), Main.rand.Next(0, 5));

                // Runs across X backwards
                WorldGen.TileRunner(x - i, y, Main.rand.Next(21, 32), Main.rand.Next(5, 8), ModContent.TileType<GlowBlock>(), i > randSize - 6 ? true : false, Main.rand.Next(-15, -10), Main.rand.Next(-8, -1));
                // Runs down Y
                WorldGen.TileRunner(x - i, y + ((randSize - 5) - i), Main.rand.Next(29, 40), 7, ModContent.TileType<GlowBlock>(), i > randSize - 6 ? true : false, Main.rand.Next(-5, 0), Main.rand.Next(-5, 0));
            }

            for (int i = 0; i < 20; i++)
            {
                WorldGen.digTunnel(x + i, y - Main.rand.Next(3), -2 * i, 0, 4, Main.rand.Next(5, 9), false);
                WorldGen.digTunnel(x - i, y - Main.rand.Next(3), 2 * i, 0, 4, Main.rand.Next(5, 9), false);
                for (int j = 21 - i; j > 0; j--)
                {
                    WorldGen.digTunnel(x + (i * 2), y, 0, (j / 2) + 3, 4, Main.rand.Next(8, 9), j > 11 ? true : false);
                    WorldGen.digTunnel(x - (i * 2), y, 0, (j / 2) + 3, 4, Main.rand.Next(5, 9), j > 11 ? true : false);
                }
            }

            // Generate walls

            // This loops across the inner space of the biome to generate walls
            // Loop across X
            for (int i = 0; i < 40; i++)
            {
                // Loop across X forwards
                Tile tileForwards = Framing.GetTileSafely(x + i, y);
                if (tileForwards.wall == 0 || tileForwards.wall == WallID.Dirt)
                {
                    if (tileForwards.type != ModContent.TileType<GlowBlock>())
                    {
                        tileForwards.wall = (ushort)ModContent.WallType<GlowWall>();
                    }
                }

                // Loop across X backwards
                Tile tileBackwards = Framing.GetTileSafely(x - i, y);
                if (tileBackwards.wall == 0 || tileBackwards.wall == WallID.Dirt)
                {
                    if (tileBackwards.type != ModContent.TileType<GlowBlock>())
                    {
                        tileBackwards.wall = (ushort)ModContent.WallType<GlowWall>();
                    }
                }

                // Loop across Y
                for (int j = 0; j < 30; j++)
                {
                    // Loop up Y
                    Tile tileUp = Framing.GetTileSafely(x + i, y + j);
                    if (tileUp.wall == 0 || tileUp.wall == WallID.Dirt)
                    {
                        if (tileUp.type != ModContent.TileType<GlowBlock>())
                        {
                            tileUp.wall = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }

                    // Loop up Y backwards
                    Tile tileUp2 = Framing.GetTileSafely(x - i, y + j);
                    if (tileUp2.wall == 0 || tileUp2.wall == WallID.Dirt)
                    {
                        if (tileUp2.type != ModContent.TileType<GlowBlock>())
                        {
                            tileUp2.wall = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }

                    // Loop down Y
                    Tile tileDown = Framing.GetTileSafely(x + i, y - j);
                    if (tileDown.wall == 0 || tileDown.wall == WallID.Dirt)
                    {
                        if (tileDown.type != ModContent.TileType<GlowBlock>())
                        {
                            tileDown.wall = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }

                    // Loop down Y backwards
                    Tile tileDown2 = Framing.GetTileSafely(x - i, y - j);
                    if (tileDown2.wall == 0 || tileDown2.wall == WallID.Dirt)
                    {
                        if (tileDown2.type != ModContent.TileType<GlowBlock>())
                        {
                            tileDown2.wall = (ushort)ModContent.WallType<GlowWall>();
                        }
                    }
                }
            }
        }
    }
}