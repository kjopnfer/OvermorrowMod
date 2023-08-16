using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class BloodGem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Gem");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = 99;
        }
    }
}