using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Misc
{
    public class GreenCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Resonance Crystal of Erudition");
            Tooltip.SetDefault("Used to enhance your mental prowess");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 26;
            item.rare = ItemRarityID.Green;
            item.maxStack = 99;
        }
    }
}