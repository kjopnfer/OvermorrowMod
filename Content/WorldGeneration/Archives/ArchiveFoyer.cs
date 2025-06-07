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

        protected override Dictionary<Color, int> TileMapping => new()
        {
            [new Color(105, 106, 106)] = ModContent.TileType<CastleBrick>(),
            [new Color(89, 86, 82)] = ModContent.TileType<DarkCastleBrick>(),
            [new Color(138, 111, 48)] = ModContent.TileType<CastlePlatform>(),
            [new Color(74, 47, 33)] = ModContent.TileType<ArchiveWood>(),
            [new Color(150, 150, 150)] = -2,
            [Color.Black] = -1
        };

        protected override Dictionary<Color, int> WallMapping => new()
        {
            [Color.Black] = -1,
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

        protected override Dictionary<Color, (int, int)> ObjectMapping => new()
        {
            [new Color(215, 186, 87)] = (ModContent.TileType<ArchivePot>(), 1),
            [new Color(178, 149, 52)] = (ModContent.TileType<SanctumGate>(), 1),
            [new Color(75, 105, 47)] = (ModContent.TileType<BookPileTable>(), 1),
            [new Color(69, 40, 60)] = (ModContent.TileType<BanquetTable>(), 1),
            [new Color(88, 27, 69)] = (ModContent.TileType<CastleChair>(), 1),
            [new Color(208, 61, 125)] = (ModContent.TileType<CozyChair>(), 1),
            [new Color(180, 58, 0)] = (ModContent.TileType<Fireplace>(), 1),
            [new Color(99, 49, 110)] = (ModContent.TileType<FireplacePillar>(), 1),
            [new Color(223, 113, 38)] = (ModContent.TileType<FloorCandles>(), 6),
            [new Color(74, 15, 56)] = (ModContent.TileType<WoodenPillar>(), 1),
            [new Color(179, 36, 136)] = (ModContent.TileType<WoodenPillar2>(), 1),
            [new Color(115, 72, 34)] = (ModContent.TileType<ArchiveBridge>(), 1),
            [new Color(135, 28, 66)] = (ModContent.TileType<WoodenArch>(), 1),
            [new Color(171, 73, 94)] = (ModContent.TileType<WoodenArchSmall>(), 1),
            [new Color(159, 131, 65)] = (ModContent.TileType<WaxCandelabra>(), 1),
            [new Color(134, 42, 104)] = (ModContent.TileType<SmallChair>(), 1),
            [new Color(148, 109, 65)] = (ModContent.TileType<WaxCandleholder>(), 1),
            [new Color(159, 183, 204)] = (ModContent.TileType<Bismarck>(), 1),
        };

        public override void PostGenerate(int x, int y)
        {
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

            #region Fireplace
            WorldGen.PlaceObject(x + 408, y + 110, ModContent.TileType<WaxCandleholder>());

            PlaceCozyArea(x + 418, y + 110, RoomID.Yellow);

            WorldGen.PlaceObject(x + 454, y + 110, ModContent.TileType<WaxCandleholder>());

            WorldGen.PlaceObject(x + 425, y + 55, ModContent.TileType<WoodenArch>());
            TileUtils.PlaceTileWithEntity<SanctumGate, SanctumGate_TE>(x + 425, y + 80);
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
            PlaceAndConfigureDoor(x + 51, y + 80, DoorID.GreenRoomEntrance, DoorID.GreenRoom);

            WorldGen.PlaceObject(x + 50, y + 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 43, y + 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 69, y + 110, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(x + 51, y + 110, DoorID.RedRoomEntrance, DoorID.RedRoom);

            WorldGen.PlaceObject(x + 800, y + 55, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 793, y + 80, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 819, y + 80, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(x + 801, y + 80, DoorID.FoyerYellowRoomDoor, DoorID.YellowPitRoomDoor);

            WorldGen.PlaceObject(x + 800, y + 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 793, y + 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 819, y + 110, ModContent.TileType<WaxCandleholder>());
            //PlaceAndConfigureDoor(x + 801, y + 110, DoorID.BlueRoomEntrance, DoorID.BlueRoom);

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