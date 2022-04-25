using OvermorrowMod.Common;
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
            Item.width = 18;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
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
            player.GetDamage(DamageClass.Ranged) *= 0.05f;
        }
    }
}