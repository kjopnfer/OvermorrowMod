using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Armor.Marble
{

    [AutoloadEquip(EquipType.Body)]
    public class MarblePlate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marble Chestplate");
            Tooltip.SetDefault("3% ranged Damage");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 20;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Blue;
            item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.rangedDamage += 0.03f;
        }
    }
}
