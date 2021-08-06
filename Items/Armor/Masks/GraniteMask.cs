using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Armor.Masks
{
    [AutoloadEquip(EquipType.Head)]
    public class GraniteMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gra-Knight Mask");
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