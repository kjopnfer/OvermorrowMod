using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Marble
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
            Item.width = 26;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.02f;
            player.GetDamage(DamageClass.Ranged) += 0.03f;
        }
    }
}
