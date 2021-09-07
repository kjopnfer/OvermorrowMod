using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Accessories
{
    public class SpiritShackle : ModItem
    {
        private int counter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Shackle");
            Tooltip.SetDefault("Grants passive Soul Gain\n" +
                "Soul Meter increases by 2% every 2 seconds\n" +
                "'Man is born free, and everywhere he is in chains'");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 30;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            counter++;
            if (modPlayer.soulPercentage < 100)
            {
                if (counter % 120 == 0)
                {
                    modPlayer.soulPercentage += 2;
                }
            }
        }
    }
}