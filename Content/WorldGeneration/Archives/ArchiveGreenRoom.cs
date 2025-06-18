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
        public override void PostGenerate(int x, int y)
        {
            #region Top Left Room
            WorldGen.PlaceObject(x + 491, y + 85, ModContent.TileType<WoodenArch>());
            WorldGen.PlaceObject(x + 484, y + 110, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 510, y + 110, ModContent.TileType<WaxCandleholder>());
            PlaceAndConfigureDoor(x + 492, y + 110, DoorID.GreenFlyingBookRoomEntrance, DoorID.GreenBridgeRoomExit);

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