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
    public class YellowStairsRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/YellowStairsRoomTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/YellowStairsRoomWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/YellowStairsRoomObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;
        public override void PostGenerate(int x, int y)
        {
            #region First Floor
            WorldGen.PlaceObject(x + 128, y + 256, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 154, y + 256, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 135, y + 231, ModContent.TileType<WoodenArch>());
            PlaceAndConfigureDoor(x + 136, y + 256, DoorID.YellowStairsRoomEntrance, DoorID.YellowPitRoomDoorExit);

            WorldGen.PlaceObject(x + 230, y + 203, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 273, y + 236, ModContent.TileType<WaxSconce>());

            PlaceStairGroup(x + 204, y + 231, direction: -1);
            PlaceStairGroup(x + 245, y + 202, direction: 0);

            PlaceHallwayArch(x + 222, y + 231);
            WorldGen.PlaceObject(x + 226, y + 231, ModContent.TileType<WaxCandleholder>());
            PlaceHallwayArch(x + 230, y + 231);
            PlaceHallwayArch(x + 238, y + 231);
            PlaceHallwayArch(x + 246, y + 231);

            WorldGen.PlaceObject(x + 163, y + 236, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 294, y + 236, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 340, y + 236, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 169, y + 256, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 177, y + 248, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 185, y + 240, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 193, y + 232, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 289, y + 256, ModContent.TileType<WoodenPillar2>());

            PlaceMultiVase(x + 172, y + 252, 1, 7);
            PlaceMultiVase(x + 221, y + 215, 1, 5);
            PlaceMultiVase(x + 260, y + 228, -1, 7);

            PlaceHallwayArch(x + 263, y + 202);
            WorldGen.PlaceObject(x + 267, y + 202, ModContent.TileType<WaxCandleholder>());
            PlaceHallwayArch(x + 271, y + 202);

            PlaceLoungeArea(x + 353, y + 256, RoomID.Yellow);
            PlaceCozyArea(x + 305, y + 256, RoomID.Yellow);
            #endregion

            #region Second Floor
            PlaceLoungeArea(x + 111, y + 189, RoomID.Yellow);

            WorldGen.PlaceObject(x + 144, y + 169, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 443, y + 194, ModContent.TileType<ArchiveBanner>());

            PlaceBookShelfObjects(x + 152, y + 189);
            PlaceTableAndChair(x + 171, y + 189, -1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 178, y + 189);
            PlaceTableAndChair(x + 197, y + 189, -1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 204, y + 189);

            PlaceBookShelfObjects(x + 127, y + 152);
            PlaceTableAndChair(x + 146, y + 152, -1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 153, y + 152);

            WorldGen.PlaceObject(x + 297, y + 169, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 289, y + 161, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 281, y + 153, ModContent.TileType<WoodenPillar2>());
            PlaceMultiVase(x + 276, y + 149, -1, 7);

            WorldGen.PlaceObject(x + 330, y + 169, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 338, y + 161, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 346, y + 153, ModContent.TileType<WoodenPillar2>());
            PlaceMultiVase(x + 325, y + 173, 1, 7);

            if (Main.rand.NextBool())
                WorldGen.PlaceObject(x + 314, y + 152, ModContent.TileType<WaxChandelier>());
            else
                PlaceHauntedChandelier(x + 314, y + 152);

            PlaceHallwayArch(x + 267, y + 189);
            PlaceStairGroup(x + 308, y + 189, 0);
            PlaceHallwayArch(x + 294, y + 189);

            PlaceStairGroup(x + 357, y + 152, -1);
            PlaceStairGroup(x + 259, y + 152, 1);

            PlaceHallwayArch(x + 326, y + 189);
            PlaceHallwayArch(x + 349, y + 189);
            PlaceHallwayArch(x + 372, y + 189);
            PlaceHallwayArch(x + 395, y + 189);

            WorldGen.PlaceObject(x + 414, y + 190, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 422, y + 198, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 430, y + 206, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 438, y + 214, ModContent.TileType<WoodenPillar2>());
            PlaceMultiVase(x + 409, y + 186, -1, 7);

            PlaceBookShelfObjects(x + 451, y + 214);
            PlaceTableAndChair(x + 469, y + 214, 1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 451, y + 184);
            PlaceTableAndChair(x + 469, y + 184, 1, RoomID.Yellow);

            PlaceBookShelfObjects(x + 477, y + 214);
            PlaceTableAndChair(x + 495, y + 214, 1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 477, y + 184);
            PlaceTableAndChair(x + 495, y + 184, 1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 503, y + 184, false);
            PlaceBookShelfObjects(x + 503, y + 214);

            PlaceTallStairs(x + 521, y + 214);

            for (int i = 0; i < 10; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 181 + offset, y + 152);
            }

            for (int i = 0; i < 5; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 375 + offset, y + 152);
            }

            for (int i = 0; i < 5; i++)
            {
                var offset = 8 * i;
                PlaceHallwayArch(x + 416 + offset, y + 123);
            }

            if (Main.rand.NextBool())
                WorldGen.PlaceObject(x + 251, y + 165, ModContent.TileType<WaxChandelier>());
            else
                PlaceHauntedChandelier(x + 251, y + 165);

            WorldGen.PlaceObject(x + 379, y + 152, ModContent.TileType<WaxCandleholder>());
            #endregion

            #region Third Floor
            PlaceLongTableAndChairs(x + 142, y + 80, RoomID.Yellow);
            PlaceLongTableAndChairs(x + 179, y + 80, RoomID.Yellow);

            PlaceLongTableAndChairs(x + 240, y + 110, RoomID.Yellow);
            PlaceLongTableAndChairs(x + 376, y + 110, RoomID.Yellow);
            PlaceLongTableAndChairs(x + 240, y + 80, RoomID.Yellow);
            PlaceLongTableAndChairs(x + 376, y + 80, RoomID.Yellow);

            PlaceMultiVase(x + 374, y + 136, 1, 5);
            WorldGen.PlaceObject(x + 208, y + 123, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 420, y + 123, ModContent.TileType<WaxCandleholder>());
            if (Main.rand.NextBool())
                WorldGen.PlaceObject(x + 405, y + 86, ModContent.TileType<WaxChandelier>());
            else
                PlaceHauntedChandelier(x + 405, y + 86);

            PlaceStairGroup(x + 398, y + 123, 0);

            WorldGen.PlaceObject(x + 244, y + 123, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 383, y + 123, ModContent.TileType<WaxSconce>());

            PlaceHallwayArch(x + 204, y + 123);
            PlaceMultiVase(x + 235, y + 120, -1, 5);

            WorldGen.PlaceObject(x + 273, y + 96, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 273, y + 66, ModContent.TileType<WaxSconce>());
            PlaceVaseGroup(x + 260, y + 105);
            PlaceVaseGroup(x + 272, y + 103);
            PlaceVaseGroup(x + 284, y + 101);

            PlaceVaseGroup(x + 260, y + 75);
            PlaceVaseGroup(x + 272, y + 73);
            PlaceVaseGroup(x + 284, y + 71);

            WorldGen.PlaceObject(x + 249, y + 152, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 298, y + 189, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 330, y + 189, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 399, y + 189, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 444, y + 214, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 354, y + 96, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 354, y + 66, ModContent.TileType<WaxSconce>());
            PlaceVaseGroup(x + 365, y + 105);
            PlaceVaseGroup(x + 353, y + 103);
            PlaceVaseGroup(x + 341, y + 101);

            PlaceVaseGroup(x + 365, y + 75);
            PlaceVaseGroup(x + 353, y + 73);
            PlaceVaseGroup(x + 341, y + 71);

            WorldGen.PlaceObject(x + 290, y + 85, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 336, y + 85, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 290, y + 55, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 336, y + 55, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 526, y + 90, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 526, y + 60, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 197, y + 60, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 526, y + 90, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 198, y + 80, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 198, y + 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 301, y + 75, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 327, y + 75, ModContent.TileType<WaxCandleholder>());

            PlaceBookShelfObjects(x + 217, y + 80);

            PlaceTallStairs(x + 419, y + 110);
            PlaceLoungeArea(x + 539, y + 110, RoomID.Yellow);

            if (Main.rand.NextBool())
                WorldGen.PlaceObject(x + 224, y + 86, ModContent.TileType<WaxChandelier>());
            else
                PlaceHauntedChandelier(x + 224, y + 86);

            PlaceStairGroup(x + 218, y + 123, 0);

            PlaceCozyArea(x + 301, y + 105, RoomID.Yellow);

            PlaceTallStairs(x + 108, y + 110);
            WorldGen.PlaceObject(x + 158, y + 81, ModContent.TileType<ArchiveBridge>());

            WorldGen.PlaceObject(x + 141, y + 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 178, y + 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 141, y + 55, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 178, y + 55, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 155, y + 110, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 175, y + 110, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 175, y + 80, ModContent.TileType<WoodenPillar2>());

            PlaceAndConfigureDoor(x + 309, y + 75, DoorID.YellowStairsTreasureEntrance, DoorID.YellowStairsTreasureExit, isLocked: true);
            PlaceBookShelfObjects(x + 397, y + 80);

            PlaceBookShelfObjects(x + 38, y + 110);
            PlaceTableAndChair(x + 57, y + 110, -1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 64, y + 110);
            PlaceTableAndChair(x + 83, y + 110, -1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 90, y + 110);

            PlaceBookShelfObjects(x + 38, y + 80);
            PlaceTableAndChair(x + 57, y + 80, -1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 64, y + 80);
            PlaceTableAndChair(x + 83, y + 80, -1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 90, y + 80, false);

            PlaceBookShelfObjects(x + 217, y + 80);

            WorldGen.PlaceObject(x + 454, y + 90, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 454, y + 60, ModContent.TileType<ArchiveBanner>());

            PlaceTableAndChair(x + 463, y + 80, 1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 471, y + 80);
            PlaceTableAndChair(x + 489, y + 80, 1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 497, y + 80, false);

            PlaceTableAndChair(x + 463, y + 110, 1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 471, y + 110);
            PlaceTableAndChair(x + 489, y + 110, 1, RoomID.Yellow);
            PlaceBookShelfObjects(x + 497, y + 110);

            PlaceTableAndChair(x + 515, y + 80, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 515, y + 110, 1, RoomID.Yellow);

            WorldGen.PlaceObject(x + 537, y + 80, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 563, y + 80, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 544, y + 55, ModContent.TileType<WoodenArch>());
            PlaceAndConfigureDoor(x + 545, y + 80, DoorID.YellowStairsRoomExit, DoorID.YellowWaxheadRoomEntrance);
            #endregion

            SetupSpawners(x, y);
        }

        private void SetupSpawners(int x, int y)
        {
            #region First Floor
            AddSpawnPoint(new Vector2(x + 185, y + 245), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 238, y + 230), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 339, y + 255), ModContent.NPCType<ClockworkSpider>());

            AddSpawnPoint(new Vector2(x + 352, y + 255), ModContent.NPCType<ClockworkSpider>());
            #endregion

            #region Second Floor
            AddSpawnPoint(new Vector2(x + 380, y + 151), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 147, y + 151), ModContent.NPCType<ClockworkSpider>());

            AddSpawnPoint(new Vector2(x + 179, y + 188), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 205, y + 175), ModContent.NPCType<BarrierBook>());
            AddSpawnPoint(new Vector2(x + 213, y + 188), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 481, y + 213), ModContent.NPCType<WaxWalker>());
            AddSpawnPoint(new Vector2(x + 464, y + 199), ModContent.NPCType<BlasterBook>());

            AddSpawnPoint(new Vector2(x + 465, y + 183), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 508, y + 178), ModContent.NPCType<InkWormBody>());
            #endregion

            #region Third Floor
            AddSpawnPoint(new Vector2(x + 65, y + 65), ModContent.NPCType<ChairBook>());
            AddSpawnPoint(new Vector2(x + 96, y + 67), ModContent.NPCType<BlasterBook>());

            AddSpawnPoint(new Vector2(x + 91, y + 95), ModContent.NPCType<BlasterBook>());
            AddSpawnPoint(new Vector2(x + 65, y + 97), ModContent.NPCType<BlasterBook>());
            AddSpawnPoint(new Vector2(x + 84, y + 109), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 166, y + 110), ModContent.NPCType<WaxWalker>());

            AddSpawnPoint(new Vector2(x + 243, y + 79), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 378, y + 109), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 473, y + 67), ModContent.NPCType<BlasterBook>());
            AddSpawnPoint(new Vector2(x + 506, y + 62), ModContent.NPCType<ChairBook>());

            AddSpawnPoint(new Vector2(x + 355, y + 76), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 499, y + 109), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 529, y + 109), ModContent.NPCType<ClockworkSpider>());
            #endregion
        }
    }
}