using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Accessories
{
    public class AncientCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Crystal");
            Tooltip.SetDefault("Increases your max Soul Essences by 1\n'Whispers emanante from the crystal, willing you to become whole'");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 38;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.AncientCrystal = true;
        }
    }
}