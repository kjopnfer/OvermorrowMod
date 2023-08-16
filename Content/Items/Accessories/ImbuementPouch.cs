using OvermorrowMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class ImbuementPouch : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Imbuement Pouch");
            // Tooltip.SetDefault("Flask buffs apply to ALL your projectiles");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().ImbuementPouch = true;
        }
    }
}