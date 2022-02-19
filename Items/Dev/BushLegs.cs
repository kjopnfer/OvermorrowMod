using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Dev
{
    [AutoloadEquip(EquipType.Legs)]
    public class BushLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bush-man's Legs");
            Tooltip.SetDefault("'Ah yes, chlamydia mod'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Expert;
            item.vanity = true;
        }
    }
}