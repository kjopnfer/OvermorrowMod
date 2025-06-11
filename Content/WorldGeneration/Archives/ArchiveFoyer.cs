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
            WorldGen.PlaceObject(x + 101, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 116, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 124, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 139, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 147, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 162, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 170, y + 273, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 176, y + 273, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 194, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 202, y + 273, ModContent.TileType<HallwayPillar>());
            #endregion

            #region Center Fireplace Area
            WorldGen.PlaceObject(x + 226, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 234, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 242, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 250, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 258, y + 236, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 263, y + 236, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 281, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 283, y + 231, ModContent.TileType<WoodenArchSmall>());
            WorldGen.PlaceObject(x + 289, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 291, y + 231, ModContent.TileType<WoodenArchSmall>());
            WorldGen.PlaceObject(x + 297, y + 236, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 308, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 310, y + 231, ModContent.TileType<WoodenArchSmall>());
            WorldGen.PlaceObject(x + 316, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 318, y + 231, ModContent.TileType<WoodenArchSmall>());
            WorldGen.PlaceObject(x + 324, y + 236, ModContent.TileType<HallwayPillar>());

            PlaceBookshelfArch(x + 339, y + 211);
            PlaceBookshelfArch(x + 365, y + 211);

            WorldGen.PlaceObject(x + 385, y + 236, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 426, y + 231, ModContent.TileType<WaxCandleholder>());

            PlaceCozyArea(x + 436, y + 231, RoomID.Yellow);

            WorldGen.PlaceObject(x + 472, y + 231, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 443, y + 174, ModContent.TileType<WoodenArch>());
            TileUtils.PlaceTileWithEntity<SanctumGate, SanctumGate_TE>(x + 443, y + 199);

            WorldGen.PlaceObject(x + 513, y + 236, ModContent.TileType<WaxCandleholder>());

            PlaceBookshelfArch(x + 521, y + 211);
            PlaceBookshelfArch(x + 547, y + 211);

            WorldGen.PlaceObject(x + 574, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 582, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 591, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 599, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 608, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 616, y + 236, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 622, y + 236, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 640, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 648, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 656, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 664, y + 236, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 672, y + 236, ModContent.TileType<HallwayPillar>());
            #endregion

            #region Bottom Right Hallway
            WorldGen.PlaceObject(x + 696, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 704, y + 273, ModContent.TileType<HallwayPillar>());

            WorldGen.PlaceObject(x + 710, y + 273, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 728, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 736, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 751, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 759, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 774, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 782, y + 273, ModContent.TileType<HallwayPillar>());
            WorldGen.PlaceObject(x + 797, y + 273, ModContent.TileType<HallwayPillar>());

            #endregion

            #region Left Stairs
            WorldGen.PlaceObject(x + 77, y + 194, ModContent.TileType<WaxCandleholder>());
            PlaceLoungeArea(x + 43, y + 194, RoomID.Red);
            WorldGen.PlaceObject(x + 121, y + 194, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 48, y + 199, ModContent.TileType<WoodenArch>());
            PlaceAndConfigureDoor(x + 49, y + 224, DoorID.RedRoomEntrance, DoorID.RedRoom);
            WorldGen.PlaceObject(x + 77, y + 224, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 121, y + 224, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 240, y + 194, ModContent.TileType<Finial>());
            WorldGen.PlaceObject(x + 240, y + 224, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 243, y + 195, ModContent.TileType<ArchiveBridge>());

            WorldGen.PlaceObject(x + 260, y + 194, ModContent.TileType<Finial>());
            WorldGen.PlaceObject(x + 260, y + 224, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 277, y + 194, ModContent.TileType<Finial>());

            WorldGen.PlaceObject(x + 306, y + 138, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 289, y + 150, ModContent.TileType<StairPillar>());
            WorldGen.PlaceObject(x + 292, y + 150, ModContent.TileType<WoodenStairs>());

            WorldGen.PlaceObject(x + 330, y + 167, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 333, y + 179, ModContent.TileType<WoodenStairs>());
            WorldGen.PlaceObject(x + 347, y + 179, ModContent.TileType<StairPillar>());
            WorldGen.PlaceObject(x + 346, y + 179, ModContent.TileType<StairPillar>());
            WorldGen.PlaceObject(x + 348, y + 179, ModContent.TileType<StairPillar>());

            WorldGen.PlaceObject(x + 402, y + 133, ModContent.TileType<WoodenStairs>());
            WorldGen.PlaceObject(x + 416, y + 121, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 443, y + 104, ModContent.TileType<WoodenStairs>());
            #endregion

            #region Right Stairs
            WorldGen.PlaceObject(x + 620, y + 224, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 777, y + 194, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 777, y + 224, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 821, y + 194, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 821, y + 224, ModContent.TileType<WaxCandleholder>());

            PlaceLoungeArea(x + 833, y + 194, RoomID.Blue);

            WorldGen.PlaceObject(x + 48, y + 199, ModContent.TileType<WoodenArch>());
            PlaceAndConfigureDoor(x + 838, y + 224, DoorID.FoyerBlueRoomDoor, DoorID.BlueShrimpRoomEntrance);

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