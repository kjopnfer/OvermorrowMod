using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class EruditeDamage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Erudite Damage Talisman");
            Tooltip.SetDefault("Increases all damage by 2");
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().EruditeDamage = true;
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