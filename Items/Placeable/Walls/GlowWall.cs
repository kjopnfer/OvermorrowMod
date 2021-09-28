using OvermorrowMod.Items.Placeable.Tiles;
using Terraria.ID;
using Terraria.ModLoader;
using GlowWallWall = OvermorrowMod.Tiles.GlowWall;

namespace OvermorrowMod.Items.Placeable.Walls
{
    public class GlowWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wet Stone Wall");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 22;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createWall = ModContent.WallType<GlowWallWall>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GlowBlock>());
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 4);
            recipe.AddRecipe();

            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(this, 4);
            recipe1.AddTile(TileID.WorkBenches);
            recipe1.SetResult(ModContent.ItemType<GlowBlock>());
            recipe1.AddRecipe();
        }
    }
}