using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items.Archives.Accessories;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Content.Tiles.Archives;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.WorldGeneration.Archives
{
    public class FoyerTreasureRoom : GrandArchiveRoom
    {
        protected override Texture2D Tiles => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/FoyerTreasureTiles", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Walls => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/FoyerTreasureWalls", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Slopes => null;
        protected override Texture2D Objects => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "GrandArchives/FoyerTreasureObjects", AssetRequestMode.ImmediateLoad).Value;
        protected override Texture2D Liquids => null;

        public override void PostGenerate(int x, int y)
        {
            PlaceAndConfigureDoor(x + 84, y + 64, DoorID.FoyerTreasureExit, DoorID.FoyerTreasureEntrance, isLocked: true);
            PlaceDoorObjects(x + 84, y + 64);

            int PlacementSuccess = WorldGen.PlaceChest(x + 61, y + 63, (ushort)ModContent.TileType<IlluminatiChest>(), false, 0);
            if (PlacementSuccess >= 0)
            {
                Chest chest = Main.chest[PlacementSuccess];

                int slot = 0;
                chest.item[slot].SetDefaults(ModContent.ItemType<WhitePage>());
            }

            WorldGen.PlaceObject(x + 61, y + 45, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 74, y + 45, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 101, y + 45, ModContent.TileType<ArchiveBanner>(), true);
            WorldGen.PlaceObject(x + 115, y + 45, ModContent.TileType<ArchiveBanner>(), true);
        }
    }
}