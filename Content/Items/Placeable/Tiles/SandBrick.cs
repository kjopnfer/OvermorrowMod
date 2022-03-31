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
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<SandBlock>();
        }
    }
}
