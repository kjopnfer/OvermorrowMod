using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Accessories
{
    public class ShroomRune : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Rune");
            Tooltip.SetDefault("Increase piercing damage by 2\nPiercing weapons have a chance to inflict Fungal Infection");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 40;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.FungalRune = true;
            modPlayer.piercingDamageAdd += 2;
        }
    }
}