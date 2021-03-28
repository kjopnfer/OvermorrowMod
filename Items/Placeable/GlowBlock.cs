using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Placeable
{
	public class GlowBlock : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wet Stone");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 16;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.createTile = TileType<Tiles.GlowBlock>();
		}
	}

}
