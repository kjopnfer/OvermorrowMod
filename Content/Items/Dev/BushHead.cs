using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Dev
{

    [AutoloadEquip(EquipType.Head)]
    public class BushHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bush-man's Visage");
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