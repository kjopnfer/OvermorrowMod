﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class AnglerTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Angler Tooth");
            // Tooltip.SetDefault("{Increase:7%} increased crit chance");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 26;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += 7;
        }
    }
}