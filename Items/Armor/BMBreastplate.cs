using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class BMBreastplate : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Battle Mage Breastplate");
			Tooltip.SetDefault("5% increased melee critical strike chance\n5% magic critical strike chance");
		}

		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 22;
			item.value = Item.sellPrice(gold: 1);
			item.rare = ItemRarityID.Blue;
			item.defense = 5;
		}

		public override void UpdateEquip(Player player)
		{
			player.meleeCrit += 5;
			player.magicCrit += 5;
		}
	}
}