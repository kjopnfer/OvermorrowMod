using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class DripplerEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Malevolent Eye");
            Tooltip.SetDefault("5% increased ranged damage\nEach killed enemy increases ranged critical hit chance by 1%\n " +
                "(x stacks out of 25)\n" +
                "'I see the future, one bathed in blood'");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 24;
            item.value = Item.buyPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                Player player = Main.LocalPlayer;
                if (line.mod == "Terraria" && line.Name == "Tooltip2")
                {
                    line.text = "[" + player.GetModPlayer<OvermorrowModPlayer>().dripplerStack + " out of 25]";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().DripplerEye = true;
            player.rangedDamage += 0.05f;
        }
    }
}