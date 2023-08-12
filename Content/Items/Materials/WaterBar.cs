using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class WaterBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lacusite Bar");
            // Tooltip.SetDefault("'Has a strong affinity for water'");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.rare = ItemRarityID.Green;
            Item.maxStack = 99;
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
                .AddIngredient<WaterOre>(3)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}