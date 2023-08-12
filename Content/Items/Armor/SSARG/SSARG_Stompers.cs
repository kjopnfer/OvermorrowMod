using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.SSARG
{
    [AutoloadEquip(EquipType.Legs)]
    public class SSARG_Stompers : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("S.S.A.R.G Stompers");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 12;
            Item.vanity = true;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
