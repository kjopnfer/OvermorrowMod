using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Accessories.Expert
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
            item.width = 26;
            item.height = 30;
            item.value = Item.buyPrice(0, 5, 0, 0);
            item.rare = ItemRarityID.Expert;
            item.accessory = true;
            item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().TreeNecklace = true;
        }
    }
}