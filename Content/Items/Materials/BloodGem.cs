using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class BloodGem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Gem");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Orange;
            item.maxStack = 99;
        }
    }
}