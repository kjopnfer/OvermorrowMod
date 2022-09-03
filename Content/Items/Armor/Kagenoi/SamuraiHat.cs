using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Kagenoi
{
    [AutoloadEquip(EquipType.Head)]
    public class SamuraiHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kagenoi Hat");
            Tooltip.SetDefault("5% increased melee speed\n5% increased melee critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SamuraiArmor>() && legs.type == ModContent.ItemType<SamuraiSandals>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Melee) += 0.05f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
        }
    }
}
