using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Dev
{
    [AutoloadEquip(EquipType.Head)]
    public class ChileanGarbageSet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chilean's Weeb Mask");
            Tooltip.SetDefault("'This is an anime server only'");
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