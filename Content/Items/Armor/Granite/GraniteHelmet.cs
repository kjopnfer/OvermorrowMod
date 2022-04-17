using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Granite
{
    [AutoloadEquip(EquipType.Head)]
    public class GraniteHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Crusher Mask");
            Tooltip.SetDefault("5% increased melee and minion damage");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GraniteBreastplate>() && legs.type == ModContent.ItemType<GraniteLeggings>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.05f;
            player.GetDamage(DamageClass.Summon) += 0.05f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "\nIncreases your defense by 1 for each minion\nIncreases melee damage by 3% for each minion";
            player.GetModPlayer<OvermorrowModPlayer>().graniteSet = true;
        }
    }
}