using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives
{
    public abstract class MonkeyStone : ModItem
    {
        public override string Texture => AssetDirectory.Items + Name;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.rare = ItemRarityID.Expert;
            Item.maxStack = 9999;
        }
    }

    public class MonkeyStoneBlue : MonkeyStone
    {
    }

    public class MonkeyStoneGreen : MonkeyStone
    {
    }

    public class MonkeyStoneOrange : MonkeyStone
    {
    }

    public class ChaosDiamond : MonkeyStone
    {
    }
}