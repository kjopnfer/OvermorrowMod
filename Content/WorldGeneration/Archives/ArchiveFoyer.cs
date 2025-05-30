using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static OvermorrowMod.Core.WorldGeneration.ArchiveSubworld.SetupGenPass;

namespace OvermorrowMod.Content.WorldGeneration.Archives
{
    public class ArchiveFoyer : Room
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "ArchiveFoyerTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "ArchiveFoyerWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "ArchiveFoyerObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        protected override Dictionary<Color, int> TileMapping => new ()
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
            //[new Color(113, 193, 107)] = ModContent.WallType<ArchiveBackground>(),
            [new Color(66, 64, 61)] = ModContent.WallType<CastleWall>(),
            [new Color(54, 36, 11)] = ModContent.WallType<ArchiveBookWallFrame>(),
            [new Color(118, 66, 138)] = ModContent.WallType<ArchiveBookWall>(),
            [new Color(100, 61, 41)] = ModContent.WallType<ArchiveWoodWall>(),
            [new Color(107, 50, 45)] = ModContent.WallType<ArchiveWoodWallRed>(),
            [new Color(67, 84, 50)] = ModContent.WallType<ArchiveWoodWallGreen>(),
            [new Color(70, 67, 117)] = ModContent.WallType<ArchiveWoodWallBlue>(),
            [new Color(121, 80, 22)] = ModContent.WallType<ArchiveWoodWallYellow>(),
            [new Color(66, 57, 46)] = ModContent.WallType<CastleWall>(),
            [new Color(101, 66, 14)] = ModContent.WallType<ArchiveWoodWall>(),
        };

        protected override Dictionary<Color, (int, int)> ObjectMapping => new()
        {
            [new Color(215, 186, 87)] = (ModContent.TileType<ArchivePot>(), 1),
            [new Color(178, 149, 52)] = (ModContent.TileType<SanctumGate>(), 1),
            [new Color(75, 105, 47)] = (ModContent.TileType<BookPileTable>(), 1),
            [new Color(69, 40, 60)] = (ModContent.TileType<BanquetTable>(), 1),
            [new Color(88, 27, 69)] = (ModContent.TileType<CastleChair>(), 1),
            [new Color(208, 61, 125)] = (ModContent.TileType<CozyChair>(), 1),
            [new Color(180, 58, 0)] = (ModContent.TileType<Fireplace>(), 1),
            [new Color(99, 49, 110)] = (ModContent.TileType<FireplacePillar>(), 1),
            [new Color(223, 113, 38)] = (ModContent.TileType<FloorCandles>(), 6),
            [new Color(74, 15, 56)] = (ModContent.TileType<WoodenPillar>(), 1),
            [new Color(179, 36, 136)] = (ModContent.TileType<WoodenPillar2>(), 1),
            [new Color(115, 72, 34)] = (ModContent.TileType<ArchiveBridge>(), 1),
            [new Color(135, 28, 66)] = (ModContent.TileType<WoodenArch>(), 1),
            [new Color(171, 73, 94)] = (ModContent.TileType<WoodenArchSmall>(), 1),
            [new Color(159, 131, 65)] = (ModContent.TileType<WaxCandelabra>(), 1),
            [new Color(134, 42, 104)] = (ModContent.TileType<SmallChair>(), 1),
            [new Color(148, 109, 65)] = (ModContent.TileType<WaxCandleholder>(), 1),
            [new Color(159, 183, 204)] = (ModContent.TileType<Bismarck>(), 1),
        };

        public override void PostGenerate(int x, int y)
        {
            #region Center Room
            PlaceBookshelfArch(x + 128, y + 60);
            PlaceBookshelfArch(x + 154, y + 60);
            PlaceBookshelfArch(x + 180, y + 60);
            PlaceBookshelfArch(x + 128, y + 90);
            PlaceBookshelfArch(x + 154, y + 90);
            PlaceBookshelfArch(x + 180, y + 90);

            PlaceBookshelfArch(x + 295, y + 60);
            PlaceBookshelfArch(x + 321, y + 60);
            PlaceBookshelfArch(x + 347, y + 60);
            PlaceBookshelfArch(x + 295, y + 90);
            PlaceBookshelfArch(x + 321, y + 90);
            PlaceBookshelfArch(x + 347, y + 90);

            WorldGen.PlaceObject(x + 233, y + 115, ModContent.TileType<ArchivePot>());
            WorldGen.PlaceObject(x + 267, y + 115, ModContent.TileType<ArchivePot>());
            WorldGen.PlaceObject(x + 276, y + 115, ModContent.TileType<ArchivePot>());

            WorldGen.PlaceObject(x + 365, y + 115, ModContent.TileType<ArchivePot>());
            WorldGen.PlaceObject(x + 376, y + 115, ModContent.TileType<ArchivePot>());
            WorldGen.PlaceObject(x + 406, y + 110, ModContent.TileType<ArchivePot>());
            WorldGen.PlaceObject(x + 400, y + 111, ModContent.TileType<ArchivePot>());
            WorldGen.PlaceObject(x + 394, y + 112, ModContent.TileType<ArchivePot>());

            WorldGen.PlaceObject(x + 456, y + 110, ModContent.TileType<ArchivePot>());
            WorldGen.PlaceObject(x + 474, y + 113, ModContent.TileType<ArchivePot>());
            WorldGen.PlaceObject(x + 486, y + 115, ModContent.TileType<ArchivePot>());

            #region Left Bridge
            WorldGen.PlaceObject(x + 205, y + 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 247, y + 86, ModContent.TileType<ArchiveBridge>());

            WorldGen.PlaceObject(x + 244, y + 115, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 264, y + 85, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 264, y + 115, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 230, y + 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 267, y + 60, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 230, y + 90, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 267, y + 90, ModContent.TileType<WoodenArch>());
            #endregion

            #region Fireplace
            PlaceCozyArea(x + 418, y + 110, RoomID.Yellow);

            WorldGen.PlaceObject(x + 425, y + 55, ModContent.TileType<WoodenArch>());
            TileUtils.PlaceTileWithEntity<SanctumGate, SanctumGate_TE>(x + 425, y + 80);
            #endregion

            #region Right Bridge
            WorldGen.PlaceObject(x + 600, y + 86, ModContent.TileType<ArchiveBridge>());
            WorldGen.PlaceObject(x + 645, y + 60, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 597, y + 115, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 617, y + 85, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 617, y + 115, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 583, y + 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 620, y + 60, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 583, y + 90, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 620, y + 90, ModContent.TileType<WoodenArch>());
            #endregion

            PlaceBookshelfArch(x + 503, y + 60);
            PlaceBookshelfArch(x + 529, y + 60);
            PlaceBookshelfArch(x + 555, y + 60);
            PlaceBookshelfArch(x + 503, y + 90);
            PlaceBookshelfArch(x + 529, y + 90);
            PlaceBookshelfArch(x + 555, y + 90);

            PlaceBookshelfArch(x + 670, y + 60);
            PlaceBookshelfArch(x + 696, y + 60);
            PlaceBookshelfArch(x + 722, y + 60);
            PlaceBookshelfArch(x + 670, y + 90);
            PlaceBookshelfArch(x + 696, y + 90);
            PlaceBookshelfArch(x + 722, y + 90);

            WorldGen.PlaceObject(x + 50, y + 55, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 43, y + 80, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 69, y + 80, ModContent.TileType<WaxCandleholder>());
            //PlaceAndConfigureDoor(x + 51, y + 80, DoorID.GreenRoomEntrance, DoorID.GreenRoom);

            WorldGen.PlaceObject(x + 50, y + 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 43, y + 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 69, y + 110, ModContent.TileType<WaxCandleholder>());
            //PlaceAndConfigureDoor(x + 51, y + 110, DoorID.RedRoomEntrance, DoorID.RedRoom);

            WorldGen.PlaceObject(x + 800, y + 55, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 793, y + 80, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 819, y + 80, ModContent.TileType<WaxCandleholder>());
            //PlaceAndConfigureDoor(x + 801, y + 80, DoorID.YellowRoomEntrance, DoorID.YellowRoom);

            WorldGen.PlaceObject(x + 800, y + 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 793, y + 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 819, y + 110, ModContent.TileType<WaxCandleholder>());
            //PlaceAndConfigureDoor(x + 801, y + 110, DoorID.BlueRoomEntrance, DoorID.BlueRoom);
            #endregion
        }

        private void PlaceLoungeArea(int x, int y, RoomID room)
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

        private void PlaceRoomBookshelfArches(int x, int y)
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

        private void PlaceCozyArea(int x, int y, RoomID room)
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
        private void PlaceBookshelfArch(int x, int y)
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

        private void PlaceBookshelfObjects(int x, int y)
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

        private void PlaceAndConfigureDoor(int x, int y, DoorID doorID, DoorID pairedDoor)
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