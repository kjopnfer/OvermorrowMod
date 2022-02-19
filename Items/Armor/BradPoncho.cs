/*using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
namespace OvermorrowMod.Items.Armor
{

    [AutoloadEquip(EquipType.Body)]
    public class BradPoncho : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wastelander Poncho");
            Tooltip.SetDefault("3% increased damage \n'It feels painful'");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 20;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.03f;
        }
    }
}*/
