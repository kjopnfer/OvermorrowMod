using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class BloodyTeeth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Tooth");
            Tooltip.SetDefault("5% increased melee damage\nMelee attacks have a 25% chance to inflict Bleeding" +
                "\n'Where would you even put this? On your head?'");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().BloodyTeeth = true;
            player.GetDamage(DamageClass.Melee) *= 1.05f;
        }
    }
}