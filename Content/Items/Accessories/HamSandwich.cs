using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Content.Items.Accessories;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class HamSandwich : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ham Sandwich");
            Tooltip.SetDefault("Yumby\nIncreases max life by 50.");
        }

        public override void SetDefaults()
        {
            item.accessory = true;
            item.width = 26;
            item.height = 24;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(0, 0, 25, 0);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 50;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StaleBread>(), 1);
            recipe.AddIngredient(ModContent.ItemType<StinkCheese>(), 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}