using OvermorrowMod.Core;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.Items.Placeable
{
	public class TestPlaceable : ModItem
	{
        public override string Texture => AssetDirectory.Textures + "ChainKnife";
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Test Placeable");
		}

		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 20;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.value = 2000;
			item.createTile = TileType<Content.Tiles.Boss.DharuudArena>();
		}
	}
}