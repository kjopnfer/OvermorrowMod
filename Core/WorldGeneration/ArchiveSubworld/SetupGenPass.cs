using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
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

            Texture2D tiles = ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveTiles", AssetRequestMode.ImmediateLoad).Value;
            Texture2D walls = ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveWalls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D slopes = ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveWalls", AssetRequestMode.ImmediateLoad).Value;
            Texture2D objects = ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveObjects", AssetRequestMode.ImmediateLoad).Value;

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
                [new Color(223, 113, 38)] = (ModContent.TileType<FloorCandles>(), 3),
                [new Color(114, 70, 123)] = (ModContent.TileType<Moose>(), 1),
                [new Color(74, 15, 56)] = (ModContent.TileType<WoodenPillar>(), 1),
                [new Color(179, 36, 136)] = (ModContent.TileType<WoodenPillar2>(), 1),
                [new Color(198, 74, 118)] = (ModContent.TileType<WoodenArch>(), 1),
                [new Color(135, 28, 66)] = (ModContent.TileType<WoodenArchL1>(), 1),
                [new Color(176, 16, 73)] = (ModContent.TileType<WoodenArchL2>(), 1),
                [new Color(189, 44, 95)] = (ModContent.TileType<WoodenArchL3>(), 1),
                [new Color(88, 68, 75)] = (ModContent.TileType<WoodenArchR1>(), 1),
                [new Color(79, 38, 52)] = (ModContent.TileType<WoodenArchR2>(), 1),
                [new Color(88, 13, 39)] = (ModContent.TileType<WoodenArchR3>(), 1),
                [new Color(171, 73, 94)] = (ModContent.TileType<WoodenArchSmall>(), 1),
                [new Color(159, 131, 65)] = (ModContent.TileType<Candelabra>(), 1),
                [new Color(134, 42, 104)] = (ModContent.TileType<SmallChair>(), 1),
                [new Color(128, 50, 1)] = (ModContent.TileType<Napoleon>(), 1),
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
            WorldGen.PlaceObject(307, 111, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(363, 111, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(404, 111, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(329, 116, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(338, 115, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(347, 116, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(328, 131, ModContent.TileType<Napoleon>());
            WorldGen.PlaceObject(347, 131, ModContent.TileType<Bismarck>());

            WorldGen.PlaceObject(337, 131, ModContent.TileType<WoodenArchSmall>());
            WorldGen.PlaceObject(337, 127, ModContent.TileType<Moose>());
            WorldGen.PlaceObject(337, 113, ModContent.TileType<WoodenArchSmall>());

            #region Center Room
            WorldGen.PlaceObject(852, 112, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(872, 112, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(852, 142, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(872, 142, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(1123, 112, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(1123, 142, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(1143, 112, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(1143, 142, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(1002, 142, ModContent.TileType<FireplacePillar>());

            WorldGen.PlaceObject(993, 112, ModContent.TileType<SanctumGate>());

            WorldGen.PlaceObject(855, 113, ModContent.TileType<ArchiveBridge>());
            WorldGen.PlaceObject(1126, 113, ModContent.TileType<ArchiveBridge>());

            PlaceAndConfigureDoor(619, 80, DoorID.GreenRoomEntrance, DoorID.GreenRoom);
            PlaceAndConfigureDoor(619, 110, DoorID.RedRoomEntrance, DoorID.RedRoom);
            PlaceAndConfigureDoor(1369, 80, DoorID.YellowRoomEntrance, DoorID.YellowRoom);
            PlaceAndConfigureDoor(1369, 110, DoorID.BlueRoomEntrance, DoorID.BlueRoom);

            #endregion

            #region Top Left Room
            PlaceAndConfigureDoor(492, 110, DoorID.GreenRoom, DoorID.GreenRoomEntrance);


            #endregion

            #region Bottom Left Room
            PlaceAndConfigureDoor(494, 256, DoorID.RedRoom, DoorID.RedRoomEntrance);

            #endregion

            #region Top Right Room
            PlaceAndConfigureDoor(1496, 110, DoorID.YellowRoom, DoorID.YellowRoomEntrance);

            #endregion

            #region Bottom Right Room
            PlaceAndConfigureDoor(1494, 256, DoorID.BlueRoom, DoorID.BlueRoomEntrance);

            #endregion

        }

        private void PlaceAndConfigureDoor(int x, int y, DoorID doorID, DoorID pairedDoor)
        {
            // Place the object (door)
            WorldGen.PlaceObject(x, y, ModContent.TileType<ArchiveDoor>());

            // Get the TileEntity associated with the placed object
            int id = ModContent.GetInstance<ArchiveDoor_TE>().Place(x, y);
            ArchiveDoor_TE te = TileEntity.ByID[id] as ArchiveDoor_TE;

            // Configure the door and its paired door ID
            if (te != null)
            {
                te.DoorID = (int)doorID;
                te.PairedDoor = (int)pairedDoor;

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