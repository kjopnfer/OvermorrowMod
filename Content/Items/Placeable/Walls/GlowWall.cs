using OvermorrowMod.Content.Items.Placeable.Tiles;
using Terraria.ID;
using Terraria.ModLoader;
using GlowWallWall = OvermorrowMod.Content.Tiles.WaterCave.GlowWall;

namespace OvermorrowMod.Content.Items.Placeable.Walls
{
    public class GlowWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wet Stone Wall");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<GlowWallWall>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<GlowBlock>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}