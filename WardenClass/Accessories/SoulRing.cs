using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Accessories
{
    public class SoulRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulstone Ring");
            Tooltip.SetDefault("Increases life regeneration\n " +
                "Regeneration rate scales with number of held Soul Essences");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 24;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.SoulRing = true;
        }
    }
}