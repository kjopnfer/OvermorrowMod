using System;
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
            Tooltip.SetDefault("Enemies killed by the holder have a 25% chance to drop a Soul Essence on death" +
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