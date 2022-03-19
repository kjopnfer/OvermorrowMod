using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class GraniteBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Crusher Breastplate");
            Tooltip.SetDefault("Increases melee damage by 5%\nIncreases your max number of minions");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 22;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Blue;
            item.defense = 5;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<GraniteHelmet>() && legs.type == ModContent.ItemType<GraniteLeggings>();
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeDamage += 0.05f;
            player.maxMinions++;
        }
    }
}