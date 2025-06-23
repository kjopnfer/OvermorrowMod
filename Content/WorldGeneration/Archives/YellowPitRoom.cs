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
    public class YellowPitRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/YellowPitRoomTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/YellowPitRoomWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/YellowPitRoomObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        public override void PostGenerate(int x, int y)
        {
            #region First Floor
            PlaceAndConfigureDoor(x + 104, y + 262, DoorID.YellowPitRoomDoorEntrance, DoorID.FoyerYellowRoomDoor);

            WorldGen.PlaceObject(x + 103, y + 237, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 96, y + 262, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 122, y + 262, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 220, y + 262, ModContent.TileType<WaxCandleholder>());

            PlaceBookshelfArch(x + 140, y + 237);
            PlaceBookshelfArch(x + 166, y + 237);
            PlaceBookshelfArch(x + 192, y + 237);

            PlaceMultiBookPiles(x + 139, y + 262);
            PlaceMultiBookPiles(x + 165, y + 262);
            PlaceMultiBookPiles(x + 191, y + 262);

            PlaceTableAndChair(x + 157, y + 262, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 183, y + 262, 1, RoomID.Yellow);

            #region Stairs
            WorldGen.PlaceObject(x + 282, y + 237, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 257, y + 237, ModContent.TileType<StairPillar>());
            WorldGen.PlaceObject(x + 260, y + 200, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 260, y + 237, ModContent.TileType<WoodenStairs>());
            WorldGen.PlaceObject(x + 274, y + 237, ModContent.TileType<SmallPillar>());
            WorldGen.PlaceObject(x + 274, y + 225, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 278, y + 237, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 280, y + 230, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 286, y + 237, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 288, y + 230, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 294, y + 237, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 296, y + 230, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 302, y + 237, ModContent.TileType<HallwayPillar>());

            #region Vases
            PlaceVaseGroup(x + 228, y + 258);
            PlaceVaseGroup(x + 236, y + 250);
            PlaceVaseGroup(x + 244, y + 242);
            PlaceVaseGroup(x + 252, y + 234);

            PlaceVaseGroup(x + 277, y + 221);
            PlaceVaseGroup(x + 281, y + 217);
            PlaceVaseGroup(x + 285, y + 213);
            PlaceVaseGroup(x + 289, y + 209);
            PlaceVaseGroup(x + 293, y + 205);

            PlaceVaseGroup(x + 293, y + 192);
            PlaceVaseGroup(x + 285, y + 184);
            PlaceVaseGroup(x + 277, y + 176);
            PlaceVaseGroup(x + 269, y + 168);
            #endregion

            WorldGen.PlaceObject(x + 286, y + 208, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 298, y + 196, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 301, y + 171, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 301, y + 208, ModContent.TileType<WoodenStairs>());

            #endregion
            #endregion

            #region Second Floor
            #region Stairs
            WorldGen.PlaceObject(x + 252, y + 134, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 252, y + 171, ModContent.TileType<WoodenStairs>());
            WorldGen.PlaceObject(x + 249, y + 159, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 237, y + 142, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 131, y + 242, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 219, y + 242, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 211, y + 142, ModContent.TileType<WoodenStairs>());

            PlaceVaseGroup(x + 244, y + 155);
            PlaceVaseGroup(x + 240, y + 151);
            PlaceVaseGroup(x + 236, y + 147);
            PlaceVaseGroup(x + 232, y + 143);
            PlaceVaseGroup(x + 228, y + 139);

            #endregion

            PlaceHallwayArch(x + 170, y + 171);
            PlaceHallwayArch(x + 193, y + 171);

            WorldGen.PlaceObject(x + 185, y + 168, ModContent.TileType<WaxSconceEven>());
            WorldGen.PlaceObject(x + 207, y + 168, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 230, y + 168, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 201, y + 142, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 242, y + 171, ModContent.TileType<WaxCandleholder>());

            PlaceHallwayArch(x + 215, y + 171);
            PlaceHallwayArch(x + 238, y + 171);

            PlaceMultiBookPiles(x + 116, y + 171);
            PlaceTableAndChair(x + 135, y + 171, -1, RoomID.Yellow);
            PlaceMultiBookPiles(x + 142, y + 171);

            PlaceBookshelfArch(x + 117, y + 146);
            PlaceBookshelfArch(x + 143, y + 146);

            PlaceHallwayArch(x + 189, y + 142);
            PlaceHallwayArch(x + 197, y + 142);
            #endregion

            #region Third Floor
            #region Vases
            PlaceVaseGroup(x + 343, y + 125);
            PlaceVaseGroup(x + 347, y + 121);
            PlaceVaseGroup(x + 351, y + 117);
            PlaceVaseGroup(x + 355, y + 113);
            PlaceVaseGroup(x + 359, y + 109);

            PlaceVaseGroup(x + 333, y + 95);
            PlaceVaseGroup(x + 337, y + 91);
            PlaceVaseGroup(x + 341, y + 87);
            PlaceVaseGroup(x + 345, y + 83);
            PlaceVaseGroup(x + 349, y + 79);
            #endregion

            #region Bookshelves
            PlaceBookshelfArch(x + 76, y + 104);
            PlaceBookshelfArch(x + 102, y + 104);
            PlaceBookshelfArch(x + 128, y + 104);

            PlaceMultiBookPiles(x + 75, y + 129);
            PlaceMultiBookPiles(x + 101, y + 129);
            PlaceMultiBookPiles(x + 127, y + 129);

            PlaceBookshelfArch(x + 76, y + 74);
            PlaceBookshelfArch(x + 102, y + 74);
            PlaceBookshelfArch(x + 128, y + 74);

            PlaceMultiBookPiles(x + 75, y + 99);
            PlaceMultiBookPiles(x + 101, y + 99);

            PlaceMultiBookPiles(x + 211, y + 99);
            PlaceBookshelfArch(x + 211, y + 74);

            PlaceBookshelfArch(x + 270, y + 104);
            PlaceMultiBookPiles(x + 269, y + 129);

            PlaceBookshelfArch(x + 296, y + 109);
            PlaceMultiBookPiles(x + 295, y + 129);

            PlaceBookshelfArch(x + 270, y + 74);
            PlaceMultiBookPiles(x + 269, y + 99);

            PlaceBookshelfArch(x + 296, y + 74);
            PlaceMultiBookPiles(x + 295, y + 99);

            PlaceBookShelfObjects(x + 386, y + 112);
            PlaceBookShelfObjects(x + 386, y + 82);

            PlaceBookShelfObjects(x + 412, y + 82);
            PlaceBookShelfObjects(x + 412, y + 112);

            PlaceBookShelfObjects(x + 438, y + 82, false);
            PlaceBookShelfObjects(x + 438, y + 112);

            PlaceBookShelfObjects(x + 464, y + 82);
            PlaceBookShelfObjects(x + 464, y + 112);

            PlaceBookShelfObjects(x + 490, y + 82);
            PlaceBookShelfObjects(x + 490, y + 112);
            #endregion

            WorldGen.PlaceObject(x + 211, y + 105, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 211, y + 105, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 211, y + 105, ModContent.TileType<WoodenArch>());

            PlaceTableAndChair(x + 94, y + 129, -1, RoomID.Yellow);
            PlaceTableAndChair(x + 120, y + 129, -1, RoomID.Yellow);

            PlaceTableAndChair(x + 94, y + 99, -1, RoomID.Yellow);
            PlaceTableAndChair(x + 120, y + 99, -1, RoomID.Yellow);

            PlaceTableAndChair(x + 261, y + 129, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 287, y + 129, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 313, y + 129, 1, RoomID.Yellow);

            PlaceTableAndChair(x + 261, y + 99, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 287, y + 99, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 313, y + 99, 1, RoomID.Yellow);

            WorldGen.PlaceObject(x + 180, y + 109, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 180, y + 79, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 252, y + 109, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 252, y + 79, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 324, y + 109, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 324, y + 79, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 369, y + 62, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 369, y + 62, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 543, y + 92, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 543, y + 62, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 335, y + 122, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 352, y + 112, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 342, y + 82, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 359, y + 75, ModContent.TileType<WaxSconce>());

            PlaceLongTableAndChairs(x + 191, y + 129, RoomID.Yellow);
            PlaceLongTableAndChairs(x + 234, y + 129, RoomID.Yellow);

            PlaceTableAndChair(x + 378, y + 112, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 378, y + 82, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 404, y + 112, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 404, y + 82, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 430, y + 112, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 430, y + 82, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 456, y + 112, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 456, y + 82, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 482, y + 112, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 482, y + 82, 1, RoomID.Yellow);

            PlaceTallStairs(x + 145, y + 129);
            PlaceTallStairs(x + 508, y + 112);

            PlaceLoungeArea(x + 556, y + 112, RoomID.Yellow);
            WorldGen.PlaceObject(x + 554, y + 82, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 580, y + 82, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 561, y + 57, ModContent.TileType<WoodenArch>());
            PlaceAndConfigureDoor(x + 562, y + 82, DoorID.YellowPitRoomDoorExit, DoorID.YellowStairsRoomEntrance);
            #endregion

            SetupSpawners(x, y);
        }

        private void SetupSpawners(int x, int y)
        {
            #region First Floor
            AddSpawnPoint(new Vector2(x + 172, y + 245), ModContent.NPCType<InkWormBody>());
            AddSpawnPoint(new Vector2(x + 192, y + 251), ModContent.NPCType<ChairBook>());
            AddSpawnPoint(new Vector2(x + 198, y + 252), ModContent.NPCType<BarrierBook>());

            AddSpawnPoint(new Vector2(x + 287, y + 236), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 243, y + 170), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 119, y + 161), ModContent.NPCType<BarrierBook>());
            AddSpawnPoint(new Vector2(x + 144, y + 156), ModContent.NPCType<BlasterBook>());
            AddSpawnPoint(new Vector2(x + 129, y + 153), ModContent.NPCType<ChairBook>());

            AddSpawnPoint(new Vector2(x + 201, y + 141), ModContent.NPCType<ArchiveRat>());
            #endregion

            #region Second Floor
            AddSpawnPoint(new Vector2(x + 102, y + 88), ModContent.NPCType<PlantBook>());
            AddSpawnPoint(new Vector2(x + 108, y + 119), ModContent.NPCType<ChairBook>());

            AddSpawnPoint(new Vector2(x + 179, y + 98), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 194, y + 98), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 393, y + 71), ModContent.NPCType<BlasterBook>());
            AddSpawnPoint(new Vector2(x + 425, y + 71), ModContent.NPCType<PlantBook>());
            AddSpawnPoint(new Vector2(x + 441, y + 67), ModContent.NPCType<BarrierBook>());
            AddSpawnPoint(new Vector2(x + 496, y + 68), ModContent.NPCType<BlasterBook>());

            AddSpawnPoint(new Vector2(x + 393, y + 110), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 406, y + 110), ModContent.NPCType<WaxWalker>());

            #endregion
        }
    }
}