using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Content.Tiles.Archives;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldGeneration.ArchiveSubworld
{
    public class SetupGenPass : GenPass
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

        public SetupGenPass(string name, double loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating tiles";
            Main.spawnTileX = 1000;
            Main.spawnTileY = 110;

            // Move the backgrounds somewhere else
            Main.worldSurface = ArchiveSubworld.GetHeight();
            Main.rockLayer = ArchiveSubworld.GetHeight();

            Texture2D tiles = ModContent.Request<Texture2D>(AssetDirectory.TexGen + "ArchiveTiles", AssetRequestMode.ImmediateLoad).Value;
            Texture2D walls = ModContent.Request<Texture2D>(AssetDirectory.TexGen + "ArchiveWalls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D slopes = ModContent.Request<Texture2D>(AssetDirectory.TexGen + "ArchiveWalls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D objects = ModContent.Request<Texture2D>(AssetDirectory.TexGen + "ArchiveObjects", AssetRequestMode.ImmediateLoad).Value;

            Dictionary<Color, int> tileMapping = new()
            {
                [new Color(105, 106, 106)] = ModContent.TileType<CastleBrick>(),
                [new Color(89, 86, 82)] = ModContent.TileType<DarkCastleBrick>(),
                [new Color(138, 111, 48)] = ModContent.TileType<CastlePlatform>(),
                [new Color(143, 86, 59)] = TileID.WoodBlock,
                [new Color(74, 47, 33)] = ModContent.TileType<ArchiveWood>(),
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Dictionary<Color, int> wallMapping = new()
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

            Dictionary<Color, (int objectId, int styleRange)> objectMapping = new()
            {
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

            SystemUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = TexGen.GetTexGenerator(tiles, tileMapping, walls, wallMapping, null, null, objects, objectMapping);
                gen.Generate(0, 0, true, true);
            });

            // Run TexGen a second time for just the objects.
            // Object anchors do not behave properly for whatever reason if done within the first pass.
            SystemUtils.InvokeOnMainThread(() =>
            {
                TexGen objectGen = TexGen.GetTexGenerator(tiles, tileMapping, null, null, null, null, objects, objectMapping);
                objectGen.Generate(0, 0, true, true);
            });

            // ...yet I have to do it manually for these fucking things anyways because they don't work??
            WorldGen.PlaceObject(307, 111, ModContent.TileType<WoodenArchSplit>());



            WorldGen.PlaceObject(328, 131, ModContent.TileType<Napoleon>());

            #region Center Room
            PlaceBookshelfArch(696, 60);
            PlaceBookshelfArch(722, 60);
            PlaceBookshelfArch(748, 60);
            PlaceBookshelfArch(696, 90);
            PlaceBookshelfArch(722, 90);
            PlaceBookshelfArch(748, 90);

            PlaceBookshelfArch(863, 60);
            PlaceBookshelfArch(889, 60);
            PlaceBookshelfArch(915, 60);
            PlaceBookshelfArch(863, 90);
            PlaceBookshelfArch(889, 90);
            PlaceBookshelfArch(915, 90);

            #region Left Bridge
            WorldGen.PlaceObject(773, 60, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(815, 86, ModContent.TileType<ArchiveBridge>());

            WorldGen.PlaceObject(812, 115, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(832, 85, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(832, 115, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(798, 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(835, 60, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(798, 90, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(835, 90, ModContent.TileType<WoodenArch>());

            #endregion

            #region Fireplace
            PlaceCozyArea(986, 110, RoomID.Yellow);

            WorldGen.PlaceObject(993, 55, ModContent.TileType<WoodenArch>());
            TileUtils.PlaceTileWithEntity<SanctumGate, SanctumGate_TE>(993, 80);
            #endregion

            #region Right Bridge
            WorldGen.PlaceObject(1168, 86, ModContent.TileType<ArchiveBridge>());

            WorldGen.PlaceObject(1213, 60, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(1165, 115, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(1185, 85, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(1185, 115, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(1151, 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(1188, 60, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(1151, 90, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(1188, 90, ModContent.TileType<WoodenArch>());

            #endregion

            PlaceBookshelfArch(1071, 60);
            PlaceBookshelfArch(1097, 60);
            PlaceBookshelfArch(1123, 60);
            PlaceBookshelfArch(1071, 90);
            PlaceBookshelfArch(1097, 90);
            PlaceBookshelfArch(1123, 90);

            PlaceBookshelfArch(1238, 60);
            PlaceBookshelfArch(1264, 60);
            PlaceBookshelfArch(1290, 60);
            PlaceBookshelfArch(1238, 90);
            PlaceBookshelfArch(1264, 90);
            PlaceBookshelfArch(1290, 90);

            WorldGen.PlaceObject(618, 55, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(611, 80, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(637, 80, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(619, 80, DoorID.GreenRoomEntrance, DoorID.GreenRoom);

            WorldGen.PlaceObject(618, 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(611, 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(637, 110, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(619, 110, DoorID.RedRoomEntrance, DoorID.RedRoom);

            WorldGen.PlaceObject(1368, 55, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(1361, 80, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(1387, 80, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(1369, 80, DoorID.YellowRoomEntrance, DoorID.YellowRoom);

            WorldGen.PlaceObject(1368, 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(1361, 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(1387, 110, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(1369, 110, DoorID.BlueRoomEntrance, DoorID.BlueRoom);
            #endregion

            #region Top Left Room
            WorldGen.PlaceObject(491, 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(484, 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(510, 110, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(492, 110, DoorID.GreenRoom, DoorID.GreenRoomEntrance);

            PlaceBookshelfArch(27, 55);
            PlaceBookshelfArch(53, 55);
            PlaceBookshelfArch(79, 55);
            PlaceBookshelfArch(27, 85);
            PlaceBookshelfArch(53, 85);
            PlaceBookshelfArch(79, 85);

            PlaceBookshelfArch(194, 60);
            PlaceBookshelfArch(220, 60);
            PlaceBookshelfArch(246, 60);
            PlaceBookshelfArch(194, 90);
            PlaceBookshelfArch(220, 90);
            PlaceBookshelfArch(246, 90);

            #region Bridge
            WorldGen.PlaceObject(271, 60, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(296, 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(296, 90, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(333, 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(333, 90, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(310, 115, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(330, 85, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(330, 115, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(313, 86, ModContent.TileType<ArchiveBridge>());

            #endregion

            PlaceBookshelfArch(361, 60);
            PlaceBookshelfArch(387, 60);
            PlaceBookshelfArch(413, 60);
            PlaceBookshelfArch(361, 90);
            PlaceBookshelfArch(387, 90);
            PlaceBookshelfArch(413, 90);

            PlaceLoungeArea(486, 80, RoomID.Green);
            PlaceCozyArea(109, 110, RoomID.Green);
            PlaceLoungeArea(111, 80, RoomID.Green);

            #endregion

            #region Bottom Left Room
            WorldGen.PlaceObject(493, 231, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(486, 256, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(512, 256, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(494, 256, DoorID.RedRoom, DoorID.RedRoomEntrance);

            PlaceRoomBookshelfArches(29, 201);

            #region Bridge
            WorldGen.PlaceObject(273, 206, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(298, 206, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(298, 236, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(335, 206, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(335, 236, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(312, 261, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(332, 231, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(332, 261, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(315, 232, ModContent.TileType<ArchiveBridge>());

            #endregion

            WorldGen.PlaceObject(300, 261, ModContent.TileType<LeftShelf>());
            WorldGen.PlaceObject(319, 261, ModContent.TileType<WizardStatue>());
            WorldGen.PlaceObject(337, 261, ModContent.TileType<RightShelf>());

            PlaceLoungeArea(113, 226, RoomID.Red);
            PlaceCozyArea(111, 256, RoomID.Red);
            PlaceLoungeArea(488, 226, RoomID.Red);

            #endregion

            #region Top Right Room
            WorldGen.PlaceObject(1495, 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(1488, 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(1514, 110, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(1496, 110, DoorID.YellowRoom, DoorID.YellowRoomEntrance);

            #region Bridge
            WorldGen.PlaceObject(1715, 60, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(1653, 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(1653, 90, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(1690, 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(1690, 90, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(1687, 85, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(1667, 115, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(1687, 115, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(1670, 86, ModContent.TileType<ArchiveBridge>());
            #endregion

            PlaceLoungeArea(1490, 80, RoomID.Yellow);
            PlaceCozyArea(1863, 110, RoomID.Yellow);
            PlaceLoungeArea(1865, 80, RoomID.Yellow);

            #endregion

            #region Bottom Right Room
            WorldGen.PlaceObject(1493, 231, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(1486, 256, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(1512, 256, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(1494, 256, DoorID.BlueRoom, DoorID.BlueRoomEntrance);

            #region Bridge
            WorldGen.PlaceObject(1713, 206, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(1651, 206, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(1651, 236, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(1688, 206, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(1688, 236, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(1665, 231, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(1665, 261, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(1685, 231, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(1685, 261, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(1668, 232, ModContent.TileType<ArchiveBridge>());
            #endregion

            PlaceLoungeArea(1488, 226, RoomID.Blue);
            PlaceCozyArea(1861, 256, RoomID.Blue);
            PlaceLoungeArea(1863, 226, RoomID.Blue);
            #endregion

            SetupSpawners();
        }

        private void SetupSpawners()
        {
            ArchiveSubworld.CenterRoom.AddSpawnPoint(new Vector2(890, 106), ModContent.NPCType<BlasterBook>());
            ArchiveSubworld.CenterRoom.AddSpawnPoint(new Vector2(920, 115), ModContent.NPCType<ArchiveRat>());

            ArchiveSubworld.CenterRoom.AddSpawnPoint(new Vector2(754, 76), ModContent.NPCType<InkWormBody>());
            ArchiveSubworld.CenterRoom.AddSpawnPoint(new Vector2(749, 115), ModContent.NPCType<ArchiveRat>());

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