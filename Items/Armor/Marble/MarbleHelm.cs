using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Armor.Marble
{

    [AutoloadEquip(EquipType.Head)]
    public class MarbleHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marble Helmet");
            Tooltip.SetDefault("3% increased ranged damage \n5% increased magic damage");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 20;
            item.value = Item.sellPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.magicDamage += 0.05f;
            player.rangedDamage += 0.03f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<MarblePlate>() && legs.type == ModContent.ItemType<MarbleLegs>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Moving leaves a damaging trail \n2% increased ranged damage \n3% increased magic damage";
            player.rangedDamage += 0.02f;
            player.magicDamage += 0.03f;
            player.GetModPlayer<OvermorrowModPlayer>().MarbleTrail = true;
        }
    }
}