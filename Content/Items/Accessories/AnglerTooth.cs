using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class AnglerTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angler Tooth");
            Tooltip.SetDefault("7% increased crit chance");
        }

        public override void SetDefaults()
        {
            item.accessory = true;
            item.width = 26;
            item.height = 24;
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