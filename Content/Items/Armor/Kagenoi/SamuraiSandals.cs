using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Kagenoi
{
    [AutoloadEquip(EquipType.Legs)]
    public class SamuraiSandals : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kagenoi Sandals");
            Tooltip.SetDefault("5% increased melee speed");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
        }
    }
}
