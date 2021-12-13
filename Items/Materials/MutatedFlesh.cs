using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Materials
{
    public class MutatedFlesh : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutated Flesh");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 26;
            item.rare = ItemRarityID.Green;
            item.maxStack = 999;
        }
    }
}