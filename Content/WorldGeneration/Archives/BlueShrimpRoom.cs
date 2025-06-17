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
    public class BlueShrimpRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/BlueShrimpRoomTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/BlueShrimpRoomWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/BlueShrimpRoomObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        public override void PostGenerate(int x, int y)
        {
            #region Upper Floor
            PlaceDoorObjects(x + 161, y + 64);
            PlaceAndConfigureDoor(x + 161, y + 64, DoorID.BlueShrimpRoomEntrance, DoorID.FoyerBlueRoomDoor);

            PlaceLongTableAndChairs(x + 228, y + 69, RoomID.Blue);

            WorldGen.PlaceObject(x + 188, y + 44, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 247, y + 49, ModContent.TileType<ArchiveBanner>(), true);

            WorldGen.PlaceObject(x + 206, y + 54, ModContent.TileType<WaxSconce>(), true);

            PlaceVaseGroup(x + 193, y + 60);
            PlaceVaseGroup(x + 205, y + 62);
            PlaceVaseGroup(x + 217, y + 64);

            WorldGen.PlaceObject(x + 275, y + 66, ModContent.TileType<WaxSconce>(), true);
            PlaceMultiVase(x + 266, y + 66, -1, 5);
            WorldGen.PlaceObject(x + 292, y + 66, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 292, y + 96, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 292, y + 126, ModContent.TileType<ArchiveBanner>(), true);

            WorldGen.PlaceObject(x + 309, y + 61, ModContent.TileType<WoodenArch>(), true);

            WorldGen.PlaceObject(x + 302, y + 76, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 327, y + 76, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 302, y + 106, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 327, y + 106, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 302, y + 136, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 327, y + 136, ModContent.TileType<WaxSconce>(), true);
            PlaceDiagonalStairStack(x + 309, y + 146, 6);

            #region Bridge
            PlaceLongTableAndChairs(x + 335, y + 116, RoomID.Blue);
            PlaceLongTableAndChairs(x + 372, y + 116, RoomID.Blue);
            PlaceLongTableAndChairs(x + 335, y + 146, RoomID.Blue);
            PlaceLongTableAndChairs(x + 372, y + 146, RoomID.Blue);

            WorldGen.PlaceObject(x + 368, y + 116, ModContent.TileType<WoodenPillar2>(), true);
            WorldGen.PlaceObject(x + 368, y + 146, ModContent.TileType<WoodenPillar2>(), true);

            WorldGen.PlaceObject(x + 334, y + 91, ModContent.TileType<WoodenArch>(), true);
            WorldGen.PlaceObject(x + 334, y + 121, ModContent.TileType<WoodenArch>(), true);
            WorldGen.PlaceObject(x + 371, y + 91, ModContent.TileType<WoodenArch>(), true);
            WorldGen.PlaceObject(x + 371, y + 121, ModContent.TileType<WoodenArch>(), true);

            WorldGen.PlaceObject(x + 351, y + 117, ModContent.TileType<ArchiveBridge>(), true);
            WorldGen.PlaceObject(x + 351, y + 147, ModContent.TileType<ArchiveBridge>(), true);

            WorldGen.PlaceObject(x + 390, y + 96, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 390, y + 126, ModContent.TileType<ArchiveBanner>(), true);

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 407 + offset, y + 116);
                PlaceBookShelfObjects(x + 407 + offset, y + 146);

                PlaceTableAndChair(x + 400 + offset, y + 116, -1, RoomID.Blue);
                PlaceTableAndChair(x + 400 + offset, y + 146, -1, RoomID.Blue);
            }

            WorldGen.PlaceObject(x + 462, y + 96, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 462, y + 126, ModContent.TileType<ArchiveBanner>(), true);

            PlaceLoungeArea(x + 475, y + 116, RoomID.Blue);
            PlaceCozyArea(x + 473, y + 146, RoomID.Blue);
            #endregion
            
            WorldGen.PlaceObject(x + 247, y + 113, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 247, y + 143, ModContent.TileType<ArchiveBanner>(), true);

            WorldGen.PlaceObject(x + 265, y + 113, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 275, y + 143, ModContent.TileType<WaxSconce>(), true);

            PlaceMultiVase(x + 256, y + 129, 1, 5);
            PlaceMultiVase(x + 266, y + 159, 1, 5);

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 192 + offset, y + 133);
                PlaceBookShelfObjects(x + 192 + offset, y + 163);

                PlaceTableAndChair(x + 185 + offset, y + 133, -1, RoomID.Blue);
                PlaceTableAndChair(x + 185 + offset, y + 163, -1, RoomID.Blue);
            }

            WorldGen.PlaceObject(x + 175, y + 113, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 175, y + 143, ModContent.TileType<ArchiveBanner>(), true);

            PlaceBookShelfObjects(x + 133, y + 133);

            PlaceLongTableAndChairs(x + 113, y + 163, RoomID.Blue);
            PlaceLongTableAndChairs(x + 156, y + 163, RoomID.Blue);
            PlaceLongTableAndChairs(x + 113, y + 133, RoomID.Blue);
            PlaceLongTableAndChairs(x + 156, y + 133, RoomID.Blue);

            WorldGen.PlaceObject(x + 103, y + 113, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 103, y + 143, ModContent.TileType<ArchiveBanner>(), true);

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 43 + offset, y + 133);
                PlaceBookShelfObjects(x + 43 + offset, y + 163);

                PlaceTableAndChair(x + 41 + offset, y + 133, -1, RoomID.Blue);
                PlaceTableAndChair(x + 41 + offset, y + 163, -1, RoomID.Blue);
            }

            WorldGen.PlaceObject(x + 140, y + 139, ModContent.TileType<WaxChandelier>(), true);
            PlaceStairGroup(x + 134, y + 176, 0);
            PlaceHallwayArch(x + 112, y + 176);
            PlaceHallwayArch(x + 120, y + 176);
            #endregion

            #region Middle Left
            PlaceHallwayArch(x + 92, y + 205);
            PlaceHallwayArch(x + 115, y + 205);
            PlaceHallwayArch(x + 138, y + 205);
            PlaceHallwayArch(x + 161, y + 205);

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 38 + offset, y + 205);
            }

            PlaceTableAndChair(x + 57, y + 205, -1, RoomID.Blue);

            PlaceMultiVase(x + 151, y + 173, -1, 5);
            WorldGen.PlaceObject(x + 160, y + 173, ModContent.TileType<WaxSconce>(), true);

            WorldGen.PlaceObject(x + 165, y + 205, ModContent.TileType<WaxCandleholder>(), true);
            WorldGen.PlaceObject(x + 124, y + 176, ModContent.TileType<WaxCandleholder>(), true);
            PlaceStairGroup(x + 175, y + 205, 1);
            PlaceMultiVase(x + 192, y + 202, -1, 7);

            PlaceDoorObjects(x + 115, y + 296);
            PlaceAndConfigureDoor(x + 115, y + 296, DoorID.BlueShrimpTreasureEntrance, DoorID.BlueShrimpTreasureExit, isLocked: true);

            PlaceStairGroup(x + 224, y + 242, 1);
            PlaceMultiVase(x + 200, y + 255, 1, 5);
            WorldGen.PlaceObject(x + 209, y + 239, ModContent.TileType<WaxSconce>(), true);
            PlaceStairGroup(x + 183, y + 271, -1);

            WorldGen.PlaceObject(x + 209, y + 268, ModContent.TileType<WaxSconce>(), true);
            PlaceMultiVase(x + 200, y + 268, -1, 5);

            WorldGen.PlaceObject(x + 142, y + 276, ModContent.TileType<ArchiveBanner>(), true);

            WorldGen.PlaceObject(x + 214, y + 300, ModContent.TileType<WaxCandleholder>(), true);
            PlaceHallwayArch(x + 202, y + 300);
            PlaceHallwayArch(x + 210, y + 300);
            WorldGen.PlaceObject(x + 214, y + 300, ModContent.TileType<WaxSconce>(), true);
            PlaceStairGroup(x + 224, y + 300, 1);

            PlaceMultiVase(x + 241, y + 297, -1, 7);
            PlaceMultiVase(x + 151, y + 292, 1, 7);
            #endregion

            #region Lower Left Floor
            WorldGen.PlaceObject(x + 285, y + 315, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 310, y + 315, ModContent.TileType<WaxSconce>(), true);
            PlaceTallStairs(x + 284, y + 355);

            WorldGen.PlaceObject(x + 147, y + 340, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 245, y + 340, ModContent.TileType<ArchiveBanner>(), true);
            PlaceLoungeArea(x + 114, y + 360, RoomID.Blue);

            for (int i = 0; i < 3; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 164 + offset, y + 360);

                PlaceTableAndChair(x + 157 + offset, y + 360, -1, RoomID.Blue);
            }

            WorldGen.PlaceObject(x + 269, y + 345, ModContent.TileType<WaxSconce>(), true);
            PlaceVaseGroup(x + 256, y + 355);
            PlaceVaseGroup(x + 268, y + 353);
            PlaceVaseGroup(x + 280, y + 351);

            WorldGen.PlaceObject(x + 326, y + 345, ModContent.TileType<WaxSconce>(), true);
            PlaceVaseGroup(x + 337, y + 355);
            PlaceVaseGroup(x + 325, y + 353);
            PlaceVaseGroup(x + 313, y + 351);

            WorldGen.PlaceObject(x + 280, y + 295, ModContent.TileType<Finial>(), true);
            WorldGen.PlaceObject(x + 295, y + 295, ModContent.TileType<NormalWizardStatue>(), true);
            WorldGen.PlaceObject(x + 315, y + 295, ModContent.TileType<Finial>(), true);
            PlaceMultiVase(x + 328, y + 321, 1, 7);

            WorldGen.PlaceObject(x + 349, y + 340, ModContent.TileType<ArchiveBanner>(), true);

            for (int i = 0; i < 3; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 366 + offset, y + 360);

                PlaceTableAndChair(x + 358 + offset, y + 360, 1, RoomID.Blue);
            }

            PlaceTableAndChair(x + 410, y + 360, 1, RoomID.Blue);
            #endregion

            #region Lower Right Floor
            PlaceStairGroup(x + 360, y + 300, -1);
            for (int i = 0; i < 3; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 395 + offset, y + 288);

                PlaceTableAndChair(x + 388 + offset, y + 288, 1, RoomID.Blue);
            }

            WorldGen.PlaceObject(x + 476, y + 268, ModContent.TileType<ArchiveBanner>(), true);
            PlaceLoungeArea(x + 489, y + 288, RoomID.Blue);
            WorldGen.PlaceObject(x + 522, y + 238, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 522, y + 268, ModContent.TileType<ArchiveBanner>(), true);

            WorldGen.PlaceObject(x + 566, y + 238, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 566, y + 268, ModContent.TileType<ArchiveBanner>(), true);

            WorldGen.PlaceObject(x + 638, y + 238, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 638, y + 268, ModContent.TileType<ArchiveBanner>(), true);

            WorldGen.PlaceObject(x + 684, y + 238, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 684, y + 268, ModContent.TileType<ArchiveBanner>(), true);

            for (int i = 0; i < 4; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 378 + offset, y + 300);
            }

            WorldGen.PlaceObject(x + 429, y + 301, ModContent.TileType<WaxSconce>(), true);
            PlaceMultiVase(x + 416, y + 297, -1, 7);

            PlaceHallwayArch(x + 434, y + 337);
            WorldGen.PlaceObject(x + 438, y + 337, ModContent.TileType<WaxCandleholder>(), true);
            PlaceStairGroup(x + 448, y + 337, 0);

            PlaceHallwayArch(x + 466, y + 337);
            PlaceHallwayArch(x + 489, y + 337);
            PlaceHallwayArch(x + 512, y + 337);
            PlaceHallwayArch(x + 535, y + 337);

            PlaceMultiVase(x + 549, y + 334, -1, 7);
            WorldGen.PlaceObject(x + 583, y + 342, ModContent.TileType<ArchiveBanner>(), true);

            for (int i = 0; i < 3; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 600 + offset, y + 332);
                PlaceBookShelfObjects(x + 600 + offset, y + 362);

                PlaceTableAndChair(x + 592 + offset, y + 332, 1, RoomID.Blue);
                PlaceTableAndChair(x + 592 + offset, y + 362, 1, RoomID.Blue);
            }

            PlaceTallStairs(x + 670, y + 362);

            PlaceDoorObjects(x + 495, y + 258);
            PlaceAndConfigureDoor(x + 495, y + 258, DoorID.BlueShrimpSecondTreasureEntrance, DoorID.BlueShrimpSecondTreasureExit, isLocked: true);
            PlaceTallStairs(x + 531, y + 288);
            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 583 + offset, y + 258);
                PlaceBookShelfObjects(x + 583 + offset, y + 288);

                PlaceTableAndChair(x + 575 + offset, y + 258, 1, RoomID.Blue);
                PlaceTableAndChair(x + 575 + offset, y + 288, 1, RoomID.Blue);
            }

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 472 + offset, y + 325);
                PlaceTableAndChair(x + 465 + offset, y + 325, 1, RoomID.Blue);
            }

            PlaceDoorObjects(x + 657, y + 258);
            PlaceAndConfigureDoor(x + 657, y + 258, DoorID.BlueShrimpRoomExit, DoorID.BlueRoom);
            PlaceCozyArea(x + 649, y + 288, RoomID.Blue);

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 701 + offset, y + 258);
                PlaceBookShelfObjects(x + 701 + offset, y + 288);

                PlaceTableAndChair(x + 693 + offset, y + 258, 1, RoomID.Blue);
                PlaceTableAndChair(x + 693 + offset, y + 288, 1, RoomID.Blue);
            }
            #endregion
        }
    }
}