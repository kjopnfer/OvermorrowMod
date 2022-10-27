using OvermorrowMod.Content.Tiles.Town;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Placeable.Furniture
{
    public class TownChair : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wooden Chair");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<TownChair>(), 0);
            Item.width = 16;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.value = 400;
        }
    }
}