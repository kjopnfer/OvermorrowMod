
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged.Ammo
{
	public class DiceAmmo : ModItem
	{
		public override void SetStaticDefaults() 
        {
            DisplayName.SetDefault("Dice Arrow");
			Tooltip.SetDefault("Unlimited random arrow");
		}

		public override void SetDefaults() 
		{
			item.damage = 12;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 1;
			item.consumable = false;             //You need to set the item consumable so that the ammo would automatically consumed
			item.knockBack = 1.5f;
			item.value = 10;
			item.rare = ItemRarityID.Expert;
			item.shoot = ModContent.ProjectileType<DiceArrow>();   //The projectile shoot when your weapon using this ammo
			item.shootSpeed = 4.4f;                  //The speed of the projectile
			item.ammo = AmmoID.Arrow;              //The ammo class this ammo belongs to.
		}
	}
}
