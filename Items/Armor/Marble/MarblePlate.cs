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
            DisplayName.SetDefault("Lacquered Champion's Chestplate");
            Tooltip.SetDefault("3% ranged damage\n2% increased magic damage");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 20;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Blue;
            item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.magicDamage += 0.02f;
            player.rangedDamage += 0.03f;
        }
    }
}
