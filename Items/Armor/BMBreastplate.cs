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
			Tooltip.SetDefault("Increases melee damage by 5%\nIncreases your max number of minions");
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
			player.meleeDamage += 0.05f;
			player.maxMinions++;
		}
	}
}