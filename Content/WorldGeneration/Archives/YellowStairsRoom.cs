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

            PlaceMultiVase(x + 172, y + 253, 1, 7);
            PlaceMultiVase(x + 221, y + 216, 1, 5);
            PlaceMultiVase(x + 260, y + 229, -1, 7);

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
            PlaceMultiVase(x + 276, y + 150, -1, 7);

            WorldGen.PlaceObject(x + 330, y + 169, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 338, y + 161, ModContent.TileType<WoodenPillar2>());
            WorldGen.PlaceObject(x + 346, y + 153, ModContent.TileType<WoodenPillar2>());
            PlaceMultiVase(x + 325, y + 174, 1, 7);

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
            PlaceMultiVase(x + 409, y + 187, -1, 7);

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
            #endregion

            PlaceAndConfigureDoor(x + 542, y + 78, DoorID.YellowStairsRoomExit, DoorID.WaxheadRoomEntrance);
        }
    }
}