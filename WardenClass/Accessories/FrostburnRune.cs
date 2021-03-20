using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Accessories
{
    public class FrostburnRune : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frostburn Rune");
            Tooltip.SetDefault("Piercing attacks have a chance to inflict Frostburn");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 36;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.FrostburnRune = true;
        }
    }
}