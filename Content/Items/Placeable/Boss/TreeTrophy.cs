using OvermorrowMod.Tiles.Boss;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Placeable.Boss
{
    public class TreeTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich, the Guardian Trophy");
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
            item.createTile = ModContent.TileType<TreeBossTrophy>();
            item.placeStyle = 0;
        }
    }
}