using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Quest
{
    public class WarmAmulet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.rare = ItemRarityID.Blue;
            Item.questItem = true;
        }
    }
}