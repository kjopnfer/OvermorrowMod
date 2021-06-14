using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Accessories
{
    public class FireRune : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire Rune");
            Tooltip.SetDefault("Piercing weapons have a chance to inflict On Fire!");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 44;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.FireRune = true;
        }
    }
}