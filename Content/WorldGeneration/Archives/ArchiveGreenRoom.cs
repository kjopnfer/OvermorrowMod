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
    public class ArchiveGreenRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/ArchiveGreenRoomTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/ArchiveGreenRoomWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/ArchiveGreenRoomObjects", AssetRequestMode.ImmediateLoad).Value;
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
            //[new Color(208, 61, 125)] = (ModContent.TileType<CozyChair>(), 1),
            [new Color(180, 58, 0)] = (ModContent.TileType<Fireplace>(), 1),
            [new Color(99, 49, 110)] = (ModContent.TileType<FireplacePillar>(), 1),
            [new Color(223, 113, 38)] = (ModContent.TileType<FloorCandles>(), 6),
            [new Color(74, 15, 56)] = (ModContent.TileType<WoodenPillar>(), 1),
            [new Color(179, 36, 136)] = (ModContent.TileType<WoodenPillar2>(), 1),
            [new Color(115, 72, 34)] = (ModContent.TileType<ArchiveBridge>(), 1),
            //[new Color(135, 28, 66)] = (ModContent.TileType<WoodenArch>(), 1),
            //[new Color(171, 73, 94)] = (ModContent.TileType<WoodenArchSmall>(), 1),
            [new Color(159, 131, 65)] = (ModContent.TileType<WaxCandelabra>(), 1),
            //[new Color(134, 42, 104)] = (ModContent.TileType<SmallChair>(), 1),
            [new Color(148, 109, 65)] = (ModContent.TileType<WaxCandleholder>(), 1),
            //[new Color(159, 183, 204)] = (ModContent.TileType<Bismarck>(), 1),
        };

        public override void PostGenerate(int x, int y)
        {
            #region Top Left Room
            WorldGen.PlaceObject(x + 491, y + 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 484, y + 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 510, y + 110, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(x + 492, y + 110, DoorID.GreenRoom, DoorID.GreenRoomEntrance);

            PlaceBookshelfArch(x + 27, y + 55);
            PlaceBookshelfArch(x + 53, y + 55);
            PlaceBookshelfArch(x + 79, y + 55);
            PlaceBookshelfArch(x + 27, y + 85);
            PlaceBookshelfArch(x + 53, y + 85);
            PlaceBookshelfArch(x + 79, y + 85);

            PlaceBookshelfArch(x + 194, y + 60);
            PlaceBookshelfArch(x + 220, y + 60);
            PlaceBookshelfArch(x + 246, y + 60);
            PlaceBookshelfArch(x + 194, y + 90);
            PlaceBookshelfArch(x + 220, y + 90);
            PlaceBookshelfArch(x + 246, y + 90);

            #region Bridge
            WorldGen.PlaceObject(x + 271, y + 60, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 296, y + 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 296, y + 90, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 333, y + 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 333, y + 90, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 310, y + 115, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 330, y + 85, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 330, y + 115, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 313, y + 86, ModContent.TileType<ArchiveBridge>());

            #endregion

            PlaceBookshelfArch(x + 361, y + 60);
            PlaceBookshelfArch(x + 387, y + 60);
            PlaceBookshelfArch(x + 413, y + 60);
            PlaceBookshelfArch(x + 361, y + 90);
            PlaceBookshelfArch(x + 387, y + 90);
            PlaceBookshelfArch(x + 413, y + 90);

            PlaceLoungeArea(x + 486, y + 80, RoomID.Green);
            PlaceLoungeArea(x + 111, y + 80, RoomID.Green);
            PlaceCozyArea(x + 109, y + 110, RoomID.Green);

            #endregion
        }
    }
}