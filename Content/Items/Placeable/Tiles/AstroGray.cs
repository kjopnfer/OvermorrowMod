using Terraria.ID;
using Terraria.ModLoader;
using Block = OvermorrowMod.Content.Tiles.AstroGray;


namespace OvermorrowMod.Content.Items.Placeable.Tiles
{
    public class AstroGray : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Wood");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Block>();
        }
    }
}
