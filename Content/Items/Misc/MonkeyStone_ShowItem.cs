using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Misc
{
    public class MonkeyStone_ShowItem : ModItem
    {
        public override string Texture => AssetDirectory.UI + "MonkeyStones";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.rare = ItemRarityID.Blue;
            item.maxStack = 1;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool CanPickup(Player player) => true;

        public override bool OnPickup(Player player) => false;
    }
}