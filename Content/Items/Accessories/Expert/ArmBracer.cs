using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.Expert
{
    public class ArmBracer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dharuud's Arm Bracer");
            Tooltip.SetDefault("Increases your max number of minions\nIncreases your max mana by 30\nCreates a shield of sand whenever your minions or magic projectiles strike an enemy\n" +
                "Press [binded hotkey] to switch your sand to attack mode\n" +
                "'Dharuud remembers when sand was the ultimate defense, not skeletons'");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = Item.buyPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
            item.expert = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string hotkey = "Bind a Hotkey in Controls!";

            // This gets the hotkey if it is assigned
            foreach (String key in OvermorrowModFile.SandModeKey.GetAssignedKeys())
            {
                hotkey = key;
            }
            string sandMode = "";

            foreach (TooltipLine line in tooltips)
            {
                Player player = Main.LocalPlayer;
                if (player.GetModPlayer<OvermorrowModPlayer>().sandMode == 0)
                {
                    sandMode = "Attack";
                }
                else
                {
                    sandMode = "Defense";
                }

                if (line.mod == "Terraria" && line.Name == "Tooltip3")
                {
                    line.text = "Press [" + hotkey + "] to swap to " + sandMode + " mode";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().ArmBracer = true;
            player.maxMinions++;
            player.statManaMax2 += 30;
        }
    }
}