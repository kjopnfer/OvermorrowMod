using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class HamSandwich : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ham Sandwich");
            Tooltip.SetDefault("Increases max life by 50\n'Yumby'");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 26;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 25, 0);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 50;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<StaleBread>()
                .AddIngredient<StinkCheese>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}