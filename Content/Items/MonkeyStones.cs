using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives
{
    public enum MonkeyStoneTier { Blue, Green, Orange, Chaos }

    public abstract class MonkeyStone : ModItem
    {
        public override string Texture => AssetDirectory.Items + Name;

        public abstract MonkeyStoneTier Tier { get; }

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
        public override MonkeyStoneTier Tier => MonkeyStoneTier.Blue;
    }

    public class MonkeyStoneGreen : MonkeyStone
    {
        public override MonkeyStoneTier Tier => MonkeyStoneTier.Green;
    }

    public class MonkeyStoneOrange : MonkeyStone
    {
        public override MonkeyStoneTier Tier => MonkeyStoneTier.Orange;
    }

    public class ChaosDiamond : MonkeyStone
    {
        public override MonkeyStoneTier Tier => MonkeyStoneTier.Chaos;
    }
}
