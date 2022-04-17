using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class ManaBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana-Infused Bar");
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 550;
            Item.rare = ItemRarityID.Green;
            Item.maxStack = 999;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            //Item.createTile = ModContent.TileType<ManaBarTile>();
            //Item.placeStyle = 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.55f * Main.essScale);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("IronBar", 5)
                .AddIngredient<CrystalMana>(3)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}