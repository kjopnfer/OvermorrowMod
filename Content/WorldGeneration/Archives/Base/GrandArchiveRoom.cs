
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using System.Collections.Generic;
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

        public enum DoorID
        {
            GreenRoom,
            RedRoom,
            GreenRoomEntrance,
            RedRoomEntrance,
            YellowRoomEntrance,
            BlueRoomEntrance,
            YellowRoom,
            BlueRoom,
        }

        protected void PlaceBookPile(int x, int y, int stackSize, bool withCandle = false)
        {
            if (stackSize < 0) stackSize = 1;

            for (int i = 0; i < stackSize; i++)
            {
                WorldGen.PlaceObject(x, y - i, ModContent.TileType<BookPile>(), true, Main.rand.Next(0, 4));
            }

            if (withCandle)
                WorldGen.PlaceObject(x, y - stackSize, ModContent.TileType<BookCandleholder>(), true);
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

        // These are split into 7 individual pieces in order to allow for objects to be placed underneath them.
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

            PlaceBookshelfObjects(x + 3, y + 5);
            PlaceBookshelfObjects(x + 5, y + 5);
            PlaceBookshelfObjects(x + 8, y + 5);
        }

        protected void PlaceBookshelfObjects(int x, int y)
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

        protected void PlaceAndConfigureDoor(int x, int y, DoorID doorID, DoorID pairedDoor)
        {
            // Place the door and get the placed entity
            var doorEntity = TileUtils.PlaceTileWithEntity<ArchiveDoor, ArchiveDoor_TE>(x, y);

            // Configure the door and its paired door ID
            if (doorEntity != null)
            {
                doorEntity.DoorID = (int)doorID;
                doorEntity.PairedDoor = (int)pairedDoor;

                // Send the necessary network data for multiplayer
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, x, y, ModContent.TileEntityType<ArchiveDoor_TE>(), 0f, 0, 0, 0);
                    NetMessage.SendTileSquare(-1, x, y, 2);
                }
            }
        }
    }
}