using OvermorrowMod.Common;
using OvermorrowMod.Core.UI.LoadoutSelection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items
{
    public class TestKey : ModItem
    {
        public override string Texture => AssetDirectory.Items + "TowerKey";

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 38;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.rare = ItemRarityID.Expert;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                LoadoutSelectionManager.Show();
            }
            return true;
        }
    }
}
