
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged.Ammo
{
	public class FlamingBullet : ModItem
	{
		public override void SetStaticDefaults() 
        {
            DisplayName.SetDefault("Flaming Bullet");
			Tooltip.SetDefault("Shoots a short ranged flame");
		}

		public override void SetDefaults() {
			item.damage = 9;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 999;
			item.consumable = true;             //You need to set the item consumable so that the ammo would automatically consumed
			item.knockBack = 0f;
			item.value = 10;
			item.rare = ItemRarityID.Blue;
			item.shoot = ModContent.ProjectileType<FlameBullet>();   //The projectile shoot when your weapon using this ammo
			item.shootSpeed = 5f;                  //The speed of the projectile
			item.ammo = AmmoID.Bullet;              //The ammo class this ammo belongs to.
		}

		public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(175, 1);
            recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this, 150);
			recipe.AddRecipe();
		}
	}
}
