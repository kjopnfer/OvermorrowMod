using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class StaleBread : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stale Bread");
            // Tooltip.SetDefault("Increases max life by {Increase:10}");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 15, 0);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 10;
        }
    }
}
