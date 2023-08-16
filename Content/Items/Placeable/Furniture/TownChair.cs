using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Placeable.Furniture
{
    public class TownChair : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wooden Chair");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Tiles.Town.TownChair>(), 0);
            Item.width = 16;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.value = 400;
        }
    }
}