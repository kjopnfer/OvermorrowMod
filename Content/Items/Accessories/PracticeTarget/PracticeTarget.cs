using OvermorrowMod.Common.Players;
using OvermorrowMod.Common.VanillaOverrides.Bow;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.PracticeTarget
{
    public class PracticeTarget : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Practice Target");
            Tooltip.SetDefault("For every consecutive arrow that hits a target, bow draw speed increases by 4%.\n" +
                "On missed shot, reset draw speed bonus.\n" +
                "Stacks up to a maximum of 20%.");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 34;
            Item.height = 40;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().PracticeTarget = true;
            player.GetModPlayer<OvermorrowModPlayer>().PracticeTargetHide = hideVisual;
        }
    }
}