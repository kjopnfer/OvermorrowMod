using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Accessories
{
    public class StinkCheese : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stink Cheese");
            Tooltip.SetDefault("Stinky");
        }

        public override void SetDefaults()
        {
            item.accessory = true;
            item.width = 26;
            item.height = 24;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(0, 0, 15, 0);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(BuffID.Stinky, 999); 
        }
    }
}