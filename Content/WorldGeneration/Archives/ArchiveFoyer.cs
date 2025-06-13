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
using Terraria.ID;
using Terraria.ModLoader;
using static OvermorrowMod.Core.WorldGeneration.ArchiveSubworld.SetupGenPass;

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
            WorldGen.PlaceObject(x + 176, y + 273, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 194, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 196, y + 266, ModContent.TileType<WoodenArchSmallHallway>());
            WorldGen.PlaceObject(x + 202, y + 273, ModContent.TileType<HallwayPillar>());
            #endregion

            #region Center Fireplace Area
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
            PlaceBookshelfArch(x + 138, y + 169);
            PlaceBookshelfArch(x + 164, y + 169);
            PlaceBookshelfArch(x + 190, y + 169);
            PlaceBookshelfArch(x + 138, y + 199);
            PlaceBookshelfArch(x + 164, y + 199);
            PlaceBookshelfArch(x + 190, y + 199);

            PlaceTableAndChairs(x + 227, y + 194, RoomID.Red);

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
            PlaceAndConfigureDoor(x + 49, y + 224, DoorID.RedRoomEntrance, DoorID.RedRoom);
            WorldGen.PlaceObject(x + 41, y + 224, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 67, y + 224, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 76, y + 174, ModContent.TileType<ArchiveBanner>());
            //WorldGen.PlaceObject(x + 86, y + 214, ModContent.TileType<WaxSconce>());
            //WorldGen.PlaceObject(x + 111, y + 214, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 120, y + 174, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 76, y + 204, ModContent.TileType<ArchiveBanner>());
            //WorldGen.PlaceObject(x + 86, y + 184, ModContent.TileType<WaxSconce>());
            //WorldGen.PlaceObject(x + 111, y + 184, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 120, y + 204, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 226, y + 199, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 240, y + 194, ModContent.TileType<Finial>());
            WorldGen.PlaceObject(x + 240, y + 224, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 243, y + 195, ModContent.TileType<ArchiveBridge>());

            WorldGen.PlaceObject(x + 266, y + 194, ModContent.TileType<NormalWizardStatue>());

            WorldGen.PlaceObject(x + 260, y + 194, ModContent.TileType<Finial>());
            WorldGen.PlaceObject(x + 260, y + 224, ModContent.TileType<WoodenPillar2>());
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
            WorldGen.PlaceObject(x + 626, y + 194, ModContent.TileType<NormalWizardStatue>());
            WorldGen.PlaceObject(x + 637, y + 194, ModContent.TileType<Finial>());

            WorldGen.PlaceObject(x + 640, y + 195, ModContent.TileType<ArchiveBridge>());

            WorldGen.PlaceObject(x + 657, y + 194, ModContent.TileType<Finial>());
            WorldGen.PlaceObject(x + 660, y + 199, ModContent.TileType<WoodenArch>());
            PlaceTableAndChairs(x + 661, y + 194, RoomID.Blue);

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
            WorldGen.PlaceObject(x + 449, y + 67, ModContent.TileType<WaxChandelier>());
            WorldGen.PlaceObject(x + 322, y + 132, ModContent.TileType<Selfie>());
            WorldGen.PlaceObject(x + 527, y + 129, ModContent.TileType<ABriefRespite>());
            WorldGen.PlaceObject(x + 425, y + 83, ModContent.TileType<GodsIris>());

            PlaceBookshelfArch(x + 350, y + 36);
            PlaceBookshelfArch(x + 350, y + 66);
            PlaceBookshelfArch(x + 536, y + 36);
            PlaceBookshelfArch(x + 536, y + 66);

            PlaceLoungeArea(x + 313, y + 138, RoomID.Yellow);
            PlaceLoungeArea(x + 563, y + 138, RoomID.Yellow);

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
            PlaceLoungeArea(x + 176, y + 66, RoomID.Green);
            PlaceAndConfigureDoor(x + 182, y + 96, DoorID.FoyerGreenRoomDoor, DoorID.GreenBridgeRoomEntrance);
            WorldGen.PlaceObject(x + 174, y + 96, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 200, y + 96, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 181, y + 71, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 210, y + 96, ModContent.TileType<WaxCandleholder>());

            PlaceMultiBookPiles(x + 350, y + 91);
            PlaceMultiBookPiles(x + 350, y + 61);

            PlaceTallStairs(x + 376, y + 91);

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
            PlaceLoungeArea(x + 700, y + 66, RoomID.Yellow);
            PlaceAndConfigureDoor(x + 706, y + 96, DoorID.FoyerYellowRoomDoor, DoorID.YellowPitRoomDoorEntrance);
            WorldGen.PlaceObject(x + 698, y + 96, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 724, y + 96, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 705, y + 71, ModContent.TileType<WoodenArch>());

            PlaceTallStairs(x + 493, y + 91);

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
            return;
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

            WorldGen.PlaceObject(x + 120, y + 115, ModContent.TileType<WaxCandleholder>());

            PlaceBookPile(x + 128, y + 115, Main.rand.Next(2, 6));
            PlaceBookPile(x + 132, y + 115, Main.rand.Next(3, 12));
            PlaceBookPile(x + 135, y + 115, Main.rand.Next(4, 8), Main.rand.NextBool());
            PlaceBookPile(x + 137, y + 115, Main.rand.Next(2, 4));

            PlaceBookPile(x + 155, y + 115, Main.rand.Next(3, 12), true);
            PlaceBookPile(x + 157, y + 115, Main.rand.Next(3, 12));
            PlaceBookPile(x + 165, y + 115, Main.rand.Next(5, 7));

            PlaceBookPile(x + 180, y + 115, Main.rand.Next(2, 6));
            PlaceBookPile(x + 182, y + 115, Main.rand.Next(3, 7));
            PlaceBookPile(x + 185, y + 115, Main.rand.Next(4, 8), Main.rand.NextBool());
            PlaceBookPile(x + 191, y + 115, Main.rand.Next(2, 6));


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
            PlaceAndConfigureDoor(x + 51, y + 80, DoorID.FoyerGreenRoomDoor, DoorID.GreenBridgeRoomEntrance);

            WorldGen.PlaceObject(x + 50, y + 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 43, y + 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 69, y + 110, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(x + 51, y + 110, DoorID.RedRoomEntrance, DoorID.RedRoom);

            WorldGen.PlaceObject(x + 800, y + 55, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 793, y + 80, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 819, y + 80, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(x + 801, y + 80, DoorID.FoyerYellowRoomDoor, DoorID.YellowPitRoomDoorEntrance);

            WorldGen.PlaceObject(x + 800, y + 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 793, y + 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 819, y + 110, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(x + 801, y + 110, DoorID.FoyerBlueRoomDoor, DoorID.BlueShrimpRoomEntrance);

            WorldGen.PlaceObject(x + 287, y + 115, ModContent.TileType<WaxCandleholder>());

            PlaceBookPile(x + 295, y + 115, Main.rand.Next(2, 6));
            PlaceBookPile(x + 298, y + 115, Main.rand.Next(2, 6));

            PlaceBookPile(x + 301, y + 115, Main.rand.Next(3, 12), true);
            PlaceBookPile(x + 303, y + 115, Main.rand.Next(3, 7));

            PlaceBookPile(x + 313, y + 115, Main.rand.Next(3, 6));

            PlaceBookPile(x + 322, y + 115, Main.rand.Next(3, 9));
            PlaceBookPile(x + 328, y + 115, Main.rand.Next(3, 9));

            PlaceBookPile(x + 338, y + 115, Main.rand.Next(3, 9));
            PlaceBookPile(x + 340, y + 115, Main.rand.Next(5, 8));

            PlaceBookPile(x + 347, y + 115, Main.rand.Next(6, 9));
            PlaceBookPile(x + 349, y + 115, Main.rand.Next(3, 5));
            PlaceBookPile(x + 353, y + 115, Main.rand.Next(8, 12), true);
            PlaceBookPile(x + 359, y + 115, Main.rand.Next(3, 8));

            WorldGen.PlaceObject(x + 367, y + 115, ModContent.TileType<WaxCandleholder>());

            SetupSpawners(x, y);
            #endregion
        }

        private void SetupSpawners(int x, int y)
        {
            AddSpawnPoint(new Vector2(x + 128, y + 110), ModContent.NPCType<InkWormBody>());
            AddSpawnPoint(new Vector2(x + 166, y + 108), ModContent.NPCType<ChairBook>());

            AddSpawnPoint(new Vector2(x + 248, y + 114), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 326, y + 106), ModContent.NPCType<BlasterBook>());
            AddSpawnPoint(new Vector2(x + 321, y + 114), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 350, y + 76), ModContent.NPCType<PlantBook>());

            AddSpawnPoint(new Vector2(x + 186, y + 82), ModContent.NPCType<InkWormBody>());
            AddSpawnPoint(new Vector2(x + 156, y + 84), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 134, y + 84), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 532, y + 84), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 513, y + 108), ModContent.NPCType<PlantBook>());
            AddSpawnPoint(new Vector2(x + 538, y + 112), ModContent.NPCType<InkWormBody>());

            AddSpawnPoint(new Vector2(x + 677, y + 84), ModContent.NPCType<ArchiveRat>());
            AddSpawnPoint(new Vector2(x + 697, y + 78), ModContent.NPCType<BarrierBook>());
            AddSpawnPoint(new Vector2(x + 728, y + 75), ModContent.NPCType<ChairBook>());

            AddSpawnPoint(new Vector2(x + 678, y + 106), ModContent.NPCType<BlasterBook>());
            AddSpawnPoint(new Vector2(x + 606, y + 114), ModContent.NPCType<ArchiveRat>());

            AddSpawnPoint(new Vector2(x + 725, y + 110), ModContent.NPCType<InkWormBody>());
        }
    }
}