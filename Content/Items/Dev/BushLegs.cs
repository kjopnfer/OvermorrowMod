using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Dev
{

    [AutoloadEquip(EquipType.Legs)]
    public class BushLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bush-man's Legs");
            // Tooltip.SetDefault("'Ah yes, chlamydia mod'");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Expert;
            Item.vanity = true;
        }
    }
}