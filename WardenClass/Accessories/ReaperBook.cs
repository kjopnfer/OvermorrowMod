using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Accessories
{
    public class ReaperBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Book of the Reaper");
            Tooltip.SetDefault("Enemies you hit have a 5% chance to create a Reaped Soul\n" +
                "Picking up Reaped Souls increases the Soul Meter by 3%" +
                "\n'Hey look! This has my name in it!'");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 40;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.ReaperBook = true;
        }
    }
}