using Terraria.ID;
using Terraria.ModLoader;
using Block = OvermorrowMod.Content.Tiles.Carts.Inkwell;

namespace OvermorrowMod.Content.Items.Placeable.Furniture
{
    public class Inkwell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Inkwell");
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 2000;
            Item.createTile = ModContent.TileType<Block>();
        }
    }
}