using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class StinkCheese : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stinky Cheese");
            Tooltip.SetDefault("'Stinky'");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 26;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 15, 0);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(BuffID.Stinky, 999); 
        }
    }
}