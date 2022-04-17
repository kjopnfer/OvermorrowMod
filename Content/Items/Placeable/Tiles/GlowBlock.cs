using OvermorrowMod.Content.Items.Placeable.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using GlowBlockBlock = OvermorrowMod.Content.Tiles.WaterCave.GlowBlock;


namespace OvermorrowMod.Content.Items.Placeable.Tiles
{
    public class GlowBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wet Stone");
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
            Item.createTile = ModContent.TileType<GlowBlockBlock>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlowWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

}
