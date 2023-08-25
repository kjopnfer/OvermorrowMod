using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Ammo
{
    public class ObsidianArrow : ModItem
    {
		public override void SetStaticDefaults()
		{
			// Tooltip.SetDefault("10 Armor Penetration\n25% Increased Critical Strike Damage");
		}

		public override void SetDefaults()
		{
			Item.width = 14; // The width of item hitbox
			Item.height = 34; // The height of item hitbox

			Item.damage = 4; // The damage for projectiles isn't actually 8, it actually is the damage combined with the projectile and the item together
			Item.DamageType = DamageClass.Ranged; // What type of damage does this ammo affect?
			Item.knockBack = 1f;

			Item.maxStack = 9999; // The maximum number of items that can be contained within a single stack
			Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible
			Item.value = Item.sellPrice(0, 0, 1, 0); // Item price in copper coins (can be converted with Item.sellPrice/Item.buyPrice)
			Item.rare = ItemRarityID.Green; // The color that the item's name will be in-game.
			Item.shoot = ModContent.ProjectileType<ObsidianArrowProjectile>(); // The projectile that weapons fire when using this item as ammunition.

			Item.ammo = AmmoID.Arrow; // Important. The first item in an ammo class sets the AmmoID to its type
		}

		public override void AddRecipes()
		{
			CreateRecipe(5)
				.AddIngredient(ItemID.WoodenArrow, 5)
				.AddIngredient(ItemID.Obsidian, 1)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}

	public class ObsidianArrowProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
			Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
			AIType = ProjectileID.WoodenArrowFriendly;
			Projectile.ArmorPenetration = 10;
		}
    }
}