using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Accessories.GuideLantern;
using OvermorrowMod.Content.Items.Archives.Accessories;
using OvermorrowMod.Content.Tiles.Archives;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.WorldGeneration.Archives
{
    public class RedTreasureRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/RedTreasureTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/RedTreasureWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/RedTreasureObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        public override void PostGenerate(int x, int y)
        {
            PlaceAndConfigureDoor(x + 84, y + 62, DoorID.RedDiagonalTreasureExit, DoorID.RedDiagonalTreasureEntrance, isLocked: true);
            PlaceDoorObjects(x + 84, y + 62);

            int PlacementSuccess = WorldGen.PlaceChest(x + 61, y + 61, (ushort)ModContent.TileType<IlluminatiChest>(), false, 0);
            if (PlacementSuccess >= 0)
            {
                Chest chest = Main.chest[PlacementSuccess];

                int slot = 0;
                chest.item[slot].SetDefaults(ModContent.ItemType<WarriorsEpic>());
            }

            WorldGen.PlaceObject(x + 61, y + 43, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 75, y + 43, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 101, y + 43, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 115, y + 43, ModContent.TileType<ArchiveBanner>(), true);
        }
    }
}