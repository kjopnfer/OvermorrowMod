using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Materials
{
    public class ManaBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana-Infused Bar");
        }

		public override void SetDefaults()
		{
			item.width = 48;
			item.height = 30;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = 550;
			item.rare = ItemRarityID.Green;
			item.maxStack = 999;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			//item.createTile = ModContent.TileType<ManaBarTile>();
			//item.placeStyle = 0;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void PostUpdate()
		{
			Lighting.AddLight(item.Center, Color.White.ToVector3() * 0.55f * Main.essScale);
		}

		public override void AddRecipes()
        {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.IronBar, 5);
			recipe.AddIngredient(ModContent.ItemType<CrystalMana>(), 3);
			recipe.AddTile(TileID.Furnaces);
			recipe.SetResult(this, 5);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LeadBar, 5);
			recipe.AddIngredient(ModContent.ItemType<CrystalMana>(), 3);
			recipe.AddTile(TileID.Furnaces);
			recipe.SetResult(this, 5);
			recipe.AddRecipe();
		}
    }
}