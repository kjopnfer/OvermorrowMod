using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Accessories
{
    public class SoulFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulstone Fragment");
            Tooltip.SetDefault("Increases Soul Meter gain by 3%");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 20;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.soulGainBonus += 3;
        }
    }
}