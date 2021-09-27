
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged.Ammo
{
	public class FungalArrowAmmo : ModItem
	{
		public override void SetStaticDefaults() 
        {
            DisplayName.SetDefault("Fungal Arrow");
			Tooltip.SetDefault("Arrow that inflicts fungal infection");
		}

		public override void SetDefaults() {
			item.damage = 7;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 999;
			item.consumable = true;             //You need to set the item consumable so that the ammo would automatically consumed
			item.knockBack = 0f;
			item.value = 10;
			item.rare = ItemRarityID.Blue;
			item.shoot = ModContent.ProjectileType<FungalArrow>();   //The projectile shoot when your weapon using this ammo
			item.shootSpeed = 4.9f;                  //The speed of the projectile
			item.ammo = AmmoID.Arrow;              //The ammo class this ammo belongs to.
		}

		public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.WoodenArrow, 50);
			recipe.AddIngredient(ItemID.GlowingMushroom, 10);
            recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this, 50);
			recipe.AddRecipe();
		}
	}
}
