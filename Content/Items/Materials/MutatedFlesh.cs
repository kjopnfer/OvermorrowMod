using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class MutatedFlesh : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutated Flesh");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 26;
            Item.rare = ItemRarityID.Green;
            Item.maxStack = 999;
        }
    }
}