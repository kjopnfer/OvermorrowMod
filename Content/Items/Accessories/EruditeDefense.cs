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
            // DisplayName.SetDefault("Erudite Defense Talisman");
            // Tooltip.SetDefault("Increases armor by {Increase:3}");
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EruditeOrb>()
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}