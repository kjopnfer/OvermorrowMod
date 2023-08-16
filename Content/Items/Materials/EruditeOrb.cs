using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class EruditeOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Erudite Orb");
            // Tooltip.SetDefault("'You know what I like a lot more than materialistic things? Knowledge!'");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.White;
            Item.maxStack = 999;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Green.ToVector3() * 0.55f * Main.essScale);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EruditeOre>(9)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}