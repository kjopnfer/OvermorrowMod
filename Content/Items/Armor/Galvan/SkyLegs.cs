using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Galvan
{
    [AutoloadEquip(EquipType.Legs)]
    public class SkyLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Galvan Boots");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.vanity = true;
        }

        public override void UpdateEquip(Player player)
        {
            //player.moveSpeed += 0.1f;
            //player.GetDamage(DamageClass.Ranged) += 0.02f;
        }
    }
}
