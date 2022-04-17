using Terraria.ID;
using Terraria.ModLoader;
using SandBlock = OvermorrowMod.Content.Tiles.DesertTemple.SandBrick;


namespace OvermorrowMod.Content.Items.Placeable.Tiles
{
    public class SandBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Brick");
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
            Item.createTile = ModContent.TileType<SandBlock>();
        }
    }
}
