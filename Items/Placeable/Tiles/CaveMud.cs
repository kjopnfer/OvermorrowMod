using Terraria.ID;
using Terraria.ModLoader;
using CaveMudBlock = OvermorrowMod.Tiles.CaveMud;

namespace OvermorrowMod.Items.Placeable.Tiles
{
    public class CaveMud : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Submerged Mud");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<CaveMudBlock>();
        }
    }
}
