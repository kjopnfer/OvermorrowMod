using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class BMHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Battle Mage Mask");
            Tooltip.SetDefault("5% increased melee and magic damage");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 22;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Blue;
            item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BMBreastplate>() && legs.type == ModContent.ItemType<BMLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.magicDamage += 0.05f;
            player.meleeDamage += 0.05f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "\nMelee attacks restore mana proportional to the damage dealt\nYou cannot regenerate mana";
            player.manaRegen -= 99999;
            player.manaRegenBonus -= 99999;
            player.GetModPlayer<OvermorrowModPlayer>().BMSet = true;
        }
    }
}