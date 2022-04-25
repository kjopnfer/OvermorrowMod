using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.Expert
{
    public class TreeNecklace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guiding Light of the Forest");
            Tooltip.SetDefault("Emits green light when equipped\nStanding still temporarily increases your defense" +
                "\nThe longer you stand still, the higher your defense is increased" +
                "\n'Dryads would hang these on trees to show the way through dark forests'");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Expert;
            Item.accessory = true;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().TreeNecklace = true;
        }
    }
}