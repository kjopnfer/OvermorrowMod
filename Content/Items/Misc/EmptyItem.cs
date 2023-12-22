using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Misc
{
    /// <summary>
    /// This thing is used for the Tile Piles to make it so hovering over things hides the item on your cursor
    /// </summary>
    public class EmptyItem : ModItem
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 1;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool CanPickup(Player player) => true;

        public override bool OnPickup(Player player) => false;
    }
}
