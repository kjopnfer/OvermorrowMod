using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Tiles.Archives;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.WorldGeneration.Archives
{
    public class GreenBridgeRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/GreenBridgeRoomTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/GreenBridgeRoomWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/GreenBridgeRoomObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        public override void PostGenerate(int x, int y)
        {
            #region Second Floor
            #region Upper Right Side
            WorldGen.PlaceObject(x + 944, y + 131, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 970, y + 131, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 951, y + 106, ModContent.TileType<WoodenArch>());

            PlaceAndConfigureDoor(x + 952, y + 131, DoorID.GreenBridgeRoomEntrance, DoorID.FoyerGreenRoomDoor);

            WorldGen.PlaceObject(x + 921, y + 117, ModContent.TileType<WaxSconce>());
            PlaceVaseGroup(x + 908, y + 122);
            PlaceVaseGroup(x + 920, y + 124);
            PlaceVaseGroup(x + 932, y + 126);

            PlaceBookShelfObjects(x + 848, y + 126);
            PlaceTableAndChair(x + 841, y + 126, -1, RoomID.Green);
            PlaceBookShelfObjects(x + 874, y + 126);
            PlaceTableAndChair(x + 867, y + 126, -1, RoomID.Green);
            PlaceBookShelfObjects(x + 848, y + 96);
            PlaceTableAndChair(x + 893, y + 126, -1, RoomID.Green);
            PlaceBookShelfObjects(x + 874, y + 96);

            WorldGen.PlaceObject(x + 785, y + 106, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 831, y + 106, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 903, y + 106, ModContent.TileType<ArchiveBanner>());

            PlaceCozyArea(x + 796, y + 126, RoomID.Green);

            #region Second Floor
            WorldGen.PlaceObject(x + 785, y + 76, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 831, y + 76, ModContent.TileType<ArchiveBanner>());

            PlaceLoungeArea(x + 798, y + 96, RoomID.Green);
            PlaceBookShelfObjects(x + 848, y + 96);
            PlaceTableAndChair(x + 840, y + 96, 1, RoomID.Green);
            PlaceBookShelfObjects(x + 876, y + 96);
            PlaceTableAndChair(x + 866, y + 96, 1, RoomID.Green);
            PlaceTableAndChair(x + 892, y + 96, 1, RoomID.Green);

            WorldGen.PlaceObject(x + 921, y + 87, ModContent.TileType<WaxSconce>());
            PlaceVaseGroup(x + 908, y + 92);
            PlaceVaseGroup(x + 920, y + 94);
            PlaceVaseGroup(x + 932, y + 96);

            PlaceLoungeArea(x + 946, y + 101, RoomID.Green);
            #endregion

            #region Third Floor
            WorldGen.PlaceObject(x + 785, y + 46, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 831, y + 46, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 796, y + 66, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 822, y + 66, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 803, y + 41, ModContent.TileType<WoodenArch>());
            PlaceAndConfigureDoor(x + 804, y + 66, DoorID.GreenBridgeTreasureEntrance, DoorID.GreenBridgeRoomExit, isLocked: true);
            PlaceLoungeArea(x + 844, y + 66, RoomID.Green);
            #endregion
            #region Stairs
            WorldGen.PlaceObject(x + 751, y + 116, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 776, y + 116, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 751, y + 86, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 776, y + 86, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 751, y + 56, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 776, y + 56, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 758, y + 41, ModContent.TileType<WoodenArch>());
            PlaceDiagonalStairStack(x + 758, y + 126, 6);
            #endregion

            #endregion

            #region Middle Side
            WorldGen.PlaceObject(x + 741, y + 106, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 741, y + 76, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 731, y + 119, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 714, y + 109, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 724, y + 79, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 707, y + 72, ModContent.TileType<WaxSconce>());

            PlaceMultiVase(x + 705, y + 106, -1, 5);
            PlaceMultiVase(x + 715, y + 76, -1, 5);

            WorldGen.PlaceObject(x + 696, y + 59, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 696, y + 89, ModContent.TileType<ArchiveBanner>());

            for (int i = 0; i < 4; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 589 + offset, y + 79);
                PlaceBookShelfObjects(x + 589 + offset, y + 109);

                PlaceTableAndChair(x + 582 + offset, y + 109, -1, RoomID.Green);
            }

            WorldGen.PlaceObject(x + 572, y + 59, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 572, y + 89, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 562, y + 102, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 545, y + 92, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 555, y + 62, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 538, y + 55, ModContent.TileType<WaxSconce>());

            PlaceMultiVase(x + 536, y + 89, -1, 5);
            PlaceMultiVase(x + 546, y + 59, -1, 5);

            WorldGen.PlaceObject(x + 527, y + 72, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 527, y + 42, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 455, y + 72, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 455, y + 42, ModContent.TileType<ArchiveBanner>());

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 472 + offset, y + 92);
                PlaceBookShelfObjects(x + 472 + offset, y + 62);

                PlaceTableAndChair(x + 465 + offset, y + 92, -1, RoomID.Green);
            }
            #endregion

            #region Upper Left Side
            WorldGen.PlaceObject(x + 416, y + 63, ModContent.TileType<ArchiveBridge>());
            WorldGen.PlaceObject(x + 329, y + 63, ModContent.TileType<ArchiveBridge>());

            #region Staircase
            WorldGen.PlaceObject(x + 367, y + 142, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 392, y + 142, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 367, y + 112, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 392, y + 112, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 367, y + 82, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 392, y + 82, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 367, y + 52, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 392, y + 52, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 374, y + 37, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 399, y + 37, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 349, y + 37, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 349, y + 67, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 399, y + 67, ModContent.TileType<WoodenArch>());

            PlaceDiagonalStairStack(x + 374, y + 152, 9);
            #endregion

            WorldGen.PlaceObject(x + 456, y + 92, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 456, y + 62, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 303, y + 42, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 304, y + 62, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 303, y + 72, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 304, y + 92, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 413, y + 92, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 433, y + 62, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 436, y + 67, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 433, y + 92, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 436, y + 37, ModContent.TileType<WoodenArch>());

            PlaceLoungeArea(x + 209, y + 62, RoomID.Green);

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 248 + offset, y + 62);
                PlaceBookShelfObjects(x + 248 + offset, y + 92);

                PlaceTableAndChair(x + 241 + offset, y + 62, -1, RoomID.Green);
                PlaceTableAndChair(x + 241 + offset, y + 92, -1, RoomID.Green);
            }

            PlaceLongTableAndChairs(x + 313, y + 62, RoomID.Green);
            PlaceLongTableAndChairs(x + 350, y + 62, RoomID.Green);
            PlaceLongTableAndChairs(x + 400, y + 62, RoomID.Green);
            PlaceLongTableAndChairs(x + 437, y + 62, RoomID.Green);

            WorldGen.PlaceObject(x + 326, y + 92, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 312, y + 37, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 309, y + 62, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 349, y + 37, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 346, y + 92, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 312, y + 67, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 346, y + 62, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 349, y + 67, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 207, y + 92, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 233, y + 92, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 214, y + 67, ModContent.TileType<WoodenArch>());
            PlaceAndConfigureDoor(x + 215, y + 92, DoorID.GreenBridgeRoomExit, DoorID.WaxheadRoomEntrance);
            #endregion
            #endregion

            #region First Floor
            WorldGen.PlaceObject(x + 318, y + 178, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 331, y + 172, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 343, y + 160, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 355, y + 148, ModContent.TileType<WaxSconce>());

            PlaceMultiVase(x + 326, y + 181, 1, 9);

            WorldGen.PlaceObject(x + 441, y + 178, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 428, y + 172, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 416, y + 160, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 404, y + 148, ModContent.TileType<WaxSconce>());

            PlaceMultiVase(x + 399, y + 149, -1, 9);

            #region Broken Bridge
            WorldGen.PlaceObject(x + 296, y + 185, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 299, y + 160, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 262, y + 160, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 212, y + 160, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 255, y + 175, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 209, y + 185, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 175, y + 160, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 143, y + 175, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 168, y + 175, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 122, y + 185, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 125, y + 160, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 88, y + 160, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 56, y + 175, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 81, y + 175, ModContent.TileType<WaxSconce>());

            PlaceDiagonalStairStack(x + 237, y + 204, 2);
            #endregion

            WorldGen.PlaceObject(x + 451, y + 165, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 523, y + 165, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 569, y + 165, ModContent.TileType<ArchiveBanner>());

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 468 + offset, y + 185);
                PlaceTableAndChair(x + 460 + offset, y + 185, 1, RoomID.Green);
            }

            PlaceCozyArea(x + 534, y + 185, RoomID.Green);

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 586 + offset, y + 185);
                PlaceTableAndChair(x + 578 + offset, y + 185, 1, RoomID.Green);
            }
            #endregion
        }
    }
}