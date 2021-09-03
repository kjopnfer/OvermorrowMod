using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using OvermorrowMod.Projectiles.Accessory;

namespace OvermorrowMod.Items.Accessories.Expert
{
    public class ArtemisAmulet : ModItem {
        //public override string Texture => "Terraria/Item_" + ItemID.CharmofMyths;

        public override void SetStaticDefaults() {
            Tooltip.SetDefault(
                "placeholder"
            );
        }

        public override void SetDefaults() {
            item.width = 24;
            item.height = 28;
            item.value = 17500;
            item.rare = ItemRarityID.Green;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.GetModPlayer<OvermorrowModPlayer>().ArtemisAmulet = true;
        }
    }
}