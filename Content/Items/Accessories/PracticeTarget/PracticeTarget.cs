using OvermorrowMod.Common.Players;
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
            Tooltip.SetDefault("For every consecutive {Type:Arrow} that hits a target, bow draw speed increases by {Increase:5%}\n" +
                "Draw speed bonus stacks up to a maximum of {Increase:20%}\n" +
                "On missed shot, reset draw speed bonus\n" +
                "'Huh, I guess they never miss.'");
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