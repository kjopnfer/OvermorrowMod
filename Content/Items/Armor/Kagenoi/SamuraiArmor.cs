using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Kagenoi
{

    [AutoloadEquip(EquipType.Body)]
    public class SamuraiArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kagenoi Armor");
            //Tooltip.SetDefault("5% increased melee damage");
        }

        public override void SetDefaults()
        {
            Item.width = 26; //filler 
            Item.height = 20;
            Item.vanity = true;
        }

        public override void UpdateEquip(Player player)
        {
            //player.GetDamage(DamageClass.Melee) += 0.05f;
        }
    }
}
