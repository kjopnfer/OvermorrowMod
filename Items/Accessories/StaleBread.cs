﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Accessories
{
    public class StaleBread : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stale Bread");
            Tooltip.SetDefault(":(\nIncreases max life by 10.");
        }

        public override void SetDefaults()
        {
            item.accessory = true;
            item.width = 26;
            item.height = 24;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(0, 0, 15, 0);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 10;
        }
    }
}