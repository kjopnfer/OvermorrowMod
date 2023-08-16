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
            // DisplayName.SetDefault("Kagenoi Hat");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.vanity = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SamuraiArmor>() && legs.type == ModContent.ItemType<SamuraiSandals>();
        }

        public override void UpdateEquip(Player player)
        {
            //player.GetCritChance(DamageClass.Melee) += 0.05f;
            //player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
        }
    }
}
