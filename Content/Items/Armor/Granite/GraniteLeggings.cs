using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Granite
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
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
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