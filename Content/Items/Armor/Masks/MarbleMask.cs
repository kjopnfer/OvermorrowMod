using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Masks
{
    [AutoloadEquip(EquipType.Head)]
    public class MarbleMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marbella Mask");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.rare = ItemRarityID.Blue;
            item.vanity = true;
        }
    }
}