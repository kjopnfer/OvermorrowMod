using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class CancerInABottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flesh Contagion");
            // Tooltip.SetDefault("'Meat flavored monster energy drink'");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = 99;
        }
    }
}