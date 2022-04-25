using OvermorrowMod.Content.Tiles.Boss;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Placeable.Boss
{
    public class DrakeTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Storm Drake Trophy");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Terraria.Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.createTile = ModContent.TileType<StormDrakeTrophy>();
            Item.placeStyle = 0;
        }
    }
}