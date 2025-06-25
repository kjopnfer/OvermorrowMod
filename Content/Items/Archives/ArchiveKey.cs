using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Core.WorldGeneration.ArchiveSubworld;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives
{
    public class ArchiveKey : ModItem
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 38;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.rare = ItemRarityID.Expert;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.maxStack = 9999;
        }

        public override bool? UseItem(Player player)
        {
            return true;
        }
    }
}