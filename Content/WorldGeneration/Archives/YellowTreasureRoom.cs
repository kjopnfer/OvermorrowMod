using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Archives.Accessories;
using OvermorrowMod.Content.Tiles.Archives;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.WorldGeneration.Archives
{
    public class YellowTreasureRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/YellowTreasureTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/YellowTreasureWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/YellowTreasureObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        public override void PostGenerate(int x, int y)
        {
            PlaceAndConfigureDoor(x + 82, y + 64, DoorID.YellowStairsTreasureExit, DoorID.YellowStairsTreasureEntrance, isLocked: true);
            PlaceDoorObjects(x + 82, y + 64);

            int PlacementSuccess = WorldGen.PlaceChest(x + 59, y + 63, (ushort)ModContent.TileType<IlluminatiChest>(), false, 0);
            if (PlacementSuccess >= 0)
            {
                Chest chest = Main.chest[PlacementSuccess];

                int slot = 0;
                chest.item[slot].SetDefaults(ModContent.ItemType<WhitePage>());
            }

            WorldGen.PlaceObject(x + 59, y + 45, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 73, y + 45, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 99, y + 45, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 113, y + 45, ModContent.TileType<ArchiveBanner>(), true);
        }
    }
}