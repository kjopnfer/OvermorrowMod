using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.BearTrap
{
    public class BearTrap : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trapper's Armaments");
            Tooltip.SetDefault("Gain a bear trap after dealing at least 70 Ranged damage in a single hit\n" +
                "Place [?] to place a Bear Trap\n" +
                "You can have only up to 3 traps active at a time\n" +
                "[?] This item requires a keybind to use!");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string hotkey = "?";

            foreach (string key in OvermorrowModFile.BearTrapKey.GetAssignedKeys())
            {
                hotkey = key;
            }

            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name == "Tooltip1")
                {
                    line.Text = "Press [c/808080:{" + hotkey + "}] to place a Bear Trap";
                }

                if (line.Mod == "Terraria" && line.Name == "Tooltip3")
                {
                    if (hotkey == "?") line.Text = "[c/808080:{?} This item requires a keybind to use]";
                    else line.Text = "";
                }
            }
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 32;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().BearTrap = true;
            player.GetModPlayer<OvermorrowModPlayer>().BearTrapHide = hideVisual;
        }
    }
}