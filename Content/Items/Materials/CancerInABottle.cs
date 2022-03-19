using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class CancerInABottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flesh Contagion");
            Tooltip.SetDefault("'Meat flavored monster energy drink'");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 34;
            item.rare = ItemRarityID.Orange;
            item.maxStack = 99;
        }
    }
}