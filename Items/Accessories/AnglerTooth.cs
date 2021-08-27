using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Accessories
{
	public class AnglerTooth : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Angler Tooth");
			Tooltip.SetDefault("Grants 7% increased crit for all classes");
		}

		public override void SetDefaults()
		{
			item.accessory = true;
			item.width = 26; //Filler
			item.height = 24; //Filler
			item.rare = ItemRarityID.Blue;
			item.value = Item.sellPrice(0, 1, 0, 0);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.magicCrit += 7;
            player.meleeCrit += 7;
			player.rangedCrit += 7;
            player.thrownCrit += 7;

		}
    }
}