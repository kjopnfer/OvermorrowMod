using OvermorrowMod.Common;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Content.Dungeons.Archive;
using OvermorrowMod.Content.Dungeons.Archive.Cells;
using OvermorrowMod.Content.NPCs.AdventurersGuild;
using OvermorrowMod.Content.Tiles.AdventurersGuild;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldGeneration.Guild
{
    public class GuildGenPass : GenPass
    {
        private const string MapPath = AssetDirectory.GuildMaps + "Guild.aseprite";

        public static readonly (byte R, byte G, byte B) GarrisonBannerMarker = (0, 146, 255);
        public static readonly (byte R, byte G, byte B) WizardStatueMarker = (103, 51, 255);
        public static readonly (byte R, byte G, byte B) MoxleyMarker = (255, 0, 0);
        public static readonly (byte R, byte G, byte B) PortalMarker = (102, 251, 0);

        public GuildGenPass() : base("Loading", 1) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Building the guild";

            GuildSubworld guild = ModContent.GetInstance<GuildSubworld>();
            Main.worldSurface = guild.Height - 200;
            Main.rockLayer = guild.Height;

            try
            {
                (int width, int height) = TexGen.GetCanvasSize(MapPath);
                int x = (guild.Width - width) / 2;
                int y = (guild.Height - height) / 2;

                ArchivePalette palette = new ArchivePalette();
                var objects = new Dictionary<(byte R, byte G, byte B), TexPlaceFunction>(palette.Objects)
                {
                    [GarrisonBannerMarker] = TexPlaceAction.PlaceObject(ModContent.TileType<GarrisonBanner>()),
                    [WizardStatueMarker] = TexPlaceAction.PlaceObject(ModContent.TileType<GuildWizardStatue>()),
                    [WritingRoom.LadderMarker] = TexPlaceAction.CustomPlaceObject(WritingRoom.PlaceLadder),
                };

                TexGen.PaintClearLayer(MapPath, x, y);
                TexGen.PaintAsepriteLayer(SheetLayer.Walls, MapPath, x, y, palette.Walls);
                TexGen.PaintAsepriteLayer(SheetLayer.Tiles, MapPath, x, y, palette.Tiles);
                TexGen.PaintAsepriteLayer(SheetLayer.Objects, MapPath, x, y, objects);
                TexGen.HarvestAsepriteLayer(SheetLayer.Spawns, MapPath, x, y, PlaceMarker);
            }
            finally
            {
                TexGen.ClearCache();
            }
        }

        private static void PlaceMarker(int x, int y, (byte R, byte G, byte B) color)
        {
            if (color == MoxleyMarker)
            {
                int id = ModContent.GetInstance<ShopkeeperSpawnPoint>().Place(x, y);
                if (TileEntity.ByID.TryGetValue(id, out TileEntity entity) && entity is ShopkeeperSpawnPoint spawnPoint)
                    spawnPoint.NPCType = ModContent.NPCType<Moxley>();

                Main.spawnTileX = x;
                Main.spawnTileY = y + 1;
            }
            else if (color == PortalMarker)
            {
                WorldGen.PlaceObject(x, y, ModContent.TileType<GuildPortal>(), true);
            }
        }
    }
}
