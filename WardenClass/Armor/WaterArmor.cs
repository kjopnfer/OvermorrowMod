using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using OvermorrowMod.Items.Materials;
using WardenClass;

namespace OvermorrowMod.WardenClass.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class WaterArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sullen Binder Plate");
            Tooltip.SetDefault("");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 24;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Orange;
            item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
        }
    }
}

