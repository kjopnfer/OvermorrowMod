using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Galvan
{

    [AutoloadEquip(EquipType.Body)]
    public class SkyChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ancient Galvan Breastplate");
        }

        public override void SetDefaults()
        {
            Item.width = 26; //filler 
            Item.height = 20;
            Item.vanity = true;
        }

        public override void UpdateEquip(Player player)
        {
            //player.moveSpeed += 0.07f;
            //player.GetDamage(DamageClass.Ranged) += 0.02f;
        }
    }
}
