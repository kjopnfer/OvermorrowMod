using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Masks
{
    [AutoloadEquip(EquipType.Head)]
    public class DrakeMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Storm Drake Mask");
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