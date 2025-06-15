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
    public class GreenBridgeRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/GreenBridgeRoomTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/GreenBridgeRoomWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/GreenBridgeRoomObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        public override void PostGenerate(int x, int y)
        {
            #region Upper Right Side
            WorldGen.PlaceObject(x + 944, y + 131, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 970, y + 131, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 951, y + 106, ModContent.TileType<WoodenArch>());

            PlaceAndConfigureDoor(x + 952, y + 131, DoorID.GreenBridgeRoomEntrance, DoorID.FoyerGreenRoomDoor);

            WorldGen.PlaceObject(x + 921, y + 117, ModContent.TileType<WaxSconce>());
            PlaceVaseGroup(x + 908, y + 122);
            PlaceVaseGroup(x + 920, y + 124);
            PlaceVaseGroup(x + 932, y + 126);

            PlaceBookShelfObjects(x + 848, y + 126);
            PlaceTableAndChair(x + 841, y + 126, -1, RoomID.Green);
            PlaceBookShelfObjects(x + 874, y + 126);
            PlaceTableAndChair(x + 867, y + 126, -1, RoomID.Green);
            PlaceBookShelfObjects(x + 848, y + 96);
            PlaceTableAndChair(x + 893, y + 126, -1, RoomID.Green);
            PlaceBookShelfObjects(x + 874, y + 96);

            WorldGen.PlaceObject(x + 785, y + 106, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 831, y + 106, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 903, y + 106, ModContent.TileType<ArchiveBanner>());

            PlaceCozyArea(x + 796, y + 126, RoomID.Green);

            #region Second Floor
            WorldGen.PlaceObject(x + 785, y + 76, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 831, y + 76, ModContent.TileType<ArchiveBanner>());

            PlaceLoungeArea(x + 798, y + 96, RoomID.Green);
            PlaceBookShelfObjects(x + 848, y + 96);
            PlaceTableAndChair(x + 840, y + 96, 1, RoomID.Green);
            PlaceBookShelfObjects(x + 876, y + 96);
            PlaceTableAndChair(x + 866, y + 96, 1, RoomID.Green);
            PlaceTableAndChair(x + 892, y + 96, 1, RoomID.Green);

            WorldGen.PlaceObject(x + 921, y + 87, ModContent.TileType<WaxSconce>());
            PlaceVaseGroup(x + 908, y + 92);
            PlaceVaseGroup(x + 920, y + 94);
            PlaceVaseGroup(x + 932, y + 96);

            PlaceLoungeArea(x + 946, y + 101, RoomID.Green);
            #endregion

            #region Third Floor
            WorldGen.PlaceObject(x + 785, y + 46, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 831, y + 46, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 796, y + 66, ModContent.TileType<WaxCandleholder>());
            WorldGen.PlaceObject(x + 822, y + 66, ModContent.TileType<WaxCandleholder>());

            PlaceAndConfigureDoor(x + 804, y + 66, DoorID.GreenBridgeRoomEntrance, DoorID.FoyerGreenRoomDoor);
            PlaceLoungeArea(x + 844, y + 66, RoomID.Green);
            #endregion
            #region Stairs
            WorldGen.PlaceObject(x + 751, y + 116, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 776, y + 116, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 751, y + 86, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 776, y + 86, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 751, y + 56, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 776, y + 56, ModContent.TileType<WaxSconce>());

            WorldGen.PlaceObject(x + 758, y + 41, ModContent.TileType<WoodenArch>());
            PlaceDiagonalStairStack(x + 758, y + 126, 6);
            #endregion

            #endregion

            #region Middle Side
            WorldGen.PlaceObject(x + 741, y + 106, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 741, y + 76, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 731, y + 119, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 714, y + 109, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 724, y + 79, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 707, y + 72, ModContent.TileType<WaxSconce>());

            PlaceMultiVase(x + 705, y + 106, -1, 5);
            PlaceMultiVase(x + 715, y + 76, -1, 5);

            WorldGen.PlaceObject(x + 696, y + 59, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 696, y + 89, ModContent.TileType<ArchiveBanner>());

            for (int i = 0; i < 4; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 589 + offset, y + 79);
                PlaceBookShelfObjects(x + 589 + offset, y + 109);

                PlaceTableAndChair(x + 582 + offset, y + 109, -1, RoomID.Green);
            }

            WorldGen.PlaceObject(x + 572, y + 59, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 572, y + 89, ModContent.TileType<ArchiveBanner>());

            WorldGen.PlaceObject(x + 562, y + 102, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 545, y + 92, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 555, y + 62, ModContent.TileType<WaxSconce>());
            WorldGen.PlaceObject(x + 538, y + 55, ModContent.TileType<WaxSconce>());

            PlaceMultiVase(x + 536, y + 89, -1, 5);
            PlaceMultiVase(x + 546, y + 59, -1, 5);

            WorldGen.PlaceObject(x + 527, y + 72, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 527, y + 42, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 455, y + 72, ModContent.TileType<ArchiveBanner>());
            WorldGen.PlaceObject(x + 455, y + 42, ModContent.TileType<ArchiveBanner>());

            for (int i = 0; i < 2; i++)
            {
                var offset = 26 * i;
                PlaceBookShelfObjects(x + 472 + offset, y + 92);
                PlaceBookShelfObjects(x + 472 + offset, y + 62);

                PlaceTableAndChair(x + 465 + offset, y + 92, -1, RoomID.Green);
            }
            #endregion
            PlaceAndConfigureDoor(x + 215, y + 92, DoorID.GreenBridgeRoomExit, DoorID.WaxheadRoomEntrance);
        }
    }
}