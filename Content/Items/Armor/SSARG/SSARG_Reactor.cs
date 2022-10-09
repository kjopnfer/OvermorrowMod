using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.SSARG
{

    [AutoloadEquip(EquipType.Body)]
    public class SSARG_Reactor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("S.S.A.R.G Reactor");
        }

        public override void SetDefaults()
        {
            Item.width = 26; //filler 
            Item.height = 34;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }
    }
}
