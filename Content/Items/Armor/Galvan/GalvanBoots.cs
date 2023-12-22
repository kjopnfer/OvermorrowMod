using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Galvan
{
    [AutoloadEquip(EquipType.Legs)]
    public class GalvanBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Galvan Boots");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.vanity = true;
            //Item.value = Item.sellPrice(0, 1, 0, 0);
            //Item.rare = ItemRarityID.Blue;
            //Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            //player.moveSpeed += 0.1f;
            //player.GetDamage(DamageClass.Ranged) += 0.02f;
        }
    }
}
