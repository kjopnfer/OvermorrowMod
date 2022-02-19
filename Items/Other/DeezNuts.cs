using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.Items.Other
{
    public class DeezNuts : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deez Nuts");
            Tooltip.SetDefault("Guess what came in the mail today");
        }
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.CloneDefaults(ItemID.Acorn);
        }

    }
}
