using Terraria.ID;
using Terraria.ModLoader;
using Block = OvermorrowMod.Content.Tiles.Town.BookPile;

namespace OvermorrowMod.Content.Items.Placeable.Furniture
{
    public class BookPile : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stack of Books");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 16;
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