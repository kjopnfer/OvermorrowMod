using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class EruditeDefense : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Erudite Defense Talisman");
            Tooltip.SetDefault("Increases armor by 3");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 40;
            item.value = Item.buyPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 3;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<EruditeOrb>(), 1);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}