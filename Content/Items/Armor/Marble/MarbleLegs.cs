using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Marble
{

    [AutoloadEquip(EquipType.Legs)]
    public class MarbleLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacquered Champion's Leggings");
            Tooltip.SetDefault("3% ranged damage \n15% decreased mana usage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.05f;
            player.GetDamage(DamageClass.Ranged) += 0.03f;
            player.manaCost -= 0.15f;
        }
    }
}
