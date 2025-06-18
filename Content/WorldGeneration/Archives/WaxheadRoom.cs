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
    public class WaxheadRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/ArchiveYellowRoomTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/ArchiveYellowRoomWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/ArchiveYellowRoomObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        public override void PostGenerate(int x, int y)
        {
            #region Top Right Room
            WorldGen.PlaceObject(x + 62, y + 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 55, y + 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 81, y + 110, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(x + 63, y + 110, DoorID.YellowWaxheadRoomEntrance, DoorID.YellowStairsRoomExit);

            PlaceBookshelfArch(x + 140, y + 60);
            PlaceBookshelfArch(x + 166, y + 60);
            PlaceBookshelfArch(x + 192, y + 60);
            PlaceBookshelfArch(x + 140, y + 90);
            PlaceBookshelfArch(x + 166, y + 90);
            PlaceBookshelfArch(x + 192, y + 90);

            PlaceBookshelfArch(x + 307, y + 60);
            PlaceBookshelfArch(x + 333, y + 60);
            PlaceBookshelfArch(x + 359, y + 60);
            PlaceBookshelfArch(x + 307, y + 90);
            PlaceBookshelfArch(x + 333, y + 90);
            PlaceBookshelfArch(x + 359, y + 90);

            #region Bridge
            WorldGen.PlaceObject(x + 282, y + 60, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 220, y + 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 220, y + 90, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 257, y + 60, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 257, y + 90, ModContent.TileType<WoodenArch>());

            WorldGen.PlaceObject(x + 254, y + 85, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 234, y + 115, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 254, y + 115, ModContent.TileType<WoodenPillar2>());

            WorldGen.PlaceObject(x + 237, y + 86, ModContent.TileType<ArchiveBridge>());
            #endregion

            PlaceBookshelfArch(x + 474, y + 55);
            PlaceBookshelfArch(x + 500, y + 55);
            PlaceBookshelfArch(x + 526, y + 55);
            PlaceBookshelfArch(x + 474, y + 85);
            PlaceBookshelfArch(x + 500, y + 85);
            PlaceBookshelfArch(x + 526, y + 85);

            //PlaceLoungeArea(1490, 80, RoomID.Yellow);
            PlaceLoungeArea(x + 57, y + 80, RoomID.Yellow);

            PlaceCozyArea(x + 430, y + 110, RoomID.Yellow);
            PlaceLoungeArea(x + 432, y + 80, RoomID.Yellow);

            #endregion
        }
    }
}