using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Core.WorldGeneration.ArchiveSubworld;
using SubworldLibrary;
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
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.rare = ItemRarityID.Expert;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Main.NewText("The key does something.", Color.Red);
                SubworldSystem.Enter<ArchiveSubworld>();
            }

            return true;
        }
    }
}