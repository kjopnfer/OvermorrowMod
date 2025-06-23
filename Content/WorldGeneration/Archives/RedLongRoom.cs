using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Content.Tiles.Archives;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.WorldGeneration.Archives
{
    public class RedLongRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/RedLongRoomTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/RedLongRoomWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/RedLongRoomObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        protected void PlaceBridgeGroup(int x, int y)
        {
            WorldGen.PlaceObject(x - 4, y - 10, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x - 4, y - 40, ModContent.TileType<WaxSconce>(), true);

            WorldGen.PlaceObject(x + 58, y - 10, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 58, y - 40, ModContent.TileType<WaxSconce>(), true);

            WorldGen.PlaceObject(x, y, ModContent.TileType<WoodenPillar2>(), true);
            WorldGen.PlaceObject(x + 3, y - 25, ModContent.TileType<WoodenArch>(), true);
            WorldGen.PlaceObject(x + 17, y, ModContent.TileType<WoodenPillar2>(), true);
            WorldGen.PlaceObject(x + 20, y - 29, ModContent.TileType<ArchiveBridge>(), true);

            WorldGen.PlaceObject(x, y - 30, ModContent.TileType<WoodenPillar2>(), true);
            WorldGen.PlaceObject(x + 3, y - 55, ModContent.TileType<WoodenArch>(), true);
            WorldGen.PlaceObject(x + 17, y - 30, ModContent.TileType<WoodenPillar2>(), true);

            PlaceLongTableAndChairs(x + 4, y - 30, RoomID.Red);
            PlaceLongTableAndChairs(x + 41, y - 30, RoomID.Red);

            WorldGen.PlaceObject(x + 37, y, ModContent.TileType<WoodenPillar2>(), true);
            WorldGen.PlaceObject(x + 40, y - 25, ModContent.TileType<WoodenArch>(), true);
            WorldGen.PlaceObject(x + 54, y, ModContent.TileType<WoodenPillar2>(), true);

            WorldGen.PlaceObject(x + 37, y - 30, ModContent.TileType<WoodenPillar2>(), true);
            WorldGen.PlaceObject(x + 40, y - 55, ModContent.TileType<WoodenArch>(), true);
            WorldGen.PlaceObject(x + 54, y - 30, ModContent.TileType<WoodenPillar2>(), true);
        }

        protected void PlaceBannersBothFloors(int x, int y)
        {
            WorldGen.PlaceObject(x, y - 20, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x, y - 50, ModContent.TileType<ArchiveBanner>(), true);
        }

        public override void PostGenerate(int x, int y)
        {
            #region Right Side
            PlaceDoorObjects(x + 845, y + 77);
            PlaceAndConfigureDoor(x + 845, y + 77, DoorID.RedLongRoomEntrance, DoorID.FoyerRedRoomDoor);

            PlaceBannersBothFloors(x + 826, y + 107);
            PlaceTallStairs(x + 791, y + 107);
            PlaceLoungeArea(x + 839, y + 107, RoomID.Red);

            for (int i = 0; i < 3; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 721 + offset, y + 77);
            }

            PlaceTableAndChair(x + 740, y + 77, -1, RoomID.Red);
            PlaceTableAndChair(x + 766, y + 77, -1, RoomID.Red);

            PlaceBookShelfObjects(x + 721, y + 107);
            PlaceBookShelfObjects(x + 773, y + 107);

            PlaceStairGroup(x + 748, y + 120, 0);

            WorldGen.PlaceObject(x + 733, y + 120, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 774, y + 120, ModContent.TileType<WaxSconce>(), true);

            PlaceMultiVase(x + 765, y + 117, -1, 5);
            PlaceMultiVase(x + 724, y + 133, 1, 5);

            WorldGen.PlaceObject(x + 698, y + 129, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 808, y + 129, ModContent.TileType<ArchiveBanner>(), true);

            for (int i = 0; i < 3; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 617 + offset, y + 149);
                PlaceTableAndChair(x + 610 + offset, y + 149, -1, RoomID.Red);
            }

            PlaceStairGroup(x + 707, y + 149, -1);
            PlaceStairGroup(x + 789, y + 149, 1);

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 825 + offset, y + 149);
                PlaceTableAndChair(x + 817 + offset, y + 149, 1, RoomID.Red);
            }

            WorldGen.PlaceObject(x + 729, y + 149, ModContent.TileType<WaxCandleholder>(), true);
            WorldGen.PlaceObject(x + 779, y + 149, ModContent.TileType<WaxCandleholder>(), true);

            for (int i = 0; i < 3; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 725 + offset, y + 149);
            }

            for (int i = 0; i < 3; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 759 + offset, y + 149);
            }

            PlaceBannersBothFloors(x + 699, y + 107);
            PlaceBridgeGroup(x + 632, y + 107);
            #endregion

            #region Middle Area
            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 582 + offset, y + 77);
                PlaceBookShelfObjects(x + 582 + offset, y + 107);
            }

            PlaceBookShelfObjects(x + 344, y + 77);

            PlaceTableAndChair(x + 601, y + 107, -1, RoomID.Red);
            PlaceTableAndChair(x + 601, y + 77, -1, RoomID.Red);

            PlaceBannersBothFloors(x + 280, y + 107);
            PlaceTallStairs(x + 289, y + 107);
            PlaceBannersBothFloors(x + 324, y + 107);

            PlaceLongTableAndChairs(x + 367, y + 77, RoomID.Red);
            PlaceLongTableAndChairs(x + 367, y + 107, RoomID.Red);

            PlaceBannersBothFloors(x + 416, y + 102);
            PlaceBannersBothFloors(x + 462, y + 102);
            PlaceDoorObjects(x + 435, y + 72);
            //PlaceAndConfigureDoor(x + 435, y + 72, DoorID.RedLongRoomSecondExit, DoorID.RedDiagonalRoomEntrance, isLocked: true);
            PlaceAndConfigureDoor(x + 435, y + 72, DoorID.REDDOORTOREDDOOR, DoorID.OTHERFUCKINGREDDOORTOREDDOOR);
            PlaceCozyArea(x + 427, y + 102, RoomID.Red);

            WorldGen.PlaceObject(x + 399, y + 63, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 399, y + 93, ModContent.TileType<WaxSconce>(), true);
            PlaceVaseGroup(x + 386, y + 72);
            PlaceVaseGroup(x + 398, y + 70);
            PlaceVaseGroup(x + 410, y + 68);
            PlaceVaseGroup(x + 386, y + 102);
            PlaceVaseGroup(x + 398, y + 100);
            PlaceVaseGroup(x + 410, y + 98);

            WorldGen.PlaceObject(x + 480, y + 63, ModContent.TileType<WaxSconce>(), true);
            WorldGen.PlaceObject(x + 480, y + 93, ModContent.TileType<WaxSconce>(), true);
            PlaceVaseGroup(x + 491, y + 72);
            PlaceVaseGroup(x + 479, y + 70);
            PlaceVaseGroup(x + 467, y + 68);
            PlaceVaseGroup(x + 491, y + 102);
            PlaceVaseGroup(x + 479, y + 100);
            PlaceVaseGroup(x + 467, y + 98);

            PlaceBridgeGroup(x + 517, y + 107);
            PlaceBannersBothFloors(x + 503, y + 107);
            #endregion

            #region Left Side
            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 163 + offset, y + 77);
                PlaceBookShelfObjects(x + 163 + offset, y + 107);
            }

            PlaceTableAndChair(x + 182, y + 107, -1, RoomID.Red);
            PlaceTableAndChair(x + 182, y + 77, -1, RoomID.Red);

            PlaceBridgeGroup(x + 213, y + 107);
            PlaceBridgeGroup(x + 99, y + 107);

            PlaceStairGroup(x + 345, y + 120, 0);

            WorldGen.PlaceObject(x + 330, y + 120, ModContent.TileType<WaxSconce>(), true);
            PlaceMultiVase(x + 362, y + 117, -1, 5);

            WorldGen.PlaceObject(x + 371, y + 120, ModContent.TileType<WaxSconce>(), true);
            PlaceMultiVase(x + 321, y + 133, 1, 5);

            PlaceStairGroup(x + 304, y + 149, -1);
            PlaceStairGroup(x + 386, y + 149, 1);

            WorldGen.PlaceObject(x + 295, y + 129, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 405, y + 129, ModContent.TileType<ArchiveBanner>(), true);

            WorldGen.PlaceObject(x + 326, y + 149, ModContent.TileType<WaxCandleholder>(), true);
            WorldGen.PlaceObject(x + 376, y + 149, ModContent.TileType<WaxCandleholder>(), true);

            for (int i = 0; i < 3; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 214 + offset, y + 149);
                PlaceTableAndChair(x + 207 + offset, y + 149, 1, RoomID.Red);
            }

            for (int i = 0; i < 3; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 322 + offset, y + 149);
            }

            for (int i = 0; i < 3; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 356 + offset, y + 149);
            }

            for (int i = 0; i < 3; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 214 + offset, y + 149);
                PlaceTableAndChair(x + 207 + offset, y + 149, -1, RoomID.Red);
            }

            PlaceTableAndChair(x + 285, y + 149, -1, RoomID.Red);

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 422 + offset, y + 149);
                PlaceTableAndChair(x + 414 + offset, y + 149, 1, RoomID.Red);
            }

            PlaceBannersBothFloors(x + 85, y + 107);
            PlaceLoungeArea(x + 52, y + 77, RoomID.Red);

            PlaceDoorObjects(x + 58, y + 107);
            PlaceAndConfigureDoor(x + 58, y + 107, DoorID.RedLongRoomExit, DoorID.RedGhostRoom);
            #endregion

            SetupSpawners(x, y);
        }

        private void SetupSpawners(int x, int y)
        {
            #region Upper Area
            AddSpawnPoint(new Vector2(x + 774, y + 89), ModContent.NPCType<BlasterBook>());
            AddSpawnPoint(new Vector2(x + 792, y + 106), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 748, y + 59), ModContent.NPCType<PlantBook>());
            AddSpawnPoint(new Vector2(x + 787, y + 62), ModContent.NPCType<ChairBook>());

            AddSpawnPoint(new Vector2(x + 164, y + 66), ModContent.NPCType<BlasterBook>());
            AddSpawnPoint(new Vector2(x + 196, y + 60), ModContent.NPCType<BlasterBook>());

            AddSpawnPoint(new Vector2(x + 279, y + 76), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 344, y + 76), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 100, y + 76), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 576, y + 76), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 595, y + 62), ModContent.NPCType<BlasterBook>());
            AddSpawnPoint(new Vector2(x + 636, y + 76), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 659, y + 106), ModContent.NPCType<WaxWalker>());
            AddSpawnPoint(new Vector2(x + 426, y + 101), ModContent.NPCType<WaxWalker>());
            AddSpawnPoint(new Vector2(x + 542, y + 106), ModContent.NPCType<WaxWalker>());
            AddSpawnPoint(new Vector2(x + 324, y + 106), ModContent.NPCType<WaxWalker>());
            AddSpawnPoint(new Vector2(x + 240, y + 106), ModContent.NPCType<WaxWalker>());
            AddSpawnPoint(new Vector2(x + 125, y + 106), ModContent.NPCType<WaxWalker>());
            #endregion

            #region Lower Area
            AddSpawnPoint(new Vector2(x + 228, y + 148), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 432, y + 135), ModContent.NPCType<BarrierBook>());
            AddSpawnPoint(new Vector2(x + 452, y + 148), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 725, y + 148), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 773, y + 127), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 618, y + 148), ModContent.NPCType<ClockworkSpider>());
            AddSpawnPoint(new Vector2(x + 859, y + 138), ModContent.NPCType<ChairBook>());
            #endregion
        }
    }
}