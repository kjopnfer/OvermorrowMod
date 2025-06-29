using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Archives.Accessories;
using OvermorrowMod.Content.Tiles.Archives;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.WorldGeneration.Archives
{
    public class GreenTreasureRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/GreenTreasureTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/GreenTreasureWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/GreenTreasureObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        public override void PostGenerate(int x, int y)
        {
            PlaceAndConfigureDoor(x + 83, y + 65, DoorID.GreenBridgeTreasureExit, DoorID.GreenBridgeTreasureEntrance, isLocked: true);
            PlaceDoorObjects(x + 83, y + 65);

            int PlacementSuccess = WorldGen.PlaceChest(x + 60, y + 64, (ushort)ModContent.TileType<IlluminatiChest>(), false, 0);
            if (PlacementSuccess >= 0)
            {
                Chest chest = Main.chest[PlacementSuccess];

                int slot = 0;
                chest.item[slot].SetDefaults(ModContent.ItemType<WhitePage>());
            }

            WorldGen.PlaceObject(x + 60, y + 46, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 74, y + 46, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 100, y + 46, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 114, y + 46, ModContent.TileType<ArchiveBanner>(), true);
        }
    }
}