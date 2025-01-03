
using OvermorrowMod.Common;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.Items
{
    public class TestPlaceable : ModItem
    {
        public override string Texture => AssetDirectory.Items + "TowerKey";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Test Placeable");
            // Tooltip.SetDefault("'THIS IS A TESTING ITEM'");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 2000;
            Item.createTile = TileType<SmallChair>();
        }
    }
}