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
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.vanity = true;
        }

        public override void UpdateEquip(Player player)
        {
            //player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
        }
    }
}
