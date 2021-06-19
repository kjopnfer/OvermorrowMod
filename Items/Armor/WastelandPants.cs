using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class WastelandPants : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("2% increased damage \n'It feels painful'");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10000;
            item.rare = ItemRarityID.Orange;
            item.defense = 10;
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.02f;
        }
    }
}
