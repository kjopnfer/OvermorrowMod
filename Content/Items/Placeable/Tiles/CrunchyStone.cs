using Terraria.ID;
using Terraria.ModLoader;
using StoneBlock = OvermorrowMod.Content.Tiles.CrunchyStone;


namespace OvermorrowMod.Content.Items.Placeable.Tiles
{
    public class CrunchyStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gabbro");
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
            item.createTile = ModContent.TileType<StoneBlock>();
        }
    }

}
