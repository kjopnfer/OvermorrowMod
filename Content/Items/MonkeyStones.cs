using OvermorrowMod.Common;
using OvermorrowMod.Core.Globals;
using Terraria;
using Terraria.Audio;
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

        public override bool ItemSpace(Player player) => true;

        public override bool OnPickup(Player player)
        {
            player.GetModPlayer<MonkeyStonePlayer>().Add(Tier, Item.stack);
            PopupText.NewText(PopupTextContext.RegularItemPickup, Item, Item.stack);
            SoundEngine.PlaySound(SoundID.Grab, player.Center);
            return false;
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
