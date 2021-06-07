using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class GraniteLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Crusher Leggings");
            Tooltip.SetDefault("Grants immunity to knockback\n6% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 18;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Blue;
            item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GraniteBreastplate>() && head.type == ModContent.ItemType<GraniteHelmet>();
        }

        public override void UpdateEquip(Player player)
        {
            player.noKnockback = true;
            player.moveSpeed += .06f;
        }
    }
}