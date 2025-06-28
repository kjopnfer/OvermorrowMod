
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.WorldGeneration.Archives
{
    public abstract class GrandArchiveRoom : Room
    {
        public enum RoomID
        {
            Green,
            Red,
            Yellow,
            Blue
        }

        // I CANT KEEP TRACK OF THESE HOLY FUCK
        public enum DoorID
        {
            FoyerRedRoomDoor,

            RedLongRoomEntrance,
            RedLongRoomExit,

            RedLongTreasureEntrance,
            RedLongTreasureExit,

            RedDiagonalRoomEntrance,
            RedDiagonalRoomExit,

            RedDiagonalTreasureEntrance,
            RedDiagonalTreasureExit,

            RedGhostRoom,

            BlueShrimpRedDiagonalDoor,
            RedDiagonalBlueShrimpDoor,

            RedLongRedDiagonalDoor,
            RedDiagonalRedLongDoor,

            FoyerTreasureEntrance,
            FoyerTreasureExit,

            FoyerGreenRoomDoor,

            GreenBridgeRoomEntrance,
            GreenBridgeRoomExit,

            GreenBridgeTreasureEntrance,
            GreenBridgeTreasureExit,

            GreenFlyingBookRoomEntrance,

            FoyerBlueRoomDoor,

            BlueShrimpRoomEntrance,
            BlueShrimpRoomExit,

            BlueShrimpTreasureEntrance,
            BlueShrimpTreasureExit,

            BlueRoom,

            FoyerYellowRoomDoor,

            YellowPitRoomDoorEntrance,
            YellowPitRoomDoorExit,

            YellowStairsRoomEntrance,
            YellowStairsRoomExit,

            YellowStairsTreasureEntrance,
            YellowStairsTreasureExit,

            YellowWaxheadRoomEntrance,
        }

        protected override Dictionary<Color, int> TileMapping => new()
        {
            [new Color(105, 106, 106)] = ModContent.TileType<CastleBrick>(),
            [new Color(89, 86, 82)] = ModContent.TileType<DarkCastleBrick>(),
            [new Color(138, 111, 48)] = ModContent.TileType<CastlePlatform>(),
            [new Color(74, 47, 33)] = ModContent.TileType<ArchiveWood>(),
            [new Color(150, 150, 150)] = -2,
            [Color.Black] = -1
        };

        protected override Dictionary<Color, int> WallMapping => new()
        {
            [Color.Black] = -1,
            [new Color(66, 64, 61)] = ModContent.WallType<CastleWall>(),
            [new Color(54, 36, 11)] = ModContent.WallType<ArchiveBookWallFrame>(),
            [new Color(118, 66, 138)] = ModContent.WallType<ArchiveBookWall>(),
            [new Color(100, 61, 41)] = ModContent.WallType<ArchiveWoodWall>(),
            [new Color(107, 50, 45)] = ModContent.WallType<ArchiveWoodWallRed>(),
            [new Color(67, 84, 50)] = ModContent.WallType<ArchiveWoodWallGreen>(),
            [new Color(70, 67, 117)] = ModContent.WallType<ArchiveWoodWallBlue>(),
            [new Color(121, 80, 22)] = ModContent.WallType<ArchiveWoodWallYellow>(),
            [new Color(32, 43, 46)] = ModContent.WallType<ArchiveWoodWallBlack>(),
            [new Color(66, 57, 46)] = ModContent.WallType<CastleWall>(),
            [new Color(101, 66, 14)] = ModContent.WallType<ArchiveWoodWall>(),
        };

        protected override Dictionary<Color, (int, int)> ObjectMapping => new()
        {
            [new Color(87, 211, 104)] = (ModContent.TileType<ArchiveBanner>(), 1),
            [new Color(237, 152, 93)] = (ModContent.TileType<WaxSconce>(), 1),
            [new Color(233, 193, 121)] = (ModContent.TileType<WoodenStairs>(), 1),
            [new Color(215, 186, 87)] = (ModContent.TileType<ArchivePot>(), 1),
            [new Color(178, 149, 52)] = (ModContent.TileType<SanctumGate>(), 1),
            [new Color(75, 105, 47)] = (ModContent.TileType<BookPileTable>(), 1),
            [new Color(69, 40, 60)] = (ModContent.TileType<BanquetTable>(), 1),
            [new Color(88, 27, 69)] = (ModContent.TileType<CastleChair>(), 1),
            [new Color(208, 61, 125)] = (ModContent.TileType<CozyChair>(), 1),
            [new Color(180, 58, 0)] = (ModContent.TileType<Fireplace>(), 1),
            [new Color(99, 49, 110)] = (ModContent.TileType<FireplacePillar>(), 1),
            [new Color(223, 113, 38)] = (ModContent.TileType<FloorCandles>(), 6),
            [new Color(193, 0, 83)] = (ModContent.TileType<StairPillar>(), 1),
            [new Color(74, 15, 56)] = (ModContent.TileType<WoodenPillar>(), 1),
            [new Color(179, 36, 136)] = (ModContent.TileType<WoodenPillar2>(), 1),
            [new Color(128, 20, 95)] = (ModContent.TileType<SmallPillar>(), 1),
            [new Color(171, 107, 152)] = (ModContent.TileType<HallwayPillar>(), 1),
            [new Color(171, 73, 94)] = (ModContent.TileType<WoodenArchSmallHallway>(), 1),
            [new Color(115, 72, 34)] = (ModContent.TileType<ArchiveBridge>(), 1),
            [new Color(135, 28, 66)] = (ModContent.TileType<WoodenArch>(), 1),
            [new Color(171, 73, 94)] = (ModContent.TileType<WoodenArchSmall>(), 1),
            [new Color(159, 131, 65)] = (ModContent.TileType<WaxCandelabra>(), 1),
            [new Color(134, 42, 104)] = (ModContent.TileType<SmallChair>(), 1),
            [new Color(148, 109, 65)] = (ModContent.TileType<WaxCandleholder>(), 1),
            [new Color(159, 183, 204)] = (ModContent.TileType<Bismarck>(), 1),
        };

        protected bool PlaceBookPile(int x, int y, int stackSize, bool withCandle = false)
        {
            if (stackSize < 1)
                stackSize = 1;

            // Place bottom tile — must succeed
            bool success = WorldGen.PlaceObject(x, y, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));
            if (!success)
                return false;

            // Place additional stack tiles (optional)
            for (int i = 1; i < stackSize; i++)
            {
                WorldGen.PlaceObject(x, y - i, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));
            }

            // Place candle if needed (optional)
            if (withCandle)
            {
                WorldGen.PlaceObject(x, y - stackSize, ModContent.TileType<BookCandleholder>(), true);
            }

            return true;
        }

        protected void PlaceDoorObjects(int x, int y)
        {
            WorldGen.PlaceObject(x - 8, y, ModContent.TileType<WaxCandleholder>(), true);
            WorldGen.PlaceObject(x + 18, y, ModContent.TileType<WaxCandleholder>(), true);
            WorldGen.PlaceObject(x - 1, y - 25, ModContent.TileType<WoodenArch>());

        }

        protected void PlaceMultiBookPiles(int x, int y)
        {
            const int spaceWidth = 14;
            const int pileWidth = 2;
            const int maxPiles = 4;
            const int maxAttempts = 50;

            bool candlePlaced = false;

            // Generate staggered possible offsets for pile placement
            List<int> possibleOffsets = new();
            for (int i = 0; i <= spaceWidth - pileWidth; i++)
            {
                if (i % 2 == 0 || Main.rand.NextBool())
                    possibleOffsets.Add(i);
            }

            if (possibleOffsets.Count < maxPiles)
            {
                possibleOffsets = Enumerable.Range(0, spaceWidth / pileWidth + 1)
                                            .Select(i => i * pileWidth)
                                            .ToList();
            }

            for (int i = possibleOffsets.Count - 1; i > 0; i--)
            {
                int j = Main.rand.Next(i + 1);
                (possibleOffsets[i], possibleOffsets[j]) = (possibleOffsets[j], possibleOffsets[i]);
            }

            int placedCount = 0;
            int attemptIndex = 0;
            int failCount = 0;

            while (placedCount < maxPiles && attemptIndex < possibleOffsets.Count && failCount < maxAttempts)
            {
                int offsetX = possibleOffsets[attemptIndex];
                int pileX = x + offsetX;

                int stackSize;
                bool withCandle = false;

                // Assign a single candle pile randomly
                if (!candlePlaced && (maxPiles - placedCount <= 1 || Main.rand.NextBool(4)))
                {
                    stackSize = Main.rand.Next(8, 13);
                    withCandle = true;
                    candlePlaced = true;
                }
                else
                {
                    // Random stack sizing logic
                    int style = Main.rand.Next(3);
                    stackSize = style switch
                    {
                        0 => Main.rand.Next(2, 4),
                        1 => Main.rand.Next(5, 7),
                        _ => Main.rand.Next(6, 9)
                    };
                }

                bool success = PlaceBookPile(pileX, y, stackSize, withCandle);
                if (success)
                {
                    placedCount++;
                }
                else
                {
                    failCount++;
                }

                attemptIndex++;
            }
        }


        protected void PlaceLoungeArea(int x, int y, RoomID room)
        {
            var cozyChairTypes = new Dictionary<RoomID, int>
            {
                { RoomID.Red, ModContent.TileType<CozyChairRed>() },
                { RoomID.Green, ModContent.TileType<CozyChairGreen>() },
                { RoomID.Blue, ModContent.TileType<CozyChairBlue>() }
            };

            var smallChairTypes = new Dictionary<RoomID, int>
            {
                { RoomID.Red, ModContent.TileType<SmallChairRed>() },
                { RoomID.Green, ModContent.TileType<SmallChairGreen>() },
                { RoomID.Blue, ModContent.TileType<SmallChairBlue>() }
            };

            int cozyChairType = cozyChairTypes.TryGetValue(room, out var type) ? type : ModContent.TileType<CozyChair>();
            int smallChairType = smallChairTypes.TryGetValue(room, out var type2) ? type2 : ModContent.TileType<SmallChair>();

            WorldGen.PlaceObject(x, y, cozyChairType, true, 0, 0, -1, 1);
            WorldGen.PlaceObject(x + 5, y, ModContent.TileType<BanquetTable>());
            WorldGen.PlaceObject(x + 5, y - 2, ModContent.TileType<WaxCandelabra>());
            WorldGen.PlaceObject(x + 8, y - 2, ModContent.TileType<BookPileTable>(), true, Main.rand.Next(0, 4));

            WorldGen.PlaceObject(x + 12, y, smallChairType, true, 0, 0, -1, -1);
            WorldGen.PlaceObject(x + 16, y, smallChairType, true, 0, 0, -1, 1);

            WorldGen.PlaceObject(x + 23, y, cozyChairType);

            WorldGen.PlaceObject(x + 11, y - 25, ModContent.TileType<WaxChandelier>());
        }

        /// <summary>
        /// Each of these tiles are 14x10
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="stack"></param>
        protected void PlaceDiagonalStairStack(int x, int y, int stack = 3)
        {
            int height = stack * 10;
            for (int i = 0; i < height; i += 10)
            {
                WorldGen.PlaceObject(x, y - i, ModContent.TileType<DiagonalStairs>());
            }

            WorldGen.PlaceObject(x, y - height, ModContent.TileType<StairCap>());
        }

        protected void PlaceTallStairs(int x, int y)
        {
            WorldGen.PlaceObject(x + 1, y - 11, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 26, y - 11, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 1, y - 40, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 26, y - 40, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 8, y - 55, ModContent.TileType<WoodenArch>());

            PlaceDiagonalStairStack(x + 8, y);
        }

        /// <summary>
        /// Used for those platforms.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void PlaceVaseGroup(int x, int y)
        {
            WorldGen.PlaceObject(x, y, ModContent.TileType<ArchivePotSmall>());
            WorldGen.PlaceObject(x + 1, y, ModContent.TileType<FatVase>());
            WorldGen.PlaceObject(x + 4, y, ModContent.TileType<ArchivePotSmall>());
        }

        /// <summary>
        /// Places a VaseGroup with each placement being offset by (4, 4).
        /// Direction is either 1 (up) or -1 (down). Which controls whether the vases are placed ascending or descending.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="repeat"></param>
        protected void PlaceMultiVase(int x, int y, int direction, int repeat)
        {
            if (direction != 1 && direction != -1) direction = 1;

            for (int i = 0; i < repeat; i++)
            {
                var offset = 4 * i;
                PlaceVaseGroup(x + offset, y - (offset * direction));
            }
        }

        /// <summary>
        /// Placement is relative to where the stairs will be placed in the bottom left corner.
        /// Valid directions are -1 (stair pillar on left), 1 (stair pillar on right), and 0 (no stair pillars)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="direction"></param>
        protected void PlaceStairGroup(int x, int y, int direction)
        {
            WorldGen.PlaceObject(x, y, ModContent.TileType<WoodenStairs>());
            WorldGen.PlaceObject(x, y - 37, ModContent.TileType<WoodenArch>());

            if (direction == -1) // StairPillar is on the left side
            {
                WorldGen.PlaceObject(x - 3, y, ModContent.TileType<StairPillar>());
                WorldGen.PlaceObject(x + 14, y, ModContent.TileType<SmallPillar>());
                WorldGen.PlaceObject(x + 14, y - 12, ModContent.TileType<WoodenPillar2>());
            }
            else if (direction == 1) // StairPillar is on the right side
            {
                WorldGen.PlaceObject(x - 3, y, ModContent.TileType<SmallPillar>());
                WorldGen.PlaceObject(x + 14, y, ModContent.TileType<StairPillar>());
                WorldGen.PlaceObject(x - 3, y - 12, ModContent.TileType<WoodenPillar2>());
            }
            else // No stair pillars
            {
                WorldGen.PlaceObject(x - 3, y, ModContent.TileType<SmallPillar>());
                WorldGen.PlaceObject(x + 14, y, ModContent.TileType<SmallPillar>());
                WorldGen.PlaceObject(x - 3, y - 12, ModContent.TileType<WoodenPillar2>());
                WorldGen.PlaceObject(x + 14, y - 12, ModContent.TileType<WoodenPillar2>());
            }
        }

        protected void PlaceRoomBookshelfArches(int x, int y)
        {
            PlaceBookshelfArch(x, y);
            PlaceBookshelfArch(x + 26, y);
            PlaceBookshelfArch(x + 52, y);
            PlaceBookshelfArch(x, y + 30);
            PlaceBookshelfArch(x + 26, y + 30);
            PlaceBookshelfArch(x + 52, y + 30);

            PlaceBookshelfArch(x + 167, y + 5);
            PlaceBookshelfArch(x + 193, y + 5);
            PlaceBookshelfArch(x + 219, y + 5);
            PlaceBookshelfArch(x + 167, y + 35);
            PlaceBookshelfArch(x + 193, y + 35);
            PlaceBookshelfArch(x + 219, y + 35);

            PlaceBookshelfArch(x + 334, y + 5);
            PlaceBookshelfArch(x + 360, y + 5);
            PlaceBookshelfArch(x + 386, y + 5);
            PlaceBookshelfArch(x + 334, y + 35);
            PlaceBookshelfArch(x + 360, y + 35);
            PlaceBookshelfArch(x + 386, y + 35);
        }

        /// <summary>
        /// Places the book piles onto the ground and the archways with objects.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void PlaceBookShelfObjects(int x, int y, bool placeBookPiles = true)
        {
            if (placeBookPiles) PlaceMultiBookPiles(x, y);
            PlaceBookshelfArch(x + 1, y - 25);
        }

        protected void PlaceHallwayArch(int x, int y)
        {
            WorldGen.PlaceObject(x, y, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 2, y - 7, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 8, y, ModContent.TileType<HallwayPillar>());
        }

        protected void PlaceCozyArea(int x, int y, RoomID room)
        {
            var cozyChairTypes = new Dictionary<RoomID, int>
            {
                { RoomID.Red, ModContent.TileType<CozyChairRed>() },
                { RoomID.Green, ModContent.TileType<CozyChairGreen>() },
                { RoomID.Blue, ModContent.TileType<CozyChairBlue>() }
            };

            var smallChairTypes = new Dictionary<RoomID, int>
            {
                { RoomID.Red, ModContent.TileType<SmallChairRed>() },
                { RoomID.Green, ModContent.TileType<SmallChairGreen>() },
                { RoomID.Blue, ModContent.TileType<SmallChairBlue>() }
            };

            int cozyChairType = cozyChairTypes.TryGetValue(room, out var type) ? type : ModContent.TileType<CozyChair>();
            int smallChairType = smallChairTypes.TryGetValue(room, out var type2) ? type2 : ModContent.TileType<SmallChair>();

            WorldGen.PlaceObject(x + 3, y - 5, ModContent.TileType<Bismarck>());
            WorldGen.PlaceObject(x + 21, y - 5, ModContent.TileType<Bismarck>());

            WorldGen.PlaceObject(x + 3, y - 19, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 12, y - 21, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 21, y - 19, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x, y, smallChairType, true, 0, 0, -1, 1);
            WorldGen.PlaceObject(x + 3, y, ModContent.TileType<BanquetTable>());
            WorldGen.PlaceObject(x + 3, y - 2, ModContent.TileType<WaxCandelabra>());
            WorldGen.PlaceObject(x + 6, y - 2, ModContent.TileType<BookPileTable>(), true, Main.rand.Next(0, 4));

            WorldGen.PlaceObject(x + 11, y - 23, ModContent.TileType<WoodenArchSmall>());
            WorldGen.PlaceObject(x + 12, y - 8, ModContent.TileType<Moose>());
            WorldGen.PlaceObject(x + 11, y - 5, ModContent.TileType<WoodenArchSmall>());

            WorldGen.PlaceObject(x + 9, y, ModContent.TileType<FireplacePillar>());
            WorldGen.PlaceObject(x + 17, y, ModContent.TileType<FireplacePillar>());

            WorldGen.PlaceObject(x + 26, y, cozyChairType);
        }

        /// <summary>
        /// These are split into 7 individual pieces in order to allow for objects to be placed underneath them.
        /// These should be placed below a ceiling for the anchor point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void PlaceBookshelfArch(int x, int y)
        {
            WorldGen.PlaceObject(x, y, ModContent.TileType<WoodenArchL1>());
            WorldGen.PlaceObject(x + 1, y, ModContent.TileType<WoodenArchL2>());
            WorldGen.PlaceObject(x + 2, y, ModContent.TileType<WoodenArchL3>());
            WorldGen.PlaceObject(x + 3, y, ModContent.TileType<WoodenArchSplit>());
            WorldGen.PlaceObject(x + 11, y, ModContent.TileType<WoodenArchR1>());
            WorldGen.PlaceObject(x + 12, y, ModContent.TileType<WoodenArchR2>());
            WorldGen.PlaceObject(x + 13, y, ModContent.TileType<WoodenArchR3>());

            if (Main.rand.NextBool())
                WorldGen.PlaceObject(x + 1, y + 5, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));

            if (Main.rand.NextBool())
                WorldGen.PlaceObject(x + 10, y + 5, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));

            PlaceShelfArchObjects(x + 3, y + 5);
            PlaceShelfArchObjects(x + 5, y + 5);
            PlaceShelfArchObjects(x + 8, y + 5);
        }

        protected void PlaceShelfArchObjects(int x, int y)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    WorldGen.PlaceObject(x, y, ModContent.TileType<Globe>());
                    break;
                case 1:
                    WorldGen.PlaceObject(x, y, ModContent.TileType<Telescope>());
                    break;
                case 2:
                    WorldGen.PlaceObject(x, y, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));
                    WorldGen.PlaceObject(x, y - 1, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));
                    if (Main.rand.NextBool())
                        WorldGen.PlaceObject(x, y - 2, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));
                    break;
                case 3:
                    WorldGen.PlaceObject(x, y, ModContent.TileType<Crates>(), true, Main.rand.Next(0, 3));
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="doorID">The ID of this door.</param>
        /// <param name="pairedDoor">The ID of the other door that this will teleport to.</param>
        protected void PlaceAndConfigureDoor(int x, int y, DoorID doorID, DoorID pairedDoor, bool isLocked = false)
        {
            // Place the door and get the placed entity
            var doorEntity = TileUtils.PlaceTileWithEntity<ArchiveDoor, ArchiveDoor_TE>(x, y);

            // Configure the door and its paired door ID
            if (doorEntity != null)
            {
                doorEntity.DoorID = (int)doorID;
                doorEntity.PairedDoor = (int)pairedDoor;
                doorEntity.IsLocked = isLocked;

                // Send the necessary network data for multiplayer
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, x, y, ModContent.TileEntityType<ArchiveDoor_TE>(), 0f, 0, 0, 0);
                    NetMessage.SendTileSquare(-1, x, y, 2);
                }
            }
        }


        protected void PlaceLongTableAndChairs(int x, int y, RoomID room)
        {
            var cozyChairTypes = new Dictionary<RoomID, int>
            {
                { RoomID.Red, ModContent.TileType<CozyChairRed>() },
                { RoomID.Green, ModContent.TileType<CozyChairGreen>() },
                { RoomID.Blue, ModContent.TileType<CozyChairBlue>() }
            };

            var smallChairTypes = new Dictionary<RoomID, int>
            {
                { RoomID.Red, ModContent.TileType<SmallChairRed>() },
                { RoomID.Green, ModContent.TileType<SmallChairGreen>() },
                { RoomID.Blue, ModContent.TileType<SmallChairBlue>() }
            };

            int cozyChairType = cozyChairTypes.TryGetValue(room, out var type) ? type : ModContent.TileType<CozyChair>();
            int smallChairType = smallChairTypes.TryGetValue(room, out var type2) ? type2 : ModContent.TileType<SmallChair>();

            WorldGen.PlaceObject(x, y, smallChairType, true, 0, 0, -1, 1);
            WorldGen.PlaceObject(x + 3, y, ModContent.TileType<BanquetTable>());
            WorldGen.PlaceObject(x + 4, y - 2, ModContent.TileType<WaxCandelabra>());
            WorldGen.PlaceObject(x + 7, y - 2, ModContent.TileType<BookPileTable>(), true, Main.rand.Next(0, 4));

            WorldGen.PlaceObject(x + 10, y, smallChairType, true, 0, 0, -1, direction: -1);
        }


        /// <summary>
        /// Places the small table. Has an 80% chance to place the cloth or the empty version.
        /// 20% chance to place one of the special variants which are shared between all room types.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="room"></param>
        protected void PlaceTableAndChair(int x, int y, int direction, RoomID room)
        {
            var tableTypes = new Dictionary<RoomID, int>
            {
                { RoomID.Red, ModContent.TileType<ArchiveTableRed>() },
                { RoomID.Green, ModContent.TileType<ArchiveTableGreen>() },
                { RoomID.Blue, ModContent.TileType<ArchiveTableBlue>() }
            };

            int[] specialTableTypes = [
                ModContent.TileType<ArchiveTable2>(),
                ModContent.TileType<ArchiveTable3>(),
                ModContent.TileType<ArchiveTable4>(),
                ModContent.TileType<ArchiveTable5>(),
                ModContent.TileType<ArchiveTable6>(),
            ];

            var smallChairTypes = new Dictionary<RoomID, int>
            {
                { RoomID.Red, ModContent.TileType<SmallChairRed>() },
                { RoomID.Green, ModContent.TileType<SmallChairGreen>() },
                { RoomID.Blue, ModContent.TileType<SmallChairBlue>() }
            };

            int tableType = tableTypes.TryGetValue(room, out var type) ? type : ModContent.TileType<ArchiveTablePink>();
            int smallChairType = smallChairTypes.TryGetValue(room, out var type2) ? type2 : ModContent.TileType<SmallChair>();

            // The table starts off as either be the empty version or the cloth version, depending on the room.
            // This gives a chance to choose the empty version.
            if (Main.rand.NextBool())
            {
                tableType = ModContent.TileType<ArchiveTable1>();
            }

            // Afterwards, randomly decide again whether to choose one of the special variants.
            // 20% chance to override with a special variant
            bool specialType = false;
            if (Main.rand.NextBool(5)) // 1 in 5 = 20%
            {
                tableType = Main.rand.Next(specialTableTypes);
                specialType = true;
            }

            if (direction == -1)
            {
                WorldGen.PlaceObject(x, y, tableType, true, 0, 0, 0, 1);
                WorldGen.PlaceObject(x + 3, y, smallChairType, true, 0, 0, -1, -1);
            }
            else
            {
                WorldGen.PlaceObject(x, y, smallChairType, true, 0, 0, -1, 1);
                WorldGen.PlaceObject(x + 2, y, tableType, true, 0, 0, 0, -1);
            }

            // If the table isn't the special type put books or the inkwell ontop
            if (!specialType)
            {
                if (Main.rand.NextBool())
                    WorldGen.PlaceObject(x, y - 2, ModContent.TileType<BookPileTable>(), true);

                if (Main.rand.NextBool())
                    WorldGen.PlaceObject(x + 2, y - 2, ModContent.TileType<Inkwell>(), true);
            }
        }
    }
}