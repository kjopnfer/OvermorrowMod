using OvermorrowMod.Tiles.Boss;
using Terraria.ModLoader;
using Terraria.ID;

namespace OvermorrowMod.Items.Placeable.Boss
{
    public class DrakeTrophy : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Storm Drake of Oris Trophy");
		}

		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 32;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.value = Terraria.Item.sellPrice(0, 1, 0, 0);
			item.rare = ItemRarityID.Blue;
			item.createTile = ModContent.TileType<StormDrakeTrophy>();
			item.placeStyle = 0;
		}
	}
}