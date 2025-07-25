using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Content.Tiles.Archives;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.WorldGeneration.Archives
{
    public class ArchiveFoyer : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/ArchiveFoyerTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/ArchiveFoyerWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/ArchiveFoyerObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        public override void PostGenerate(int x, int y)
        {
            #region Bottom Left Hallway
            #region Vases
            PlaceVaseGroup(x + 193, y + 257);
            PlaceVaseGroup(x + 197, y + 253);
            PlaceVaseGroup(x + 201, y + 249);
            PlaceVaseGroup(x + 205, y + 245);
            PlaceVaseGroup(x + 209, y + 241);
            PlaceVaseGroup(x + 213, y + 237);
            PlaceVaseGroup(x + 217, y + 233);
            #endregion

            PlaceTableAndChair(x + 143, y + 261, -1, RoomID.Yellow);

            PlaceBookshelfArch(x + 125, y + 236);
            PlaceBookshelfArch(x + 151, y + 236);

            WorldGen.PlaceObject(x + 206, y + 238, ModContent.TileType<WaxSconce>());

            PlaceBookPile(x + 116, y + 261, Main.rand.Next(3, 7));
            PlaceBookPile(x + 118, y + 261, Main.rand.Next(5, 8));

            PlaceBookPile(x + 128, y + 261, Main.rand.Next(5, 7));
            PlaceBookPile(x + 131, y + 261, Main.rand.Next(2, 4));
            PlaceBookPile(x + 136, y + 261, Main.rand.Next(5, 7));

            PlaceBookPile(x + 150, y + 261, Main.rand.Next(5, 7));
            PlaceBookPile(x + 153, y + 261, Main.rand.Next(6, 9));
            PlaceBookPile(x + 160, y + 261, Main.rand.Next(3, 12), true);
            PlaceBookPile(x + 163, y + 261, Main.rand.Next(3, 8));

            WorldGen.PlaceObject(x + 101, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 116, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 118, y + 266, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 124, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 139, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 141, y + 266, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 147, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 162, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 164, y + 266, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 170, y + 273, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 176, y + 236, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 182, y + 255, ModContent.TileType<WaxSconceEven>());
            WorldGen.PlaceObject(x + 176, y + 273, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 194, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 196, y + 266, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 202, y + 273, ModContent.TileType<HallwayPillar>());
            #endregion

            #region Center Fireplace Area
            // Since platforms are placed 1 tile down, these are also placed 1 tile down
            #region Vases
            PlaceVaseGroup(x + 419, y + 227);
            PlaceVaseGroup(x + 407, y + 229);
            PlaceVaseGroup(x + 395, y + 231);

            PlaceVaseGroup(x + 476, y + 227);
            PlaceVaseGroup(x + 488, y + 229);
            PlaceVaseGroup(x + 500, y + 231);

            PlaceVaseGroup(x + 419, y + 195);
            PlaceVaseGroup(x + 407, y + 197);
            PlaceVaseGroup(x + 395, y + 199);

            PlaceVaseGroup(x + 476, y + 195);
            PlaceVaseGroup(x + 488, y + 197);
            PlaceVaseGroup(x + 500, y + 199);
            #endregion

            PlaceTableAndChair(x + 331, y + 236, -1, RoomID.Yellow);
            PlaceTableAndChair(x + 357, y + 236, -1, RoomID.Yellow);
            PlaceTableAndChair(x + 538, y + 236, 1, RoomID.Yellow);
            PlaceTableAndChair(x + 564, y + 236, 1, RoomID.Yellow);

            PlaceBookshelfArch(x + 339, y + 211);
            PlaceBookshelfArch(x + 365, y + 211);

            PlaceBookshelfArch(x + 521, y + 211);
            PlaceBookshelfArch(x + 547, y + 211);

            PlaceBookPile(x + 341, y + 236, Main.rand.Next(2, 4));
            PlaceBookPile(x + 345, y + 236, Main.rand.Next(5, 7));
            PlaceBookPile(x + 351, y + 236, Main.rand.Next(2, 4));
            PlaceBookPile(x + 365, y + 236, Main.rand.Next(5, 7));

            PlaceBookPile(x + 377, y + 236, Main.rand.Next(5, 8));
            PlaceBookPile(x + 375, y + 236, Main.rand.Next(3, 6));
            PlaceBookPile(x + 372, y + 236, Main.rand.Next(5, 8));

            WorldGen.PlaceObject(x + 241, y + 233, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 302, y + 233, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 592, y + 233, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 656, y + 233, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 226, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 228, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 234, y + 236, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 249, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 251, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 257, y + 236, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 263, y + 199, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 263, y + 236, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 281, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 283, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 289, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 291, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 297, y + 236, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 308, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 310, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 316, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 318, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 324, y + 236, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 385, y + 204, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 385, y + 236, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 408, y + 222, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 408, y + 190, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 595, y + 233, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 656, y + 233, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 426, y + 231, ModContent.TileType<WaxCandleholder>());

            PlaceCozyArea(x + 436, y + 231, RoomID.Yellow);

            WorldGen.PlaceObject(x + 472, y + 231, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 489, y + 190, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 489, y + 222, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 426, y + 199, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 443, y + 174, ModContent.TileType<WoodenArch>());
            TileUtils.PlaceTileWithEntity<SanctumGate, SanctumGate_TE>(x + 443, y + 199);
            WorldGen.PlaceObject(x + 472, y + 199, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 513, y + 204, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 513, y + 236, ModContent.TileType<WaxCandleholder>());

            PlaceBookPile(x + 524, y + 236, Main.rand.Next(2, 4));
            PlaceBookPile(x + 527, y + 236, Main.rand.Next(5, 7));
            PlaceBookPile(x + 532, y + 236, Main.rand.Next(2, 4));
            PlaceBookPile(x + 547, y + 236, Main.rand.Next(5, 7));

            PlaceBookPile(x + 550, y + 236, Main.rand.Next(5, 8));
            PlaceBookPile(x + 555, y + 236, Main.rand.Next(3, 6));
            PlaceBookPile(x + 557, y + 236, Main.rand.Next(5, 8));
            PlaceBookPile(x + 559, y + 236, Main.rand.Next(3, 6));

            PlaceBookshelfArch(x + 521, y + 211);
            PlaceBookshelfArch(x + 547, y + 211);

            WorldGen.PlaceObject(x + 574, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 576, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 582, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 584, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 590, y + 236, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 601, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 603, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 609, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 611, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 617, y + 236, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 623, y + 199, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 623, y + 236, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 641, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 643, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 649, y + 236, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 664, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 666, y + 229, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 672, y + 236, ModContent.TileType<HallwayPillar>());
            #endregion

            #region Bottom Right Hallway
            #region Vases
            PlaceVaseGroup(x + 702, y + 257);
            PlaceVaseGroup(x + 698, y + 253);
            PlaceVaseGroup(x + 694, y + 249);
            PlaceVaseGroup(x + 690, y + 245);
            PlaceVaseGroup(x + 686, y + 241);
            PlaceVaseGroup(x + 682, y + 237);
            PlaceVaseGroup(x + 678, y + 233);
            #endregion

            PlaceTableAndChair(x + 752, y + 261, 1, RoomID.Yellow);

            PlaceBookshelfArch(x + 735, y + 236);
            PlaceBookshelfArch(x + 761, y + 236);

            WorldGen.PlaceObject(x + 691, y + 238, ModContent.TileType<WaxSconce>());

            PlaceBookPile(x + 737, y + 261, Main.rand.Next(3, 7));
            PlaceBookPile(x + 740, y + 261, Main.rand.Next(5, 8));

            PlaceBookPile(x + 747, y + 261, Main.rand.Next(5, 7));
            PlaceBookPile(x + 762, y + 261, Main.rand.Next(7, 9), true);
            PlaceBookPile(x + 764, y + 261, Main.rand.Next(5, 7));

            PlaceBookPile(x + 770, y + 261, Main.rand.Next(5, 7));
            PlaceBookPile(x + 778, y + 261, Main.rand.Next(6, 9));
            PlaceBookPile(x + 780, y + 261, Main.rand.Next(3, 12));
            PlaceBookPile(x + 782, y + 261, Main.rand.Next(3, 8));

            WorldGen.PlaceObject(x + 696, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 698, y + 266, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 704, y + 273, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 710, y + 236, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 716, y + 255, ModContent.TileType<WaxSconceEven>());
            WorldGen.PlaceObject(x + 710, y + 273, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 728, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 730, y + 266, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 736, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 751, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 753, y + 266, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 759, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 774, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 776, y + 266, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 782, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 797, y + 273, ModContent.TileType<HallwayPillar>());

            #endregion

            #region Left Stairs
            #region Vases
            PlaceVaseGroup(x + 281, y + 220);
            PlaceVaseGroup(x + 285, y + 216);
            PlaceVaseGroup(x + 289, y + 212);
            PlaceVaseGroup(x + 293, y + 208);
            PlaceVaseGroup(x + 297, y + 204);
            PlaceVaseGroup(x + 301, y + 200);

            PlaceVaseGroup(x + 309, y + 192);
            PlaceVaseGroup(x + 313, y + 188);
            PlaceVaseGroup(x + 317, y + 184);
            PlaceVaseGroup(x + 321, y + 180);
            PlaceVaseGroup(x + 325, y + 176);

            PlaceVaseGroup(x + 350, y + 176);
            PlaceVaseGroup(x + 358, y + 184);
            PlaceVaseGroup(x + 366, y + 192);
            PlaceVaseGroup(x + 374, y + 200);

            PlaceVaseGroup(x + 325, y + 163);
            PlaceVaseGroup(x + 321, y + 159);
            PlaceVaseGroup(x + 317, y + 155);
            PlaceVaseGroup(x + 313, y + 151);
            PlaceVaseGroup(x + 309, y + 147);
            #endregion

            #region Tables
            PlaceTableAndChair(x + 182, y + 194, -1, RoomID.Red);
            PlaceTableAndChair(x + 156, y + 194, -1, RoomID.Red);
            PlaceTableAndChair(x + 130, y + 194, -1, RoomID.Red);

            PlaceTableAndChair(x + 182, y + 224, -1, RoomID.Red);
            PlaceTableAndChair(x + 156, y + 224, -1, RoomID.Red);
            PlaceTableAndChair(x + 130, y + 224, -1, RoomID.Red);
            #endregion

            PlaceBookshelfArch(x + 138, y + 169);
            PlaceBookshelfArch(x + 164, y + 169);
            PlaceBookshelfArch(x + 190, y + 169);
            PlaceBookshelfArch(x + 138, y + 199);
            PlaceBookshelfArch(x + 164, y + 199);
            PlaceBookshelfArch(x + 190, y + 199);

            PlaceLongTableAndChairs(x + 227, y + 194, RoomID.Red);

            PlaceMultiBookPiles(x + 138, y + 194);
            PlaceMultiBookPiles(x + 164, y + 194);
            PlaceMultiBookPiles(x + 190, y + 194);

            PlaceMultiBookPiles(x + 138, y + 224);
            PlaceMultiBookPiles(x + 164, y + 224);
            PlaceMultiBookPiles(x + 190, y + 224);

            PlaceLoungeArea(x + 43, y + 194, RoomID.Red);

            PlaceTallStairs(x + 85, y + 224);

            WorldGen.PlaceObject(x + 217, y + 186, ModContent.TileType<Chameleon>());

            WorldGen.PlaceObject(x + 48, y + 199, ModContent.TileType<WoodenArch>());
            PlaceAndConfigureDoor(x + 49, y + 224, DoorID.FoyerRedRoomDoor, DoorID.RedLongRoomEntrance);
            WorldGen.PlaceObject(x + 41, y + 224, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 67, y + 224, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 76, y + 174, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 120, y + 174, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 76, y + 204, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 120, y + 204, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 226, y + 199, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 240, y + 194, ModContent.TileType<Finial>());
            WorldGen.PlaceObject(x + 240, y + 224, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 243, y + 195, ModContent.TileType<ArchiveBridge>());

            WorldGen.PlaceObject(x + 266, y + 194, ModContent.TileType<NormalWizardStatue>());

            WorldGen.PlaceObject(x + 260, y + 194, ModContent.TileType<Finial>());
            WorldGen.PlaceObject(x + 260, y + 224, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 269, y + 218, ModContent.TileType<WaxSconceEven>());
            WorldGen.PlaceObject(x + 277, y + 194, ModContent.TileType<Finial>());

            WorldGen.PlaceObject(x + 306, y + 138, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 360, y + 138, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 306, y + 196, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 289, y + 150, ModContent.TileType<StairPillar>());
            WorldGen.PlaceObject(x + 292, y + 113, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 292, y + 150, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 318, y + 179, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 318, y + 150, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 330, y + 167, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 333, y + 142, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 333, y + 179, ModContent.TileType<WoodenStairs>());
            WorldGen.PlaceObject(x + 347, y + 179, ModContent.TileType<StairPillar>());
            WorldGen.PlaceObject(x + 346, y + 179, ModContent.TileType<StairPillar>());
            WorldGen.PlaceObject(x + 348, y + 179, ModContent.TileType<StairPillar>());

            WorldGen.PlaceObject(x + 402, y + 96, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 402, y + 133, ModContent.TileType<WoodenStairs>());
            WorldGen.PlaceObject(x + 416, y + 121, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 443, y + 104, ModContent.TileType<WoodenStairs>());
            #endregion

            #region Right Stairs
            #region Vases
            PlaceVaseGroup(x + 614, y + 220);
            PlaceVaseGroup(x + 610, y + 216);
            PlaceVaseGroup(x + 606, y + 212);
            PlaceVaseGroup(x + 602, y + 208);
            PlaceVaseGroup(x + 598, y + 204);
            PlaceVaseGroup(x + 594, y + 200);

            PlaceVaseGroup(x + 586, y + 192);
            PlaceVaseGroup(x + 582, y + 188);
            PlaceVaseGroup(x + 578, y + 184);
            PlaceVaseGroup(x + 574, y + 180);
            PlaceVaseGroup(x + 570, y + 176);

            PlaceVaseGroup(x + 545, y + 176);
            PlaceVaseGroup(x + 537, y + 184);
            PlaceVaseGroup(x + 529, y + 192);
            PlaceVaseGroup(x + 521, y + 200);

            PlaceVaseGroup(x + 570, y + 163);
            PlaceVaseGroup(x + 574, y + 159);
            PlaceVaseGroup(x + 578, y + 155);
            PlaceVaseGroup(x + 582, y + 151);
            PlaceVaseGroup(x + 586, y + 147);
            #endregion

            //WorldGen.PlaceObject(x + 565, y + 122, TileID.Painting3X3, true, Main.rand.Next(1, 3));
            //WorldGen.PlaceObject(x + 565, y + 125, TileID.Painting3X3, true, Main.rand.Next(1, 3));
            WorldGen.PlaceObject(x + 564, y + 132, ModContent.TileType<TheGleeks>());

            PlaceTableAndChair(x + 713, y + 224, 1, RoomID.Blue);
            PlaceTableAndChair(x + 739, y + 224, 1, RoomID.Blue);
            PlaceTableAndChair(x + 765, y + 224, 1, RoomID.Blue);
            PlaceTableAndChair(x + 713, y + 194, 1, RoomID.Blue);
            PlaceTableAndChair(x + 739, y + 194, 1, RoomID.Blue);
            PlaceTableAndChair(x + 765, y + 194, 1, RoomID.Blue);

            PlaceBookshelfArch(x + 696, y + 169);
            PlaceBookshelfArch(x + 722, y + 169);
            PlaceBookshelfArch(x + 748, y + 169);
            PlaceBookshelfArch(x + 696, y + 199);
            PlaceBookshelfArch(x + 722, y + 199);
            PlaceBookshelfArch(x + 748, y + 199);

            PlaceMultiBookPiles(x + 696, y + 194);
            PlaceMultiBookPiles(x + 722, y + 194);
            PlaceMultiBookPiles(x + 748, y + 194);

            PlaceMultiBookPiles(x + 696, y + 224);
            PlaceMultiBookPiles(x + 722, y + 224);
            PlaceMultiBookPiles(x + 748, y + 224);

            PlaceTallStairs(x + 785, y + 224);

            WorldGen.PlaceObject(x + 776, y + 174, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 820, y + 174, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 776, y + 204, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 820, y + 204, ModContent.TileType<ArchiveBanner>());

            //WorldGen.PlaceObject(x + 793, y + 169, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 481, y + 121, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 484, y + 96, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 484, y + 133, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 538, y + 138, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 591, y + 138, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 594, y + 113, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 594, y + 150, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 553, y + 142, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 553, y + 179, ModContent.TileType<WoodenStairs>());
            WorldGen.PlaceObject(x + 567, y + 167, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 579, y + 150, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 579, y + 179, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 620, y + 224, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 620, y + 194, ModContent.TileType<Finial>());
            WorldGen.PlaceObject(x + 629, y + 218, ModContent.TileType<WaxSconceEven>());
            WorldGen.PlaceObject(x + 626, y + 194, ModContent.TileType<NormalWizardStatue>());
            WorldGen.PlaceObject(x + 637, y + 194, ModContent.TileType<Finial>());

            WorldGen.PlaceObject(x + 640, y + 195, ModContent.TileType<ArchiveBridge>());

            WorldGen.PlaceObject(x + 657, y + 194, ModContent.TileType<Finial>());
            WorldGen.PlaceObject(x + 660, y + 199, ModContent.TileType<WoodenArch>());
            PlaceLongTableAndChairs(x + 661, y + 194, RoomID.Blue);

            WorldGen.PlaceObject(x + 657, y + 224, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 674, y + 224, ModContent.TileType<WoodenPillar2>());

            //WorldGen.PlaceObject(x + 777, y + 194, ModContent.TileType<WaxCandleholder>());
            //WorldGen.PlaceObject(x + 777, y + 224, ModContent.TileType<WaxCandleholder>());

            //WorldGen.PlaceObject(x + 821, y + 194, ModContent.TileType<WaxCandleholder>());
            //WorldGen.PlaceObject(x + 821, y + 224, ModContent.TileType<WaxCandleholder>());

            PlaceLoungeArea(x + 833, y + 194, RoomID.Blue);

            WorldGen.PlaceObject(x + 838, y + 199, ModContent.TileType<WoodenArch>());
            PlaceAndConfigureDoor(x + 839, y + 224, DoorID.FoyerBlueRoomDoor, DoorID.BlueShrimpRoomEntrance);
            WorldGen.PlaceObject(x + 831, y + 224, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 857, y + 224, ModContent.TileType<WaxCandleholder>());
            #endregion

            #region Upper Middle Area
            #region Vases
            PlaceVaseGroup(x + 370, y + 133);
            PlaceVaseGroup(x + 382, y + 131);
            PlaceVaseGroup(x + 394, y + 129);

            PlaceVaseGroup(x + 419, y + 117);
            PlaceVaseGroup(x + 423, y + 113);
            PlaceVaseGroup(x + 427, y + 109);
            PlaceVaseGroup(x + 431, y + 105);
            PlaceVaseGroup(x + 435, y + 101);

            PlaceVaseGroup(x + 476, y + 117);
            PlaceVaseGroup(x + 472, y + 113);
            PlaceVaseGroup(x + 468, y + 109);
            PlaceVaseGroup(x + 464, y + 105);
            PlaceVaseGroup(x + 460, y + 101);

            PlaceVaseGroup(x + 525, y + 133);
            PlaceVaseGroup(x + 513, y + 131);
            PlaceVaseGroup(x + 501, y + 129);

            #endregion
            WorldGen.PlaceObject(x + 432, y + 130, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 449, y + 130, ModContent.TileType<WaxSconceEven>());
            WorldGen.PlaceObject(x + 465, y + 130, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 449, y + 67, ModContent.TileType<WaxChandelier>());
            WorldGen.PlaceObject(x + 322, y + 132, ModContent.TileType<Selfie>());
            WorldGen.PlaceObject(x + 572, y + 132, ModContent.TileType<ABriefRespite>());

            WorldGen.PlaceObject(x + 446, y + 86, ModContent.TileType<TheDayIMetHer>());

            PlaceAndConfigureDoor(x + 444, y + 61, DoorID.FoyerTreasureEntrance, DoorID.FoyerTreasureExit, isLocked: true);

            PlaceBookshelfArch(x + 350, y + 36);
            PlaceBookshelfArch(x + 350, y + 66);
            PlaceBookshelfArch(x + 536, y + 36);
            PlaceBookshelfArch(x + 536, y + 66);

            PlaceTableAndChair(x + 342, y + 91, direction: -1, RoomID.Yellow);
            PlaceTableAndChair(x + 368, y + 91, direction: -1, RoomID.Yellow);
            PlaceTableAndChair(x + 342, y + 61, direction: -1, RoomID.Yellow);
            PlaceTableAndChair(x + 368, y + 61, direction: -1, RoomID.Yellow);

            PlaceLoungeArea(x + 313, y + 138, RoomID.Yellow);
            PlaceLoungeArea(x + 563, y + 138, RoomID.Yellow);

            PlaceTableAndChair(x + 527, y + 91, direction: 1, RoomID.Yellow);
            PlaceTableAndChair(x + 553, y + 91, direction: 1, RoomID.Yellow);
            PlaceTableAndChair(x + 527, y + 61, direction: 1, RoomID.Yellow);
            PlaceTableAndChair(x + 553, y + 61, direction: 1, RoomID.Yellow);

            PlaceLongTableAndChairs(x + 423, y + 91, RoomID.Yellow);
            PlaceLongTableAndChairs(x + 423, y + 61, RoomID.Yellow);
            PlaceLongTableAndChairs(x + 466, y + 91, RoomID.Yellow);
            PlaceLongTableAndChairs(x + 466, y + 61, RoomID.Yellow);

            WorldGen.PlaceObject(x + 383, y + 124, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 428, y + 104, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 469, y + 104, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 514, y + 124, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 420, y + 133, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 422, y + 126, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 428, y + 133, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 437, y + 133, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 439, y + 126, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 445, y + 133, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 453, y + 133, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 455, y + 126, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 461, y + 133, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 470, y + 133, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 472, y + 126, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 478, y + 133, ModContent.TileType<HallwayPillar>());
            #endregion

            #region Top Left Bridges
            #region Vases
            PlaceVaseGroup(x + 326, y + 87);
            PlaceVaseGroup(x + 326, y + 57);
            PlaceVaseGroup(x + 314, y + 89);
            PlaceVaseGroup(x + 314, y + 59);
            PlaceVaseGroup(x + 302, y + 91);
            PlaceVaseGroup(x + 302, y + 61);
            #endregion

            WorldGen.PlaceObject(x + 184, y + 60, ModContent.TileType<GodsIris>());
            PlaceLoungeArea(x + 176, y + 66, RoomID.Green);
            PlaceAndConfigureDoor(x + 182, y + 96, DoorID.FoyerGreenRoomDoor, DoorID.GreenBridgeRoomEntrance);
            WorldGen.PlaceObject(x + 174, y + 96, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 200, y + 96, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 181, y + 71, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 210, y + 96, ModContent.TileType<WaxCandleholder>());

            PlaceMultiBookPiles(x + 350, y + 91);
            PlaceMultiBookPiles(x + 350, y + 61);

            PlaceDiagonalStairStack(x + 385, y + 91);

            WorldGen.PlaceObject(x + 385, y + 36, ModContent.TileType<WoodenArch>());
            //PlaceDiagonalStairStack(x + 385, y + 91);
            WorldGen.PlaceObject(x + 403, y + 81, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 403, y + 51, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 378, y + 81, ModContent.TileType<WaxSconceEven>());
            WorldGen.PlaceObject(x + 378, y + 51, ModContent.TileType<WaxSconceEven>());

            WorldGen.PlaceObject(x + 385, y + 36, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 226, y + 71, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 226, y + 41, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 263, y + 71, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 263, y + 41, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 243, y + 67, ModContent.TileType<ArchiveBridge>());
            WorldGen.PlaceObject(x + 243, y + 97, ModContent.TileType<ArchiveBridge>());

            WorldGen.PlaceObject(x + 260, y + 66, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 260, y + 96, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 210, y + 66, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 292, y + 66, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 292, y + 96, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 333, y + 61, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 333, y + 91, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 315, y + 52, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 315, y + 82, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 413, y + 61, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 413, y + 91, ModContent.TileType<WaxCandleholder>());
            #endregion

            #region Top Right Bridges
            #region Vases
            PlaceVaseGroup(x + 569, y + 87);
            PlaceVaseGroup(x + 569, y + 57);
            PlaceVaseGroup(x + 581, y + 89);
            PlaceVaseGroup(x + 581, y + 59);
            PlaceVaseGroup(x + 593, y + 91);
            PlaceVaseGroup(x + 593, y + 61);
            #endregion

            PlaceLoungeArea(x + 700, y + 66, RoomID.Yellow);
            PlaceAndConfigureDoor(x + 706, y + 96, DoorID.FoyerYellowRoomDoor, DoorID.YellowPitRoomDoorEntrance);
            WorldGen.PlaceObject(x + 698, y + 96, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 724, y + 96, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 705, y + 71, ModContent.TileType<WoodenArch>());

            //PlaceTallStairs(x + 493, y + 91);

            WorldGen.PlaceObject(x + 501, y + 36, ModContent.TileType<WoodenArch>());
            PlaceDiagonalStairStack(x + 501, y + 91);
            WorldGen.PlaceObject(x + 494, y + 81, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 494, y + 51, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 520, y + 81, ModContent.TileType<WaxSconceEven>());
            WorldGen.PlaceObject(x + 520, y + 51, ModContent.TileType<WaxSconceEven>());

            PlaceMultiBookPiles(x + 536, y + 91);
            PlaceMultiBookPiles(x + 536, y + 61);

            WorldGen.PlaceObject(x + 501, y + 36, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 660, y + 71, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 623, y + 71, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 660, y + 41, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 623, y + 41, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 640, y + 67, ModContent.TileType<ArchiveBridge>());
            WorldGen.PlaceObject(x + 640, y + 97, ModContent.TileType<ArchiveBridge>());

            WorldGen.PlaceObject(x + 657, y + 66, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 657, y + 96, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 688, y + 66, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 688, y + 96, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 606, y + 66, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 606, y + 96, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 565, y + 61, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 565, y + 91, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 582, y + 52, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 582, y + 82, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 485, y + 61, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 485, y + 91, ModContent.TileType<WaxCandleholder>());
            #endregion

            SetupSpawners(x, y);
        }

        private void SetupSpawners(int x, int y)
        {
            #region Bottom Left Side
            AddSpawnPoint(new Vector2(x + 156, y + 250), ModContent.NPCType<BlasterBook>());
            #endregion

            #region Red Door
            AddSpawnPoint(new Vector2(x + 138, y + 219), ModContent.NPCType<InkWormBody>());
            AddSpawnPoint(new Vector2(x + 172, y + 211), ModContent.NPCType<ChairBook>());

            AddSpawnPoint(new Vector2(x + 63, y + 192), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 148, y + 180), ModContent.NPCType<PlantBook>());
            #endregion

            #region Middle Area
            AddSpawnPoint(new Vector2(x + 234, y + 221), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 653, y + 221), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 385, y + 202), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 489, y + 200), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 562, y + 137), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 444, y + 132), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 357, y + 136), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 485, y + 90), ModContent.NPCType<ClockworkSpider>());
            #endregion

            #region Green Door
            AddSpawnPoint(new Vector2(x + 333, y + 89), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 379, y + 95), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 356, y + 47), ModContent.NPCType<ChairBook>());
            #endregion

            #region Yellow Door
            AddSpawnPoint(new Vector2(x + 538, y + 77), ModContent.NPCType<PlantBook>());
            AddSpawnPoint(new Vector2(x + 665, y + 95), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 529, y + 60), ModContent.NPCType<ArchiveRat>());
            #endregion

            #region Blue Door
            AddSpawnPoint(new Vector2(x + 727, y + 176), ModContent.NPCType<InkWormBody>());

            AddSpawnPoint(new Vector2(x + 727, y + 214), ModContent.NPCType<BarrierBook>());
            AddSpawnPoint(new Vector2(x + 722, y + 223), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 768, y + 193), ModContent.NPCType<ArchiveRat>());
            #endregion

            #region Bottom Right Side
            AddSpawnPoint(new Vector2(x + 747, y + 248), ModContent.NPCType<BlasterBook>());
            AddSpawnPoint(new Vector2(x + 761, y + 246), ModContent.NPCType<BlasterBook>()); 
            #endregion
        }
    }
}