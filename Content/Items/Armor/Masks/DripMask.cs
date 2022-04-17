using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Masks
{
    [AutoloadEquip(EquipType.Head)]
    public class DripMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dripplord Mask");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
    }
}