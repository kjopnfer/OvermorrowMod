using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.SSARG
{
    [AutoloadEquip(EquipType.Head)]
    public class SSARG_Helmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("S.S.A.R.G Helmet");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
            //Item.defense = 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SSARG_Reactor>() && legs.type == ModContent.ItemType<SSARG_Stompers>();
        }
    }
}
