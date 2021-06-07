using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Accessories
{
    public class BloodyTeeth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Teeth");
            Tooltip.SetDefault("5% increased melee damage\nMelee attacks have a 25% chance to inflict Bleeding" +
                "\n'I didn't see any teeth on those things... Unless...?'");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 22;
            item.value = Item.buyPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().BloodyTeeth = true;
            player.meleeDamage += 0.05f;
        }
    }
}