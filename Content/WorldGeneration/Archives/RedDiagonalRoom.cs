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
    public class RedDiagonalRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/RedDiagonalRoomTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/RedDiagonalRoomWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/RedDiagonalRoomObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        /// <summary>
        /// Got tired of trying to figure out the correct offset from the ceiling.
        /// </summary>
        protected void PlaceBanner(int x, int y)
        {
            WorldGen.PlaceObject(x, y - 20, ModContent.TileType<ArchiveBanner>(), true);
        }

        protected void PlaceBannersBothFloors(int x, int y)
        {
            WorldGen.PlaceObject(x, y - 20, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x, y - 50, ModContent.TileType<ArchiveBanner>(), true);
        }

        public override void PostGenerate(int x, int y)
        {
            #region Bottom Right
            PlaceDoorObjects(x + 722, y + 273);
            PlaceAndConfigureDoor(x + 722, y + 273, DoorID.RedDiagonalRoomEntrance, DoorID.RedLongRoomExit);

            PlaceBanner(x + 703, y + 273);

            WorldGen.PlaceObject(x + 678, y + 248, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 690, y + 260, ModContent.TileType<WaxSconce>(), true);

            PlaceMultiVase(x + 669, y + 245, -1, 7);

            WorldGen.PlaceObject(x + 642, y + 248, ModContent.TileType<WaxCandleholder>(), true);
            for (int i = 0; i < 2; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 630 + offset, y + 248);
            }
            PlaceStairGroup(x + 652, y + 248, 1);

            WorldGen.PlaceObject(x + 637, y + 219, ModContent.TileType<WaxSconce>(), true);
            PlaceMultiVase(x + 628, y + 216, -1, 5);

            for (int i = 0; i < 3; i++)
            {
                var offset = 23 * i;
                PlaceHallwayArch(x + 553 + offset, y + 219);
            }

            PlaceHallwayArch(x + 521, y + 219);
            WorldGen.PlaceObject(x + 525, y + 219, ModContent.TileType<WaxCandleholder>(), true);
            PlaceStairGroup(x + 535, y + 219, 0);

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 559 + offset, y + 207);
                PlaceTableAndChair(x + 577 + offset, y + 207, 1, RoomID.Red);
            }

            WorldGen.PlaceObject(x + 512, y + 182, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 524, y + 194, ModContent.TileType<WaxSconce>(), true);
            PlaceMultiVase(x + 503, y + 179, -1, 7);

            for (int i = 0; i < 4; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 465 + offset, y + 182);
            }

            WorldGen.PlaceObject(x + 437, y + 182, ModContent.TileType<WaxCandleholder>(), true);
            for (int i = 0; i < 2; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 425 + offset, y + 182);
            }

            PlaceStairGroup(x + 447, y + 182, 0);
            #endregion

            #region Middle
            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 471 + offset, y + 170);
            }
            PlaceTableAndChair(x + 489, y + 170, 1, RoomID.Red);

            PlaceDoorObjects(x + 525, y + 170);
            PlaceAndConfigureDoor(x + 525, y + 170, DoorID.RedDiagonalTreasureEntrance, DoorID.RedDiagonalTreasureExit, isLocked: true);

            WorldGen.PlaceObject(x + 432, y + 153, ModContent.TileType<WaxSconce>(), true);
            PlaceMultiVase(x + 423, y + 150, -1, 5);

            for (int i = 0; i < 3; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 285 + offset, y + 178);
                PlaceTableAndChair(x + 278 + offset, y + 178, -1, RoomID.Red);
            }

            PlaceMultiVase(x + 374, y + 174, 1, 7);
            PlaceStairGroup(x + 406, y + 153, -1);

            WorldGen.PlaceObject(x + 432, y + 124, ModContent.TileType<WaxSconce>(), true);
            PlaceMultiVase(x + 423, y + 137, 1, 5);

            WorldGen.PlaceObject(x + 469, y + 124, ModContent.TileType<WaxCandleholder>(), true);
            for (int i = 0; i < 3; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 465 + offset, y + 124);
            }

            PlaceStairGroup(x + 447, y + 124, 0);
            PlaceTallStairs(x + 464, y + 112);

            PlaceLoungeArea(x + 504, y + 82, RoomID.Red);
            PlaceDoorObjects(x + 510, y + 112);
            PlaceAndConfigureDoor(x + 510, y + 112, DoorID.RedDiagonalSecondTreasureEntrance, DoorID.RedDiagonalSecondTreasureExit, isLocked: true);

            for (int i = 0; i < 3; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 369 + offset, y + 112);
                PlaceBookShelfObjects(x + 369 + offset, y + 82);

                PlaceTableAndChair(x + 362 + offset, y + 112, -1, RoomID.Red);
                PlaceTableAndChair(x + 362 + offset, y + 82, -1, RoomID.Red);
            }


            #endregion

            #region Top Left
            PlaceBannersBothFloors(x + 352, y + 112);
            PlaceBannersBothFloors(x + 311, y + 117);

            WorldGen.PlaceObject(x + 335, y + 103, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 335, y + 73, ModContent.TileType<WaxSconce>(), true);
            for (int i = 0; i < 3; i++)
            {
                var xOffset = 12 * i;
                var yOffset = 2 * i;

                PlaceVaseGroup(x + 322 + xOffset, y + 82 - yOffset);
                PlaceVaseGroup(x + 322 + xOffset, y + 112 - yOffset);
            }

            PlaceLoungeArea(x + 278, y + 117, RoomID.Red);
            PlaceCozyArea(x + 276, y + 87, RoomID.Red);

            PlaceBanner(x + 265, y + 87);
            WorldGen.PlaceObject(x + 242, y + 73, ModContent.TileType<WaxSconce>(), true);
            for (int i = 0; i < 3; i++)
            {
                var xOffset = 12 * i;
                var yOffset = 2 * i;

                PlaceVaseGroup(x + 229 + xOffset, y + 78 + yOffset);
            }
            PlaceBanner(x + 224, y + 82);

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 169 + offset, y + 82);
                PlaceTableAndChair(x + 162 + offset, y + 82, -1, RoomID.Red);
            }
            PlaceTableAndChair(x + 214, y + 82, -1, RoomID.Red);

            PlaceBanner(x + 152, y + 82);

            WorldGen.PlaceObject(x + 135, y + 65, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 118, y + 58, ModContent.TileType<WaxSconce>(), true);
            PlaceMultiVase(x + 126, y + 62, -1, 5);

            PlaceBanner(x + 107, y + 65);

            PlaceDoorObjects(x + 80, y + 65);
            PlaceAndConfigureDoor(x + 80, y + 65, DoorID.RedDiagonalRoomExit, DoorID.RedGhostRoom);
            #endregion
        }
    }
}