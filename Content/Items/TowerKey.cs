using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items
{
    public class TowerKey : ModItem
    {
        public override string Texture => AssetDirectory.Items + Name;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 38;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(silver: 50);
        }
    }
}